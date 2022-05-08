using System.Linq;
using SchoolRegister.DAL.EF;
using SchoolRegister.Services.Interfaces;
using SchoolRegister.ViewModels.VM;
using Xunit;

namespace SchoolRegister.Tests.UnitTests
{
    public class GroupServiceUnitTests : BaseUnitTests
    {
        private readonly IGroupService _groupService;

        public GroupServiceUnitTests(ApplicationDbContext dbContext, IGroupService groupService)
            : base(dbContext)
        {
            _groupService = groupService;
        }

        [Fact]
        public void GetGroup()
        {
            var addedGroup = _groupService.GetGroup(x => x.Name == "PAI");
            Assert.NotNull(addedGroup);
        }

        [Fact]
        public void GetGroups()
        {
            var groups = _groupService.GetGroups(x => x.Id >= 1 && x.Id <= 2)
                .ToList();
            Assert.NotNull(groups);
            Assert.NotEmpty(groups);
            Assert.Equal(2, groups.Count());
        }

        [Fact]
        public void GetAllGroups()
        {
            var groups = _groupService.GetGroups().ToList();
            Assert.NotNull(groups);
            Assert.NotEmpty(groups);
            Assert.Equal(3, groups.Count());
        }


        [Fact]
        public void AddGroup()
        {
            var addOrUpdateGroupVm = new AddOrUpdateGroupVm
            {
                Name = "SK"
            };
            
            _groupService.AddOrUpdateGroup(addOrUpdateGroupVm);
            Assert.Equal(4, DbContext.Groups.Count());
            var addedGroup = _groupService.GetGroup(x => x.Name == "SK");
            Assert.NotNull(addedGroup);
        }

        [Fact]
        public void UpdateGroup()
        {
            var addOrUpdateGroupVm = new AddOrUpdateGroupVm
            {
                Name = "SIiDM",
                Id = 3
            };

            _groupService.AddOrUpdateGroup(addOrUpdateGroupVm);
            var addedGroup = _groupService.GetGroup(x => x.Name == "SIiDM");
            Assert.NotNull(addedGroup);
        }

        [Fact]
        public void AttachStudentToGroup()
        {
            var attachStudentToGroupVm = new AttachDetachStudentToGroupVm()
            {
                GroupId = 1,
                StudentId = 7
            };

            var student = _groupService.AttachStudentToGroup(attachStudentToGroupVm);
            Assert.True(student.GroupName == "IO");

            var group = _groupService.GetGroup(g => g.Id == attachStudentToGroupVm.GroupId);
            Assert.NotNull(group);
            Assert.NotNull(group.Students.FirstOrDefault(x => x.Id == 7));
        }


    }
}