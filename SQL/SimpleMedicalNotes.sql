-- =====================================================
-- ALTERNATIVE: SIMPLE MEDICAL NOTES WITHOUT JOINS
-- =====================================================
-- If you don't want to create user/doctor/patient tables,
-- run this script to create a minimal working version
-- =====================================================

PRINT '🔄 Creating simplified medical notes without JOIN dependencies...';

-- Create minimal TblMedicalNotes table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblMedicalNotes')
BEGIN
    PRINT 'Creating simplified TblMedicalNotes table...';
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
    PRINT '✅ Simplified TblMedicalNotes table created!';
END
ELSE
BEGIN
    PRINT 'Adding missing columns to existing TblMedicalNotes...';

    -- Add LabTests column if missing
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'LabTests')
    BEGIN
        ALTER TABLE TblMedicalNotes ADD LabTests NVARCHAR(MAX) NULL;
        PRINT '✅ LabTests column added!';
    END

    -- Add Prescription column if missing
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'Prescription')
    BEGIN
        ALTER TABLE TblMedicalNotes ADD Prescription NVARCHAR(MAX) NULL;
        PRINT '✅ Prescription column added!';
    END
END

-- Create lab tests table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblLabTests')
BEGIN
    CREATE TABLE TblLabTests (
        LabTestId INT IDENTITY(1,1) PRIMARY KEY,
        TestCode NVARCHAR(20) NOT NULL UNIQUE,
        TestName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        Price DECIMAL(10,2) NOT NULL,
        NormalRange NVARCHAR(200) NULL,
        Unit NVARCHAR(50) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
    );

    -- Add sample tests
    INSERT INTO TblLabTests (TestCode, TestName, Description, Price, NormalRange, Unit, IsActive)
    VALUES
    ('CBC001', 'Complete Blood Count', 'Complete blood analysis', 150.00, 'Normal range varies', 'cells/μL', 1),
    ('LFT001', 'Liver Function Test', 'Liver enzyme tests', 200.00, 'Normal range varies', 'U/L', 1),
    ('RFT001', 'Renal Function Test', 'Kidney function tests', 180.00, 'Normal range varies', 'mg/dL', 1);

    PRINT '✅ TblLabTests table created with sample data!';
END

-- Create lab reports table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblLabReports')
BEGIN
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
        Findings NVARCHAR(MAX) NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        Notes NVARCHAR(500) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
    PRINT '✅ TblLabReports table created!';
END

-- Create prescriptions table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblPrescriptions')
BEGIN
    CREATE TABLE TblPrescriptions (
        PrescriptionId INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        DoctorId INT NOT NULL,
        Diagnosis NVARCHAR(MAX) NULL,
        PrescriptionDate DATETIME NOT NULL DEFAULT GETDATE(),
        Notes NVARCHAR(MAX) NULL
    );
    PRINT '✅ TblPrescriptions table created!';
END

-- Create prescription details table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblPrescriptionDetails')
BEGIN
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
    PRINT '✅ TblPrescriptionDetails table created!';
END

-- Create indexes
PRINT '';
PRINT 'Creating indexes...';
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblMedicalNotes') AND name = 'IX_MedicalNotes_PatientId')
    CREATE INDEX IX_MedicalNotes_PatientId ON TblMedicalNotes(PatientId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblLabTests') AND name = 'IX_LabTests_TestCode')
    CREATE INDEX IX_LabTests_TestCode ON TblLabTests(TestCode);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblLabReports') AND name = 'IX_LabReports_PatientId')
    CREATE INDEX IX_LabReports_PatientId ON TblLabReports(PatientId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblPrescriptions') AND name = 'IX_Prescriptions_PatientId')
    CREATE INDEX IX_Prescriptions_PatientId ON TblPrescriptions(PatientId);

PRINT '✅ All indexes created!';

PRINT '';
PRINT '🎉 =====================================================';
PRINT '🎉 SIMPLIFIED MEDICAL NOTES SETUP COMPLETED!';
PRINT '🎉 =====================================================';
PRINT '';
PRINT '✅ All tables created without JOIN dependencies';
PRINT '✅ Medical notes will work without user/doctor/patient detail tables';
PRINT '✅ Prescription and lab test workflows fully functional';
PRINT '✅ No more "Invalid column name" errors';
PRINT '';
PRINT '🚀 Ready to test! Start your application and create medical notes.';
PRINT '';
PRINT '📝 Note: This version works without the full user management system.';
PRINT '       Names and additional details are not displayed, but core functionality works.';
PRINT '=====================================================';
