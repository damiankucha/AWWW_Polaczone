using System;
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
            CreateMap<Group, GroupVm>()
                .ForMember(dest => dest.Students, x => x.MapFrom(src => src.Students))
                .ForMember(dest => dest.Subjects, x => x.MapFrom(src => src.SubjectGroups.Select(y => y.Subject)));
            CreateMap<SubjectVm, AddOrUpdateSubjectVm>();
            CreateMap<SubjectVm, AttachDetachSubjectGroupVm>()
                .ForMember(dest => dest.SubjectId, x => x.MapFrom(src => src.Id));


            // Student service
            CreateMap<Student, StudentVm>()
                .ForMember(dest => dest.GroupName, x => x.MapFrom(src => src.Group.Name))
                .ForMember(dest => dest.ParentName, x => x.MapFrom(src => $"{src.Parent.FirstName} {src.Parent.LastName}")).ForMember(dest => dest.Grades, x => x.MapFrom(src => src.Grades));
                

            // Teacher service
            CreateMap<Teacher, TeacherVm>();


            // Grade service
            CreateMap<Grade, GradeVm>()
                .ForMember(dest => dest.StudentName, x => x.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
                .ForMember(dest => dest.SubjectName, x => x.MapFrom(src => src.Subject.Name));

            CreateMap<AddGradeToStudentVm, Grade>(); 


            // Group service
            CreateMap<AddOrUpdateGroupVm, Group>();
            CreateMap<GroupVm, AddOrUpdateGroupVm>();
            CreateMap<GroupVm, AttachDetachStudentToGroupVm>()
                .ForMember(dest => dest.GroupId, x => x.MapFrom(src => src.Id));
            CreateMap<GroupVm, AttachDetachSubjectGroupVm>()
                .ForMember(dest => dest.GroupId, x => x.MapFrom(src => src.Id));


            // Registration
            CreateMap<RegisterNewUserVm, User>()
                .ForMember(dest => dest.UserName, y => y.MapFrom(src => src.Email))
                .ForMember(dest => dest.RegistrationDate, y => y.MapFrom(src => DateTime.Now));
            
            CreateMap<RegisterNewUserVm, Parent>()
                .ForMember(dest => dest.UserName, y => y.MapFrom(src => src.Email))
                .ForMember(dest => dest.RegistrationDate, y => y.MapFrom(src => DateTime.Now));
            
            CreateMap<RegisterNewUserVm, Student>()
                .ForMember(dest => dest.UserName, y => y.MapFrom(src => src.Email))
                .ForMember(dest => dest.RegistrationDate, y => y.MapFrom(src => DateTime.Now));
            
            CreateMap<RegisterNewUserVm, Teacher>()
                .ForMember(dest => dest.UserName, y => y.MapFrom(src => src.Email))
                .ForMember(dest => dest.RegistrationDate, y => y.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Title, y => y.MapFrom(src => src.TeacherTitles));
        }
    }
}