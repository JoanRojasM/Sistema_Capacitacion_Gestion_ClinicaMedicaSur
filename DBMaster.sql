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
    contrase�a VARCHAR(255), -- Almacenar contrase�as hasheadas
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
    FOREIGN KEY (id_estado_ticket) REFERENCES estado_ticket(id_estado), -- Clave for�nea hacia estado_ticket
    CONSTRAINT unique_ticket_num UNIQUE (numero_ticket) -- Restricci�n de unicidad para el n�mero de ticket
);

CREATE TABLE disponibilidad_doctor (
    id_disponibilidad INT PRIMARY KEY IDENTITY(1,1),
    id_doctor INT,
    dia_semana VARCHAR(10), -- D�a de la semana (ejemplo: 'Lunes', 'Martes', etc.)
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
    FOREIGN KEY (id_paciente) REFERENCES usuarios(id_usuario), -- Relaci�n con la tabla usuarios
    FOREIGN KEY (id_expediente) REFERENCES expedientes(id_expediente) -- Relaci�n con la tabla expedientes
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
('Cirug�a Menor'),
('Consulta de Medicina General'),
('Control de Enfermedades Cr�nicas'),
('Electrocardiograma'),
('Nebulizaciones'),
('Papanicolaou'),
('Terapia F�sica');

CREATE TABLE doctor_especialidades (
    id_doctor_especialidad INT PRIMARY KEY IDENTITY(1,1),
    id_usuario INT NOT NULL,
    id_especialidad INT NOT NULL,
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_especialidad) REFERENCES especialidades(id_especialidad)
);

-- Insertar un usuario para cada rol con estado "activo"
INSERT INTO usuarios (nombre, apellido, correo, contrase�a, telefono, id_rol, fecha_nacimiento) VALUES
('Maria', 'Perez', 'maria.perez@example.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '1234567890', (SELECT id_rol FROM roles WHERE nombre_rol = 'asistente_limpieza'), '1985-02-15'),
('Carlos', 'Rodriguez', 'carlos.rodriguez@example.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '2345678901', (SELECT id_rol FROM roles WHERE nombre_rol = 'asistente_medico'), '1990-06-10'),
('Ana', 'Lopez', 'ana.lopez@example.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '3456789012', (SELECT id_rol FROM roles WHERE nombre_rol = 'paciente'), '1995-08-25'),
('Jorge', 'Martinez', 'jorge.martinez@example.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '4567890123', (SELECT id_rol FROM roles WHERE nombre_rol = 'doctor'), '1982-11-30'),
('Laura', 'Fernandez', 'laura.fernandez@example.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '5678901234', (SELECT id_rol FROM roles WHERE nombre_rol = 'administrador'), '1980-04-12');

-- Insertar pacientes
INSERT INTO usuarios (nombre, apellido, correo, contrase�a, telefono, id_rol, fecha_nacimiento) VALUES
('Juancho', 'Torres', 'juan.perez@mail.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '12345678', 3, '1992-01-20'),
('Anita', 'Flores', 'ana.gomez@mail.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '87654321', 3, '1993-05-05');

-- Insertar doctores
INSERT INTO usuarios (nombre, apellido, correo, contrase�a, telefono, id_rol, fecha_nacimiento) VALUES
('Dr. Francisco', 'Rodr�guez', 'francisco.rodriguez@mail.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '12349876', 4, '1978-03-15'),
('Dra. Pepe', 'Lopez', 'pepe.lopez@mail.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '87651234', 4, '1985-07-30');


INSERT INTO citas (id_paciente, id_doctor, fecha_inicio, fecha_fin, motivo_cita, id_estado_cita, fecha_creacion) 
VALUES 
(1, 2, '2024-11-01 14:00:00', '2024-11-01 15:00:00', 'Revisi�n General', 1, GETDATE()),

(3, 2, '2024-11-02 09:30:00', '2024-11-02 10:30:00', 'Consulta de Seguimiento', 1, GETDATE()),

(2, 1, '2024-11-03 11:00:00', '2024-11-03 12:00:00', 'Consulta de Resultados', 3, GETDATE());

-- Disponibilidad para el Dr. Francisco Rodr�guez
INSERT INTO disponibilidad_doctor (id_doctor, dia_semana, hora_inicio, hora_fin) VALUES
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Lunes', '09:00', '11:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Lunes', '14:00', '16:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Martes', '09:00', '11:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Martes', '14:00', '16:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Mi�rcoles', '09:00', '11:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'francisco.rodriguez@mail.com'), 'Mi�rcoles', '14:00', '16:00'),
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
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Mi�rcoles', '10:00', '12:00'),
((SELECT id_usuario FROM usuarios WHERE correo = 'pepe.lopez@mail.com'), 'Mi�rcoles', '15:00', '17:00'),
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
('Antibi�ticos'),
('Frutas c�tricas'),
('Chocolate'),
('Pescado'),
('Soya'),
('Aditivos alimentarios'),
('Moho'),
('L�tex'),
('Gatos');

INSERT INTO capacitacion (titulo, descripcion, duracion, id_usuario, estado) VALUES
('Introducci�n a la Seguridad Laboral', 'Capacitaci�n b�sica sobre medidas de seguridad en el trabajo para prevenir accidentes.', '02:00:00', 1, 'activo'),
('Limpieza y Desinfecci�n en �reas Cr�ticas', 'T�cnicas adecuadas para la limpieza y desinfecci�n de quir�fanos y �reas cr�ticas.', '02:00:00', 1, 'activo'),
('Primeros Auxilios B�sicos', 'Capacitaci�n para aprender t�cnicas de primeros auxilios en situaciones de emergencia.', '02:00:00', 2, 'activo'),
('Manejo de Residuos M�dicos', 'Procedimientos correctos para manejar y desechar residuos m�dicos de forma segura.', '02:00:00', 2, 'activo'),
('Uso Seguro de Productos Qu�micos', 'Precauciones y manejo adecuado de productos qu�micos utilizados en limpieza.', '02:00:00', 1, 'activo'),
('Relaciones Interpersonales en el Trabajo', 'Mejora de las habilidades de comunicaci�n y trabajo en equipo.', '02:00:00', 2, 'activo'),
('Prevenci�n de Infecciones', 'T�cnicas para prevenir la propagaci�n de infecciones en el entorno hospitalario.', '02:00:00', 2, 'activo'),
('Manejo de Herramientas de Limpieza', 'Uso y mantenimiento correcto de equipos de limpieza.', '02:00:00', 1, 'activo'),
('Protocolos de Emergencia en Hospitales', 'C�mo actuar en caso de emergencias dentro del hospital.', '02:00:00', 2, 'activo'),
('Convivencia y Respeto en el Entorno Laboral', 'Fomentar el respeto y la convivencia pac�fica en el entorno de trabajo.', '02:00:00', 1, 'activo'),
('Capacitaci�n en Ergonom�a', 'C�mo evitar lesiones relacionadas con malas posturas o esfuerzos innecesarios.', '02:00:00', 1, 'activo'),
('Higiene Personal y Profesional', 'Buenas pr�cticas de higiene personal para garantizar un entorno limpio.', '02:00:00', 2, 'activo'),
('Control de Plagas en Instalaciones M�dicas', 'M�todos para prevenir y controlar plagas en instalaciones de salud.', '02:00:00', 1, 'activo'),
('Atenci�n al Paciente con Movilidad Reducida', 'T�cnicas para asistir de manera segura a pacientes con movilidad limitada.', '02:00:00', 2, 'activo'),
('Recolecci�n y Separaci�n de Residuos', 'Normas para la recolecci�n y separaci�n correcta de residuos.', '02:00:00', 1, 'activo'),
('Actualizaci�n en Protocolos de Bioseguridad', 'Nuevas normativas y protocolos de bioseguridad en el entorno hospitalario.', '02:00:00', 2, 'activo'),
('T�cnicas de Desinfecci�n Avanzada', 'M�todos avanzados para desinfectar �reas cr�ticas.', '02:00:00', 1, 'activo'),
('Identificaci�n de Riesgos Laborales', 'C�mo identificar y mitigar riesgos en el entorno laboral.', '02:00:00', 2, 'activo'),
('Capacitaci�n en Manejo de Equipos de Protecci�n', 'Uso adecuado de equipos de protecci�n personal en el trabajo.', '02:00:00', 2, 'activo'),
('Organizaci�n y Planeaci�n de Tareas', 'C�mo organizar de manera eficiente las tareas diarias.', '02:00:00', 1, 'activo');

INSERT INTO citas (id_paciente, id_doctor, fecha_inicio, fecha_fin, motivo_cita, id_estado_cita, fecha_creacion) 
VALUES 
(3, 4, '2024-11-04 08:00:00', '2024-11-04 09:00:00', 'Consulta de Seguimiento', 1, GETDATE()),
(3, 4, '2024-11-04 10:00:00', '2024-11-04 11:00:00', 'Chequeo Preventivo', 2, GETDATE()),
(3, 4, '2024-11-05 14:00:00', '2024-11-05 15:00:00', 'Revisi�n General', 1, GETDATE()),
(3, 4, '2024-11-06 09:00:00', '2024-11-06 10:00:00', 'Consulta Inicial', 1, GETDATE()),
(3, 4, '2024-11-06 11:30:00', '2024-11-06 12:30:00', 'Evaluaci�n de Tratamiento', 3, GETDATE()),
(3, 4, '2024-11-07 15:00:00', '2024-11-07 16:00:00', 'Control de Peso', 1, GETDATE()),
(3, 4, '2024-11-08 08:30:00', '2024-11-08 09:30:00', 'Chequeo Preventivo', 2, GETDATE());

INSERT INTO contabilidad (concepto, monto, tipo, fecha_registro) 
VALUES 
('Pago de Suministros M�dicos', 150000, 'Gasto', GETDATE()),
('Ingreso por Consulta M�dica', 20000, 'Ingreso', GETDATE()),
('Compra de Material de Limpieza', 30000, 'Gasto', GETDATE()),
('Ingreso por Consulta de Seguimiento', 25000, 'Ingreso', GETDATE()),
('Pago a Proveedores de Equipos M�dicos', 500000, 'Gasto', GETDATE()),
('Ingreso por Revisi�n General', 18000, 'Ingreso', GETDATE()),
('Pago de Servicios de Mantenimiento de Equipos', 120000, 'Gasto', GETDATE()),
('Ingreso por Consulta Inicial', 22000, 'Ingreso', GETDATE()),
('Compra de Medicamentos', 80000, 'Gasto', GETDATE()),
('Ingreso por Consulta de Resultados', 20000, 'Ingreso', GETDATE()),
('Pago de Salarios al Personal', 1500000, 'Gasto', GETDATE()),
('Ingreso por Chequeo Preventivo', 15000, 'Ingreso', GETDATE()),
('Pago de Servicios de Limpieza', 60000, 'Gasto', GETDATE()),
('Ingreso por Control de Peso', 18000, 'Ingreso', GETDATE()),
('Compra de Insumos para Emergencias', 70000, 'Gasto', GETDATE()),
('Ingreso por Evaluaci�n de Tratamiento', 23000, 'Ingreso', GETDATE()),
('Pago de Seguros M�dicos', 200000, 'Gasto', GETDATE()),
('Ingreso por Control de Plagas', 50000, 'Ingreso', GETDATE()),
('Compra de Equipos de Protecci�n Personal', 120000, 'Gasto', GETDATE()),
('Ingreso por Capacitaci�n a Personal', 35000, 'Ingreso', GETDATE());

INSERT INTO usuarios (nombre, apellido, correo, contrase�a, telefono, id_rol, fecha_registro, estado)
VALUES ('Joan', 'Rojas', 'joanda0804@gmail.com', 'IaSw3izPQ21dyC4/iEyvyoPcxdbKPx2NeFIb/YMq8ko=:UxQo8LX8x/CeYISiLAWY4w==', '1234567890', 5, '2024-12-10 11:16:16.267', 'activo');�--123--

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
