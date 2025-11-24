using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreAPIs.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addcolumnpriceafterdiscountinbooktable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceAfterDiscount",
                table: "Books",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceAfterDiscount",
                table: "Books");
        }
    }
}
