-- QUICK FIX: Run this in SQL Server Management Studio
-- Copy and paste this entire script into your SQL query window and execute it

USE ClinicalManagementSystem
GO

-- Drop table if exists and recreate it
IF EXISTS (SELECT * FROM sysobjects WHERE name='TblLabTestPrescriptions' AND xtype='U')
    DROP TABLE TblLabTestPrescriptions
GO

-- Create the missing TblLabTestPrescriptions table
CREATE TABLE TblLabTestPrescriptions (
    LabTestPrescriptionId INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentId INT NULL,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    LabTestId INT NOT NULL,
    PrescriptionDate DATETIME NOT NULL DEFAULT GETDATE(),
    ClinicalNotes NVARCHAR(500) NULL,
    Priority NVARCHAR(100) NOT NULL DEFAULT 'Normal',
    Status NVARCHAR(50) NOT NULL DEFAULT 'Prescribed',
    SampleCollectionDate DATETIME NULL,
    AssignedTechnicianId INT NULL,
    CompletedDate DATETIME NULL,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);

-- Create indexes for better performance
CREATE INDEX IX_LabTestPrescriptions_PatientId ON TblLabTestPrescriptions(PatientId);
CREATE INDEX IX_LabTestPrescriptions_DoctorId ON TblLabTestPrescriptions(DoctorId);
CREATE INDEX IX_LabTestPrescriptions_LabTestId ON TblLabTestPrescriptions(LabTestId);
CREATE INDEX IX_LabTestPrescriptions_Status ON TblLabTestPrescriptions(Status);
CREATE INDEX IX_LabTestPrescriptions_Priority ON TblLabTestPrescriptions(Priority);
CREATE INDEX IX_LabTestPrescriptions_AssignedTechnicianId ON TblLabTestPrescriptions(AssignedTechnicianId);

PRINT '✅ TblLabTestPrescriptions table created successfully!'
PRINT '✅ Lab Technician module should now work!'
GO
