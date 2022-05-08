using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolRegister.DAL.EF;
using SchoolRegister.Model.DataModels;
using SchoolRegister.Services.Interfaces;
using SchoolRegister.ViewModels.VM;

namespace SchoolRegister.Services.ConcreteServices
{
    public class GroupService : BaseService, IGroupService
    {
        public GroupService(ApplicationDbContext dbContext, IMapper mapper, ILogger logger)
            : base(dbContext, mapper, logger)
        {
        }

        public GroupVm AddOrUpdateGroup(AddOrUpdateGroupVm addOrUpdateGroupVm)
        {
            try
            {
                if (addOrUpdateGroupVm == null)
                    throw new ArgumentNullException("View model parameter is null");

                var groupEntity = Mapper.Map<Group>(addOrUpdateGroupVm);

                if (!addOrUpdateGroupVm.Id.HasValue || addOrUpdateGroupVm.Id == 0)
                    DbContext.Groups.Add(groupEntity);
                else
                    DbContext.Groups.Update(groupEntity);

                DbContext.SaveChanges();

                var groupVm = Mapper.Map<GroupVm>(groupEntity);
                return groupVm;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public StudentVm AttachStudentToGroup(AttachDetachStudentToGroupVm attachStudentToGroupVm)
        {
            try
            {
                if (attachStudentToGroupVm == null)
                    throw new ArgumentNullException("View model parameter is null");

                var student = DbContext.Users.OfType<Student>()
                    .FirstOrDefault(x => x.Id == attachStudentToGroupVm.StudentId);

                student.GroupId = attachStudentToGroupVm.GroupId;
                DbContext.SaveChanges();

                var studentVm = Mapper.Map<StudentVm>(student);
                return studentVm;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public GroupVm AttachSubjectToGroup(AttachDetachSubjectGroupVm attachDetachSubjectGroupVm)
        {
            try
            {
                if (attachDetachSubjectGroupVm == null)
                    throw new ArgumentNullException("View model parameter is null");

                var group = DbContext.Groups
                    .Include(p => p.SubjectGroups)
                    .Single(p => p.Id == attachDetachSubjectGroupVm.GroupId);

                var subject = DbContext.Subjects
                    .Single(p => p.Id == attachDetachSubjectGroupVm.SubjectId);

                group.SubjectGroups.Add(new SubjectGroup
                {
                    Group = group,
                    Subject = subject
                });
                DbContext.SaveChanges();

                var groupVm = Mapper.Map<GroupVm>(group);
                return groupVm;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public StudentVm DetachStudentFromGroup(AttachDetachStudentToGroupVm detachStudentToGroupVm)
        {
            try
            {
                if (detachStudentToGroupVm == null)
                    throw new ArgumentNullException("View model parameter is null");

                var student = DbContext.Users.OfType<Student>()
                    .FirstOrDefault(x => x.Id == detachStudentToGroupVm.StudentId);

                student.GroupId = null;
                DbContext.SaveChanges();

                var studentVm = Mapper.Map<StudentVm>(student);
                return studentVm;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public GroupVm GetGroup(Expression<Func<Group, bool>> filterPredicate)
        {
            try 
            {
                if (filterPredicate == null)
                    throw new ArgumentNullException("Filter predicate is null");

                var groupEntity = DbContext.Groups.FirstOrDefault(filterPredicate);
                var groupVm = Mapper.Map<GroupVm>(groupEntity);
                return groupVm;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public IEnumerable<GroupVm> GetGroups(Expression<Func<Group, bool>> filterPredicate = null)
        {
            try
            {
                var groupEntities = DbContext.Groups.AsQueryable();

                if (filterPredicate != null)
                    groupEntities = groupEntities.Where(filterPredicate);

                var groupVms = Mapper.Map<IEnumerable<GroupVm>>(groupEntities);
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
