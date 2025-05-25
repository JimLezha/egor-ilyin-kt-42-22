using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using egorIlyinKT_42_22.Models;

namespace egorIlyinKT_42_22.Data.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        private const string TableName = "Teachers";
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.FirstName)
                .IsRequired();

            builder.Property(t => t.LastName)
                .IsRequired();

            builder.HasMany(t => t.Loads)
                .WithOne(l => l.Teacher)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Department)
                .WithMany(d => d.Teachers)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}