using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryServices.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departaments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    identifier = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    path = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    depth = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departaments", x => x.id);
                    table.ForeignKey(
                        name: "FK_departaments_departaments_parent_id",
                        column: x => x.parent_id,
                        principalTable: "departaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    timezone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    adress = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_positions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "departament_locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    departament_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departament_location", x => x.id);
                    table.ForeignKey(
                        name: "FK_departament_locations_departaments_departament_id",
                        column: x => x.departament_id,
                        principalTable: "departaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_departament_locations_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "departament_positions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    departament_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departament_position", x => x.id);
                    table.ForeignKey(
                        name: "FK_departament_positions_departaments_departament_id",
                        column: x => x.departament_id,
                        principalTable: "departaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_departament_positions_positions_position_id",
                        column: x => x.position_id,
                        principalTable: "positions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_departament_locations_departament_id",
                table: "departament_locations",
                column: "departament_id");

            migrationBuilder.CreateIndex(
                name: "IX_departament_locations_location_id",
                table: "departament_locations",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_departament_positions_departament_id",
                table: "departament_positions",
                column: "departament_id");

            migrationBuilder.CreateIndex(
                name: "IX_departament_positions_position_id",
                table: "departament_positions",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_departaments_parent_id",
                table: "departaments",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_locations_name",
                table: "locations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_positions_name",
                table: "positions",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "departament_locations");

            migrationBuilder.DropTable(
                name: "departament_positions");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "departaments");

            migrationBuilder.DropTable(
                name: "positions");
        }
    }
}
