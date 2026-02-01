using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryServices.Infrastructure.Postgres.Configurations
{
    public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
    {
        public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
        {
            builder.ToTable("departament_positions");

            builder
                .HasKey(dp => dp.Id)
                .HasName("pk_departament_position");

            builder
                .Property(dp => dp.Id)
                .IsRequired()
                .HasColumnName("id");

            builder
                .Property(dp => dp.DepartamentId)
                .IsRequired()
                .HasColumnName("departament_id")
                .HasConversion(
                    value => value.Value,
                    value => DepId.GetCurrent(value));

            builder
                .Property(dp => dp.PositionId)
                .IsRequired()
                .HasColumnName("position_id")
                .HasConversion(
                    value => value.Value,
                    value => PosId.GetCurrent(value));
        }
    }
}
