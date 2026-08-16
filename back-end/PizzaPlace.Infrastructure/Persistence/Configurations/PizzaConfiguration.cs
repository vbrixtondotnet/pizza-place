using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaPlace.Domain.Entities;

namespace PizzaPlace.Infrastructure.Persistence.Configurations;

public class PizzaConfiguration : IEntityTypeConfiguration<Pizza>
{
    public void Configure(EntityTypeBuilder<Pizza> builder)
    {
        builder.ToTable("pizzas");

        builder.HasKey(p => p.PizzaId);

        builder.Property(p => p.PizzaId)
            .HasColumnName("pizza_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.PizzaTypeId)
            .HasColumnName("pizza_type_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Size)
            .HasColumnName("size")
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(p => p.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.HasOne(p => p.PizzaType)
            .WithMany(pt => pt.Pizzas)
            .HasForeignKey(p => p.PizzaTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
