using System.Linq;
using AutoMapper;
using SchoolRegister.Model.DataModels;
using SchoolRegister.ViewModels.VM;

namespace SchoolRegister.Services.Configuration.AutoMapperProfiles
{
    public class MainProfile : Profile
    {
        //AutoMapper maps

        public MainProfile()
        {
            // Subject service
            CreateMap<Subject, SubjectVm>() // map from Subject(src) to SubjectVm(dst)
                // custom mapping: FirstName and LastName concat string to TeacherName
                .ForMember(dest => dest.TeacherName, x => x.MapFrom(src => $"{src.Teacher.FirstName} {src.Teacher.LastName}"))
                // custom mapping: IList<Group> to IList<GroupVm>
                .ForMember(dest => dest.Groups, x => x.MapFrom(src => src.SubjectGroups.Select(y => y.Group)));

            CreateMap<AddOrUpdateSubjectVm, Subject>();
            CreateMap<Group, GroupVm>();
            CreateMap<SubjectVm, AddOrUpdateSubjectVm>();


            // Student service
            CreateMap<Student, StudentVm>()
                .ForMember(dest => dest.GroupName, x => x.MapFrom(src => src.Group.Name))
                .ForMember(dest => dest.ParentName, x => x.MapFrom(src => $"{src.Parent.FirstName} {src.Parent.LastName}"));
                

            // Teacher service
            CreateMap<Teacher, TeacherVm>();            
        }
    }
}