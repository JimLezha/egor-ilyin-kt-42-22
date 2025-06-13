using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using egorIlyinKT_42_22.Database;
using egorIlyinKT_42_22.Models;
using egorIlyinKT_42_22.Models.DTO;
using egorIlyinKT_42_22.Services.DisciplineServices;
using egorIlyinKT_42_22.Services.DepartmentServices;

namespace egorIlyinKT_42_22.Tests
{
    public class DisciplineServiceTests
    {
        private readonly DbContextOptions<UniversityContext> _dbContextOptions;

        public DisciplineServiceTests()
        {
            var dbName = $"TestDatabase_Discipline_{Guid.NewGuid()}"; 
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
        public async Task AddDisciplineAsync_AddsDisciplineSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var disciplineService = new DisciplineService(ctx);
                var disciplineDto = new DisciplineDto { Name = "New Discipline" };

                var result = await disciplineService.AddDisciplineAsync(disciplineDto);

                var addedDiscipline = await ctx.Disciplines.FindAsync(result.Id);
                Assert.NotNull(addedDiscipline);
                Assert.Equal("New Discipline", addedDiscipline.Name);
            }
        }

        [Fact]
        public async Task UpdateDisciplineAsync_UpdatesDisciplineSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var disciplineService = new DisciplineService(ctx);
                var disciplineDto = new DisciplineDto { Name = "Old Discipline" };

                var addedDiscipline = await disciplineService.AddDisciplineAsync(disciplineDto);
                var updateDto = new DisciplineDto { Name = "Updated Discipline" };

                var updatedDiscipline = await disciplineService.UpdateDisciplineAsync(addedDiscipline.Id, updateDto);

                Assert.NotNull(updatedDiscipline);
                Assert.Equal("Updated Discipline", updatedDiscipline.Name);
            }
        }

        [Fact]
        public async Task DeleteDisciplineAsync_DeletesDisciplineSuccessfully()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var disciplineService = new DisciplineService(ctx);
                var disciplineDto = new DisciplineDto { Name = "Discipline to Delete" };

                var addedDiscipline = await disciplineService.AddDisciplineAsync(disciplineDto);
                var result = await disciplineService.DeleteDisciplineAsync(addedDiscipline.Id);

                Assert.True(result);
                var deletedDiscipline = await ctx.Disciplines.FindAsync(addedDiscipline.Id);
                Assert.Null(deletedDiscipline);
            }
        }

        [Fact]
        public async Task GetDisciplinesAsync_ReturnsFilteredDisciplines()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var disciplineService = new DisciplineService(ctx);

                
                var department = new Department { Name = "Department of Science" };
                await ctx.Departments.AddAsync(department);
                await ctx.SaveChangesAsync();

                var discipline1 = new Discipline { Name = "Mathematics" };
                var discipline2 = new Discipline { Name = "Physics" };

                
                var teacher1 = new Teacher { FirstName = "John", LastName = "Doe", Department = department };
                var teacher2 = new Teacher { FirstName = "Jane", LastName = "Doe", Department = department };

                var load1 = new Load { Teacher = teacher1, Discipline = discipline1, Hours = 10 };
                var load2 = new Load { Teacher = teacher2, Discipline = discipline2, Hours = 20 };

                await ctx.Disciplines.AddRangeAsync(new[] { discipline1, discipline2 });
                await ctx.Teachers.AddRangeAsync(new[] { teacher1, teacher2 });
                await ctx.Loads.AddRangeAsync(new[] { load1, load2 });
                await ctx.SaveChangesAsync();
            }

            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var disciplineService = new DisciplineService(ctx);
                var result = await disciplineService.GetDisciplinesAsync("John");

                Assert.Single(result);
                Assert.Equal("Mathematics", result.First().Name);
            }
        }

        [Fact]
        public async Task GetDisciplineByIdAsync_ReturnsDiscipline()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var disciplineService = new DisciplineService(ctx);
                var disciplineDto = new DisciplineDto { Name = "Chemistry" };

                var addedDiscipline = await disciplineService.AddDisciplineAsync(disciplineDto);
                var result = await disciplineService.GetDisciplineByIdAsync(addedDiscipline.Id);

                Assert.NotNull(result);
                Assert.Equal("Chemistry", result.Name);
            }
        }


        [Fact]
        public async Task GetDisciplinesByHeadLastNameAsync_ReturnsDisciplinesWithHeadLastName()
        {
            using (var ctx = new UniversityContext(_dbContextOptions))
            {
                var disciplineService = new DisciplineService(ctx);

                // Создаем кафедры
                var department1 = new Department { Id = 1, Name = "Department A", HeadId = 1 };
                var department2 = new Department { Id = 2, Name = "Department B", HeadId = 2 };

                // Преподаватели включая заведующих (с Id соответствующими HeadId)
                var head1 = new Teacher { Id = 1, FirstName = "John", LastName = "Smith", DepartmentId = department1.Id };
                var head2 = new Teacher { Id = 2, FirstName = "Jane", LastName = "Johnson", DepartmentId = department2.Id };

                // Преподаватели, которые ведут дисциплины (можно использовать заведующих или других)
                var teacher1 = new Teacher { Id = 3, FirstName = "Mark", LastName = "Brown", DepartmentId = department1.Id };
                var teacher2 = new Teacher { Id = 4, FirstName = "Lucy", LastName = "Davis", DepartmentId = department1.Id };
                var teacher3 = new Teacher { Id = 5, FirstName = "Paul", LastName = "Wilson", DepartmentId = department2.Id };

                // Дисциплины
                var discipline1 = new Discipline { Id = 1, Name = "Mathematics" };
                var discipline2 = new Discipline { Id = 2, Name = "Physics" };
                var discipline3 = new Discipline { Id = 3, Name = "Chemistry" };

                // Нагрузки (связь преподавателей и дисциплин)
                var load1 = new Load { TeacherId = teacher1.Id, DisciplineId = discipline1.Id, Hours = 10 };
                var load2 = new Load { TeacherId = teacher2.Id, DisciplineId = discipline2.Id, Hours = 15 };
                var load3 = new Load { TeacherId = teacher3.Id, DisciplineId = discipline3.Id, Hours = 20 };

                // Добавляем данные в контекст
                await ctx.Departments.AddRangeAsync(department1, department2);
                await ctx.Teachers.AddRangeAsync(head1, head2, teacher1, teacher2, teacher3);
                await ctx.Disciplines.AddRangeAsync(discipline1, discipline2, discipline3);
                await ctx.Loads.AddRangeAsync(load1, load2, load3);
                await ctx.SaveChangesAsync();

                // Act — вызываем метод вашего DisciplineService
                var result = await disciplineService.GetDisciplinesByHeadLastNameAsync("Smith");

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count); // Ожидаем 2 дисциплины под заведующим Smith
                Assert.Contains(result, d => d.Name == "Mathematics");
                Assert.Contains(result, d => d.Name == "Physics");
                Assert.DoesNotContain(result, d => d.Name == "Chemistry");
            }
        }


    }
}