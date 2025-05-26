using Microsoft.EntityFrameworkCore;
using egorIlyinKT_42_22.Interfaces;
using egorIlyinKT_42_22.Database;
using egorIlyinKT_42_22.Filters.LoadFilters;
using egorIlyinKT_42_22.Models.DTO;
using egorIlyinKT_42_22.Models;



namespace egorIlyinKT_42_22.Services.LoadServices
{
    public class LoadService : ILoadService
    {
        private readonly UniversityContext _context;

        public LoadService(UniversityContext context)
        {
            _context = context;
        }

        public async Task<List<LoadFilter>> GetLoadsAsync(string teacherFirstName = null, string teacherLastName = null, string departmentName = null, string disciplineName = null)
        {
            var query = _context.Loads
                .Include(l => l.Teacher)
                .ThenInclude(t => t.Department)
                .Include(l => l.Discipline)
                .AsQueryable();

            if (!string.IsNullOrEmpty(teacherFirstName))
            {
                query = query.Where(l => l.Teacher.FirstName.Contains(teacherFirstName));
            }

            if (!string.IsNullOrEmpty(teacherLastName))
            {
                query = query.Where(l => l.Teacher.LastName.Contains(teacherLastName));
            }

            if (!string.IsNullOrEmpty(departmentName))
            {
                query = query.Where(l => l.Teacher.Department.Name.Contains(departmentName));
            }

            if (!string.IsNullOrEmpty(disciplineName))
            {
                query = query.Where(l => l.Discipline.Name.Contains(disciplineName));
            }

            var loads = await query.Select(l => new LoadFilter
            {
                Id = l.Id,
                TeacherName = $"{l.Teacher.FirstName} {l.Teacher.LastName}",
                DepartmentName = l.Teacher.Department.Name,
                DisciplineName = l.Discipline.Name,
                Hours = l.Hours
            }).ToListAsync();

            return loads;
        }


        public async Task<LoadFilter> AddLoadAsync(int teacherId, int disciplineId, int hours)
        {
            var load = new Load
            {
                TeacherId = teacherId,
                DisciplineId = disciplineId,
                Hours = hours
            };

            _context.Loads.Add(load);
            await _context.SaveChangesAsync();

            return await GetLoadFilterAsync(load.Id);
        }

        public async Task<LoadFilter> UpdateLoadAsync(int loadId, int teacherId, int disciplineId, int hours)
        {
            var load = await _context.Loads.FindAsync(loadId);
            if (load == null)
            {
                throw new KeyNotFoundException("Load not found");
            }

            load.TeacherId = teacherId;
            load.DisciplineId = disciplineId;
            load.Hours = hours;

            _context.Loads.Update(load);
            await _context.SaveChangesAsync();

            return await GetLoadFilterAsync(load.Id);
        }

        private async Task<LoadFilter> GetLoadFilterAsync(int loadId)
        {
            return await _context.Loads
                .Include(l => l.Teacher)
                .ThenInclude(t => t.Department)
                .Include(l => l.Discipline)
                .Where(l => l.Id == loadId)
                .Select(l => new LoadFilter
                {
                    Id = l.Id,
                    TeacherName = $"{l.Teacher.FirstName} {l.Teacher.LastName}",
                    DepartmentName = l.Teacher.Department.Name,
                    DisciplineName = l.Discipline.Name,
                    Hours = l.Hours
                })
                .FirstOrDefaultAsync();
        }


    }
}