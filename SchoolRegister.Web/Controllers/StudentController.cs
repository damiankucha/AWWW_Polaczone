using System.Linq;
using System.Security.Claims;
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
    public class StudentController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        private readonly IGradeService _gradeService;

        public StudentController(ILogger logger, IMapper mapper, IStringLocalizer localizer,
            UserManager<User> userManager, IStudentService studentService, IGroupService groupService,
            IGradeService gradeService) 
            : base(logger, mapper, localizer)
        {
            _userManager = userManager;
            _studentService = studentService;
            _groupService = groupService;
            _gradeService = gradeService;
        }

        public IActionResult Index()
        {
            var studentVms = _studentService.GetStudents();
            return View(studentVms);
        }

        [Authorize(Roles = "Admin, Student, Teacher")]
        public IActionResult Details(int id)
        {
            var studentVm = _studentService.GetStudent(x => x.Id == id);
            return View(studentVm);
        }


        // Change access - only teacher
        [Authorize(Roles = "Teacher")]
        public IActionResult AddGradeToStudent(int id)
        {
            var studentVm = _studentService.GetStudent(x => x.Id == id);

            // Get logged user id
            var teacherId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.teacherId = teacherId;

            var studentSubjects = _groupService.GetGroup(x => x.Id == studentVm.GroupId).Subjects.Where(z => z.TeacherId == int.Parse(teacherId));
            ViewBag.StudentSubjectSelectList = new SelectList(studentSubjects.Select(t => new
            {
                Text = $"{t.Name}",
                Value = t.Id
            }), "Value", "Text");

            return View(Mapper.Map<AddGradeToStudentVm>(studentVm));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public IActionResult AddGradeToStudent(AddGradeToStudentVm addGradeToStudentVm)
        {
            if (ModelState.IsValid)
            {
                _gradeService.AddGradeToStudent(addGradeToStudentVm);
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}