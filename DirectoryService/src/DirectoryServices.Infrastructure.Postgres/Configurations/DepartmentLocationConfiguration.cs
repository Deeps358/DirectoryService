using DirectoryServices.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryServices.Infrastructure.Postgres.Configurations
{
    public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
    {
        public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
        {
            builder.ToTable("departament_locations");

            builder
                .HasKey(dl => dl.Id)
                .HasName("pk_departament_location");

            builder
                .Property(dl => dl.Id)
                .HasColumnName("id");

            builder
                .HasOne<Departament>()
                .WithMany(d => d.Locations)
                .HasForeignKey(d => d.DepartamentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne<Location>()
                .WithMany()
                .HasForeignKey(l => l.LocationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
