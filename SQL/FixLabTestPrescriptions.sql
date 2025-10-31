-- Fix for Lab Test Prescriptions Database Issue
-- Run this script in your SQL Server Management Studio or database tool

USE [YourDatabaseName]
GO

-- Create the missing TblLabTestPrescriptions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TblLabTestPrescriptions' AND xtype='U')
BEGIN
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
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),

        -- Foreign Key Constraints
        CONSTRAINT FK_LabTestPrescriptions_Patient FOREIGN KEY (PatientId) REFERENCES TblPatients(PatientId),
        CONSTRAINT FK_LabTestPrescriptions_Doctor FOREIGN KEY (DoctorId) REFERENCES TblUsers(UserId),
        CONSTRAINT FK_LabTestPrescriptions_LabTest FOREIGN KEY (LabTestId) REFERENCES TblLabTests(LabTestId),
        CONSTRAINT FK_LabTestPrescriptions_Appointment FOREIGN KEY (AppointmentId) REFERENCES TblAppointments(AppointmentId),
        CONSTRAINT FK_LabTestPrescriptions_Technician FOREIGN KEY (AssignedTechnicianId) REFERENCES TblUsers(UserId)
    );

    -- Create indexes for better performance
    CREATE INDEX IX_LabTestPrescriptions_PatientId ON TblLabTestPrescriptions(PatientId);
    CREATE INDEX IX_LabTestPrescriptions_DoctorId ON TblLabTestPrescriptions(DoctorId);
    CREATE INDEX IX_LabTestPrescriptions_LabTestId ON TblLabTestPrescriptions(LabTestId);
    CREATE INDEX IX_LabTestPrescriptions_Status ON TblLabTestPrescriptions(Status);
    CREATE INDEX IX_LabTestPrescriptions_Priority ON TblLabTestPrescriptions(Priority);
    CREATE INDEX IX_LabTestPrescriptions_AssignedTechnicianId ON TblLabTestPrescriptions(AssignedTechnicianId);
    CREATE INDEX IX_LabTestPrescriptions_AppointmentId ON TblLabTestPrescriptions(AppointmentId);
    CREATE INDEX IX_LabTestPrescriptions_PrescriptionDate ON TblLabTestPrescriptions(PrescriptionDate);

    PRINT 'TblLabTestPrescriptions table created successfully!';
END
ELSE
BEGIN
    PRINT 'TblLabTestPrescriptions table already exists.';
END

-- Also ensure TblLabTests table exists (in case it's also missing)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TblLabTests' AND xtype='U')
BEGIN
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

    -- Insert sample lab tests
    INSERT INTO TblLabTests (TestCode, TestName, Description, Price, NormalRange, Unit, DepartmentId, IsActive)
    VALUES
    ('CBC001', 'Complete Blood Count', 'Complete blood analysis including RBC, WBC, platelets', 150.00, 'RBC: 4.2-5.4 million/μL, WBC: 4,000-11,000/μL', 'cells/μL', 1, 1),
    ('LFT001', 'Liver Function Test', 'Tests for liver enzymes and function', 200.00, 'ALT: 7-56 U/L, AST: 10-40 U/L', 'U/L', 1, 1),
    ('RFT001', 'Renal Function Test', 'Tests for kidney function', 180.00, 'Creatinine: 0.7-1.3 mg/dL', 'mg/dL', 1, 1),
    ('BS001', 'Blood Sugar', 'Fasting and post-prandial blood glucose', 100.00, 'Fasting: 70-100 mg/dL', 'mg/dL', 1, 1),
    ('LIPID001', 'Lipid Profile', 'Cholesterol and triglyceride levels', 250.00, 'Total Cholesterol: <200 mg/dL', 'mg/dL', 1, 1);

    PRINT 'TblLabTests table created successfully with sample data!';
END
ELSE
BEGIN
    PRINT 'TblLabTests table already exists.';
END

PRINT 'Database setup completed successfully! The lab technician module should now work properly.'
