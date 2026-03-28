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
            migrationBuilder.CreateTable(
                name: "EntityTypes",
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
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Iso3166Countries_EntityTypes_DeletedByTypeId",
                        column: x => x.DeletedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Iso3166Countries_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Labels",
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
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Labels_EntityTypes_DeletedByTypeId",
                        column: x => x.DeletedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Labels_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Processes",
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
                    StartedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FaileddOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Message = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Processes_EntityTypes_CreatedByTypeId",
                        column: x => x.CreatedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Processes_EntityTypes_DeletedByTypeId",
                        column: x => x.DeletedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Processes_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
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
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_EntityTypes_DeletedByTypeId",
                        column: x => x.DeletedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
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
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_EntityTypes_DeletedByTypeId",
                        column: x => x.DeletedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypes_Key",
                table: "EntityTypes",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypes_Name",
                table: "EntityTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_Alpha2",
                table: "Iso3166Countries",
                column: "Alpha2");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_Alpha3",
                table: "Iso3166Countries",
                column: "Alpha3");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_CreatedById",
                table: "Iso3166Countries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_CreatedByTypeId",
                table: "Iso3166Countries",
                column: "CreatedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_CreatedOn",
                table: "Iso3166Countries",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_DeletedById",
                table: "Iso3166Countries",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_DeletedByTypeId",
                table: "Iso3166Countries",
                column: "DeletedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_DeletedOn",
                table: "Iso3166Countries",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_IsBeingSeeded",
                table: "Iso3166Countries",
                column: "IsBeingSeeded");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_IsoId",
                table: "Iso3166Countries",
                column: "IsoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_ModifiedById",
                table: "Iso3166Countries",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_ModifiedByTypeId",
                table: "Iso3166Countries",
                column: "ModifiedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_ModifiedOn",
                table: "Iso3166Countries",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_Name",
                table: "Iso3166Countries",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedById",
                table: "Labels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedByTypeId",
                table: "Labels",
                column: "CreatedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedOn",
                table: "Labels",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_DeletedById",
                table: "Labels",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_DeletedByTypeId",
                table: "Labels",
                column: "DeletedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_DeletedOn",
                table: "Labels",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_ModifiedById",
                table: "Labels",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_ModifiedByTypeId",
                table: "Labels",
                column: "ModifiedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_ModifiedOn",
                table: "Labels",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_Name",
                table: "Labels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_CompletedOn",
                table: "Processes",
                column: "CompletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_CreatedById",
                table: "Processes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_CreatedByTypeId",
                table: "Processes",
                column: "CreatedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_CreatedOn",
                table: "Processes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_DeletedById",
                table: "Processes",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_DeletedByTypeId",
                table: "Processes",
                column: "DeletedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_DeletedOn",
                table: "Processes",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_FaileddOn",
                table: "Processes",
                column: "FaileddOn");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_ModifiedById",
                table: "Processes",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_ModifiedByTypeId",
                table: "Processes",
                column: "ModifiedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_ModifiedOn",
                table: "Processes",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_Name",
                table: "Processes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_StartedOn",
                table: "Processes",
                column: "StartedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedById",
                table: "Tags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedByTypeId",
                table: "Tags",
                column: "CreatedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedOn",
                table: "Tags",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DeletedById",
                table: "Tags",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DeletedByTypeId",
                table: "Tags",
                column: "DeletedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DeletedOn",
                table: "Tags",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ModifiedById",
                table: "Tags",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ModifiedByTypeId",
                table: "Tags",
                column: "ModifiedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ModifiedOn",
                table: "Tags",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Value",
                table: "Tags",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedById",
                table: "Users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedByTypeId",
                table: "Users",
                column: "CreatedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedOn",
                table: "Users",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedById",
                table: "Users",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedByTypeId",
                table: "Users",
                column: "DeletedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedOn",
                table: "Users",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModifiedById",
                table: "Users",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModifiedByTypeId",
                table: "Users",
                column: "ModifiedByTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModifiedOn",
                table: "Users",
                column: "ModifiedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Iso3166Countries");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "Processes");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "EntityTypes");
        }
    }
}
