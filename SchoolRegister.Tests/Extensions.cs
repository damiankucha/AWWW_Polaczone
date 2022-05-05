using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SchoolRegister.DAL.EF;
using SchoolRegister.Model.DataModels;

namespace SchoolRegister.Tests
{
    public static class Extensions
    {
        // Create sample data
        public static async void SeedData(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            // Roles
            var teacherRole = new Role()
            {
                Id = 3,
                Name = "Teacher",
                RoleValue = RoleValue.Teacher
            };
            await roleManager.CreateAsync(teacherRole);

            var studentRole = new Role()
            {
                Id = 1,
                Name = "Student",
                RoleValue = RoleValue.Student
            };
            await roleManager.CreateAsync(studentRole);

            var parentRole = new Role()
            {
                Id = 2,
                Name = "Parent",
                RoleValue = RoleValue.Parent
            };
            await roleManager.CreateAsync(parentRole);

            var adminRole = new Role()
            {
                Id = 4,
                Name = "Admin",
                RoleValue = RoleValue.Admin
            };
            await roleManager.CreateAsync(adminRole);

        }
    }
}