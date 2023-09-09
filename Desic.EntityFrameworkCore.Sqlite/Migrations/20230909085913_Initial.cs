using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desic.EntityFrameworkCore.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SequentialId = table.Column<long>(type: "INTEGER", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "DATETIME('now)"),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedByType = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "DATETIME('now)"),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedByType = table.Column<string>(type: "TEXT", nullable: true),
                    Hidden = table.Column<bool>(type: "INTEGER", nullable: true, defaultValue: false),
                    Username = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UX_Users_SequentialId",
                schema: "app",
                table: "Users",
                column: "SequentialId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Users_Username",
                schema: "app",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: "app");
        }
    }
}
