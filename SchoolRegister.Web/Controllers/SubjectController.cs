using System;
using System.Collections.Generic;
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
    [Authorize(Roles = "Teacher, Admin, Student, Parent")]
    public class SubjectController : BaseController
    {
        private readonly ISubjectService _subjectService;
        private readonly ITeacherService _teacherService;
        private readonly IGroupService _groupService;
        private readonly UserManager<User> _userManager;

        public SubjectController(ISubjectService subjectService, ITeacherService teacherService, 
            IGroupService groupService, UserManager<User> userManager,
            ILogger logger, IMapper mapper, IStringLocalizer localizer) : base(logger, mapper, localizer)
        {
            _subjectService = subjectService;
            _teacherService = teacherService;
            _groupService = groupService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (_userManager.IsInRoleAsync(user, "Admin").Result)
                return View(_subjectService.GetSubjects());
            
            else if (_userManager.IsInRoleAsync(user, "Teacher").Result)
            {
                if (user is Teacher teacher)
                    return View(_subjectService.GetSubjects(x => x.TeacherId == teacher.Id));
                throw new Exception("Teacher is assinged to role, but to the Teacher type.");
            }

            else if (_userManager.IsInRoleAsync(user, "Student").Result)
                return RedirectToAction("Details", "Student", new { studentId = user.Id });
            
            else if (_userManager.IsInRoleAsync(user, "Parent").Result)
                return RedirectToAction("Index", "Student");

            else 
                return View("Error");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddOrEditSubject(int? id = null)
        {
            var teachersVm = _teacherService.GetTeachers();
            ViewBag.TeachersSelectList = new SelectList(teachersVm.Select(t => new
            {
                Text = $"{t.FirstName} {t.LastName}",
                Value = t.Id
            }), "Value", "Text");

            if (id.HasValue)
            {
                var subjectVm = _subjectService.GetSubject(x => x.Id == id);
                ViewBag.ActionType = "Edit";
                return View(Mapper.Map<AddOrUpdateSubjectVm>(subjectVm));
            }

            ViewBag.ActionType = "Add";
            return View();
        }


        [Authorize(Roles = "Teacher, Admin")]
        public IActionResult Details(int id)
        {
            var subjectVm = _subjectService.GetSubject(x => x.Id == id);
            return View(subjectVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult AddOrEditSubject(AddOrUpdateSubjectVm addOrUpdateSubjectVm)
        {
            if (ModelState.IsValid)
            {
                _subjectService.AddOrUpdateSubject(addOrUpdateSubjectVm);
                return RedirectToAction("Index");
            }

            return View();
        }


        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var subjectVm = _subjectService.GetSubject(x => x.Id == id);
            return View(Mapper.Map<AddOrUpdateSubjectVm>(subjectVm));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(AddOrUpdateSubjectVm deleteSubject)
        {
            _subjectService.DeleteSubject(deleteSubject);
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AttachSubjectToGroup(int id)
        {
            var groupVms = _groupService.GetGroups(x => x.SubjectGroups.All(z => z.SubjectId != id));

            ViewBag.GroupSelectList = new SelectList(groupVms.Select(t => new 
            {
                Text = $"{t.Name}",
                Value = t.Id
            }), "Value", "Text");

            var subjectVm = _subjectService.GetSubject(x => x.Id == id);
            return View(Mapper.Map<AttachDetachSubjectGroupVm>(subjectVm));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult AttachSubjectToGroup(AttachDetachSubjectGroupVm attachDetachSubjectGroupVm)
        {
            if (ModelState.IsValid)
            {
                _groupService.AttachSubjectToGroup(attachDetachSubjectGroupVm);
                return RedirectToAction("Index");
            }
            
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DetachSubjectFromGroup(int id)
        {
            var groupVms = _groupService.GetGroups(x => x.SubjectGroups.Any(z => z.SubjectId == id));

            ViewBag.GroupSelectList = new SelectList(groupVms.Select(t => new 
            {
                Text = $"{t.Name}",
                Value = t.Id
            }), "Value", "Text");

            var subjectVm = _subjectService.GetSubject(x => x.Id == id);
            return View(Mapper.Map<AttachDetachSubjectGroupVm>(subjectVm));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DetachSubjectFromGroup(AttachDetachSubjectGroupVm attachDetachSubjectGroupVm, 
            List<int> groupCodes)
        {
            if (ModelState.IsValid)
            {
                foreach (var code in groupCodes)
                {
                    var detachSubjectFromGroupVm = new AttachDetachSubjectGroupVm()
                    {
                        GroupId = code,
                        SubjectId = attachDetachSubjectGroupVm.SubjectId
                    };

                    _groupService.DetachSubjectFromGroup(detachSubjectFromGroupVm);
                }

                return RedirectToAction("Index");
            }

            return View();
        }

    }
}