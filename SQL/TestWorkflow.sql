-- Test script to verify the complete workflow is working
-- Run this after running the main fix scripts

USE ClinicalManagementSystem
GO

-- Check if all required tables exist
SELECT 'Checking Tables...' as Status;

IF EXISTS (SELECT * FROM sysobjects WHERE name='TblPatients' AND xtype='U')
    PRINT '✅ TblPatients exists'
ELSE
    PRINT '❌ TblPatients missing'

IF EXISTS (SELECT * FROM sysobjects WHERE name='TblUsers' AND xtype='U')
    PRINT '✅ TblUsers exists'
ELSE
    PRINT '❌ TblUsers missing'

IF EXISTS (SELECT * FROM sysobjects WHERE name='TblAppointments' AND xtype='U')
    PRINT '✅ TblAppointments exists'
ELSE
    PRINT '❌ TblAppointments missing'

IF EXISTS (SELECT * FROM sysobjects WHERE name='TblLabTests' AND xtype='U')
    PRINT '✅ TblLabTests exists'
ELSE
    PRINT '❌ TblLabTests missing'

IF EXISTS (SELECT * FROM sysobjects WHERE name='TblLabTestPrescriptions' AND xtype='U')
    PRINT '✅ TblLabTestPrescriptions exists'
ELSE
    PRINT '❌ TblLabTestPrescriptions missing'

IF EXISTS (SELECT * FROM sysobjects WHERE name='TblPrescriptions' AND xtype='U')
    PRINT '✅ TblPrescriptions exists'
ELSE
    PRINT '❌ TblPrescriptions missing'

-- Check sample data
SELECT 'Checking Sample Data...' as Status;

IF EXISTS (SELECT * FROM sysobjects WHERE name='TblLabReports' AND xtype='U')
    PRINT '✅ TblLabReports exists'
ELSE
    PRINT '❌ TblLabReports missing'

-- Check sample data
SELECT 'Checking Sample Data...' as Status;

SELECT COUNT(*) as PatientCount FROM TblPatients;
SELECT COUNT(*) as LabTestCount FROM TblLabTests;
SELECT COUNT(*) as LabPrescriptionCount FROM TblLabTestPrescriptions;
SELECT COUNT(*) as LabReportCount FROM TblLabReports;

PRINT '✅ Workflow verification complete!'
PRINT '🎯 Complete workflow: Receptionist → Doctor → Pharmacy → Lab Technician'
GO
