using System;
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

        public async Task<GradeVm> AddGradeToStudent(AddGradeToStudentVm addGradeToStudentVm)
        {
            try
            {
                if (addGradeToStudentVm == null)
                    throw new ArgumentNullException($"View model parameter is null");

                var teacher = DbContext.Users.OfType<Teacher>().FirstOrDefault(t => t.Id == addGradeToStudentVm.TeacherId);

                if (await _userManager.IsInRoleAsync(teacher, "Teacher") == false)
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
            throw new System.NotImplementedException();
        }
    }
}