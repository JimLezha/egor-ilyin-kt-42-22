using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using egorIlyinKT_42_22.Models;

namespace egorIlyinKT_42_22.Database.Configurations
{
    public class DegreeConfiguration : IEntityTypeConfiguration<Degree>
    {
        private const string TableName = "Degrees";
        public void Configure(EntityTypeBuilder<Degree> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired();
        }
    }
}
