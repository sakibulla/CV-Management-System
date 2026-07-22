using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueCVConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CVs_UserId_PositionId",
                table: "CVs",
                columns: new[] { "UserId", "PositionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CVs_UserId_PositionId",
                table: "CVs");
        }
    }
}
