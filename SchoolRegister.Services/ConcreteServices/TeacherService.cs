using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SchoolRegister.DAL.EF;
using SchoolRegister.Model.DataModels;
using SchoolRegister.Services.Interfaces;
using SchoolRegister.ViewModels.VM;

namespace SchoolRegister.Services.ConcreteServices
{
    public class TeacherService : BaseService, ITeacherService
    {
        private readonly UserManager<User> _userManager;

        public TeacherService(ApplicationDbContext dbContext, IMapper mapper, ILogger logger, UserManager<User> userManager)
            : base(dbContext, mapper, logger)
        {
            _userManager = userManager;
        }

        public TeacherVm GetTeacher(Expression<Func<Teacher, bool>> filterPredicate)
        {
            try
            {
                if (filterPredicate == null)
                    throw new ArgumentNullException($"Filter expression is null");

                var teacherEntity = DbContext.Users.OfType<Teacher>().FirstOrDefault(filterPredicate);
                var teacherVm = Mapper.Map<TeacherVm>(teacherEntity);
                return teacherVm;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public IEnumerable<TeacherVm> GetTeachers(Expression<Func<Teacher, bool>> filterPredictae = null)
        {
            try
            {
                var teacherEntities = DbContext.Users.OfType<Teacher>().AsQueryable();

                if (filterPredictae != null)
                    teacherEntities = teacherEntities.Where(filterPredictae);

                var teacherVms = Mapper.Map<IEnumerable<TeacherVm>>(teacherEntities);
                return teacherVms;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public IEnumerable<GroupVm> GetTeachersGroups(TeachersGroupsVm getTeachersGroups)
        {
            try
            {
                if (getTeachersGroups == null)
                    throw new ArgumentNullException($"View model parameter is null");

                var teacherSubjectIds = DbContext.Subjects
                                        .Where(x => x.TeacherId == getTeachersGroups.TeacherId)
                                        .Select(x => x.Id)
                                        .ToList();

                var teacherGroups = from grp in DbContext.Groups
                                   join subGrp in DbContext.SubjectGroups on grp.Id equals subGrp.GroupId
                                   where teacherSubjectIds.Contains(subGrp.SubjectId)
                                   select grp;
                
                var groupVms = Mapper.Map<IEnumerable<GroupVm>>(teacherGroups);
                return groupVms;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}