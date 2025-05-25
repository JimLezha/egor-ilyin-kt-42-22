using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using egorIlyinKT_42_22.Models;
using System.Collections.Generic;

namespace egorIlyinKT_42_22.Database.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        private const string TableName = "Departments";
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired();

            builder.Property(d => d.FoundedDate)
                .IsRequired();

            
            builder.HasMany(d => d.Teachers)
                .WithOne(t => t.Department)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Head)
                .WithOne()
                .HasForeignKey<Department>(d => d.HeadId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
