using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;
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
                .HasKey(dl => new { dl.DepartamentId, dl.LocationId })
                .HasName("pk_departament_location");

            builder
                .Property(dl => dl.DepartamentId)
                .IsRequired()
                .HasColumnName("departament_id")
                .HasConversion(
                    value => value.Value,
                    value => DepId.GetCurrent(value));

            builder
                .Property(dl => dl.LocationId)
                .IsRequired()
                .HasColumnName("location_id")
                .HasConversion(
                    value => value.Value,
                    value => LocId.GetCurrent(value));
        }
    }
}
