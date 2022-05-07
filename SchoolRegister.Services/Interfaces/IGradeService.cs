using System.Threading.Tasks;
using SchoolRegister.ViewModels.VM;

namespace SchoolRegister.Services.Interfaces
{
    public interface IGradeService
    {
        Task<GradeVm> AddGradeToStudent(AddGradeToStudentVm addGradeToStudentVm);
        GradesReportVm GetGradesReportForStudent(GetGradesReportVm getGradesReportVm);
    }
}