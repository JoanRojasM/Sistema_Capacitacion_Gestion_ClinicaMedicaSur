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
    fecha_registro DATETIME DEFAULT GETDATE(),
    estado VARCHAR(10) DEFAULT 'activo' CHECK (estado IN ('activo', 'inactivo')),
    FOREIGN KEY (id_rol) REFERENCES roles(id_rol)
);

CREATE TABLE capacitacion (
    id_capacitacion INT PRIMARY KEY IDENTITY(1,1),
    titulo NVARCHAR(255) NOT NULL,
    descripcion NVARCHAR(1000) NULL,
    duracion TIME(7) NULL,
    id_usuario INT NULL,
    fecha_creacion DATETIME DEFAULT GETDATE(),
    archivo NVARCHAR(255) NULL,
    estado VARCHAR(10) NULL,
	FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

CREATE TABLE evaluacion (
    id_evaluacion INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    descripcion TEXT,
    tiempo_prueba TIME(7),
    archivo VARCHAR(255),
    id_usuario INT,
    fecha_creacion DATETIME DEFAULT GETDATE(),
	FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
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

CREATE TABLE expedientes (
    id_expediente INT PRIMARY KEY IDENTITY(1,1),
    id_paciente INT NOT NULL,
    nombre_paciente TEXT NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    ultima_consulta DATETIME,
    diagnostico TEXT,
    descripcion TEXT,
    tratamientos_previos TEXT,
    fecha_creacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario)
);

CREATE TABLE contabilidad (
    id_contabilidad INT PRIMARY KEY IDENTITY(1,1),
    id_usuario INT,
    concepto VARCHAR(255),
    monto DECIMAL(10, 2),
    fecha_registro DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

CREATE TABLE notificacion (
    id_recordatorio INT PRIMARY KEY IDENTITY(1,1),
    id_usuario INT,
    tipo_recordatorio VARCHAR(50), -- 'cita' o 'capacitacion'
    id_referencia INT, -- Referencia a una cita o capacitación
    fecha_envio DATETIME,
    mensaje VARCHAR(255),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

CREATE TABLE resultadoslaboratorio (
    IdResultado INT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT NOT NULL,
    TipoPrueba VARCHAR(255) NOT NULL,
    Resultado VARCHAR(255) NOT NULL,
    FechaPrueba DATETIME NOT NULL,
    FOREIGN KEY (IdPaciente) REFERENCES usuarios(id_usuario) -- Relación con la tabla usuarios
);

-- Insertar un usuario para cada rol con estado "activo"
INSERT INTO usuarios (nombre, apellido, correo, contraseña, telefono, id_rol, estado)
VALUES
('Maria', 'Perez', 'maria.perez@example.com', '123', '1234567890', (SELECT id_rol FROM roles WHERE nombre_rol = 'asistente_limpieza'), 'activo'),
('Carlos', 'Rodriguez', 'carlos.rodriguez@example.com', '123', '2345678901', (SELECT id_rol FROM roles WHERE nombre_rol = 'asistente_medico'), 'activo'),
('Ana', 'Lopez', 'ana.lopez@example.com', '123', '3456789012', (SELECT id_rol FROM roles WHERE nombre_rol = 'paciente'), 'activo'),
('Jorge', 'Martinez', 'jorge.martinez@example.com', '123', '4567890123', (SELECT id_rol FROM roles WHERE nombre_rol = 'doctor'), 'activo'),
('Laura', 'Fernandez', 'laura.fernandez@example.com', '123', '5678901234', (SELECT id_rol FROM roles WHERE nombre_rol = 'administrador'), 'activo');

-- Insertar pacientes
INSERT INTO usuarios (nombre, apellido, correo, contraseña, telefono, id_rol) VALUES
('Juancho', 'Torres', 'juan.perez@mail.com', '123456', '12345678', 3),
('Anita', 'Flores', 'ana.gomez@mail.com', '123456', '87654321', 3);

-- Insertar doctores
INSERT INTO usuarios (nombre, apellido, correo, contraseña, telefono, id_rol) VALUES
('Dr. Francisco', 'Rodríguez', 'francisco.rodriguez@mail.com', '123456', '12349876', 4),
('Dra. Pepe', 'Lopez', 'pepe.lopez@mail.com', '123456', '87651234', 4);

INSERT INTO citas (id_paciente, id_doctor, fecha_inicio, fecha_fin, motivo_cita, id_estado_cita, fecha_creacion) 
VALUES 
(1, 2, '2024-11-01 14:00:00', '2024-11-01 15:00:00', 'Revisión General', 1, GETDATE()),

(3, 2, '2024-11-02 09:30:00', '2024-11-02 10:30:00', 'Consulta de Seguimiento', 1, GETDATE()),

(2, 1, '2024-11-03 11:00:00', '2024-11-03 12:00:00', 'Consulta de Resultados', 3, GETDATE());