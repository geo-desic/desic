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
                name: "EntityTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    ModifiedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    ModifiedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UX_EntityTypes_Username",
                schema: "app",
                table: "EntityTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Tags_Name",
                schema: "app",
                table: "Tags",
                column: "Name");

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
                name: "EntityTypes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "app");
        }
    }
}
