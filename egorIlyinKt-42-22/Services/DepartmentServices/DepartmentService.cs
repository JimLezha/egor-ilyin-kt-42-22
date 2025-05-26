using Microsoft.EntityFrameworkCore;
using egorIlyinKT_42_22.Database;
using egorIlyinKT_42_22.Models;
using egorIlyinKT_42_22.Interfaces;
using egorIlyinKT_42_22.Models.DTO;
using egorIlyinKT_42_22.Filters.DepartmentFilters;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;

namespace egorIlyinKT_42_22.Services.DepartmentServices
{
    public class DepartmentService : IDepartmentService
    {
        private readonly UniversityContext _context;

        public DepartmentService(UniversityContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentFilter>> GetDepartmentsAsync(DateTime? foundedAfter = null, int? minTeacherCount = null)
        {
            var query = _context.Departments
                .Include(d => d.Head)
                .Include(d => d.Teachers)
                .AsQueryable();

            if (foundedAfter.HasValue)
            {
                query = query.Where(d => d.FoundedDate >= foundedAfter.Value);
            }

            if (minTeacherCount.HasValue)
            {
                query = query.Where(d => d.Teachers.Count >= minTeacherCount.Value);
            }

            var departments = await query.Select(d => new DepartmentFilter
            {
                Id = d.Id,
                Name = d.Name,
                FoundedDate = d.FoundedDate,
                Head = d.Head != null ? $"{d.Head.FirstName} {d.Head.LastName}" : "Нет заведующего",
                TeacherCount = d.Teachers.Count
            }).ToListAsync();

            return departments;
        }

        public async Task AddDepartmentAsync(Department department)
        {
            if (!department.IsValidName())
            {
                throw new ArgumentException("Department name must contain 'Department' or 'Кафедра'.");
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task<Department> UpdateDepartmentAsync(Department department)
        {
            if (!department.IsValidName())
            {
                throw new ArgumentException("Department name must contain 'Department' or 'Кафедра'.");
            }

            var existingDepartment = await _context.Departments.FindAsync(department.Id);
            if (existingDepartment == null)
            {
                return null;
            }

            existingDepartment.Name = department.Name;
            existingDepartment.FoundedDate = department.FoundedDate;
            existingDepartment.HeadId = department.HeadId;

            await _context.SaveChangesAsync();
            return existingDepartment;
        }

        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            var department = await _context.Departments
                .Include(d => d.Teachers)
                .FirstOrDefaultAsync(d => d.Id == departmentId);

            if (department == null)
            {
                return false; 
            }

            
            department.HeadId = null;
            await _context.SaveChangesAsync();

            
            _context.Teachers.RemoveRange(department.Teachers);
            await _context.SaveChangesAsync();

            
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return true;
        }




        public async Task<List<string>> Zashita1(
        string disciplineName,
        int? minHours = null,
        int? maxHours = null)
        {
            if (string.IsNullOrEmpty(disciplineName))
            {
                throw new ArgumentException("Имя дисциплины обязательно для заполнения.", nameof(disciplineName));
            }

            return await (from department in _context.Departments
                          join teacher in _context.Teachers on department.Id equals teacher.DepartmentId
                          join load in _context.Loads on teacher.Id equals load.TeacherId
                          join discipline in _context.Disciplines on load.DisciplineId equals discipline.Id
                          where discipline.Name.Contains(disciplineName) &&
                                (!minHours.HasValue || load.Hours >= minHours.Value) &&
                                (!maxHours.HasValue || load.Hours <= maxHours.Value)
                          select department.Name)
                          .Distinct() 
                          .ToListAsync();
        }


    



    }

}
