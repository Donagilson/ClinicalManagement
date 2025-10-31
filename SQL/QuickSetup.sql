-- =====================================================
-- Quick Setup Script - Creates tables without foreign keys
-- =====================================================
-- Run this to get started quickly, then add FKs later
-- =====================================================

PRINT 'Creating tables without foreign key constraints...';

-- Create Medical Notes Table
CREATE TABLE TblMedicalNotes (
    MedicalNoteId INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    AppointmentId INT NULL,
    Title NVARCHAR(200) NOT NULL,
    Notes NVARCHAR(MAX) NOT NULL,
    Diagnosis NVARCHAR(MAX) NULL,
    Prescription NVARCHAR(MAX) NULL,
    FollowUpInstructions NVARCHAR(MAX) NULL,
    NextAppointmentDate DATETIME NULL,
    NoteType NVARCHAR(50) NOT NULL DEFAULT 'Consultation',
    Priority NVARCHAR(20) NOT NULL DEFAULT 'Normal',
    LabTests NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedBy INT NULL,
    UpdatedDate DATETIME NULL
);

-- Create Lab Tests Table
CREATE TABLE TblLabTests (
    LabTestId INT IDENTITY(1,1) PRIMARY KEY,
    TestCode NVARCHAR(20) NOT NULL UNIQUE,
    TestName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Price DECIMAL(10,2) NOT NULL,
    NormalRange NVARCHAR(200) NULL,
    Unit NVARCHAR(50) NULL,
    DepartmentId INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);

-- Create Lab Reports Table
CREATE TABLE TblLabReports (
    LabReportId INT IDENTITY(1,1) PRIMARY KEY,
    ReportCode NVARCHAR(20) NOT NULL UNIQUE,
    PatientId INT NOT NULL,
    LabTestId INT NOT NULL,
    DoctorId INT NOT NULL,
    LabTechnicianId INT NOT NULL,
    CollectionDate DATETIME NOT NULL,
    ReportDate DATETIME NULL,
    ResultValue NVARCHAR(100) NULL,
    ResultUnit NVARCHAR(50) NULL,
    NormalRange NVARCHAR(200) NULL,
    Findings NVARCHAR(MAX) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);

-- Create Prescriptions Table
CREATE TABLE TblPrescriptions (
    PrescriptionId INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentId INT NULL,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    Diagnosis NVARCHAR(MAX) NULL,
    PrescriptionDate DATETIME NOT NULL DEFAULT GETDATE(),
    Notes NVARCHAR(MAX) NULL
);

-- Create Prescription Details Table
CREATE TABLE TblPrescriptionDetails (
    PrescriptionDetailId INT IDENTITY(1,1) PRIMARY KEY,
    PrescriptionId INT NOT NULL,
    MedicineId INT NOT NULL,
    Dosage NVARCHAR(100) NULL,
    Frequency NVARCHAR(100) NULL,
    Duration NVARCHAR(100) NULL,
    Instructions NVARCHAR(500) NULL,
    Quantity INT NOT NULL
);

-- Create indexes
CREATE INDEX IX_MedicalNotes_PatientId ON TblMedicalNotes(PatientId);
CREATE INDEX IX_MedicalNotes_DoctorId ON TblMedicalNotes(DoctorId);
CREATE INDEX IX_LabTests_TestCode ON TblLabTests(TestCode);
CREATE INDEX IX_LabReports_PatientId ON TblLabReports(PatientId);
CREATE INDEX IX_Prescriptions_PatientId ON TblPrescriptions(PatientId);

PRINT 'All tables created successfully!';
PRINT 'You can now run your application.';
