using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasoPractico2_PrograAvanzada.Migrations
{
    /// <inheritdoc />
    public partial class Asistencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Asistencia",
                table: "Inscripciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asistencia",
                table: "Inscripciones");
        }
    }
}
