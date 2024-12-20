USE [master]
GO
/****** Object:  Database [ClinicaMedicaSur]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[Capacitaciones]    Script Date: 17/10/2024 03:55:48 ******/
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
PRIMARY KEY CLUSTERED 
(
	[id_capacitacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[citas]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[contabilidad]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[estado_citas]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[estado_ticket]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[Evaluaciones]    Script Date: 17/10/2024 03:55:48 ******/
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
PRIMARY KEY CLUSTERED 
(
	[id_evaluacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[expedientes]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[notificacion]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[roles]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[Tareas]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[tickets_citas]    Script Date: 17/10/2024 03:55:48 ******/
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
/****** Object:  Table [dbo].[usuarios]    Script Date: 17/10/2024 03:55:48 ******/
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

INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario]) VALUES (1, N'Uso Seguro de Equipos Médicos', N'Entrenamiento sobre el uso adecuado y seguro de equipos médicos.', CAST(N'02:30:00' AS Time), 2)
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario]) VALUES (2, N'Técnicas de Limpieza y Desinfección', N'Capacitación en técnicas de limpieza profunda y desinfección.', CAST(N'01:45:00' AS Time), 1)
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario]) VALUES (3, N'Seguridad en el Trabajo', N'Curso sobre medidas de seguridad y prevención de riesgos en el lugar de trabajo.', CAST(N'01:15:00' AS Time), 1)
INSERT [dbo].[Capacitaciones] ([id_capacitacion], [titulo], [descripcion], [duracion], [id_usuario]) VALUES (4, N'Respuesta en Situaciones de Emergencia', N'Entrenamiento en protocolos de emergencia y evacuación.', CAST(N'02:00:00' AS Time), 1)
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

INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario]) VALUES (2, N'Evaluación de Primeros Auxilios', N'Evaluación teórica y práctica sobre primeros auxilios', CAST(N'01:30:00' AS Time), N'primeros_auxilios.pdf', 1)
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario]) VALUES (3, N'Evaluación de Higiene', N'Evaluación de conocimientos sobre higiene en el lugar de trabajo', CAST(N'00:45:00' AS Time), N'higiene.docx', 2)
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario]) VALUES (4, N'Evaluación de Uso de Equipos', N'Evaluación sobre el uso adecuado de equipos médicos', CAST(N'01:15:00' AS Time), N'uso_equipos.pdf', 3)
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario]) VALUES (5, N'Evaluación de Limpieza', N'Evaluación de técnicas de limpieza y desinfección', CAST(N'01:00:00' AS Time), N'limpieza.pdf', 4)
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario]) VALUES (7, N'Evaluación de Emergencias', N'Evaluación de respuesta en situaciones de emergencia', CAST(N'01:00:00' AS Time), N'emergencias.pdf', 1)
INSERT [dbo].[Evaluaciones] ([id_evaluacion], [nombre], [descripcion], [tiempo_prueba], [archivo], [id_usuario]) VALUES (8, N'Evaluación de Atención al Paciente', N'Evaluación de protocolos de atención al paciente', CAST(N'01:20:00' AS Time), N'atencion_paciente.pdf', 2)
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
/****** Object:  Index [UQ__estado_c__5CEDF906E433CCD0]    Script Date: 17/10/2024 03:55:48 ******/
ALTER TABLE [dbo].[estado_citas] ADD UNIQUE NONCLUSTERED 
(
	[estado_nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__roles__673CB4353AA89723]    Script Date: 17/10/2024 03:55:48 ******/
ALTER TABLE [dbo].[roles] ADD UNIQUE NONCLUSTERED 
(
	[nombre_rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [unique_ticket_num]    Script Date: 17/10/2024 03:55:48 ******/
ALTER TABLE [dbo].[tickets_citas] ADD  CONSTRAINT [unique_ticket_num] UNIQUE NONCLUSTERED 
(
	[numero_ticket] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__usuarios__2A586E0BA3D334A6]    Script Date: 17/10/2024 03:55:48 ******/
ALTER TABLE [dbo].[usuarios] ADD UNIQUE NONCLUSTERED 
(
	[correo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[citas] ADD  DEFAULT (getdate()) FOR [fecha_creacion]
GO
ALTER TABLE [dbo].[contabilidad] ADD  DEFAULT (getdate()) FOR [fecha_registro]
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
ALTER TABLE [dbo].[usuarios]  WITH CHECK ADD CHECK  (([estado]='inactivo' OR [estado]='activo'))
GO
USE [master]
GO
ALTER DATABASE [ClinicaMedicaSur] SET  READ_WRITE 
GO
