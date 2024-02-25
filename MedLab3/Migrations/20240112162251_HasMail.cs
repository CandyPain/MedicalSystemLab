using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedLab3.Migrations
{
    /// <inheritdoc />
    public partial class HasMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasMail",
                table: "Inspections",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasMail",
                table: "Inspections");
        }
    }
}
