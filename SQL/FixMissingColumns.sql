-- =====================================================
-- Add Missing LabTests Column to TblMedicalNotes
-- =====================================================
-- Run this if you get "invalid column called labtest" error
-- =====================================================

PRINT 'Checking and adding LabTests column...';

-- Check if LabTests column exists
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'LabTests'
)
BEGIN
    PRINT 'Adding LabTests column to TblMedicalNotes...';
    ALTER TABLE TblMedicalNotes ADD LabTests NVARCHAR(MAX) NULL;
    PRINT 'LabTests column added successfully!';
END
ELSE
BEGIN
    PRINT 'LabTests column already exists.';
END

-- Also check for other potentially missing columns
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'Prescription'
)
BEGIN
    PRINT 'Adding Prescription column to TblMedicalNotes...';
    ALTER TABLE TblMedicalNotes ADD Prescription NVARCHAR(MAX) NULL;
    PRINT 'Prescription column added successfully!';
END

PRINT 'Column check completed!';
