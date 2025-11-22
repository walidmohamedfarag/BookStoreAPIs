using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreAPIs.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addagecolmuntotableauthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Authors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Authors");
        }
    }
}
