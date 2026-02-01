using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryServices.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexDepNameAndIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_departaments_identifier",
                table: "departaments",
                column: "identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_departaments_name",
                table: "departaments",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_departaments_identifier",
                table: "departaments");

            migrationBuilder.DropIndex(
                name: "IX_departaments_name",
                table: "departaments");
        }
    }
}
