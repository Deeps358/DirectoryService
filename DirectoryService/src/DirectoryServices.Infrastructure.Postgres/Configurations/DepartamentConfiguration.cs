using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryServices.Infrastructure.Postgres.Configurations
{
    public class DepartamentConfiguration : IEntityTypeConfiguration<Departament>
    {
        public void Configure(EntityTypeBuilder<Departament> builder)
        {
            builder.ToTable("departaments");

            builder.HasKey(d => d.Id).HasName("pk_departaments");

            builder.Property(d => d.Id)
                .HasConversion(d => d.Value, id => DepId.GetCurrent(id))
                .HasColumnName("id");

            builder.OwnsOne(d => d.Name, nb =>
            {
                nb.Property(n => n.Value)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_150)
                    .HasColumnName("name");
            });

            builder.OwnsOne(d => d.Identifier, ib =>
            {
                ib.Property(i => i.Value)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_150)
                    .HasColumnName("identifier");
            });

            builder
                .HasOne(d => d.Parent)
                .WithMany(d => d.Childrens)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // родителя с детьми не удалить

            builder.Navigation(d => d.Parent).IsRequired(false);
            builder.HasIndex(d => d.ParentId);

            builder.Property(d => d.ParentId)
                .IsRequired(false)
                .HasColumnName("parentId");

            builder.OwnsOne(d => d.Path, pb =>
            {
                pb.Property(p => p.Value)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_150)
                    .HasColumnName("path");
            });

            builder.Property(d => d.Depth)
                .IsRequired()
                .HasColumnName("depth");

            builder.Property(d => d.IsActive)
                .IsRequired()
                .HasColumnName("isActive");

            builder.Property(d => d.CreatedAt)
                .IsRequired()
                .HasColumnName("createdAt");

            builder.Property(d => d.UpdatedAt)
                .IsRequired()
                .HasColumnName("updatedAt");

            builder.HasMany(d => d.DepartamentLocations)
                .WithOne()
                .HasForeignKey(d => d.DepartamentId);

            builder.HasMany(d => d.DepartamentPositions)
                .WithOne()
                .HasForeignKey(d => d.DepartamentId);
        }
    }
}
