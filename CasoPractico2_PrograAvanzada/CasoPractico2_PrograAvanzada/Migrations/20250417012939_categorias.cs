using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasoPractico2_PrograAvanzada.Migrations
{
    /// <inheritdoc />
    public partial class categorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Usuarios_UsuarioRegistroId",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UsuarioRegistroId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UsuarioRegistroId",
                table: "Categorias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioRegistroId",
                table: "Categorias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UsuarioRegistroId",
                table: "Categorias",
                column: "UsuarioRegistroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Usuarios_UsuarioRegistroId",
                table: "Categorias",
                column: "UsuarioRegistroId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
