using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaPlace.Domain.Entities;

namespace PizzaPlace.Infrastructure.Persistence.Configurations;

public class PizzaTypeConfiguration : IEntityTypeConfiguration<PizzaType>
{
    public void Configure(EntityTypeBuilder<PizzaType> builder)
    {
        builder.ToTable("pizza_types");

        builder.HasKey(pt => pt.PizzaTypeId);

        builder.Property(pt => pt.PizzaTypeId)
            .HasColumnName("pizza_type_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pt => pt.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pt => pt.Category)
            .HasColumnName("category")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pt => pt.Ingredients)
            .HasColumnName("ingredients")
            .HasMaxLength(1000)
            .IsRequired();
    }
}
