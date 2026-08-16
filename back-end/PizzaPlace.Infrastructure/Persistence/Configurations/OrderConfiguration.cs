using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaPlace.Domain.Entities;

namespace PizzaPlace.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.OrderId);

        builder.Property(o => o.OrderId)
            .HasColumnName("order_id")
            .ValueGeneratedNever();

        builder.Property(o => o.Date)
            .HasColumnName("date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(o => o.Time)
            .HasColumnName("time")
            .HasColumnType("time")
            .IsRequired();
    }
}
