using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using egorIlyinKT_42_22.Models;

namespace egorIlyinKT_42_22.Database.Configurations
{
    public class LoadConfiguration : IEntityTypeConfiguration<Load>
    {
        private const string TableName = "Loads";

        public void Configure(EntityTypeBuilder<Load> builder)
        {
            builder.ToTable(TableName); 

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Hours)
                .IsRequired();

            
            builder.HasOne(l => l.Teacher)
                .WithMany(t => t.Loads) 
                .HasForeignKey(l => l.TeacherId)
                .OnDelete(DeleteBehavior.Cascade); 

            
            builder.HasOne(l => l.Discipline)
                .WithMany(d => d.Loads) 
                .HasForeignKey(l => l.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}