using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using egorIlyinKT_42_22.Database;
using egorIlyinKT_42_22.Models;
using egorIlyinKT_42_22.Models.DTO;
using egorIlyinKT_42_22.Services.LoadServices;

namespace egorIlyinKT_42_22.Tests
{
    public class LoadServiceTests
    {
        private readonly DbContextOptions<UniversityContext> _dbContextOptions;

        public LoadServiceTests()
        {
            var dbName = $"TestDatabase_Load_{Guid.NewGuid()}";
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
        public async Task AddLoadAsync_AddsLoadSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var loadService = new LoadService(ctx);
                var department = new Department { Name = "Department A" };
                var teacher = new Teacher { FirstName = "John", LastName = "Doe", Department = department };
                var discipline = new Discipline { Name = "Mathematics" };

                await ctx.Departments.AddAsync(department);
                await ctx.Teachers.AddAsync(teacher);
                await ctx.Disciplines.AddAsync(discipline);
                await ctx.SaveChangesAsync();

                var result = await loadService.AddLoadAsync(teacher.Id, discipline.Id, 10);

                Assert.NotNull(result);
                Assert.Equal(10, result.Hours);
                Assert.Equal("John Doe", result.TeacherName);
                Assert.Equal("Department A", result.DepartmentName);
                Assert.Equal("Mathematics", result.DisciplineName);
            }
        }

        [Fact]
        public async Task UpdateLoadAsync_UpdatesLoadSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var loadService = new LoadService(ctx);
                var department = new Department { Name = "Department A" };
                var teacher = new Teacher { FirstName = "John", LastName = "Doe", Department = department };
                var discipline = new Discipline { Name = "Mathematics" };

                await ctx.Departments.AddAsync(department);
                await ctx.Teachers.AddAsync(teacher);
                await ctx.Disciplines.AddAsync(discipline);
                await ctx.SaveChangesAsync();

                var load = new Load { TeacherId = teacher.Id, DisciplineId = discipline.Id, Hours = 10 };
                await ctx.Loads.AddAsync(load);
                await ctx.SaveChangesAsync();

                var updatedLoad = await loadService.UpdateLoadAsync(load.Id, teacher.Id, discipline.Id, 20);

                Assert.NotNull(updatedLoad);
                Assert.Equal(20, updatedLoad.Hours);
            }
        }

        [Fact]
        public async Task GetLoadsAsync_ReturnsFilteredLoads()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var loadService = new LoadService(ctx);
                var department = new Department { Name = "Department A" };
                var teacher1 = new Teacher { FirstName = "John", LastName = "Doe", Department = department };
                var teacher2 = new Teacher { FirstName = "Jane", LastName = "Doe", Department = department };
                var discipline1 = new Discipline { Name = "Mathematics" };
                var discipline2 = new Discipline { Name = "Physics" };

                await ctx.Departments.AddAsync(department);
                await ctx.Teachers.AddRangeAsync(teacher1, teacher2);
                await ctx.Disciplines.AddRangeAsync(discipline1, discipline2);
                await ctx.SaveChangesAsync();

                await ctx.Loads.AddRangeAsync(
                    new Load { TeacherId = teacher1.Id, DisciplineId = discipline1.Id, Hours = 10 },
                    new Load { TeacherId = teacher2.Id, DisciplineId = discipline2.Id, Hours = 20 }
                );
                await ctx.SaveChangesAsync();

                var result = await loadService.GetLoadsAsync(teacherFirstName: "John");

                Assert.Single(result);
                Assert.Equal("Mathematics", result.First().DisciplineName);
            }
        }

        [Fact]
        public async Task UpdateLoadAsync_ThrowsKeyNotFoundException_WhenLoadDoesNotExist()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var loadService = new LoadService(ctx);
                var department = new Department { Name = "Department A" };
                var teacher = new Teacher { FirstName = "John", LastName = "Doe", Department = department };
                var discipline = new Discipline { Name = "Mathematics" };

                await ctx.Departments.AddAsync(department);
                await ctx.Teachers.AddAsync(teacher);
                await ctx.Disciplines.AddAsync(discipline);
                await ctx.SaveChangesAsync();

                await Assert.ThrowsAsync<KeyNotFoundException>(() => loadService.UpdateLoadAsync(999, teacher.Id, discipline.Id, 10));
            }
        }
    }
}