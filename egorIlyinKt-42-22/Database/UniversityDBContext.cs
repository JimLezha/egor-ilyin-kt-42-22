using egorIlyinKT_42_22.Models;
using Microsoft.EntityFrameworkCore;

namespace egorIlyinKT_42_22.Database
{
    public class UniversityContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Load> Loads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherConfiguration());
            modelBuilder.ApplyConfiguration(new DegreeConfiguration());
            modelBuilder.ApplyConfiguration(new PositionConfiguration());
            modelBuilder.ApplyConfiguration(new DisciplineConfiguration());
            modelBuilder.ApplyConfiguration(new LoadConfiguration());
        }

        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options)
        {
        }
    }
}
