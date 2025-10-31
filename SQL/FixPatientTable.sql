-- Run this script in SQL Server Management Studio
-- This will add the missing columns to TblPatients if needed

USE ClinicalManagementSystem
GO

-- Check if FirstName and LastName columns exist in TblPatients
IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('TblPatients') AND name = 'FirstName')
BEGIN
    -- Add missing columns to TblPatients if they don't exist
    ALTER TABLE TblPatients ADD FirstName NVARCHAR(50) NULL;
    ALTER TABLE TblPatients ADD LastName NVARCHAR(50) NULL;

    -- Update existing records (if any) with placeholder data
    UPDATE TblPatients SET FirstName = 'Unknown', LastName = 'Patient' WHERE FirstName IS NULL;

    -- Make columns NOT NULL after updating
    ALTER TABLE TblPatients ALTER COLUMN FirstName NVARCHAR(50) NOT NULL;
    ALTER TABLE TblPatients ALTER COLUMN LastName NVARCHAR(50) NOT NULL;

    PRINT '✅ Added FirstName and LastName columns to TblPatients';
END
ELSE
BEGIN
    PRINT '✅ FirstName and LastName columns already exist in TblPatients';
END

-- Ensure FullName computed column exists (for SQL queries)
IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('TblPatients') AND name = 'FullName')
BEGIN
    -- This might fail if computed columns aren't supported in your SQL version
    -- In that case, the application will use the C# FullName property instead
    EXEC('ALTER TABLE TblPatients ADD FullName AS (FirstName + '' '' + LastName) PERSISTED');
    PRINT '✅ Added FullName computed column to TblPatients';
END

PRINT '✅ Patient table structure verified!';
GO
