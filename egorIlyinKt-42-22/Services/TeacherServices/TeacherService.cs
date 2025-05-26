using Microsoft.EntityFrameworkCore;
using egorIlyinKT_42_22.Database;
using egorIlyinKT_42_22.Filters.TeacherFilters;
using egorIlyinKT_42_22.Interfaces;
using egorIlyinKT_42_22.Models;
using egorIlyinKT_42_22.Models.DTO;


namespace egorIlyinKT_42_22.Services.TeacherServices
{

    public class TeacherService : ITeacherService
    {
        private readonly UniversityContext _context;

        public TeacherService(UniversityContext context)
        {
            _context = context;
        }

        public async Task<List<TeacherFilter>> GetTeachersAsync(string departmentName = null, string degreeName = null, string positionName = null)
        {
            var query = _context.Teachers
                .Include(t => t.Degree)
                .Include(t => t.Position)
                .Include(t => t.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(departmentName))
            {
                query = query.Where(t => t.Department.Name == departmentName);
            }

            if (!string.IsNullOrEmpty(degreeName))
            {
                query = query.Where(t => t.Degree.Name == degreeName);
            }

            if (!string.IsNullOrEmpty(positionName))
            {
                query = query.Where(t => t.Position.Name == positionName);
            }

            var teachers = await query.Select(t => new TeacherFilter
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Degree = t.Degree.Name,
                Position = t.Position.Name,
                Department = t.Department.Name
            }).ToListAsync();

            return teachers;
        }

        public async Task<TeacherDto> GetTeacherByIdAsync(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Degree)
                .Include(t => t.Position)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
            {
                return null;
            }

            
            return new TeacherDto
            {
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                PositionId = teacher.PositionId,
                DegreeId = teacher.DegreeId,
                DepartmentId = teacher.DepartmentId
            };
        }

        public async Task<TeacherResponseDto> AddTeacherAsync(string firstName, string lastName, int positionId, int degreeId, int? departmentId)
        {

            

            var positionExists = await _context.Positions.AnyAsync(p => p.Id == positionId);
            if (!positionExists)
            {
                throw new ArgumentException($"Должности с id {positionId} не существует.");
            }

            var degreeExists = await _context.Degrees.AnyAsync(d => d.Id == degreeId);
            if (!degreeExists)
            {
                throw new ArgumentException($"Степени с id {degreeId} не существует.");
            }

            if (departmentId.HasValue)
            {
                var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId.Value);
                if (!departmentExists)
                {
                    throw new ArgumentException($"Кафедры с id {departmentId} не существует.");
                }
            }

            
            var teacher = new Teacher
            {
                FirstName = firstName,
                LastName = lastName,
                PositionId = positionId,
                DegreeId = degreeId,
                DepartmentId = departmentId
            };

            if (!teacher.IsValid())
            {
                throw new ArgumentException("Имя и фамилия преподавателя должны начинаться с заглавной буквы.");
            }

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            
            var degree = await _context.Degrees.FindAsync(degreeId);
            var position = await _context.Positions.FindAsync(positionId);
            var department = departmentId.HasValue ? await _context.Departments.FindAsync(departmentId.Value) : null;

            
            var responseDto = new TeacherResponseDto
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                DegreeId = teacher.DegreeId,
                Degree = degree.Name,
                PositionId = teacher.PositionId,
                Position = position.Name, 
                DepartmentId = teacher.DepartmentId,
                Department = department.Name, 
                Loads = new List<Load>() 
            };

            return responseDto;
        }

        public async Task<TeacherResponseDto> UpdateTeacherAsync(int id, string firstName, string lastName, int positionId, int degreeId, int? departmentId)
        {

            
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return null; 
            }

            
            var positionExists = await _context.Positions.AnyAsync(p => p.Id == positionId);
            if (!positionExists)
            {
                throw new ArgumentException($"Должности с id {positionId} не существует.");
            }

            
            var degreeExists = await _context.Degrees.AnyAsync(d => d.Id == degreeId);
            if (!degreeExists)
            {
                throw new ArgumentException($"Степени с id {degreeId} не существует.");
            }

            
            if (departmentId.HasValue)
            {
                var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId.Value);
                if (!departmentExists)
                {
                    throw new ArgumentException($"Кафедры с id {departmentId} не существует.");
                }
            }

            
            teacher.FirstName = firstName;
            teacher.LastName = lastName;
            teacher.PositionId = positionId;
            teacher.DegreeId = degreeId;
            teacher.DepartmentId = departmentId;

            if (!teacher.IsValid())
            {
                throw new ArgumentException("Имя и фамилия преподавателя должны начинаться с заглавной буквы.");
            }

            await _context.SaveChangesAsync();

            
            var degree = await _context.Degrees.FindAsync(degreeId);
            var position = await _context.Positions.FindAsync(positionId);
            var department = departmentId.HasValue ? await _context.Departments.FindAsync(departmentId.Value) : null;

            
            var responseDto = new TeacherResponseDto
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                DegreeId = teacher.DegreeId,
                Degree = degree.Name,
                PositionId = teacher.PositionId,
                Position = position.Name,
                DepartmentId = teacher.DepartmentId,
                Department = department.Name,
                Loads = new List<Load>()
            };

            return responseDto;
        }


        public async Task<bool> DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }


}
