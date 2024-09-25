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
    contrasena VARCHAR(255), -- Almacenar contraseñas hasheadas
    telefono VARCHAR(15),
    id_rol INT,
    fecha_registro DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_rol) REFERENCES roles(id_rol)
);

CREATE TABLE capacitacion (
    id_capacitacion INT PRIMARY KEY IDENTITY(1,1),
    titulo VARCHAR(100),
    descripcion TEXT,
    fecha_inicio DATE,
    fecha_maxima DATE,
    duracion_horas INT,
    fecha_creacion DATETIME DEFAULT GETDATE()
);

CREATE TABLE estado_capacitacion (
    id_estado INT PRIMARY KEY IDENTITY(1,1),
    estado_nombre VARCHAR(50) UNIQUE
);

-- Insertar los valores de estado
INSERT INTO estado_capacitacion (estado_nombre) VALUES
('pendiente'),
('en_progreso'),
('completado');

CREATE TABLE capacitacion_usuarios (
    id_capacitacion_usuarios INT PRIMARY KEY IDENTITY(1,1),
    id_usuario INT,
    id_capacitacion INT,
    fecha_inscripcion DATETIME DEFAULT GETDATE(),
    nota DECIMAL(5,2) DEFAULT NULL,
    id_estado INT, -- Se usará para la referencia a la tabla estado_capacitacion
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_capacitacion) REFERENCES capacitacion(id_capacitacion),
    FOREIGN KEY (id_estado) REFERENCES estado_capacitacion(id_estado), -- Referencia a la tabla estado_capacitacion
    CONSTRAINT unique_user_cap UNIQUE (id_usuario, id_capacitacion) -- Restricción para evitar múltiples inscripciones
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
    fecha_cita DATETIME,
    motivo_cita VARCHAR(255),
    id_estado_cita INT, -- Columna que referencia la tabla estado_citas
    fecha_creacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_doctor) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_estado_cita) REFERENCES estado_citas(id_estado_cita) -- Relación con estado_citas
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
    id_paciente INT,
    descripcion TEXT,
    diagnostico TEXT,
    tratamiento TEXT,
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

-- Trigger para validar que solo los administradores pueden insertar datos en contabilidad
CREATE TRIGGER verificar_administrador
ON contabilidad
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @rol_usuario VARCHAR(50);
    
    -- Obtenemos el rol del usuario
    SELECT @rol_usuario = nombre_rol FROM roles
    INNER JOIN usuarios ON roles.id_rol = usuarios.id_rol
    WHERE usuarios.id_usuario = (SELECT id_usuario FROM inserted);
    
    -- Verificamos que el rol sea "administrador"
    IF @rol_usuario != 'administrador'
    BEGIN
        RAISERROR ('Solo los administradores pueden acceder a la contabilidad', 16, 1);
        ROLLBACK;
    END
    ELSE
    BEGIN
        INSERT INTO contabilidad(id_usuario, concepto, monto, fecha_registro)
        SELECT id_usuario, concepto, monto, fecha_registro FROM inserted;
    END
END;

CREATE TABLE notificacion (
    id_recordatorio INT PRIMARY KEY IDENTITY(1,1),
    id_usuario INT,
    tipo_recordatorio VARCHAR(50), -- 'cita' o 'capacitacion'
    id_referencia INT, -- Referencia a una cita o capacitación
    fecha_envio DATETIME,
    mensaje VARCHAR(255),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

-- Insertar un usuario para cada rol
INSERT INTO usuarios (nombre, apellido, correo, contrasena, telefono, id_rol)
VALUES
('Maria', 'Perez', 'maria.perez@example.com', '123', '1234567890', (SELECT id_rol FROM roles WHERE nombre_rol = 'asistente_limpieza')),
('Carlos', 'Rodriguez', 'carlos.rodriguez@example.com', '123', '2345678901', (SELECT id_rol FROM roles WHERE nombre_rol = 'asistente_medico')),
('Ana', 'Lopez', 'ana.lopez@example.com', '123', '3456789012', (SELECT id_rol FROM roles WHERE nombre_rol = 'paciente')),
('Jorge', 'Martinez', 'jorge.martinez@example.com', '123', '4567890123', (SELECT id_rol FROM roles WHERE nombre_rol = 'doctor')),
('Laura', 'Fernandez', 'laura.fernandez@example.com', '123', '5678901234', (SELECT id_rol FROM roles WHERE nombre_rol = 'administrador'));