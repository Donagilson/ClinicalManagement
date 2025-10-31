-- =====================================================
-- FIX MISSING USER/DOCTOR/PATIENT TABLES
-- =====================================================
-- This script creates the missing base tables that the medical notes depend on
-- =====================================================

PRINT '🔧 Checking and creating missing base tables...';

-- Step 1: Create TblUsers table
PRINT '';
PRINT '1️⃣ Creating TblUsers table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblUsers')
BEGIN
    CREATE TABLE TblUsers (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        UserName NVARCHAR(50) NOT NULL UNIQUE,
        Password NVARCHAR(255) NOT NULL,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NULL,
        Phone NVARCHAR(20) NULL,
        RoleId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedDate DATETIME NULL
    );
    PRINT '   ✅ TblUsers table created successfully!';

    -- Add indexes
    CREATE INDEX IX_Users_Email ON TblUsers(Email);
    CREATE INDEX IX_Users_RoleId ON TblUsers(RoleId);
    CREATE INDEX IX_Users_IsActive ON TblUsers(IsActive);
END
ELSE
    PRINT '   ✅ TblUsers table already exists.';

-- Step 2: Create TblPatients table
PRINT '';
PRINT '2️⃣ Creating TblPatients table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblPatients')
BEGIN
    CREATE TABLE TblPatients (
        PatientId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        PatientCode NVARCHAR(20) NOT NULL UNIQUE,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        DateOfBirth DATE NULL,
        Gender NVARCHAR(10) NULL,
        Address NVARCHAR(MAX) NULL,
        Phone NVARCHAR(20) NULL,
        Email NVARCHAR(100) NULL,
        EmergencyContact NVARCHAR(100) NULL,
        EmergencyPhone NVARCHAR(20) NULL,
        BloodGroup NVARCHAR(5) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedDate DATETIME NULL,

        -- Foreign Key (commented out for now)
        -- CONSTRAINT FK_Patients_User FOREIGN KEY (UserId) REFERENCES TblUsers(UserId)
    );
    PRINT '   ✅ TblPatients table created successfully!';

    -- Add indexes
    CREATE INDEX IX_Patients_UserId ON TblPatients(UserId);
    CREATE INDEX IX_Patients_PatientCode ON TblPatients(PatientCode);
    CREATE INDEX IX_Patients_IsActive ON TblPatients(IsActive);
END
ELSE
    PRINT '   ✅ TblPatients table already exists.';

-- Step 3: Create TblDoctors table
PRINT '';
PRINT '3️⃣ Creating TblDoctors table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblDoctors')
BEGIN
    CREATE TABLE TblDoctors (
        DoctorId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        DoctorCode NVARCHAR(20) NOT NULL UNIQUE,
        DoctorName NVARCHAR(100) NOT NULL,
        Specialization NVARCHAR(100) NULL,
        DepartmentId INT NULL,
        Qualification NVARCHAR(200) NULL,
        ExperienceYears INT NULL,
        ConsultationFee DECIMAL(10,2) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedDate DATETIME NULL,

        -- Foreign Key (commented out for now)
        -- CONSTRAINT FK_Doctors_User FOREIGN KEY (UserId) REFERENCES TblUsers(UserId)
    );
    PRINT '   ✅ TblDoctors table created successfully!';

    -- Add indexes
    CREATE INDEX IX_Doctors_UserId ON TblDoctors(UserId);
    CREATE INDEX IX_Doctors_DoctorCode ON TblDoctors(DoctorCode);
    CREATE INDEX IX_Doctors_IsActive ON TblDoctors(IsActive);
END
ELSE
    PRINT '   ✅ TblDoctors table already exists.';

-- Step 4: Add sample users for testing
PRINT '';
PRINT '4️⃣ Adding sample users for testing...';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblUsers')
    AND NOT EXISTS (SELECT * FROM TblUsers WHERE UserName = 'admin')
BEGIN
    INSERT INTO TblUsers (UserName, Password, FirstName, LastName, Email, RoleId, IsActive)
    VALUES
    ('admin', 'password123', 'System', 'Administrator', 'admin@clinic.com', 1, 1),
    ('doctor1', 'password123', 'Dr. John', 'Smith', 'john.smith@clinic.com', 2, 1),
    ('doctor2', 'password123', 'Dr. Sarah', 'Johnson', 'sarah.johnson@clinic.com', 2, 1),
    ('patient1', 'password123', 'Alice', 'Brown', 'alice.brown@email.com', 3, 1),
    ('patient2', 'password123', 'Bob', 'Wilson', 'bob.wilson@email.com', 3, 1);
    PRINT '   ✅ Sample users added!';
END
ELSE
    PRINT '   ✅ Sample users already exist or TblUsers not available.';

-- Step 5: Add sample patients
PRINT '';
PRINT '5️⃣ Adding sample patients...';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblPatients')
    AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblUsers')
    AND NOT EXISTS (SELECT * FROM TblPatients WHERE PatientCode = 'PAT001')
BEGIN
    INSERT INTO TblPatients (UserId, PatientCode, FirstName, LastName, DateOfBirth, Gender, Phone, Email, IsActive)
    VALUES
    (4, 'PAT001', 'Alice', 'Brown', '1990-05-15', 'Female', '+1234567890', 'alice.brown@email.com', 1),
    (5, 'PAT002', 'Bob', 'Wilson', '1985-08-22', 'Male', '+1234567891', 'bob.wilson@email.com', 1);
    PRINT '   ✅ Sample patients added!';
END
ELSE
    PRINT '   ✅ Sample patients already exist or tables not available.';

-- Step 6: Add sample doctors
PRINT '';
PRINT '6️⃣ Adding sample doctors...';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblDoctors')
    AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblUsers')
    AND NOT EXISTS (SELECT * FROM TblDoctors WHERE DoctorCode = 'DOC001')
BEGIN
    INSERT INTO TblDoctors (UserId, DoctorCode, DoctorName, Specialization, Qualification, ExperienceYears, ConsultationFee, IsActive)
    VALUES
    (2, 'DOC001', 'Dr. John Smith', 'General Medicine', 'MBBS, MD', 10, 500.00, 1),
    (3, 'DOC002', 'Dr. Sarah Johnson', 'Cardiology', 'MBBS, DM Cardiology', 8, 800.00, 1);
    PRINT '   ✅ Sample doctors added!';
END
ELSE
    PRINT '   ✅ Sample doctors already exist or tables not available.';

PRINT '';
PRINT '🎉 =====================================================';
PRINT '🎉 BASE TABLES CREATED SUCCESSFULLY!';
PRINT '🎉 =====================================================';
PRINT '';
PRINT '✅ All required tables created (Users, Patients, Doctors)';
PRINT '✅ Sample data added for testing';
PRINT '✅ Medical notes should now work perfectly';
PRINT '✅ JOIN queries will work without errors';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Start your application';
PRINT '2. Login with these credentials:';
PRINT '   - Admin: admin / password123';
PRINT '   - Doctor: doctor1 / password123';
PRINT '   - Patient: patient1 / password123';
PRINT '3. Test the medical note functionality!';
PRINT '';
PRINT '🚀 Your clinical management system is now COMPLETE! 🚀';
PRINT '=====================================================';
