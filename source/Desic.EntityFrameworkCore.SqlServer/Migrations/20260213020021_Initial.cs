using Desic.EntityFrameworkCore.CustomMigrations;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Desic.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.EnsureSchema(
                name: "ref");

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedByTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedByTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsoId = table.Column<int>(type: "int", nullable: false),
                    Alpha2 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Alpha3 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iso3166Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Iso3166Countries_EntityTypes_CreatedByTypeId",
                        column: x => x.CreatedByTypeId,
                        principalSchema: "app",
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Iso3166Countries_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalSchema: "app",
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedByTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_EntityTypes_CreatedByTypeId",
                        column: x => x.CreatedByTypeId,
                        principalSchema: "app",
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tags_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalSchema: "app",
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedByTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_EntityTypes_CreatedByTypeId",
                        column: x => x.CreatedByTypeId,
                        principalSchema: "app",
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_EntityTypes_ModifiedByTypeId",
                        column: x => x.ModifiedByTypeId,
                        principalSchema: "app",
                        principalTable: "EntityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypes_Name",
                schema: "app",
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
                name: "IX_Iso3166Countries_IsDeleted",
                schema: "ref",
                table: "Iso3166Countries",
                column: "IsDeleted");

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
                name: "IX_Iso3166Countries_Name",
                schema: "ref",
                table: "Iso3166Countries",
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
                name: "IX_Tags_Name",
                schema: "app",
                table: "Tags",
                column: "Name");

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
                name: "IX_Users_Username",
                schema: "app",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateAppUserAndPermissions(password: "2d4ba4c0-6cd1-4c7c-b08c-0db156c44116");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UndoCreateAppUserAndPermissions();

            migrationBuilder.DropTable(
                name: "Iso3166Countries",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EntityTypes",
                schema: "app");
        }
    }
}
