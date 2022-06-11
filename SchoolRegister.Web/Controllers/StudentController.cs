using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SchoolRegister.Model.DataModels;
using SchoolRegister.Services.Interfaces;

namespace SchoolRegister.Web.Controllers
{
    public class StudentController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IStudentService _studentService;

        public StudentController(ILogger logger, IMapper mapper, IStringLocalizer localizer,
            UserManager<User> userManager, IStudentService studentService) 
            : base(logger, mapper, localizer)
        {
            _userManager = userManager;
            _studentService = studentService;
        }

        public IActionResult Index()
        {
            var studentVms = _studentService.GetStudents();
            return View(studentVms);
        }

        [Authorize(Roles = "Admin, Teacher")]
        public IActionResult Details(int id)
        {
            var studentVm = _studentService.GetStudent(x => x.Id == id);
            return View(studentVm);
        }
    }
}