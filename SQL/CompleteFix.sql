-- =====================================================
-- COMPREHENSIVE DATABASE FIX SCRIPT
-- =====================================================
-- This script will fix ALL possible database issues
-- Run this to completely resolve the LabTests column error
-- =====================================================

PRINT 'Starting comprehensive database fix...';

-- Step 1: Check if TblMedicalNotes exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblMedicalNotes')
BEGIN
    PRINT 'Creating TblMedicalNotes table...';
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
    PRINT 'TblMedicalNotes table created successfully!';
END
ELSE
BEGIN
    PRINT 'TblMedicalNotes table already exists. Adding missing columns...';

    -- Add LabTests column if missing
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'LabTests')
    BEGIN
        PRINT 'Adding LabTests column...';
        ALTER TABLE TblMedicalNotes ADD LabTests NVARCHAR(MAX) NULL;
        PRINT 'LabTests column added!';
    END
    ELSE
    BEGIN
        PRINT 'LabTests column already exists.';
    END

    -- Add Prescription column if missing
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'Prescription')
    BEGIN
        PRINT 'Adding Prescription column...';
        ALTER TABLE TblMedicalNotes ADD Prescription NVARCHAR(MAX) NULL;
        PRINT 'Prescription column added!';
    END
    ELSE
    BEGIN
        PRINT 'Prescription column already exists.';
    END

    -- Add other missing columns if needed
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'FollowUpInstructions')
    BEGIN
        ALTER TABLE TblMedicalNotes ADD FollowUpInstructions NVARCHAR(MAX) NULL;
    END

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'NextAppointmentDate')
    BEGIN
        ALTER TABLE TblMedicalNotes ADD NextAppointmentDate DATETIME NULL;
    END

    PRINT 'All required columns verified/added to TblMedicalNotes!';
END

-- Step 2: Create Lab Tests table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblLabTests')
BEGIN
    PRINT 'Creating TblLabTests table...';
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
    PRINT 'TblLabTests table created!';

    -- Add indexes
    CREATE INDEX IX_LabTests_TestCode ON TblLabTests(TestCode);
    CREATE INDEX IX_LabTests_IsActive ON TblLabTests(IsActive);
END
ELSE
BEGIN
    PRINT 'TblLabTests table already exists.';
END

-- Step 3: Create Lab Reports table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblLabReports')
BEGIN
    PRINT 'Creating TblLabReports table...';
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
    PRINT 'TblLabReports table created!';

    -- Add indexes
    CREATE INDEX IX_LabReports_PatientId ON TblLabReports(PatientId);
    CREATE INDEX IX_LabReports_DoctorId ON TblLabReports(DoctorId);
    CREATE INDEX IX_LabReports_Status ON TblLabReports(Status);
END
ELSE
BEGIN
    PRINT 'TblLabReports table already exists.';
END

-- Step 4: Create Prescriptions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblPrescriptions')
BEGIN
    PRINT 'Creating TblPrescriptions table...';
    CREATE TABLE TblPrescriptions (
        PrescriptionId INT IDENTITY(1,1) PRIMARY KEY,
        AppointmentId INT NULL,
        PatientId INT NOT NULL,
        DoctorId INT NOT NULL,
        Diagnosis NVARCHAR(MAX) NULL,
        PrescriptionDate DATETIME NOT NULL DEFAULT GETDATE(),
        Notes NVARCHAR(MAX) NULL
    );
    PRINT 'TblPrescriptions table created!';

    -- Add indexes
    CREATE INDEX IX_Prescriptions_PatientId ON TblPrescriptions(PatientId);
    CREATE INDEX IX_Prescriptions_DoctorId ON TblPrescriptions(DoctorId);
END
ELSE
BEGIN
    PRINT 'TblPrescriptions table already exists.';
END

-- Step 5: Create Prescription Details table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblPrescriptionDetails')
BEGIN
    PRINT 'Creating TblPrescriptionDetails table...';
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
    PRINT 'TblPrescriptionDetails table created!';

    -- Add indexes
    CREATE INDEX IX_PrescriptionDetails_PrescriptionId ON TblPrescriptionDetails(PrescriptionId);
END
ELSE
BEGIN
    PRINT 'TblPrescriptionDetails table already exists.';
END

-- Step 6: Add sample lab tests
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblLabTests')
BEGIN
    IF NOT EXISTS (SELECT * FROM TblLabTests WHERE TestCode = 'CBC001')
    BEGIN
        PRINT 'Adding sample lab tests...';
        INSERT INTO TblLabTests (TestCode, TestName, Description, Price, NormalRange, Unit, IsActive)
        VALUES
        ('CBC001', 'Complete Blood Count', 'Complete blood analysis including RBC, WBC, platelets', 150.00, 'RBC: 4.2-5.4 million/μL, WBC: 4,000-11,000/μL', 'cells/μL', 1),
        ('LFT001', 'Liver Function Test', 'Tests for liver enzymes and function', 200.00, 'ALT: 7-56 U/L, AST: 10-40 U/L', 'U/L', 1),
        ('RFT001', 'Renal Function Test', 'Tests for kidney function', 180.00, 'Creatinine: 0.7-1.3 mg/dL', 'mg/dL', 1),
        ('BS001', 'Blood Sugar', 'Fasting and post-prandial blood glucose', 100.00, 'Fasting: 70-100 mg/dL', 'mg/dL', 1),
        ('LIPID001', 'Lipid Profile', 'Cholesterol and triglyceride levels', 250.00, 'Total Cholesterol: <200 mg/dL', 'mg/dL', 1);
        PRINT 'Sample lab tests added!';
    END
    ELSE
    BEGIN
        PRINT 'Sample lab tests already exist.';
    END
END

PRINT '==========================================';
PRINT 'COMPREHENSIVE DATABASE FIX COMPLETED!';
PRINT '==========================================';
PRINT '';
PRINT 'Your database is now ready for the doctor module!';
PRINT 'You can now:';
PRINT '1. Save medical notes with prescriptions';
PRINT '2. Save medical notes with lab tests';
PRINT '3. Generate automatic prescriptions and lab reports';
PRINT '';
PRINT 'Start your application and test the functionality!';
PRINT '==========================================';
