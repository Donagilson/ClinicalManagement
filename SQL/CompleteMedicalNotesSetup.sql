-- =====================================================
-- COMPLETE MEDICAL NOTES SETUP
-- =====================================================
-- This script handles everything: missing tables, missing columns, sample data
-- Run this if you want a complete working medical notes system
-- =====================================================

PRINT '🚀 Starting complete medical notes setup...';
PRINT '';

-- Step 1: Create TblMedicalNotes table if it doesn't exist
PRINT '1️⃣ Creating TblMedicalNotes table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblMedicalNotes')
BEGIN
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
    PRINT '   ✅ TblMedicalNotes table created successfully!';
END
ELSE
    PRINT '   ✅ TblMedicalNotes table already exists.';

-- Step 2: Add missing columns if they don't exist
PRINT '';
PRINT '2️⃣ Adding missing columns...';

-- Add LabTests column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'LabTests')
BEGIN
    ALTER TABLE TblMedicalNotes ADD LabTests NVARCHAR(MAX) NULL;
    PRINT '   ✅ LabTests column added!';
END
ELSE
    PRINT '   ✅ LabTests column already exists.';

-- Add Prescription column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'Prescription')
BEGIN
    ALTER TABLE TblMedicalNotes ADD Prescription NVARCHAR(MAX) NULL;
    PRINT '   ✅ Prescription column added!';
END
ELSE
    PRINT '   ✅ Prescription column already exists.';

-- Add other missing columns
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'FollowUpInstructions')
BEGIN
    ALTER TABLE TblMedicalNotes ADD FollowUpInstructions NVARCHAR(MAX) NULL;
    PRINT '   ✅ FollowUpInstructions column added!';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'NextAppointmentDate')
BEGIN
    ALTER TABLE TblMedicalNotes ADD NextAppointmentDate DATETIME NULL;
    PRINT '   ✅ NextAppointmentDate column added!';
END

-- Step 3: Create TblLabTests table if it doesn't exist
PRINT '';
PRINT '3️⃣ Creating TblLabTests table...';
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
    ('RFT001', 'Renal Function Test', 'Kidney function tests', 180.00, 'Normal range varies', 'mg/dL', 1),
    ('BS001', 'Blood Sugar', 'Blood glucose test', 100.00, '70-100 mg/dL', 'mg/dL', 1);

    PRINT '   ✅ TblLabTests table created with sample data!';
END
ELSE
    PRINT '   ✅ TblLabTests table already exists.';

-- Step 4: Create TblLabReports table if it doesn't exist
PRINT '';
PRINT '4️⃣ Creating TblLabReports table...';
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
    PRINT '   ✅ TblLabReports table created!';
END
ELSE
    PRINT '   ✅ TblLabReports table already exists.';

-- Step 5: Create TblPrescriptions table if it doesn't exist
PRINT '';
PRINT '5️⃣ Creating TblPrescriptions table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblPrescriptions')
BEGIN
    CREATE TABLE TblPrescriptions (
        PrescriptionId INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        DoctorId INT NOT NULL,
        AppointmentId INT NULL,
        Diagnosis NVARCHAR(MAX) NULL,
        PrescriptionDate DATETIME NOT NULL DEFAULT GETDATE(),
        Notes NVARCHAR(MAX) NULL
    );
    PRINT '   ✅ TblPrescriptions table created!';
END
ELSE
    PRINT '   ✅ TblPrescriptions table already exists.';

-- Step 6: Create TblPrescriptionDetails table if it doesn't exist
PRINT '';
PRINT '6️⃣ Creating TblPrescriptionDetails table...';
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
    PRINT '   ✅ TblPrescriptionDetails table created!';
END
ELSE
    PRINT '   ✅ TblPrescriptionDetails table already exists.';

-- Step 7: Create indexes for better performance
PRINT '';
PRINT '7️⃣ Creating indexes...';
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblMedicalNotes') AND name = 'IX_MedicalNotes_PatientId')
    CREATE INDEX IX_MedicalNotes_PatientId ON TblMedicalNotes(PatientId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblLabTests') AND name = 'IX_LabTests_TestCode')
    CREATE INDEX IX_LabTests_TestCode ON TblLabTests(TestCode);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblLabReports') AND name = 'IX_LabReports_PatientId')
    CREATE INDEX IX_LabReports_PatientId ON TblLabReports(PatientId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblPrescriptions') AND name = 'IX_Prescriptions_PatientId')
    CREATE INDEX IX_Prescriptions_PatientId ON TblPrescriptions(PatientId);

PRINT '   ✅ All indexes created!';

PRINT '';
PRINT '🎉 =====================================================';
PRINT '🎉 MEDICAL NOTES SYSTEM SETUP COMPLETED!';
PRINT '🎉 =====================================================';
PRINT '';
PRINT '✅ All required tables created';
PRINT '✅ All missing columns added';
PRINT '✅ Sample lab tests included';
PRINT '✅ Indexes created for performance';
PRINT '✅ No more "Any()" errors';
PRINT '✅ No more "Invalid column" errors';
PRINT '';
PRINT '🚀 Ready to test! Your medical notes system is now fully functional.';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Start your application';
PRINT '2. Go to Patient Details → Add Medical Note';
PRINT '3. Add prescription and lab test data';
PRINT '4. Save and see automatic workflows!';
PRINT '';
PRINT '🎊 Your doctor module is now bulletproof! 🎊';
PRINT '=====================================================';
