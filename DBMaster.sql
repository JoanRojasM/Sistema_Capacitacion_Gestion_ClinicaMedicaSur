CREATE DATABASE ClinicaMedicaSur;

USE ClinicaMedicaSur;

CREATE TABLE roles (
    id_rol INT PRIMARY KEY IDENTITY(1,1),
    nombre_rol VARCHAR(50) UNIQUE
);

INSERT INTO roles (nombre_rol) VALUES
('asistente_limpieza'),
('asistente_medico'),
('paciente'),
('doctor'),
('administrador');

CREATE TABLE usuarios (
    id_usuario INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    apellido VARCHAR(100),
    correo VARCHAR(100) UNIQUE,
    contraseña VARCHAR(255), -- Almacenar contraseñas hasheadas
    telefono VARCHAR(15),
    id_rol INT,
	fecha_nacimiento DATE,
    fecha_registro DATETIME DEFAULT GETDATE(),
    estado VARCHAR(10) DEFAULT 'activo' CHECK (estado IN ('activo', 'inactivo')),
    FOREIGN KEY (id_rol) REFERENCES roles(id_rol)
);

CREATE TABLE capacitacion (
    id_capacitacion INT PRIMARY KEY IDENTITY(1,1),
    titulo NVARCHAR(255) NOT NULL,
    descripcion NVARCHAR(255) NULL,
    duracion NVARCHAR(255) NULL,
    id_usuario INT NULL,
    fecha_creacion DATETIME DEFAULT GETDATE(),
    estado VARCHAR(10) NULL,
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

CREATE TABLE recursos_aprendizaje (
    id_recurso INT PRIMARY KEY IDENTITY(1,1),
    id_capacitacion INT NOT NULL,
	titulo NVARCHAR(255) NOT NULL,
    archivo NVARCHAR(255) NULL,
    enlace NVARCHAR(500) NULL,
    fecha_creacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_capacitacion) REFERENCES capacitacion(id_capacitacion)
);

CREATE TABLE evaluacion (
    id_evaluacion INT PRIMARY KEY IDENTITY(1,1),
	id_capacitacion INT NOT NULL,
    nombre VARCHAR(100),
    descripcion TEXT,
    tiempo_prueba VARCHAR(255),
    archivo VARCHAR(255),
    id_usuario INT,
    fecha_creacion DATETIME DEFAULT GETDATE(),
	FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
	FOREIGN KEY (id_capacitacion) REFERENCES capacitacion(id_capacitacion)
);

CREATE TABLE estado_citas (
    id_estado_cita INT PRIMARY KEY IDENTITY(1,1),
    estado_nombre VARCHAR(50) UNIQUE
);

-- Insertar los valores de estado
INSERT INTO estado_citas (estado_nombre) VALUES
('programada'),
('cancelada'),
('completada');

CREATE TABLE citas (
    id_cita INT PRIMARY KEY IDENTITY(1,1),
    id_paciente INT,
    id_doctor INT,
    fecha_inicio DATETIME, -- Nueva columna para la fecha de inicio
    fecha_fin DATETIME, -- Nueva columna para la fecha de fin
    motivo_cita VARCHAR(255),
    id_estado_cita INT, -- Columna que referencia la tabla estado_citas
    fecha_creacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_doctor) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_estado_cita) REFERENCES estado_citas(id_estado_cita)
);

CREATE TABLE estado_ticket (
    id_estado INT PRIMARY KEY IDENTITY(1,1),
    estado_nombre VARCHAR(50)
);

INSERT INTO estado_ticket (estado_nombre) VALUES
('pendiente'),
('resuelto');

CREATE TABLE tickets_citas (
    id_ticket INT PRIMARY KEY IDENTITY(1,1),
    id_cita INT,
    numero_ticket VARCHAR(20),
    fecha_creacion DATETIME DEFAULT GETDATE(),
    id_estado_ticket INT, -- Columna que referencia la tabla estado_ticket
    FOREIGN KEY (id_cita) REFERENCES citas(id_cita),
    FOREIGN KEY (id_estado_ticket) REFERENCES estado_ticket(id_estado), -- Clave foránea hacia estado_ticket
    CONSTRAINT unique_ticket_num UNIQUE (numero_ticket) -- Restricción de unicidad para el número de ticket
);

CREATE TABLE disponibilidad_doctor (
    id_disponibilidad INT PRIMARY KEY IDENTITY(1,1),
    id_doctor INT,
    dia_semana VARCHAR(10), -- Día de la semana (ejemplo: 'Lunes', 'Martes', etc.)
    hora_inicio TIME, -- Hora de inicio de disponibilidad
    hora_fin TIME, -- Hora de fin de disponibilidad
    FOREIGN KEY (id_doctor) REFERENCES usuarios(id_usuario)
);

CREATE TABLE expedientes (
    id_expediente INT PRIMARY KEY IDENTITY(1,1),
    id_paciente INT NOT NULL,
    nombre_paciente TEXT NOT NULL,
    ultima_consulta DATETIME,
    diagnostico TEXT,
    descripcion TEXT,
    tratamientos_previos TEXT,
    fecha_creacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario),
);

CREATE TABLE contabilidad (
    id_contabilidad INT PRIMARY KEY IDENTITY(1,1),
    concepto VARCHAR(255) NOT NULL,
    monto DECIMAL(10, 2) NOT NULL,
    tipo VARCHAR(50) NOT NULL, -- "Ingreso" o "Gasto"
    fecha_registro DATETIME DEFAULT GETDATE()
);

CREATE TABLE notificacion (
    id_notificacion INT PRIMARY KEY IDENTITY(1,1),
    id_usuario INT NOT NULL,
    titulo NVARCHAR(255) NOT NULL,
    mensaje NVARCHAR(1000) NOT NULL,
    fecha_envio DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

CREATE TABLE resultadoslaboratorio (
    id_resultado INT PRIMARY KEY IDENTITY(1,1),
    id_paciente INT NOT NULL,
    id_expediente INT NOT NULL, -- Nueva columna para relacionar con expedientes
    fechaPrueba DATETIME NOT NULL,
    ArchivoPDF VARBINARY(MAX) NOT NULL, -- Columna para almacenar el archivo PDF
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario), -- Relación con la tabla usuarios
    FOREIGN KEY (id_expediente) REFERENCES expedientes(id_expediente) -- Relación con la tabla expedientes
);

CREATE TABLE Alergias (
    id_alergia INT PRIMARY KEY IDENTITY(1,1),
    nombre_alergia VARCHAR(255) NOT NULL
);

CREATE TABLE pacientealergias (
    id_pacientealergias INT PRIMARY KEY IDENTITY(1,1),
    id_paciente INT NOT NULL,
    id_alergia INT NOT NULL,
	fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_alergia) REFERENCES alergias(id_alergia)
);

CREATE TABLE contactosemergencia (
    id_contacto_emergencia INT PRIMARY KEY IDENTITY(1,1),
    id_paciente INT NOT NULL,
    nombre_contacto VARCHAR(100) NOT NULL,
    relacion VARCHAR(50) NOT NULL,
    telefono_contacto VARCHAR(15) NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario)
);

CREATE TABLE antecedentesfamiliares (
    id_antecedente INT PRIMARY KEY IDENTITY(1,1),
    id_paciente INT NOT NULL,
    descripcion NVARCHAR(1000) NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario)
);

CREATE TABLE habitosvida (
    id_habito INT PRIMARY KEY IDENTITY(1,1),
    id_paciente INT NOT NULL,
    descripcion NVARCHAR(1000) NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario)
);

CREATE TABLE medicamentosprescritos (
    id_medicamento INT PRIMARY KEY IDENTITY(1,1),
    id_paciente INT NOT NULL,
    nombre_medicamento NVARCHAR(255) NOT NULL,
    dosis NVARCHAR(100) NOT NULL,
    fecha_prescripcion DATETIME NOT NULL DEFAULT GETDATE(),
    estado NVARCHAR(20) NOT NULL DEFAULT 'activo',  -- activo o descontinuado
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario)
);

CREATE TABLE especialidades (
    id_especialidad INT PRIMARY KEY IDENTITY(1,1),
    nombre_especialidad NVARCHAR(255) NOT NULL
);

INSERT INTO especialidades (nombre_especialidad)
VALUES 
('Cirugía Menor'),
('Consulta de Medicina General'),
('Control de Enfermedades Crónicas'),
('Electrocardiograma'),
('Nebulizaciones'),
('Papanicolaou'),
('Terapia Física');

CREATE TABLE doctor_especialidades (
    id_doctor_especialidad INT PRIMARY KEY IDENTITY(1,1),
    id_usuario INT NOT NULL,
    id_especialidad INT NOT NULL,
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_especialidad) REFERENCES especialidades(id_especialidad)
);

-- Insertar un usuario para cada rol con estado "activo"
INSERT INTO usuarios (nombre, apellido, correo, contraseña, telefono, id_rol, fecha_nacimiento) VALUES
('Laura', 'Fernandez', 'laura.fernandez@example.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '5678901234', (SELECT id_rol FROM roles WHERE nombre_rol = 'administrador'), '1980-04-12');

-- Insertar doctores
INSERT INTO usuarios (nombre, apellido, correo, contraseña, telefono, id_rol, fecha_nacimiento) VALUES
('Dr. Francisco', 'Rodríguez', 'francisco.rodriguez@mail.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '12349876', 4, '1978-03-15'),
('Dra. Pepe', 'Lopez', 'pepe.lopez@mail.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '87651234', 4, '1985-07-30');

-- Disponibilidad para el Dr. Francisco Rodríguez
INSERT INTO disponibilidad_doctor (id_doctor, dia_semana, hora_inicio, hora_fin) VALUES
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Lunes', '09:00', '11:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Lunes', '14:00', '16:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Martes', '09:00', '11:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Martes', '14:00', '16:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Miércoles', '09:00', '11:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Miércoles', '14:00', '16:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Jueves', '09:00', '11:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Jueves', '14:00', '16:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Viernes', '09:00', '11:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Viernes', '14:00', '16:00');

-- Disponibilidad para la Dra. Pepe Lopez
INSERT INTO disponibilidad_doctor (id_doctor, dia_semana, hora_inicio, hora_fin) VALUES
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Lunes', '10:00', '12:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Lunes', '15:00', '17:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Martes', '10:00', '12:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Martes', '15:00', '17:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Miércoles', '10:00', '12:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Miércoles', '15:00', '17:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Jueves', '10:00', '12:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Jueves', '15:00', '17:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Viernes', '10:00', '12:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Viernes', '15:00', '17:00');

INSERT INTO Alergias (nombre_alergia) VALUES 
('Penicilina'),
('Polen'),
('Polvo'),
('Gluten'),
('Mariscos'),
('Frutos secos'),
('Picadura de abeja'),
('Leche'),
('Huevo'),
('Sulfatos'),
('Aspirina'),
('Antibióticos'),
('Frutas cítricas'),
('Chocolate'),
('Pescado'),
('Soya'),
('Aditivos alimentarios'),
('Moho'),
('Látex'),
('Gatos');

INSERT INTO doctor_especialidades (id_usuario, id_especialidad)
VALUES 
(8, 5),
(8, 6),
(8, 2),
(9, 7),
(9, 6),
(9, 4);

CREATE PROCEDURE [dbo].[RegistrarNotificacion]
    @id_usuario INT,
    @titulo NVARCHAR(255),
    @mensaje NVARCHAR(1000),
    @fecha_envio DATETIME = NULL
AS
BEGIN

    INSERT INTO notificacion (id_usuario, titulo, mensaje, fecha_envio)
    VALUES (@id_usuario, @titulo, @mensaje, GETDATE());
END;
