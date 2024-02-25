using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedLab3.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Speciality = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MkbRecords",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ACTUAL = table.Column<int>(type: "int", nullable: false),
                    MKB_CODE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MKB_NAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    REC_CODE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_PARENT = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MkbRecords", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    LatestInspectionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialitys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialitys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokensBan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BannedToken = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokensBan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionCommentModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionCommentModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionCommentModel_Doctors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Anamnesis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Complaints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Treatment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Conclusion = table.Column<int>(type: "int", nullable: false),
                    NextVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeathDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaseInspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousInspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inspections_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inspections_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosisModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mkbId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    inspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InspectionModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiagnosisModels_Inspections_InspectionModelId",
                        column: x => x.InspectionModelId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionConsultationModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RootCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentsNumber = table.Column<int>(type: "int", nullable: false),
                    InspectionModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionConsultationModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionConsultationModel_InspectionCommentModel_RootCommentId",
                        column: x => x.RootCommentId,
                        principalTable: "InspectionCommentModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionConsultationModel_Inspections_InspectionModelId",
                        column: x => x.InspectionModelId,
                        principalTable: "Inspections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InspectionConsultationModel_Specialitys_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialitys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisModels_InspectionModelId",
                table: "DiagnosisModels",
                column: "InspectionModelId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionCommentModel_AuthorId",
                table: "InspectionCommentModel",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultationModel_InspectionModelId",
                table: "InspectionConsultationModel",
                column: "InspectionModelId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultationModel_RootCommentId",
                table: "InspectionConsultationModel",
                column: "RootCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultationModel_SpecialityId",
                table: "InspectionConsultationModel",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_DoctorId",
                table: "Inspections",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_PatientId",
                table: "Inspections",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiagnosisModels");

            migrationBuilder.DropTable(
                name: "InspectionConsultationModel");

            migrationBuilder.DropTable(
                name: "MkbRecords");

            migrationBuilder.DropTable(
                name: "TokensBan");

            migrationBuilder.DropTable(
                name: "InspectionCommentModel");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "Specialitys");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
