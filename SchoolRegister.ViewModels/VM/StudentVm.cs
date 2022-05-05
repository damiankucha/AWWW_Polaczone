using System;

namespace SchoolRegister.ViewModels.VM
{
    public class StudentVm
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
    }
}