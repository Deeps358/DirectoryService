using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryServices.Infrastructure.Postgres.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("locations");

            builder.HasKey(l => l.Id).HasName("pk_locations");

            builder.Property(l => l.Id)
                .HasConversion(l => l.Value, id => LocId.GetCurrent(id))
                .HasColumnName("id");

            builder.OwnsOne(l => l.Name, nb =>
            {
                nb.Property(l => l.Value)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_150)
                    .HasColumnName("name");

                nb.HasIndex(l => new { l.Value })
                    .IsUnique();
            });

            builder.OwnsOne(l => l.Adress, ab =>
            {
                ab.ToJson("adress");

                ab.Property(l => l.City)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_100)
                    .HasColumnName("city");

                ab.Property(l => l.Street)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_100)
                    .HasColumnName("street");

                ab.Property(l => l.Building)
                    .IsRequired()
                    .HasColumnName("building");

                ab.Property(l => l.Room)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_100)
                    .HasColumnName("room");

                // здесь индекс на уникальный адрес не сработал. это надо делать в миграции
            });

            builder.OwnsOne(l => l.Timezone, tb =>
            {
                tb.Property(l => l.Value)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_100)
                    .HasColumnName("timezone");
            });

            builder.Property(l => l.IsActive)
                .IsRequired()
                .HasColumnName("is_active");

            builder.Property(l => l.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");

            builder.Property(l => l.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at");

            builder.HasMany(l => l.DepartmentLocations)
                .WithOne()
                .HasForeignKey(l => l.LocationId);
        }
    }
}
