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

                nb.HasIndex(d => new { d.Value })
                    .IsUnique();
            });

            builder.OwnsOne(d => d.Identifier, ib =>
            {
                ib.Property(i => i.Value)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.LENGTH_150)
                    .HasColumnName("identifier");

                ib.HasIndex(d => new { d.Value })
                    .IsUnique();
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
                .HasColumnName("parent_id");

            builder.Property(d => d.Path)
                .IsRequired()
                .HasMaxLength(LengthConstants.LENGTH_150)
                .HasColumnName("path")
                .HasColumnType("ltree")
                .HasConversion(
                    toBase => toBase.Value,
                    toUs => DepPath.GetCurrent(toUs));

            builder.HasIndex(x => x.Path).HasMethod("gist").HasDatabaseName("idx_departaments_path");

            builder.Property(d => d.Depth)
                .IsRequired()
                .HasColumnName("depth");

            builder.Property(d => d.IsActive)
                .IsRequired()
                .HasColumnName("is_active");

            builder.Property(d => d.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");

            builder.Property(d => d.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at");

            builder.HasMany(d => d.Locations)
                .WithOne()
                .HasForeignKey(d => d.DepartamentId);

            builder.HasMany(d => d.Positions)
                .WithOne()
                .HasForeignKey(d => d.DepartamentId);
        }
    }
}
