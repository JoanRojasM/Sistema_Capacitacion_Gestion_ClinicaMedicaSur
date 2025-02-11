using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace scg_clinicasur.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePathToEvaluacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Capacitaciones_Usuarios_id_usuario",
                table: "Capacitaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluaciones_Usuarios_id_usuario",
                table: "Evaluaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_resultados_laboratorio_Expedientes_IdExpediente",
                table: "resultados_laboratorio");

            migrationBuilder.DropForeignKey(
                name: "FK_resultados_laboratorio_Usuarios_IdPaciente",
                table: "resultados_laboratorio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_resultados_laboratorio",
                table: "resultados_laboratorio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Evaluaciones",
                table: "Evaluaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Capacitaciones",
                table: "Capacitaciones");

            migrationBuilder.DropColumn(
                name: "Resultado",
                table: "resultados_laboratorio");

            migrationBuilder.DropColumn(
                name: "TipoPrueba",
                table: "resultados_laboratorio");

            migrationBuilder.DropColumn(
                name: "archivo",
                table: "Evaluaciones");

            migrationBuilder.DropColumn(
                name: "archivo",
                table: "Capacitaciones");

            migrationBuilder.RenameTable(
                name: "resultados_laboratorio",
                newName: "resultadoslaboratorio");

            migrationBuilder.RenameTable(
                name: "Evaluaciones",
                newName: "evaluacion");

            migrationBuilder.RenameTable(
                name: "Capacitaciones",
                newName: "capacitacion");

            migrationBuilder.RenameColumn(
                name: "FechaPrueba",
                table: "resultadoslaboratorio",
                newName: "fechaPrueba");

            migrationBuilder.RenameColumn(
                name: "IdPaciente",
                table: "resultadoslaboratorio",
                newName: "id_paciente");

            migrationBuilder.RenameColumn(
                name: "IdExpediente",
                table: "resultadoslaboratorio",
                newName: "id_expediente");

            migrationBuilder.RenameColumn(
                name: "IdResultado",
                table: "resultadoslaboratorio",
                newName: "id_resultado");

            migrationBuilder.RenameIndex(
                name: "IX_resultados_laboratorio_IdPaciente",
                table: "resultadoslaboratorio",
                newName: "IX_resultadoslaboratorio_id_paciente");

            migrationBuilder.RenameIndex(
                name: "IX_resultados_laboratorio_IdExpediente",
                table: "resultadoslaboratorio",
                newName: "IX_resultadoslaboratorio_id_expediente");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluaciones_id_usuario",
                table: "evaluacion",
                newName: "IX_evaluacion_id_usuario");

            migrationBuilder.RenameIndex(
                name: "IX_Capacitaciones_id_usuario",
                table: "capacitacion",
                newName: "IX_capacitacion_id_usuario");

            migrationBuilder.AddColumn<byte[]>(
                name: "ArchivoPDF",
                table: "resultadoslaboratorio",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "tiempo_prueba",
                table: "evaluacion",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "evaluacion",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "id_usuario",
                table: "evaluacion",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "descripcion",
                table: "evaluacion",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "evaluacion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "id_capacitacion",
                table: "evaluacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "id_usuario",
                table: "capacitacion",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "duracion",
                table: "capacitacion",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddPrimaryKey(
                name: "PK_resultadoslaboratorio",
                table: "resultadoslaboratorio",
                column: "id_resultado");

            migrationBuilder.AddPrimaryKey(
                name: "PK_evaluacion",
                table: "evaluacion",
                column: "id_evaluacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_capacitacion",
                table: "capacitacion",
                column: "id_capacitacion");

            migrationBuilder.CreateTable(
                name: "Alergias",
                columns: table => new
                {
                    id_alergia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_alergia = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergias", x => x.id_alergia);
                });

            migrationBuilder.CreateTable(
                name: "antecedentesfamiliares",
                columns: table => new
                {
                    id_antecedente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_antecedentesfamiliares", x => x.id_antecedente);
                    table.ForeignKey(
                        name: "FK_antecedentesfamiliares_Usuarios_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contabilidad",
                columns: table => new
                {
                    id_contabilidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    concepto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contabilidad", x => x.id_contabilidad);
                });

            migrationBuilder.CreateTable(
                name: "contactosemergencia",
                columns: table => new
                {
                    id_contacto_emergencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    nombre_contacto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    relacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    telefono_contacto = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contactosemergencia", x => x.id_contacto_emergencia);
                    table.ForeignKey(
                        name: "FK_contactosemergencia_Usuarios_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "disponibilidad_doctor",
                columns: table => new
                {
                    id_disponibilidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_doctor = table.Column<int>(type: "int", nullable: false),
                    dia_semana = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    hora_inicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    hora_fin = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disponibilidad_doctor", x => x.id_disponibilidad);
                    table.ForeignKey(
                        name: "FK_disponibilidad_doctor_Usuarios_id_doctor",
                        column: x => x.id_doctor,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "especialidades",
                columns: table => new
                {
                    id_especialidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_especialidades", x => x.id_especialidad);
                });

            migrationBuilder.CreateTable(
                name: "estado_citas",
                columns: table => new
                {
                    id_estado_cita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    estado_nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estado_citas", x => x.id_estado_cita);
                });

            migrationBuilder.CreateTable(
                name: "habitosvida",
                columns: table => new
                {
                    id_habito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitosvida", x => x.id_habito);
                    table.ForeignKey(
                        name: "FK_habitosvida_Usuarios_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "medicamentosprescritos",
                columns: table => new
                {
                    id_medicamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    nombre_medicamento = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    dosis = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    fecha_prescripcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicamentosprescritos", x => x.id_medicamento);
                    table.ForeignKey(
                        name: "FK_medicamentosprescritos_Usuarios_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notificacion",
                columns: table => new
                {
                    id_notificacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mensaje = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    fecha_envio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificacion", x => x.id_notificacion);
                });

            migrationBuilder.CreateTable(
                name: "recursos_aprendizaje",
                columns: table => new
                {
                    id_recurso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    id_capacitacion = table.Column<int>(type: "int", nullable: false),
                    archivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    enlace = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recursos_aprendizaje", x => x.id_recurso);
                    table.ForeignKey(
                        name: "FK_recursos_aprendizaje_capacitacion_id_capacitacion",
                        column: x => x.id_capacitacion,
                        principalTable: "capacitacion",
                        principalColumn: "id_capacitacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PacienteAlergias",
                columns: table => new
                {
                    id_pacientealergias = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_alergia = table.Column<int>(type: "int", nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacienteAlergias", x => x.id_pacientealergias);
                    table.ForeignKey(
                        name: "FK_PacienteAlergias_Alergias_id_alergia",
                        column: x => x.id_alergia,
                        principalTable: "Alergias",
                        principalColumn: "id_alergia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PacienteAlergias_Usuarios_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "doctor_especialidades",
                columns: table => new
                {
                    id_doctor_especialidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    id_especialidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_especialidades", x => x.id_doctor_especialidad);
                    table.ForeignKey(
                        name: "FK_doctor_especialidades_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_doctor_especialidades_especialidades_id_especialidad",
                        column: x => x.id_especialidad,
                        principalTable: "especialidades",
                        principalColumn: "id_especialidad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    id_cita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_doctor = table.Column<int>(type: "int", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    motivo_cita = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_estado_cita = table.Column<int>(type: "int", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citas", x => x.id_cita);
                    table.ForeignKey(
                        name: "FK_citas_Usuarios_id_doctor",
                        column: x => x.id_doctor,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_citas_Usuarios_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_citas_estado_citas_id_estado_cita",
                        column: x => x.id_estado_cita,
                        principalTable: "estado_citas",
                        principalColumn: "id_estado_cita",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_evaluacion_id_capacitacion",
                table: "evaluacion",
                column: "id_capacitacion");

            migrationBuilder.CreateIndex(
                name: "IX_antecedentesfamiliares_id_paciente",
                table: "antecedentesfamiliares",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_citas_id_doctor",
                table: "citas",
                column: "id_doctor");

            migrationBuilder.CreateIndex(
                name: "IX_citas_id_estado_cita",
                table: "citas",
                column: "id_estado_cita");

            migrationBuilder.CreateIndex(
                name: "IX_citas_id_paciente",
                table: "citas",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_contactosemergencia_id_paciente",
                table: "contactosemergencia",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_disponibilidad_doctor_id_doctor",
                table: "disponibilidad_doctor",
                column: "id_doctor");

            migrationBuilder.CreateIndex(
                name: "IX_doctor_especialidades_id_especialidad",
                table: "doctor_especialidades",
                column: "id_especialidad");

            migrationBuilder.CreateIndex(
                name: "IX_doctor_especialidades_id_usuario",
                table: "doctor_especialidades",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_habitosvida_id_paciente",
                table: "habitosvida",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_medicamentosprescritos_id_paciente",
                table: "medicamentosprescritos",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_PacienteAlergias_id_alergia",
                table: "PacienteAlergias",
                column: "id_alergia");

            migrationBuilder.CreateIndex(
                name: "IX_PacienteAlergias_id_paciente",
                table: "PacienteAlergias",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_recursos_aprendizaje_id_capacitacion",
                table: "recursos_aprendizaje",
                column: "id_capacitacion");

            migrationBuilder.AddForeignKey(
                name: "FK_capacitacion_Usuarios_id_usuario",
                table: "capacitacion",
                column: "id_usuario",
                principalTable: "Usuarios",
                principalColumn: "id_usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_evaluacion_Usuarios_id_usuario",
                table: "evaluacion",
                column: "id_usuario",
                principalTable: "Usuarios",
                principalColumn: "id_usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_evaluacion_capacitacion_id_capacitacion",
                table: "evaluacion",
                column: "id_capacitacion",
                principalTable: "capacitacion",
                principalColumn: "id_capacitacion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_resultadoslaboratorio_Expedientes_id_expediente",
                table: "resultadoslaboratorio",
                column: "id_expediente",
                principalTable: "Expedientes",
                principalColumn: "id_expediente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_resultadoslaboratorio_Usuarios_id_paciente",
                table: "resultadoslaboratorio",
                column: "id_paciente",
                principalTable: "Usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_capacitacion_Usuarios_id_usuario",
                table: "capacitacion");

            migrationBuilder.DropForeignKey(
                name: "FK_evaluacion_Usuarios_id_usuario",
                table: "evaluacion");

            migrationBuilder.DropForeignKey(
                name: "FK_evaluacion_capacitacion_id_capacitacion",
                table: "evaluacion");

            migrationBuilder.DropForeignKey(
                name: "FK_resultadoslaboratorio_Expedientes_id_expediente",
                table: "resultadoslaboratorio");

            migrationBuilder.DropForeignKey(
                name: "FK_resultadoslaboratorio_Usuarios_id_paciente",
                table: "resultadoslaboratorio");

            migrationBuilder.DropTable(
                name: "antecedentesfamiliares");

            migrationBuilder.DropTable(
                name: "citas");

            migrationBuilder.DropTable(
                name: "contabilidad");

            migrationBuilder.DropTable(
                name: "contactosemergencia");

            migrationBuilder.DropTable(
                name: "disponibilidad_doctor");

            migrationBuilder.DropTable(
                name: "doctor_especialidades");

            migrationBuilder.DropTable(
                name: "habitosvida");

            migrationBuilder.DropTable(
                name: "medicamentosprescritos");

            migrationBuilder.DropTable(
                name: "notificacion");

            migrationBuilder.DropTable(
                name: "PacienteAlergias");

            migrationBuilder.DropTable(
                name: "recursos_aprendizaje");

            migrationBuilder.DropTable(
                name: "estado_citas");

            migrationBuilder.DropTable(
                name: "especialidades");

            migrationBuilder.DropTable(
                name: "Alergias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_resultadoslaboratorio",
                table: "resultadoslaboratorio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_evaluacion",
                table: "evaluacion");

            migrationBuilder.DropIndex(
                name: "IX_evaluacion_id_capacitacion",
                table: "evaluacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_capacitacion",
                table: "capacitacion");

            migrationBuilder.DropColumn(
                name: "ArchivoPDF",
                table: "resultadoslaboratorio");

            migrationBuilder.DropColumn(
                name: "file_path",
                table: "evaluacion");

            migrationBuilder.DropColumn(
                name: "id_capacitacion",
                table: "evaluacion");

            migrationBuilder.RenameTable(
                name: "resultadoslaboratorio",
                newName: "resultados_laboratorio");

            migrationBuilder.RenameTable(
                name: "evaluacion",
                newName: "Evaluaciones");

            migrationBuilder.RenameTable(
                name: "capacitacion",
                newName: "Capacitaciones");

            migrationBuilder.RenameColumn(
                name: "fechaPrueba",
                table: "resultados_laboratorio",
                newName: "FechaPrueba");

            migrationBuilder.RenameColumn(
                name: "id_paciente",
                table: "resultados_laboratorio",
                newName: "IdPaciente");

            migrationBuilder.RenameColumn(
                name: "id_expediente",
                table: "resultados_laboratorio",
                newName: "IdExpediente");

            migrationBuilder.RenameColumn(
                name: "id_resultado",
                table: "resultados_laboratorio",
                newName: "IdResultado");

            migrationBuilder.RenameIndex(
                name: "IX_resultadoslaboratorio_id_paciente",
                table: "resultados_laboratorio",
                newName: "IX_resultados_laboratorio_IdPaciente");

            migrationBuilder.RenameIndex(
                name: "IX_resultadoslaboratorio_id_expediente",
                table: "resultados_laboratorio",
                newName: "IX_resultados_laboratorio_IdExpediente");

            migrationBuilder.RenameIndex(
                name: "IX_evaluacion_id_usuario",
                table: "Evaluaciones",
                newName: "IX_Evaluaciones_id_usuario");

            migrationBuilder.RenameIndex(
                name: "IX_capacitacion_id_usuario",
                table: "Capacitaciones",
                newName: "IX_Capacitaciones_id_usuario");

            migrationBuilder.AddColumn<string>(
                name: "Resultado",
                table: "resultados_laboratorio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoPrueba",
                table: "resultados_laboratorio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "tiempo_prueba",
                table: "Evaluaciones",
                type: "time",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "Evaluaciones",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "id_usuario",
                table: "Evaluaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "descripcion",
                table: "Evaluaciones",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "archivo",
                table: "Evaluaciones",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "id_usuario",
                table: "Capacitaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "duracion",
                table: "Capacitaciones",
                type: "time",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "archivo",
                table: "Capacitaciones",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_resultados_laboratorio",
                table: "resultados_laboratorio",
                column: "IdResultado");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Evaluaciones",
                table: "Evaluaciones",
                column: "id_evaluacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Capacitaciones",
                table: "Capacitaciones",
                column: "id_capacitacion");

            migrationBuilder.AddForeignKey(
                name: "FK_Capacitaciones_Usuarios_id_usuario",
                table: "Capacitaciones",
                column: "id_usuario",
                principalTable: "Usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluaciones_Usuarios_id_usuario",
                table: "Evaluaciones",
                column: "id_usuario",
                principalTable: "Usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_resultados_laboratorio_Expedientes_IdExpediente",
                table: "resultados_laboratorio",
                column: "IdExpediente",
                principalTable: "Expedientes",
                principalColumn: "id_expediente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_resultados_laboratorio_Usuarios_IdPaciente",
                table: "resultados_laboratorio",
                column: "IdPaciente",
                principalTable: "Usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
