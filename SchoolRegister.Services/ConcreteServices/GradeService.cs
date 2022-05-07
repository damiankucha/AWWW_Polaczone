using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SchoolRegister.DAL.EF;
using SchoolRegister.Model.DataModels;
using SchoolRegister.Services.Interfaces;
using SchoolRegister.ViewModels.VM;

namespace SchoolRegister.Services.ConcreteServices
{
    public class GradeService : BaseService, IGradeService
    {
        private readonly UserManager<User> _userManager;

        public GradeService(ApplicationDbContext dbContext, IMapper mapper, ILogger logger, UserManager<User> userManager)
            : base(dbContext, mapper, logger)
        {
            _userManager = userManager;
        }

        public GradeVm AddGradeToStudent(AddGradeToStudentVm addGradeToStudentVm)
        {
            try
            {
                if (addGradeToStudentVm == null)
                    throw new ArgumentNullException($"View model parameter is null");

                var teacher = DbContext.Users.OfType<Teacher>().FirstOrDefault(t => t.Id == addGradeToStudentVm.TeacherId);

                if (_userManager.IsInRoleAsync(teacher, "Teacher").Result == false)
                    throw new ArgumentException($"User is not a teacher");
                    
                var gradeEntity = Mapper.Map<Grade>(addGradeToStudentVm);

                DbContext.Grades.Add(gradeEntity);
                DbContext.SaveChanges();

                var gradeVm = Mapper.Map<GradeVm>(gradeEntity);
                return gradeVm;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public GradesReportVm GetGradesReportForStudent(GetGradesReportVm getGradesReportVm)
        {
            try 
            {
                if (getGradesReportVm == null)
                    throw new ArgumentNullException($"View model parameter is null");

                var getterEntity = DbContext.Users.FirstOrDefault(x => x.Id == getGradesReportVm.GetterUserId);

                if (_userManager.IsInRoleAsync(getterEntity, "Student").Result || _userManager.IsInRoleAsync(getterEntity, "Parent").Result)
                {
                    var grades = DbContext.Grades.Where(x => x.StudentId == getGradesReportVm.StudentId);
                    var gradesVm = Mapper.Map<IList<GradeVm>>(grades);

                    var gradesReportVm = new GradesReportVm()
                    {
                        Grades = gradesVm,
                        GetterUserId = getGradesReportVm.GetterUserId,
                        GetterUserName = $"{getterEntity.FirstName} {getterEntity.LastName}"
                    };

                    return gradesReportVm;
                }

                else 
                    throw new ArgumentException("Invalid user role");
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}