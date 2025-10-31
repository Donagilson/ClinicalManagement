-- =====================================================
-- PRESCRIPTION DETAILS ENHANCEMENT - DATABASE UPDATES
-- =====================================================
-- Run this script to add medicine name and price tracking to prescription details

USE [ClinicalManagementSystemDB]
GO

-- Add MedicineName and Price columns to TblPrescriptionDetails
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TblPrescriptionDetails') AND name = 'MedicineName')
BEGIN
    ALTER TABLE TblPrescriptionDetails
    ADD MedicineName NVARCHAR(255) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TblPrescriptionDetails') AND name = 'Price')
BEGIN
    ALTER TABLE TblPrescriptionDetails
    ADD Price DECIMAL(10,2) DEFAULT 0 NULL;
END
GO

-- Update existing prescription details with medicine names from TblMedicines
UPDATE pd
SET pd.MedicineName = m.MedicineName,
    pd.Price = m.UnitPrice
FROM TblPrescriptionDetails pd
JOIN TblMedicines m ON pd.MedicineId = m.MedicineId
WHERE pd.MedicineName IS NULL;
GO

-- Add index for better performance on MedicineName queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblPrescriptionDetails') AND name = 'IX_PrescriptionDetails_MedicineName')
BEGIN
    CREATE INDEX IX_PrescriptionDetails_MedicineName ON TblPrescriptionDetails(MedicineName);
END
GO

-- Add index for Price queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblPrescriptionDetails') AND name = 'IX_PrescriptionDetails_Price')
BEGIN
    CREATE INDEX IX_PrescriptionDetails_Price ON TblPrescriptionDetails(Price);
END
GO

-- Display success message
PRINT '✅ Prescription Details Enhancement Added Successfully!';
PRINT '';
PRINT '📋 Database Updates Applied:';
PRINT '   - Added MedicineName column (NVARCHAR(255)) for storing actual medicine names';
PRINT '   - Added Price column (DECIMAL(10,2)) for medicine pricing';
PRINT '   - Updated existing records with medicine names and prices from TblMedicines';
PRINT '   - Created indexes for better query performance';
PRINT '';
PRINT '🚀 Enhanced Prescription Features Now Available:';
PRINT '   - Prescriptions show actual medicine names instead of IDs';
PRINT '   - Price information available for each prescribed medicine';
PRINT '   - Better prescription details display in pharmacy dashboard';
PRINT '   - Enhanced reporting capabilities with medicine names and prices';
PRINT '';
PRINT '🎯 Next Steps:';
PRINT '   1. Build and run the application';
PRINT '   2. Login as Doctor and create a medical note with prescription';
PRINT '   3. Login as Pharmacist and check prescription details';
PRINT '   4. Verify medicine names appear correctly in prescription management';
PRINT '';
PRINT '🎉 Doctor-to-Pharmacist Medicine Integration Complete!';
