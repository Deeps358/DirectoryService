using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryServices.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class UniqueNameActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departaments_departaments_parentId",
                table: "departaments");

            migrationBuilder.RenameColumn(
                name: "updatedAt",
                table: "positions",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "positions",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "positions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updatedAt",
                table: "locations",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "locations",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "locations",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updatedAt",
                table: "departaments",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "parentId",
                table: "departaments",
                newName: "parent_id");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "departaments",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "departaments",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_departaments_parentId",
                table: "departaments",
                newName: "IX_departaments_parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_positions_name",
                table: "positions",
                column: "name",
                unique: true,
                filter: "\"is_active\" IS TRUE");

            migrationBuilder.AddForeignKey(
                name: "FK_departaments_departaments_parent_id",
                table: "departaments",
                column: "parent_id",
                principalTable: "departaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departaments_departaments_parent_id",
                table: "departaments");

            migrationBuilder.DropIndex(
                name: "IX_positions_name",
                table: "positions");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "positions",
                newName: "updatedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "positions",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "positions",
                newName: "createdAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "locations",
                newName: "updatedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "locations",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "locations",
                newName: "createdAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "departaments",
                newName: "updatedAt");

            migrationBuilder.RenameColumn(
                name: "parent_id",
                table: "departaments",
                newName: "parentId");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "departaments",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "departaments",
                newName: "createdAt");

            migrationBuilder.RenameIndex(
                name: "IX_departaments_parent_id",
                table: "departaments",
                newName: "IX_departaments_parentId");

            migrationBuilder.AddForeignKey(
                name: "FK_departaments_departaments_parentId",
                table: "departaments",
                column: "parentId",
                principalTable: "departaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
