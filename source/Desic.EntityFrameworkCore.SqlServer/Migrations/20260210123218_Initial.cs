using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                schema: "app",
                columns: table => new
                {
                    IsoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alpha2 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Alpha3 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iso3166Countries", x => x.IsoId);
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

            migrationBuilder.InsertData(
                schema: "app",
                table: "Iso3166Countries",
                columns: new[] { "IsoId", "Alpha2", "Alpha3", "Name" },
                values: new object[,]
                {
                    { 4, "af", "afg", "Afghanistan" },
                    { 8, "al", "alb", "Albania" },
                    { 10, "aq", "ata", "Antarctica" },
                    { 12, "dz", "dza", "Algeria" },
                    { 16, "as", "asm", "American Samoa" },
                    { 20, "ad", "and", "Andorra" },
                    { 24, "ao", "ago", "Angola" },
                    { 28, "ag", "atg", "Antigua and Barbuda" },
                    { 31, "az", "aze", "Azerbaijan" },
                    { 32, "ar", "arg", "Argentina" },
                    { 36, "au", "aus", "Australia" },
                    { 40, "at", "aut", "Austria" },
                    { 44, "bs", "bhs", "Bahamas" },
                    { 48, "bh", "bhr", "Bahrain" },
                    { 50, "bd", "bgd", "Bangladesh" },
                    { 51, "am", "arm", "Armenia" },
                    { 52, "bb", "brb", "Barbados" },
                    { 56, "be", "bel", "Belgium" },
                    { 60, "bm", "bmu", "Bermuda" },
                    { 64, "bt", "btn", "Bhutan" },
                    { 68, "bo", "bol", "Bolivia, Plurinational State of" },
                    { 70, "ba", "bih", "Bosnia and Herzegovina" },
                    { 72, "bw", "bwa", "Botswana" },
                    { 74, "bv", "bvt", "Bouvet Island" },
                    { 76, "br", "bra", "Brazil" },
                    { 84, "bz", "blz", "Belize" },
                    { 86, "io", "iot", "British Indian Ocean Territory" },
                    { 90, "sb", "slb", "Solomon Islands" },
                    { 92, "vg", "vgb", "Virgin Islands (British)" },
                    { 96, "bn", "brn", "Brunei Darussalam" },
                    { 100, "bg", "bgr", "Bulgaria" },
                    { 104, "mm", "mmr", "Myanmar" },
                    { 108, "bi", "bdi", "Burundi" },
                    { 112, "by", "blr", "Belarus" },
                    { 116, "kh", "khm", "Cambodia" },
                    { 120, "cm", "cmr", "Cameroon" },
                    { 124, "ca", "can", "Canada" },
                    { 132, "cv", "cpv", "Cabo Verde" },
                    { 136, "ky", "cym", "Cayman Islands" },
                    { 140, "cf", "caf", "Central African Republic" },
                    { 144, "lk", "lka", "Sri Lanka" },
                    { 148, "td", "tcd", "Chad" },
                    { 152, "cl", "chl", "Chile" },
                    { 156, "cn", "chn", "China" },
                    { 158, "tw", "twn", "Taiwan, Province of China" },
                    { 162, "cx", "cxr", "Christmas Island" },
                    { 166, "cc", "cck", "Cocos (Keeling) Islands" },
                    { 170, "co", "col", "Colombia" },
                    { 174, "km", "com", "Comoros" },
                    { 175, "yt", "myt", "Mayotte" },
                    { 178, "cg", "cog", "Congo" },
                    { 180, "cd", "cod", "Congo, Democratic Republic of the" },
                    { 184, "ck", "cok", "Cook Islands" },
                    { 188, "cr", "cri", "Costa Rica" },
                    { 191, "hr", "hrv", "Croatia" },
                    { 192, "cu", "cub", "Cuba" },
                    { 196, "cy", "cyp", "Cyprus" },
                    { 203, "cz", "cze", "Czechia" },
                    { 204, "bj", "ben", "Benin" },
                    { 208, "dk", "dnk", "Denmark" },
                    { 212, "dm", "dma", "Dominica" },
                    { 214, "do", "dom", "Dominican Republic" },
                    { 218, "ec", "ecu", "Ecuador" },
                    { 222, "sv", "slv", "El Salvador" },
                    { 226, "gq", "gnq", "Equatorial Guinea" },
                    { 231, "et", "eth", "Ethiopia" },
                    { 232, "er", "eri", "Eritrea" },
                    { 233, "ee", "est", "Estonia" },
                    { 234, "fo", "fro", "Faroe Islands" },
                    { 238, "fk", "flk", "Falkland Islands (Malvinas)" },
                    { 239, "gs", "sgs", "South Georgia and the South Sandwich Islands" },
                    { 242, "fj", "fji", "Fiji" },
                    { 246, "fi", "fin", "Finland" },
                    { 248, "ax", "ala", "Åland Islands" },
                    { 250, "fr", "fra", "France" },
                    { 254, "gf", "guf", "French Guiana" },
                    { 258, "pf", "pyf", "French Polynesia" },
                    { 260, "tf", "atf", "French Southern Territories" },
                    { 262, "dj", "dji", "Djibouti" },
                    { 266, "ga", "gab", "Gabon" },
                    { 268, "ge", "geo", "Georgia" },
                    { 270, "gm", "gmb", "Gambia" },
                    { 275, "ps", "pse", "Palestine, State of" },
                    { 276, "de", "deu", "Germany" },
                    { 288, "gh", "gha", "Ghana" },
                    { 292, "gi", "gib", "Gibraltar" },
                    { 296, "ki", "kir", "Kiribati" },
                    { 300, "gr", "grc", "Greece" },
                    { 304, "gl", "grl", "Greenland" },
                    { 308, "gd", "grd", "Grenada" },
                    { 312, "gp", "glp", "Guadeloupe" },
                    { 316, "gu", "gum", "Guam" },
                    { 320, "gt", "gtm", "Guatemala" },
                    { 324, "gn", "gin", "Guinea" },
                    { 328, "gy", "guy", "Guyana" },
                    { 332, "ht", "hti", "Haiti" },
                    { 334, "hm", "hmd", "Heard Island and McDonald Islands" },
                    { 336, "va", "vat", "Holy See" },
                    { 340, "hn", "hnd", "Honduras" },
                    { 344, "hk", "hkg", "Hong Kong" },
                    { 348, "hu", "hun", "Hungary" },
                    { 352, "is", "isl", "Iceland" },
                    { 356, "in", "ind", "India" },
                    { 360, "id", "idn", "Indonesia" },
                    { 364, "ir", "irn", "Iran, Islamic Republic of" },
                    { 368, "iq", "irq", "Iraq" },
                    { 372, "ie", "irl", "Ireland" },
                    { 376, "il", "isr", "Israel" },
                    { 380, "it", "ita", "Italy" },
                    { 384, "ci", "civ", "Côte d'Ivoire" },
                    { 388, "jm", "jam", "Jamaica" },
                    { 392, "jp", "jpn", "Japan" },
                    { 398, "kz", "kaz", "Kazakhstan" },
                    { 400, "jo", "jor", "Jordan" },
                    { 404, "ke", "ken", "Kenya" },
                    { 408, "kp", "prk", "Korea, Democratic People's Republic of" },
                    { 410, "kr", "kor", "Korea, Republic of" },
                    { 414, "kw", "kwt", "Kuwait" },
                    { 417, "kg", "kgz", "Kyrgyzstan" },
                    { 418, "la", "lao", "Lao People's Democratic Republic" },
                    { 422, "lb", "lbn", "Lebanon" },
                    { 426, "ls", "lso", "Lesotho" },
                    { 428, "lv", "lva", "Latvia" },
                    { 430, "lr", "lbr", "Liberia" },
                    { 434, "ly", "lby", "Libya" },
                    { 438, "li", "lie", "Liechtenstein" },
                    { 440, "lt", "ltu", "Lithuania" },
                    { 442, "lu", "lux", "Luxembourg" },
                    { 446, "mo", "mac", "Macao" },
                    { 450, "mg", "mdg", "Madagascar" },
                    { 454, "mw", "mwi", "Malawi" },
                    { 458, "my", "mys", "Malaysia" },
                    { 462, "mv", "mdv", "Maldives" },
                    { 466, "ml", "mli", "Mali" },
                    { 470, "mt", "mlt", "Malta" },
                    { 474, "mq", "mtq", "Martinique" },
                    { 478, "mr", "mrt", "Mauritania" },
                    { 480, "mu", "mus", "Mauritius" },
                    { 484, "mx", "mex", "Mexico" },
                    { 492, "mc", "mco", "Monaco" },
                    { 496, "mn", "mng", "Mongolia" },
                    { 498, "md", "mda", "Moldova, Republic of" },
                    { 499, "me", "mne", "Montenegro" },
                    { 500, "ms", "msr", "Montserrat" },
                    { 504, "ma", "mar", "Morocco" },
                    { 508, "mz", "moz", "Mozambique" },
                    { 512, "om", "omn", "Oman" },
                    { 516, "na", "nam", "Namibia" },
                    { 520, "nr", "nru", "Nauru" },
                    { 524, "np", "npl", "Nepal" },
                    { 528, "nl", "nld", "Netherlands" },
                    { 531, "cw", "cuw", "Curaçao" },
                    { 533, "aw", "abw", "Aruba" },
                    { 534, "sx", "sxm", "Sint Maarten (Dutch part)" },
                    { 535, "bq", "bes", "Bonaire, Sint Eustatius and Saba" },
                    { 540, "nc", "ncl", "New Caledonia" },
                    { 548, "vu", "vut", "Vanuatu" },
                    { 554, "nz", "nzl", "New Zealand" },
                    { 558, "ni", "nic", "Nicaragua" },
                    { 562, "ne", "ner", "Niger" },
                    { 566, "ng", "nga", "Nigeria" },
                    { 570, "nu", "niu", "Niue" },
                    { 574, "nf", "nfk", "Norfolk Island" },
                    { 578, "no", "nor", "Norway" },
                    { 580, "mp", "mnp", "Northern Mariana Islands" },
                    { 581, "um", "umi", "United States Minor Outlying Islands" },
                    { 583, "fm", "fsm", "Micronesia, Federated States of" },
                    { 584, "mh", "mhl", "Marshall Islands" },
                    { 585, "pw", "plw", "Palau" },
                    { 586, "pk", "pak", "Pakistan" },
                    { 591, "pa", "pan", "Panama" },
                    { 598, "pg", "png", "Papua New Guinea" },
                    { 600, "py", "pry", "Paraguay" },
                    { 604, "pe", "per", "Peru" },
                    { 608, "ph", "phl", "Philippines" },
                    { 612, "pn", "pcn", "Pitcairn" },
                    { 616, "pl", "pol", "Poland" },
                    { 620, "pt", "prt", "Portugal" },
                    { 624, "gw", "gnb", "Guinea-Bissau" },
                    { 626, "tl", "tls", "Timor-Leste" },
                    { 630, "pr", "pri", "Puerto Rico" },
                    { 634, "qa", "qat", "Qatar" },
                    { 638, "re", "reu", "Réunion" },
                    { 642, "ro", "rou", "Romania" },
                    { 643, "ru", "rus", "Russian Federation" },
                    { 646, "rw", "rwa", "Rwanda" },
                    { 652, "bl", "blm", "Saint Barthélemy" },
                    { 654, "sh", "shn", "Saint Helena, Ascension and Tristan da Cunha" },
                    { 659, "kn", "kna", "Saint Kitts and Nevis" },
                    { 660, "ai", "aia", "Anguilla" },
                    { 662, "lc", "lca", "Saint Lucia" },
                    { 663, "mf", "maf", "Saint Martin (French part)" },
                    { 666, "pm", "spm", "Saint Pierre and Miquelon" },
                    { 670, "vc", "vct", "Saint Vincent and the Grenadines" },
                    { 674, "sm", "smr", "San Marino" },
                    { 678, "st", "stp", "Sao Tome and Principe" },
                    { 682, "sa", "sau", "Saudi Arabia" },
                    { 686, "sn", "sen", "Senegal" },
                    { 688, "rs", "srb", "Serbia" },
                    { 690, "sc", "syc", "Seychelles" },
                    { 694, "sl", "sle", "Sierra Leone" },
                    { 702, "sg", "sgp", "Singapore" },
                    { 703, "sk", "svk", "Slovakia" },
                    { 704, "vn", "vnm", "Viet Nam" },
                    { 705, "si", "svn", "Slovenia" },
                    { 706, "so", "som", "Somalia" },
                    { 710, "za", "zaf", "South Africa" },
                    { 716, "zw", "zwe", "Zimbabwe" },
                    { 724, "es", "esp", "Spain" },
                    { 728, "ss", "ssd", "South Sudan" },
                    { 729, "sd", "sdn", "Sudan" },
                    { 732, "eh", "esh", "Western Sahara" },
                    { 740, "sr", "sur", "Suriname" },
                    { 744, "sj", "sjm", "Svalbard and Jan Mayen" },
                    { 748, "sz", "swz", "Eswatini" },
                    { 752, "se", "swe", "Sweden" },
                    { 756, "ch", "che", "Switzerland" },
                    { 760, "sy", "syr", "Syrian Arab Republic" },
                    { 762, "tj", "tjk", "Tajikistan" },
                    { 764, "th", "tha", "Thailand" },
                    { 768, "tg", "tgo", "Togo" },
                    { 772, "tk", "tkl", "Tokelau" },
                    { 776, "to", "ton", "Tonga" },
                    { 780, "tt", "tto", "Trinidad and Tobago" },
                    { 784, "ae", "are", "United Arab Emirates" },
                    { 788, "tn", "tun", "Tunisia" },
                    { 792, "tr", "tur", "Türkiye" },
                    { 795, "tm", "tkm", "Turkmenistan" },
                    { 796, "tc", "tca", "Turks and Caicos Islands" },
                    { 798, "tv", "tuv", "Tuvalu" },
                    { 800, "ug", "uga", "Uganda" },
                    { 804, "ua", "ukr", "Ukraine" },
                    { 807, "mk", "mkd", "North Macedonia" },
                    { 818, "eg", "egy", "Egypt" },
                    { 826, "gb", "gbr", "United Kingdom of Great Britain and Northern Ireland" },
                    { 831, "gg", "ggy", "Guernsey" },
                    { 832, "je", "jey", "Jersey" },
                    { 833, "im", "imn", "Isle of Man" },
                    { 834, "tz", "tza", "Tanzania, United Republic of" },
                    { 840, "us", "usa", "United States of America" },
                    { 850, "vi", "vir", "Virgin Islands (U.S.)" },
                    { 854, "bf", "bfa", "Burkina Faso" },
                    { 858, "uy", "ury", "Uruguay" },
                    { 860, "uz", "uzb", "Uzbekistan" },
                    { 862, "ve", "ven", "Venezuela, Bolivarian Republic of" },
                    { 876, "wf", "wlf", "Wallis and Futuna" },
                    { 882, "ws", "wsm", "Samoa" },
                    { 887, "ye", "yem", "Yemen" },
                    { 894, "zm", "zmb", "Zambia" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypes_Name",
                schema: "app",
                table: "EntityTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_Alpha2",
                schema: "app",
                table: "Iso3166Countries",
                column: "Alpha2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Iso3166Countries_Alpha3",
                schema: "app",
                table: "Iso3166Countries",
                column: "Alpha3",
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Iso3166Countries",
                schema: "app");

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
