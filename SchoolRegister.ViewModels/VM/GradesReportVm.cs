using System.Collections.Generic;

namespace SchoolRegister.ViewModels.VM
{
    public class GradesReportVm
    {
        public int? Id { get; set; }
        public IList<GradeVm> Grades { get; set; }
        public int GetterUserId { get; set; }
        public string GetterUserName { get; set; }
    }
}