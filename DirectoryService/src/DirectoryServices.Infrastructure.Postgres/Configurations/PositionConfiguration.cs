using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryServices.Infrastructure.Postgres.Configurations
{

    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("positions");

            builder.HasKey(p => p.Id).HasName("pk_positions");

            builder.Property(p => p.Id)
                .HasConversion(p => p.Value, id => PosId.GetCurrent(id))
                .HasColumnName("id");

            builder.OwnsOne(p => p.Name, nb =>
            {
                nb.Property(p => p.Value)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_150)
                    .HasColumnName("name");
            });

            builder.OwnsOne(p => p.Description, nb =>
            {
                nb.Property(p => p.Value)
                    .HasMaxLength(LengthConstants.LENGTH_1000)
                    .HasColumnName("description");
            });

            builder.Navigation(p => p.Description).IsRequired(false);

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasColumnName("isActive");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnName("createdAt");

            builder.Property(p => p.UpdatedAt)
                .IsRequired()
                .HasColumnName("updatedAt");
        }
    }
}
