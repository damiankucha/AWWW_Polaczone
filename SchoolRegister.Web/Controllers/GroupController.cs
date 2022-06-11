using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SchoolRegister.Model.DataModels;
using SchoolRegister.Services.Interfaces;
using SchoolRegister.ViewModels.VM;

namespace SchoolRegister.Web.Controllers
{
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly IStudentService _studentService;
        private readonly ISubjectService _subjectService;
        private readonly UserManager<User> _userManager;

        public GroupController(IGroupService groupService, UserManager<User> userManager,
            IStudentService studentService, ISubjectService subjectService,
            ILogger logger, IMapper mapper, IStringLocalizer localizer) : base(logger, mapper, localizer)
        {
            _groupService = groupService;
            _studentService = studentService;
            _subjectService = subjectService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var groupVms = _groupService.GetGroups();
            return View(groupVms);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddOrEditGroup(int? id = null)
        {
            if (id.HasValue)
            {
                var groupVm = _groupService.GetGroup(x => x.Id == id);
                ViewBag.ActionType = "Edit";
                return View(Mapper.Map<AddOrUpdateGroupVm>(groupVm));
            }

            ViewBag.ActionType = "Add";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult AddOrEditGroup(AddOrUpdateGroupVm addOrUpdateGroupVm)
        {
            if (ModelState.IsValid)
            {
                _groupService.AddOrUpdateGroup(addOrUpdateGroupVm);
                return RedirectToAction("Index");
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _groupService.RemoveGroup(x => x.Id == id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Teacher, Admin")]
        public IActionResult Details(int id)
        {
            var groupVm = _groupService.GetGroup(x => x.Id == id);
            return View(groupVm);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AttachStudentToGroup(int id)
        {
            var studentVms = _studentService.GetStudents(x => x.GroupId == null);
            ViewBag.StudentSelectList = new SelectList(studentVms.Select(t => new
            {
                Text = $"{t.FirstName} {t.LastName}",
                Value = t.Id
            }), "Value", "Text");

            var groupVm = _groupService.GetGroup(x => x.Id == id);
            return View(Mapper.Map<AttachDetachStudentToGroupVm>(groupVm));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult AttachStudentToGroup(AttachDetachStudentToGroupVm attachDetachStudentToGroupVm)
        {
            if (ModelState.IsValid)
            {
                _groupService.AttachStudentToGroup(attachDetachStudentToGroupVm);
                return RedirectToAction("Details", new { id = attachDetachStudentToGroupVm.GroupId });
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DetachStudentFromGroup(int id, int stdId)
        {
            var detachStudentFromGroupVm = new AttachDetachStudentToGroupVm()
            {
                GroupId = id,
                StudentId = stdId
            };
            _groupService.DetachStudentFromGroup(detachStudentFromGroupVm);

            return RedirectToAction("Details", new { id = id });
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AttachSubjectToGroup(int id)
        {
            var subjectVms = _subjectService.GetSubjects(x => x.SubjectGroups.All(z => z.GroupId != id));

            ViewBag.SubjectSelectList = new SelectList(subjectVms.Select(t => new
            {
                Text = $"{t.Name}",
                Value = t.Id
            }), "Value", "Text");

            var groupVm = _groupService.GetGroup(x => x.Id == id);
            return View(Mapper.Map<AttachDetachSubjectGroupVm>(groupVm));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult AttachSubjectToGroup(AttachDetachSubjectGroupVm attachDetachSubjectGroupVm)
        {
            if (ModelState.IsValid)
            {
                _groupService.AttachSubjectToGroup(attachDetachSubjectGroupVm);
                return RedirectToAction("Details", new { id = attachDetachSubjectGroupVm.GroupId });
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DetachSubjectFromGroup(int id, int subId)
        {
            var detachSubjectFromGroupVm = new AttachDetachSubjectGroupVm()
            {
                GroupId = id,
                SubjectId = subId
            };
            _groupService.DetachSubjectFromGroup(detachSubjectFromGroupVm);

            return RedirectToAction("Details", new { id = id });
        }
    }
}