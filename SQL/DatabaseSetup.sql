-- =====================================================
-- Clinical Management System - Database Setup Script
-- =====================================================
-- Run these scripts in order to avoid foreign key errors
-- =====================================================

PRINT 'Starting Clinical Management System database setup...';

-- 1. Create Medical Notes Table (no dependencies)
PRINT 'Creating TblMedicalNotes...';
-- Copy and paste the contents of CreateMedicalNotesTable.sql here

-- 2. Create Lab Tests Table (no dependencies)
PRINT 'Creating TblLabTests...';
-- Copy and paste the contents of CreateLabTables.sql here

-- 3. Create Prescriptions Table (no dependencies)
PRINT 'Creating TblPrescriptions...';
-- Copy and paste the contents of CreatePrescriptionTables.sql here

-- 4. Create Lab Reports Table (depends on TblLabTests)
PRINT 'Creating TblLabReports...';
-- Copy and paste the TblLabReports section from CreateLabTables.sql here

-- =====================================================
-- After running this script successfully, you can uncomment
-- the foreign key constraints in each table script
-- =====================================================

PRINT 'Database setup completed successfully!';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Uncomment foreign key constraints in table scripts when referenced tables exist';
PRINT '2. Add sample data if needed';
PRINT '3. Test the application';
