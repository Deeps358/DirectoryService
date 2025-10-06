using DirectoryServices.Entities;
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
                .HasColumnName("id");

            builder
                .HasOne<Departament>()
                .WithMany()
                .HasForeignKey(d => d.DepartamentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne<Position>()
                .WithMany()
                .HasForeignKey(p => p.PositionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
