using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaPlace.Domain.Entities;

namespace PizzaPlace.Infrastructure.Persistence.Configurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("order_details");

        builder.HasKey(od => od.OrderDetailsId);

        builder.Property(od => od.OrderDetailsId)
            .HasColumnName("order_details_id")
            .ValueGeneratedNever();

        builder.Property(od => od.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(od => od.PizzaId)
            .HasColumnName("pizza_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(od => od.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(od => od.Pizza)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.PizzaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
