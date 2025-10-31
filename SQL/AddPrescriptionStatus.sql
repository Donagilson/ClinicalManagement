-- =====================================================
-- PHARMACY PRESCRIPTION INTEGRATION - DATABASE UPDATES
-- =====================================================
-- Run this script to add prescription status tracking for pharmacy integration

USE [ClinicalManagementSystemDB]
GO

-- Add Status and FulfilledDate columns to TblPrescriptions
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TblPrescriptions') AND name = 'Status')
BEGIN
    ALTER TABLE TblPrescriptions
    ADD Status NVARCHAR(50) DEFAULT 'Pending' NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TblPrescriptions') AND name = 'FulfilledDate')
BEGIN
    ALTER TABLE TblPrescriptions
    ADD FulfilledDate DATETIME NULL;
END
GO

-- Update existing prescriptions to have Pending status
UPDATE TblPrescriptions
SET Status = 'Pending'
WHERE Status IS NULL;
GO

-- Add index for better performance on Status queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblPrescriptions') AND name = 'IX_Prescriptions_Status')
BEGIN
    CREATE INDEX IX_Prescriptions_Status ON TblPrescriptions(Status);
END
GO

-- Add index for FulfilledDate queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblPrescriptions') AND name = 'IX_Prescriptions_FulfilledDate')
BEGIN
    CREATE INDEX IX_Prescriptions_FulfilledDate ON TblPrescriptions(FulfilledDate);
END
GO

-- Display success message
PRINT '✅ Prescription Status Tracking Added Successfully!';
PRINT '';
PRINT '📋 Database Updates Applied:';
PRINT '   - Added Status column (NVARCHAR(50)) with default "Pending"';
PRINT '   - Added FulfilledDate column (DATETIME) for tracking completion';
PRINT '   - Created indexes for better query performance';
PRINT '   - Updated existing prescriptions to "Pending" status';
PRINT '';
PRINT '🚀 Pharmacy Integration Features Now Available:';
PRINT '   - Prescriptions display in pharmacy dashboard';
PRINT '   - Status tracking (Pending/Fulfilled/Cancelled)';
PRINT '   - Fulfillment date tracking';
PRINT '   - Prescription details view for pharmacists';
PRINT '';
PRINT '🎯 Next Steps:';
PRINT '   1. Build and run the application';
PRINT '   2. Login as Pharmacist';
PRINT '   3. Click "Prescriptions" button in dashboard';
PRINT '   4. View and fulfill doctor prescriptions';
PRINT '';
PRINT '🎉 Pharmacy-Doctor Integration Complete!';
