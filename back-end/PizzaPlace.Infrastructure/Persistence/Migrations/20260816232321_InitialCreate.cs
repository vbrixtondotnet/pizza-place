using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaPlace.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    time = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.order_id);
                });

            migrationBuilder.CreateTable(
                name: "pizza_types",
                columns: table => new
                {
                    pizza_type_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ingredients = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pizza_types", x => x.pizza_type_id);
                });

            migrationBuilder.CreateTable(
                name: "pizzas",
                columns: table => new
                {
                    pizza_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    pizza_type_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    size = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pizzas", x => x.pizza_id);
                    table.ForeignKey(
                        name: "FK_pizzas_pizza_types_pizza_type_id",
                        column: x => x.pizza_type_id,
                        principalTable: "pizza_types",
                        principalColumn: "pizza_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    order_details_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    pizza_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_details", x => x.order_details_id);
                    table.ForeignKey(
                        name: "FK_order_details_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_details_pizzas_pizza_id",
                        column: x => x.pizza_id,
                        principalTable: "pizzas",
                        principalColumn: "pizza_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_details_order_id",
                table: "order_details",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_pizza_id",
                table: "order_details",
                column: "pizza_id");

            migrationBuilder.CreateIndex(
                name: "IX_pizzas_pizza_type_id",
                table: "pizzas",
                column: "pizza_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "pizzas");

            migrationBuilder.DropTable(
                name: "pizza_types");
        }
    }
}
