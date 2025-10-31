-- =====================================================
-- QUICK FIX: ADD MISSING COLUMNS
-- =====================================================
-- Run this to add LabTests and Prescription columns
-- =====================================================

PRINT '🔧 Adding missing columns to TblMedicalNotes...';

-- Add LabTests column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'LabTests')
BEGIN
    ALTER TABLE TblMedicalNotes ADD LabTests NVARCHAR(MAX) NULL;
    PRINT '✅ LabTests column added!';
END
ELSE
    PRINT '✅ LabTests column already exists.';

-- Add Prescription column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'Prescription')
BEGIN
    ALTER TABLE TblMedicalNotes ADD Prescription NVARCHAR(MAX) NULL;
    PRINT '✅ Prescription column added!';
END
ELSE
    PRINT '✅ Prescription column already exists.';

-- Add other missing columns if needed
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'FollowUpInstructions')
BEGIN
    ALTER TABLE TblMedicalNotes ADD FollowUpInstructions NVARCHAR(MAX) NULL;
    PRINT '✅ FollowUpInstructions column added!';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblMedicalNotes' AND COLUMN_NAME = 'NextAppointmentDate')
BEGIN
    ALTER TABLE TblMedicalNotes ADD NextAppointmentDate DATETIME NULL;
    PRINT '✅ NextAppointmentDate column added!';
END

PRINT '';
PRINT '🎉 All missing columns added successfully!';
PRINT 'Your medical notes should now work perfectly!';
PRINT '';
PRINT 'Next: Start your application and test the medical note functionality.';
