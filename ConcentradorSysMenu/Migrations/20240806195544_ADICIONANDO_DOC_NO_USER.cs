using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcentradorSysMenu.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDO_DOC_NO_USER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Doc",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "sem doc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Doc",
                table: "AspNetUsers");
        }
    }
}
