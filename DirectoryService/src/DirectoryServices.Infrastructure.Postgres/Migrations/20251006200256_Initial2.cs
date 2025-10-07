using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryServices.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departaments_departaments_ParentId",
                table: "departaments");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "departaments",
                newName: "parentId");

            migrationBuilder.RenameIndex(
                name: "IX_departaments_ParentId",
                table: "departaments",
                newName: "IX_departaments_parentId");

            migrationBuilder.AlterColumn<Guid>(
                name: "parentId",
                table: "departaments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_departaments_departaments_parentId",
                table: "departaments",
                column: "parentId",
                principalTable: "departaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departaments_departaments_parentId",
                table: "departaments");

            migrationBuilder.RenameColumn(
                name: "parentId",
                table: "departaments",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_departaments_parentId",
                table: "departaments",
                newName: "IX_departaments_ParentId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentId",
                table: "departaments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_departaments_departaments_ParentId",
                table: "departaments",
                column: "ParentId",
                principalTable: "departaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
