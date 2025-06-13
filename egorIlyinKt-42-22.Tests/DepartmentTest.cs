using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using egorIlyinKT_42_22.Database;
using egorIlyinKT_42_22.Models;
using egorIlyinKT_42_22.Services.DepartmentServices;

namespace egorIlyinKT_42_22.Tests
{
    public class DepartmentServiceTests
    {
        private readonly DbContextOptions<UniversityContext> _dbContextOptions;

        public DepartmentServiceTests()
        {
            var dbName = $"TestDatabase_Department_{Guid.NewGuid()}";
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
        public async Task AddDepartmentAsync_AddsDepartmentSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var departmentService = new DepartmentService(ctx);
                var department = new Department
                {
                    Name = "New Department",
                    FoundedDate = DateTime.Now
                };

                await departmentService.AddDepartmentAsync(department);

                var result = await ctx.Departments.FindAsync(department.Id);
                Assert.NotNull(result);
                Assert.Equal("New Department", result.Name);
            }
        }

        [Fact]
        public async Task UpdateDepartmentAsync_UpdatesDepartmentSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var departmentService = new DepartmentService(ctx);
                var department = new Department
                {
                    Name = "Old Department",
                    FoundedDate = DateTime.Now
                };
                var teacher1 = new Teacher { FirstName = "John", LastName = "Doe", Department = department };

                await ctx.Departments.AddAsync(department);
                await ctx.SaveChangesAsync();

                department.Name = "Updated Department";
                department.HeadId = 1;
                var updatedDepartment = await departmentService.UpdateDepartmentAsync(department);

                Assert.NotNull(updatedDepartment);
                Assert.Equal("Updated Department", updatedDepartment.Name);
                Assert.Equal(1, updatedDepartment.HeadId);
                Assert.NotNull(updatedDepartment.HeadId);
            }
        }

        [Fact]
        public async Task DeleteDepartmentAsync_DeletesDepartmentSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var departmentService = new DepartmentService(ctx);
                var department = new Department
                {
                    Name = "Department to Delete",
                    FoundedDate = DateTime.Now
                };

                await ctx.Departments.AddAsync(department);
                await ctx.SaveChangesAsync();

                var result = await departmentService.DeleteDepartmentAsync(department.Id);
                Assert.True(result);

                var deletedDepartment = await ctx.Departments.FindAsync(department.Id);
                Assert.Null(deletedDepartment);
            }
        }

        [Fact]
        public async Task GetDepartmentsAsync_FoundedAfter_ReturnsFilteredDepartments()
        {

            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var departmentService = new DepartmentService(ctx);

                var departments = new List<Department>
        {
            new Department
            {
                Name = "Department A",
                FoundedDate = new DateTime(2020, 1, 1)
            },
            new Department
            {
                Name = "Department B",
                FoundedDate = new DateTime(2021, 1, 1)
            },
            new Department
            {
                Name = "Department C",
                FoundedDate = new DateTime(2019, 1, 1)
            }
        };

                await ctx.Departments.AddRangeAsync(departments);
                await ctx.SaveChangesAsync();
            }


            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var departmentService = new DepartmentService(ctx);
                var foundedAfter = new DateTime(2020, 1, 1);
                var result = await departmentService.GetDepartmentsAsync(foundedAfter, null);


                Assert.Equal(2, result.Count);
                Assert.Contains(result, d => d.Name == "Department A");
                Assert.Contains(result, d => d.Name == "Department B");
                Assert.DoesNotContain(result, d => d.Name == "Department C");
            }
        }

        [Fact]
        public async Task GetDepartmentsAsync_MinTeacherCount_ReturnsFilteredDepartments()
        {

            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var departmentService = new DepartmentService(ctx);

                var degree = new Degree { Name = "PhD" };
                var position = new Position { Name = "Professor" };

                await ctx.Degrees.AddAsync(degree);
                await ctx.Positions.AddAsync(position);
                await ctx.SaveChangesAsync();

                var departmentWithTeachers = new Department
                {
                    Name = "Department A",
                    FoundedDate = DateTime.Now,
                    Teachers = new List<Teacher>
                    {
                        new Teacher
                        {
                            FirstName = "John",
                            LastName = "Doe",
                            DegreeId = degree.Id,
                            PositionId = position.Id,
                            DepartmentId = 1
                        },
                        new Teacher
                        {
                            FirstName = "Jane",
                            LastName = "Doe",
                            DegreeId = degree.Id,
                            PositionId = position.Id,
                            DepartmentId = 1
                        }
                    }
                };

                var departmentWithoutTeachers = new Department
                {
                    Name = "Department B",
                    FoundedDate = DateTime.Now
                };

                await ctx.Departments.AddRangeAsync(new[] { departmentWithTeachers, departmentWithoutTeachers });
                await ctx.SaveChangesAsync();
            }


            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var departmentService = new DepartmentService(ctx);
                var result = await departmentService.GetDepartmentsAsync(null, 2);


                Assert.Single(result);
                Assert.Equal("Department A", result.First().Name);
            }
        }


        [Fact]
        public void Department_IsValidName_ShouldReturnFalse_WhenNameDoesNotStartWithDepartmentOrCafedra()
        {
            var department = new Department
            {
                Name = "New Dep"
            };

            var isValid = department.IsValidName();

            Assert.False(isValid);
        }

        [Fact]
        public void Department_IsValidName_ShouldReturnTrue_WhenNameStartsWithDepartment()
        {
            var department = new Department
            {
                Name = "Department of Mathematics" 
            };

            var isValid = department.IsValidName();

            Assert.True(isValid);
        }

        [Fact]
        public void Department_IsValidName_ShouldReturnTrue_WhenNameStartsWithCafedra()
        {
            var department = new Department
            {
                Name = "Кафедра математики" 
            };

            var isValid = department.IsValidName();

            Assert.True(isValid);
        }



    }
}