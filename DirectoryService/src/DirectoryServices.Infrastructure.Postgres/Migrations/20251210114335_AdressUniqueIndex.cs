using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryServices.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AdressUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE UNIQUE INDEX IX_locations_unique_adress 
            ON locations (
                (adress->>'city'), 
                (adress->>'street'), 
                (adress->>'building'), 
                (adress->>'room')
            );
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX IX_locations_unique_address;");
        }
    }
}
