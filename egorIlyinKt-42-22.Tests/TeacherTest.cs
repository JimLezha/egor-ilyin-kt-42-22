using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using egorIlyinKT_42_22.Database;
using egorIlyinKT_42_22.Models;
using egorIlyinKT_42_22.Models.DTO;
using egorIlyinKT_42_22.Services.TeacherServices;
using System.ComponentModel.DataAnnotations;

namespace egorIlyinKT_42_22.Tests
{
    public class TeacherServiceTests
    {
        private readonly DbContextOptions<UniversityContext> _dbContextOptions;

        public TeacherServiceTests()
        {
            var dbName = $"TestDatabase_Teacher_{Guid.NewGuid()}";
            _dbContextOptions = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
        }

        [Fact]
        public async Task AddTeacherAsync_AddsTeacherSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var teacherService = new TeacherService(ctx);
                var department = new Department { Name = "Department A" };
                var degree = new Degree { Name = "PhD" };
                var position = new Position { Name = "Professor" };

                await ctx.Departments.AddAsync(department);
                await ctx.Degrees.AddAsync(degree);
                await ctx.Positions.AddAsync(position);
                await ctx.SaveChangesAsync();

                var result = await teacherService.AddTeacherAsync("John", "Doe", position.Id, degree.Id, department.Id);

                Assert.NotNull(result);
                Assert.Equal("John", result.FirstName);
                Assert.Equal("Doe", result.LastName);
                Assert.Equal("PhD", result.Degree);
                Assert.Equal("Professor", result.Position);
                Assert.Equal("Department A", result.Department);
            }
        }

        [Fact]
        public async Task UpdateTeacherAsync_UpdatesTeacherSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var teacherService = new TeacherService(ctx);
                var department = new Department { Name = "Department A" };
                var degree = new Degree { Name = "PhD" };
                var position = new Position { Name = "Professor" };

                await ctx.Departments.AddAsync(department);
                await ctx.Degrees.AddAsync(degree);
                await ctx.Positions.AddAsync(position);
                await ctx.SaveChangesAsync();

                var teacher = new Teacher
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PositionId = position.Id,
                    DegreeId = degree.Id,
                    DepartmentId = department.Id
                };

                await ctx.Teachers.AddAsync(teacher);
                await ctx.SaveChangesAsync();

                var updatedTeacher = await teacherService.UpdateTeacherAsync(teacher.Id, "Jane", "Doe", position.Id, degree.Id, department.Id);

                Assert.NotNull(updatedTeacher);
                Assert.Equal("Jane", updatedTeacher.FirstName);
                Assert.Equal("Doe", updatedTeacher.LastName);
            }
        }

        [Fact]
        public async Task DeleteTeacherAsync_DeletesTeacherSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var teacherService = new TeacherService(ctx);
                var department = new Department { Name = "Department A" };
                var degree = new Degree { Name = "PhD" };
                var position = new Position { Name = "Professor" };

                await ctx.Departments.AddAsync(department);
                await ctx.Degrees.AddAsync(degree);
                await ctx.Positions.AddAsync(position);
                await ctx.SaveChangesAsync();

                var teacher = new Teacher
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PositionId = position.Id,
                    DegreeId = degree.Id,
                    DepartmentId = department.Id
                };

                await ctx.Teachers.AddAsync(teacher);
                await ctx.SaveChangesAsync();

                var result = await teacherService.DeleteTeacherAsync(teacher.Id);
                Assert.True(result);

                var deletedTeacher = await ctx.Teachers.FindAsync(teacher.Id);
                Assert.Null(deletedTeacher);
            }
        }

        [Fact]
        public async Task GetTeachersAsync_ReturnsFilteredTeachers()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var teacherService = new TeacherService(ctx);
                var department = new Department { Name = "Department A" };
                var degree = new Degree { Name = "PhD" };
                var position = new Position { Name = "Professor" };

                await ctx.Departments.AddAsync(department);
                await ctx.Degrees.AddAsync(degree);
                await ctx.Positions.AddAsync(position);
                await ctx.SaveChangesAsync();

                var teacher1 = new Teacher
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PositionId = position.Id,
                    DegreeId = degree.Id,
                    DepartmentId = department.Id
                };

                var teacher2 = new Teacher
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    PositionId = position.Id,
                    DegreeId = degree.Id,
                    DepartmentId = department.Id
                };

                await ctx.Teachers.AddRangeAsync(teacher1, teacher2);
                await ctx.SaveChangesAsync();

                var result = await teacherService.GetTeachersAsync(departmentName: "Department A");

                Assert.Equal(2, result.Count);
                Assert.Contains(result, t => t.FirstName == "John" && t.LastName == "Doe");
                Assert.Contains(result, t => t.FirstName == "Jane" && t.LastName == "Doe");
            }
        }

        [Fact]
        public async Task GetTeacherByIdAsync_ReturnsTeacher()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var teacherService = new TeacherService(ctx);
                var department = new Department { Name = "Department A" };
                var degree = new Degree { Name = "PhD" };
                var position = new Position { Name = "Professor" };

                await ctx.Departments.AddAsync(department);
                await ctx.Degrees.AddAsync(degree);
                await ctx.Positions.AddAsync(position);
                await ctx.SaveChangesAsync();

                var teacher = new Teacher
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PositionId = position.Id,
                    DegreeId = degree.Id,
                    DepartmentId = department.Id
                };

                await ctx.Teachers.AddAsync(teacher);
                await ctx.SaveChangesAsync();

                var result = await teacherService.GetTeacherByIdAsync(teacher.Id);

                Assert.NotNull(result);
                Assert.Equal("John", result.FirstName);
                Assert.Equal("Doe", result.LastName);
            }
        }


        [Fact]
        public void Teacher_IsFirstNameCapitalized_ShouldReturnFalse_WhenFirstNameIsNotCapitalized()
        {
            var teacher = new Teacher
            {
                FirstName = "john",
                LastName = "Doe"
            };

            var isValid = teacher.IsFirstNameValid();

            Assert.False(isValid);
        }

        [Fact]
        public void Teacher_IsFirstNameCapitalized_ShouldReturnFalse_WhenFirstNameHasSpaces()
        {
            var teacher = new Teacher
            {
                FirstName = "Jo hn",
                LastName = "Doe"
            };

            var isValid = teacher.IsFirstNameValid();

            Assert.False(isValid);
        }

        [Fact]
        public void Teacher_IsFirstNameCapitalized_ShouldReturnTrue_WhenFirstNameIsCapitalized()
        {
            var teacher = new Teacher
            {
                FirstName = "John",
                LastName = "Doe"
            };

            var isValid = teacher.IsFirstNameValid();

            Assert.True(isValid);
        }

        [Fact]
        public void Teacher_IsLastNameCapitalized_ShouldReturnFalse_WhenLastNameIsNotCapitalized()
        {
            var teacher = new Teacher
            {
                FirstName = "John",
                LastName = "doe"
            };

            var isValid = teacher.IsLastNameValid();

            Assert.False(isValid);
        }
    }
}