using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desic.Infrastructure.Data.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ref");

            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Iso3166Countries",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    ModifiedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    DeletedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedByTypeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsBeingSeeded = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Alpha2 = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Alpha3 = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iso3166Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Iso3166Countries_EntityTypes_CreatedByTypeId",
                        column: x => x.CreatedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Iso3166Countries_EntityTypes_DeletedByTypeId",
                        column: x => x.DeletedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Iso3166Countries_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    ModifiedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    DeletedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedByTypeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labels_EntityTypes_CreatedByTypeId",
                        column: x => x.CreatedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Labels_EntityTypes_DeletedByTypeId",
                        column: x => x.DeletedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Labels_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    ModifiedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    DeletedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedByTypeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_EntityTypes_CreatedByTypeId",
                        column: x => x.CreatedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_EntityTypes_DeletedByTypeId",
                        column: x => x.DeletedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    ModifiedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModifiedByTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')"),
                    DeletedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedByName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeletedByTypeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_EntityTypes_CreatedByTypeId",
                        column: x => x.CreatedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_EntityTypes_DeletedByTypeId",
                        column: x => x.DeletedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalSchema: "ref",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypes_Key",
                schema: "ref",
                table: "EntityTypes",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypes_Name",
                schema: "ref",
                table: "EntityTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_Alpha2",
                schema: "ref",
                table: "Iso3166Countries",
                column: "Alpha2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_Alpha3",
                schema: "ref",
                table: "Iso3166Countries",
                column: "Alpha3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_CreatedById",
                schema: "ref",
                table: "Iso3166Countries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_CreatedByTypeId",
                schema: "ref",
                table: "Iso3166Countries",
                column: "CreatedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_CreatedOn",
                schema: "ref",
                table: "Iso3166Countries",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_DeletedById",
                schema: "ref",
                table: "Iso3166Countries",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_DeletedByTypeId",
                schema: "ref",
                table: "Iso3166Countries",
                column: "DeletedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_DeletedOn",
                schema: "ref",
                table: "Iso3166Countries",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_IsBeingSeeded",
                schema: "ref",
                table: "Iso3166Countries",
                column: "IsBeingSeeded");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_IsoId",
                schema: "ref",
                table: "Iso3166Countries",
                column: "IsoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_ModifiedById",
                schema: "ref",
                table: "Iso3166Countries",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_ModifiedByTypeId",
                schema: "ref",
                table: "Iso3166Countries",
                column: "ModifiedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_ModifiedOn",
                schema: "ref",
                table: "Iso3166Countries",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_Name",
                schema: "ref",
                table: "Iso3166Countries",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedById",
                schema: "app",
                table: "Labels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedByTypeId",
                schema: "app",
                table: "Labels",
                column: "CreatedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedOn",
                schema: "app",
                table: "Labels",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_DeletedById",
                schema: "app",
                table: "Labels",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_DeletedByTypeId",
                schema: "app",
                table: "Labels",
                column: "DeletedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_DeletedOn",
                schema: "app",
                table: "Labels",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_ModifiedById",
                schema: "app",
                table: "Labels",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_ModifiedByTypeId",
                schema: "app",
                table: "Labels",
                column: "ModifiedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_ModifiedOn",
                schema: "app",
                table: "Labels",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_Name",
                schema: "app",
                table: "Labels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedById",
                schema: "app",
                table: "Tags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedByTypeId",
                schema: "app",
                table: "Tags",
                column: "CreatedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedOn",
                schema: "app",
                table: "Tags",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DeletedById",
                schema: "app",
                table: "Tags",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DeletedByTypeId",
                schema: "app",
                table: "Tags",
                column: "DeletedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DeletedOn",
                schema: "app",
                table: "Tags",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ModifiedById",
                schema: "app",
                table: "Tags",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ModifiedByTypeId",
                schema: "app",
                table: "Tags",
                column: "ModifiedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ModifiedOn",
                schema: "app",
                table: "Tags",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                schema: "app",
                table: "Tags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Value",
                schema: "app",
                table: "Tags",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedById",
                schema: "app",
                table: "Users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedByTypeId",
                schema: "app",
                table: "Users",
                column: "CreatedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedOn",
                schema: "app",
                table: "Users",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedById",
                schema: "app",
                table: "Users",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedByTypeId",
                schema: "app",
                table: "Users",
                column: "DeletedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedOn",
                schema: "app",
                table: "Users",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModifiedById",
                schema: "app",
                table: "Users",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModifiedByTypeId",
                schema: "app",
                table: "Users",
                column: "ModifiedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModifiedOn",
                schema: "app",
                table: "Users",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "app",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Iso3166Countries",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "Labels",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EntityTypes",
                schema: "ref");
        }
    }
}
