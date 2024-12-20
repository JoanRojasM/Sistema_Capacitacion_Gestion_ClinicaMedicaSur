USE [master]
GO
/****** Object:  Database [ClinicaMedicaSur]    Script Date: 22/10/2024 20:19:50 ******/
CREATE DATABASE [ClinicaMedicaSur]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ClinicaMedicaSur', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ClinicaMedicaSur.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ClinicaMedicaSur_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ClinicaMedicaSur_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [ClinicaMedicaSur] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ClinicaMedicaSur].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ClinicaMedicaSur] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET ARITHABORT OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ClinicaMedicaSur] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ClinicaMedicaSur] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ClinicaMedicaSur] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ClinicaMedicaSur] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ClinicaMedicaSur] SET  MULTI_USER 
GO
ALTER DATABASE [ClinicaMedicaSur] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ClinicaMedicaSur] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ClinicaMedicaSur] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ClinicaMedicaSur] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ClinicaMedicaSur] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ClinicaMedicaSur] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [ClinicaMedicaSur] SET QUERY_STORE = OFF
GO
USE [ClinicaMedicaSur]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Capacitaciones]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Capacitaciones](
	[id_capacitacion] [int] IDENTITY(1,1) NOT NULL,
	[titulo] [nvarchar](255) NOT NULL,
	[descripcion] [nvarchar](1000) NULL,
	[duracion] [time](7) NULL,
	[id_usuario] [int] NULL,
	[fecha_creacion] [datetime] NULL,
	[archivo] [nvarchar](255) NULL,
	[estado] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_capacitacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[citas]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[citas](
	[id_cita] [int] IDENTITY(1,1) NOT NULL,
	[id_paciente] [int] NULL,
	[id_doctor] [int] NULL,
	[fecha_cita] [datetime] NULL,
	[motivo_cita] [varchar](255) NULL,
	[id_estado_cita] [int] NULL,
	[fecha_creacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_cita] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[contabilidad]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[contabilidad](
	[id_contabilidad] [int] IDENTITY(1,1) NOT NULL,
	[id_usuario] [int] NULL,
	[concepto] [varchar](255) NULL,
	[monto] [decimal](10, 2) NULL,
	[fecha_registro] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_contabilidad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[estado_citas]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[estado_citas](
	[id_estado_cita] [int] IDENTITY(1,1) NOT NULL,
	[estado_nombre] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_estado_cita] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[estado_ticket]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[estado_ticket](
	[id_estado] [int] IDENTITY(1,1) NOT NULL,
	[estado_nombre] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Evaluaciones]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Evaluaciones](
	[id_evaluacion] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](255) NOT NULL,
	[descripcion] [nvarchar](1000) NULL,
	[tiempo_prueba] [time](7) NULL,
	[archivo] [nvarchar](255) NULL,
	[id_usuario] [int] NULL,
	[fecha_creacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_evaluacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[expedientes]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[expedientes](
	[id_expediente] [int] IDENTITY(1,1) NOT NULL,
	[id_paciente] [int] NULL,
	[descripcion] [text] NULL,
	[diagnostico] [text] NULL,
	[tratamiento] [text] NULL,
	[fecha_creacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_expediente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[notificacion]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notificacion](
	[id_recordatorio] [int] IDENTITY(1,1) NOT NULL,
	[id_usuario] [int] NULL,
	[tipo_recordatorio] [varchar](50) NULL,
	[id_referencia] [int] NULL,
	[fecha_envio] [datetime] NULL,
	[mensaje] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_recordatorio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[roles](
	[id_rol] [int] IDENTITY(1,1) NOT NULL,
	[nombre_rol] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tareas]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tareas](
	[id_tarea] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](255) NOT NULL,
	[descripcion] [nvarchar](1000) NULL,
	[tiempo_prueba] [time](7) NULL,
	[archivo] [nvarchar](255) NULL,
	[id_usuario] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_tarea] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tickets_citas]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tickets_citas](
	[id_ticket] [int] IDENTITY(1,1) NOT NULL,
	[id_cita] [int] NULL,
	[numero_ticket] [varchar](20) NULL,
	[fecha_creacion] [datetime] NULL,
	[id_estado_ticket] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_ticket] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[usuarios]    Script Date: 22/10/2024 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[usuarios](
	[id_usuario] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](100) NULL,
	[apellido] [varchar](100) NULL,
	[correo] [varchar](100) NULL,
	[contraseña] [varchar](255) NULL,
	[telefono] [varchar](15) NULL,
	[id_rol] [int] NULL,
	[fecha_registro] [datetime] NULL,
	[estado] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Capacitaciones] ON 

INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (19, N'Técnicas de Limpieza y Desinfección', N'Capacitación en técnicas de limpieza profunda y desinfección.', CAST(N'01:00:00' AS Time), 1, CAST(N'2024-10-19T03:28:38.927' AS DateTime), N'archivoB.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (20, N'Seguridad en el Trabajo', N'Curso sobre medidas de seguridad y prevención de riesgos en el lugar de trabajo.', CAST(N'01:00:00' AS Time), 2, CAST(N'2024-10-19T03:28:38.927' AS DateTime), N'archivoC.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (21, N'Respuesta en Situaciones de Emergencia', N'Entrenamiento en protocolos de emergencia y evacuación.', CAST(N'00:45:00' AS Time), 2, CAST(N'2024-10-19T03:28:38.927' AS DateTime), N'archivoD.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (22, N'Capacitación en Higiene Laboral', N'Capacitación sobre las mejores prácticas de higiene en el entorno laboral', CAST(N'01:30:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'higiene_laboral.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (23, N'Uso Seguro de Equipos de Limpieza', N'Capacitación sobre el uso seguro y mantenimiento de equipos de limpieza', CAST(N'01:00:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'equipos_limpieza.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (24, N'Seguridad en el Trabajo', N'Capacitación sobre la seguridad y prevención de riesgos laborales', CAST(N'02:00:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'seguridad_trabajo.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (25, N'Atención de Emergencias Médicas', N'Capacitación sobre el manejo de emergencias en un entorno médico', CAST(N'02:15:00' AS Time), 2, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'emergencias_medicas.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (26, N'Manejo de Residuos Peligrosos', N'Capacitación sobre el manejo y eliminación de residuos peligrosos', CAST(N'01:15:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'residuos_peligrosos.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (27, N'Ética Profesional Médica', N'Capacitación sobre la ética en el ejercicio de la medicina', CAST(N'01:30:00' AS Time), 2, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'etica_profesional_medica.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (28, N'Uso de Equipos de Protección Personal', N'Capacitación sobre el uso correcto de equipos de protección personal', CAST(N'01:45:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'proteccion_personal.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (29, N'Tratamiento de Pacientes', N'Capacitación sobre el tratamiento adecuado de pacientes en un entorno clínico', CAST(N'02:30:00' AS Time), 2, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'tratamiento_pacientes.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (30, N'Limpieza de Áreas Críticas', N'Capacitación sobre la desinfección de áreas críticas dentro de la clínica', CAST(N'01:45:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'limpieza_areas_criticas.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (31, N'Manejo de Herramientas Médicas', N'Capacitación sobre el manejo seguro de herramientas médicas', CAST(N'02:15:00' AS Time), 2, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'herramientas_medicas.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (32, N'Conocimiento de Productos de Limpieza', N'Capacitación sobre los productos de limpieza utilizados en la clínica', CAST(N'01:00:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'productos_limpieza.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (33, N'Administración de Medicamentos', N'Capacitación sobre la administración segura de medicamentos', CAST(N'02:00:00' AS Time), 2, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'administracion_medicamentos.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (34, N'Procedimientos de Limpieza', N'Capacitación sobre procedimientos estándar de limpieza y esterilización', CAST(N'01:20:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'procedimientos_limpieza.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (35, N'Bioseguridad en Áreas Médicas', N'Capacitación sobre bioseguridad y prevención de infecciones', CAST(N'02:15:00' AS Time), 2, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'bioseguridad_medica.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (36, N'Responsabilidad en el Trabajo de Limpieza', N'Capacitación sobre responsabilidad y compromiso en el área de limpieza', CAST(N'01:30:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'responsabilidad_trabajo_limpieza.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (37, N'Procedimientos Médicos Básicos', N'Capacitación sobre procedimientos médicos básicos para asistentes médicos', CAST(N'02:10:00' AS Time), 2, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'procedimientos_medicos_basicos.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (38, N'Bioseguridad en la Limpieza', N'Capacitación sobre la bioseguridad en el trabajo de limpieza', CAST(N'01:35:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'bioseguridad_limpieza.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (39, N'Técnicas Avanzadas en Asistencia Médica', N'Capacitación sobre técnicas avanzadas para asistentes médicos', CAST(N'02:25:00' AS Time), 2, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'tecnicas_avanzadas_asistencia.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (40, N'Técnicas de Lavado de Manos', N'Capacitación sobre las técnicas correctas de lavado de manos para la prevención de infecciones', CAST(N'00:45:00' AS Time), 1, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'lavado_manos.pdf', N'Pendiente')
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario], [fecha_creacion], [archivo], [estado]) VALUES (41, N'Documentación Clínica', N'Capacitación sobre el manejo de la documentación clínica en la clínica médica', CAST(N'02:00:00' AS Time), 2, CAST(N'2024-10-19T03:45:28.553' AS DateTime), N'documentacion_clinica.pdf', N'Pendiente')
SET IDENTITY_INSERT [dbo].[Capacitaciones] OFF
GO
SET IDENTITY_INSERT [dbo].[estado_citas] ON 

INSERT [dbo].[estado_citas] ([id_estado_cita], [estado_nombre]) VALUES (2, N'cancelada')
INSERT [dbo].[estado_citas] ([id_estado_cita], [estado_nombre]) VALUES (3, N'completada')
INSERT [dbo].[estado_citas] ([id_estado_cita], [estado_nombre]) VALUES (1, N'programada')
SET IDENTITY_INSERT [dbo].[estado_citas] OFF
GO
SET IDENTITY_INSERT [dbo].[estado_ticket] ON 

INSERT [dbo].[estado_ticket] ([id_estado], [estado_nombre]) VALUES (1, N'pendiente')
INSERT [dbo].[estado_ticket] ([id_estado], [estado_nombre]) VALUES (2, N'resuelto')
SET IDENTITY_INSERT [dbo].[estado_ticket] OFF
GO
SET IDENTITY_INSERT [dbo].[Evaluaciones] ON 

INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (15, N'Uso Seguro de Equipos Médicos', N'Entrenamiento sobre el uso adecuado y seguro de equipos médicos.', CAST(N'01:30:00' AS Time), N'archivoA.pdf', 2, CAST(N'2024-10-19T03:02:58.520' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (17, N'Seguridad en el Trabajo', N'Curso sobre medidas de seguridad y prevención de riesgos en el lugar de trabajo.', CAST(N'01:00:00' AS Time), N'archivoC.pdf', 2, CAST(N'2024-10-19T03:02:58.520' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (18, N'Respuesta en Situaciones de Emergencia', N'Entrenamiento en protocolos de emergencia y evacuación.', CAST(N'00:45:00' AS Time), N'archivoD.pdf', 2, CAST(N'2024-10-19T03:02:58.520' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (19, N'Evaluación de Higiene', N'Evaluación sobre las mejores prácticas de higiene en el trabajo', CAST(N'01:30:00' AS Time), N'higiene_limpieza.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (20, N'Evaluación de Equipos de Limpieza', N'Examen sobre el uso y mantenimiento de equipos de limpieza', CAST(N'01:00:00' AS Time), N'equipos_limpieza.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (21, N'Evaluación de Seguridad en el Trabajo', N'Evaluación de seguridad en el ambiente de trabajo', CAST(N'02:00:00' AS Time), N'seguridad_trabajo.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (22, N'Capacitación de Primeros Auxilios', N'Examen sobre primeros auxilios y atención médica inmediata', CAST(N'02:00:00' AS Time), N'primeros_auxilios.pdf', 2, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (23, N'Evaluación de Manejo de Residuos', N'Examen sobre la gestión y eliminación de residuos', CAST(N'01:15:00' AS Time), N'manejo_residuos.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (24, N'Evaluación de Ética Profesional', N'Examen de ética para asistentes médicos', CAST(N'01:30:00' AS Time), N'etica_profesional.pdf', 2, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (25, N'Capacitación de Uso de EPIs', N'Evaluación sobre el uso correcto de equipos de protección individual', CAST(N'01:45:00' AS Time), N'uso_epi.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (26, N'Evaluación de Tratamiento de Pacientes', N'Evaluación sobre el tratamiento adecuado de pacientes', CAST(N'02:15:00' AS Time), N'tratamiento_pacientes.pdf', 2, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (27, N'Capacitación en Limpeza de Áreas Críticas', N'Examen sobre limpieza y desinfección de áreas críticas', CAST(N'01:45:00' AS Time), N'limpieza_areas_criticas.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (28, N'Capacitación en Manejo de Herramientas Médicas', N'Examen sobre el uso correcto de herramientas médicas', CAST(N'02:30:00' AS Time), N'herramientas_medicas.pdf', 2, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (29, N'Evaluación de Productos de Limpieza', N'Conocimiento sobre productos de limpieza utilizados en la clínica', CAST(N'01:00:00' AS Time), N'productos_limpieza.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (30, N'Capacitación de Emergencias Médicas', N'Examen sobre cómo actuar ante emergencias médicas', CAST(N'02:00:00' AS Time), N'emergencias_medicas.pdf', 2, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (31, N'Evaluación de Procedimientos de Limpieza', N'Examen sobre los procedimientos de limpieza y esterilización', CAST(N'01:20:00' AS Time), N'procedimientos_limpieza.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (32, N'Capacitación en Administración de Medicamentos', N'Evaluación sobre administración segura de medicamentos', CAST(N'02:15:00' AS Time), N'administracion_medicamentos.pdf', 2, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (33, N'Evaluación de Responsabilidad Laboral', N'Examen sobre la responsabilidad en el trabajo de limpieza', CAST(N'01:30:00' AS Time), N'responsabilidad_laboral.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (34, N'Capacitación en Procedimientos Médicos', N'Evaluación sobre procedimientos médicos estándar', CAST(N'02:10:00' AS Time), N'procedimientos_medicos.pdf', 2, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (35, N'Capacitación de Bioseguridad', N'Examen sobre bioseguridad en áreas de limpieza', CAST(N'01:35:00' AS Time), N'bioseguridad_limpieza.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (36, N'Evaluación en Técnicas Médicas Avanzadas', N'Evaluación sobre técnicas avanzadas de asistencia médica', CAST(N'02:25:00' AS Time), N'tecnicas_medicas_avanzadas.pdf', 2, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (37, N'Capacitación en Lavado de Manos', N'Evaluación sobre las técnicas adecuadas para el lavado de manos', CAST(N'00:45:00' AS Time), N'lavado_manos.pdf', 1, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario], [fecha_creacion]) VALUES (38, N'Evaluación de Documentación Clínica', N'Examen sobre el manejo de la documentación clínica', CAST(N'02:00:00' AS Time), N'documentacion_clinica.pdf', 2, CAST(N'2024-10-19T03:40:07.643' AS DateTime))
SET IDENTITY_INSERT [dbo].[Evaluaciones] OFF
GO
SET IDENTITY_INSERT [dbo].[roles] ON 

INSERT [dbo].[roles] ([id_rol], [nombre_rol]) VALUES (5, N'administrador')
INSERT [dbo].[roles] ([id_rol], [nombre_rol]) VALUES (1, N'asistente_limpieza')
INSERT [dbo].[roles] ([id_rol], [nombre_rol]) VALUES (2, N'asistente_medico')
INSERT [dbo].[roles] ([id_rol], [nombre_rol]) VALUES (4, N'doctor')
INSERT [dbo].[roles] ([id_rol], [nombre_rol]) VALUES (3, N'paciente')
SET IDENTITY_INSERT [dbo].[roles] OFF
GO
SET IDENTITY_INSERT [dbo].[Tareas] ON 

INSERT [dbo].[Tareas] ([id_tarea], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario]) VALUES (1, N'Diseño de Base de Datos', N'Crear una base de datos con todas las tablas necesarias para el sistema', CAST(N'02:30:00' AS Time), N'diseno_base_datos.sql', 1)
SET IDENTITY_INSERT [dbo].[Tareas] OFF
GO
SET IDENTITY_INSERT [dbo].[usuarios] ON 

INSERT [dbo].[usuarios] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [telefono], [id_rol], [fecha_registro], [estado]) VALUES (1, N'Maria', N'Perez', N'maria.perez@example.com', N'123', N'1234567890', 1, CAST(N'2024-10-12T00:56:36.930' AS DateTime), N'activo')
INSERT [dbo].[usuarios] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [telefono], [id_rol], [fecha_registro], [estado]) VALUES (2, N'Carlos', N'Rodriguez', N'carlos.rodriguez@example.com', N'123', N'2345678901', 2, CAST(N'2024-10-12T00:56:36.930' AS DateTime), N'activo')
INSERT [dbo].[usuarios] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [telefono], [id_rol], [fecha_registro], [estado]) VALUES (3, N'Ana', N'Lopez', N'ana.lopez@example.com', N'123', N'3456789012', 3, CAST(N'2024-10-12T00:56:36.930' AS DateTime), N'activo')
INSERT [dbo].[usuarios] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [telefono], [id_rol], [fecha_registro], [estado]) VALUES (4, N'Jorge', N'Martinez', N'jorge.martinez@example.com', N'123', N'4567890123', 4, CAST(N'2024-10-12T00:56:36.930' AS DateTime), N'activo')
INSERT [dbo].[usuarios] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [telefono], [id_rol], [fecha_registro], [estado]) VALUES (5, N'Laura', N'Fernandez', N'laura.fernandez@example.com', N'123', N'5678901234', 5, CAST(N'2024-10-12T00:56:36.930' AS DateTime), N'activo')
SET IDENTITY_INSERT [dbo].[usuarios] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__estado_c__5CEDF906E433CCD0]    Script Date: 22/10/2024 20:19:50 ******/
ALTER TABLE [dbo].[estado_citas] ADD UNIQUE NONCLUSTERED 
(
	[estado_nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__roles__673CB4353AA89723]    Script Date: 22/10/2024 20:19:50 ******/
ALTER TABLE [dbo].[roles] ADD UNIQUE NONCLUSTERED 
(
	[nombre_rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [unique_ticket_num]    Script Date: 22/10/2024 20:19:50 ******/
ALTER TABLE [dbo].[tickets_citas] ADD  CONSTRAINT [unique_ticket_num] UNIQUE NONCLUSTERED 
(
	[numero_ticket] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__usuarios__2A586E0BA3D334A6]    Script Date: 22/10/2024 20:19:50 ******/
ALTER TABLE [dbo].[usuarios] ADD UNIQUE NONCLUSTERED 
(
	[correo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Capacitaciones] ADD  DEFAULT (getdate()) FOR [fecha_creacion]
GO
ALTER TABLE [dbo].[Capacitaciones] ADD  DEFAULT ('Pendiente') FOR [estado]
GO
ALTER TABLE [dbo].[citas] ADD  DEFAULT (getdate()) FOR [fecha_creacion]
GO
ALTER TABLE [dbo].[contabilidad] ADD  DEFAULT (getdate()) FOR [fecha_registro]
GO
ALTER TABLE [dbo].[Evaluaciones] ADD  DEFAULT (getdate()) FOR [fecha_creacion]
GO
ALTER TABLE [dbo].[expedientes] ADD  DEFAULT (getdate()) FOR [fecha_creacion]
GO
ALTER TABLE [dbo].[tickets_citas] ADD  DEFAULT (getdate()) FOR [fecha_creacion]
GO
ALTER TABLE [dbo].[usuarios] ADD  DEFAULT (getdate()) FOR [fecha_registro]
GO
ALTER TABLE [dbo].[usuarios] ADD  DEFAULT ('activo') FOR [estado]
GO
ALTER TABLE [dbo].[Capacitaciones]  WITH CHECK ADD FOREIGN KEY([id_usuario])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[citas]  WITH CHECK ADD FOREIGN KEY([id_doctor])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[citas]  WITH CHECK ADD FOREIGN KEY([id_estado_cita])
REFERENCES [dbo].[estado_citas] ([id_estado_cita])
GO
ALTER TABLE [dbo].[citas]  WITH CHECK ADD FOREIGN KEY([id_paciente])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[contabilidad]  WITH CHECK ADD FOREIGN KEY([id_usuario])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[Evaluaciones]  WITH CHECK ADD FOREIGN KEY([id_usuario])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[expedientes]  WITH CHECK ADD FOREIGN KEY([id_paciente])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[notificacion]  WITH CHECK ADD FOREIGN KEY([id_usuario])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[Tareas]  WITH CHECK ADD FOREIGN KEY([id_usuario])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[tickets_citas]  WITH CHECK ADD FOREIGN KEY([id_cita])
REFERENCES [dbo].[citas] ([id_cita])
GO
ALTER TABLE [dbo].[tickets_citas]  WITH CHECK ADD FOREIGN KEY([id_estado_ticket])
REFERENCES [dbo].[estado_ticket] ([id_estado])
GO
ALTER TABLE [dbo].[usuarios]  WITH CHECK ADD FOREIGN KEY([id_rol])
REFERENCES [dbo].[roles] ([id_rol])
GO
ALTER TABLE [dbo].[Capacitaciones]  WITH CHECK ADD CHECK  (([estado]='Completada' OR [estado]='Pendiente'))
GO
ALTER TABLE [dbo].[usuarios]  WITH CHECK ADD CHECK  (([estado]='inactivo' OR [estado]='activo'))
GO
USE [master]
GO
ALTER DATABASE [ClinicaMedicaSur] SET  READ_WRITE 
GO
