-- AFTER RUNNING THE ABOVE SCRIPT, run this to add sample lab tests
-- This ensures the lab tests exist for doctors to prescribe

USE ClinicalManagementSystem
GO

-- Ensure TblLabTests exists with sample data
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TblLabTests' AND xtype='U')
BEGIN
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
END

-- Insert sample lab tests (only if they don't exist)
IF NOT EXISTS (SELECT 1 FROM TblLabTests WHERE TestCode = 'CBC001')
BEGIN
    INSERT INTO TblLabTests (TestCode, TestName, Description, Price, NormalRange, Unit, DepartmentId, IsActive)
    VALUES
    ('CBC001', 'Complete Blood Count', 'Complete blood analysis including RBC, WBC, platelets', 150.00, 'RBC: 4.2-5.4 million/μL, WBC: 4,000-11,000/μL', 'cells/μL', 1, 1),
    ('LFT001', 'Liver Function Test', 'Tests for liver enzymes and function', 200.00, 'ALT: 7-56 U/L, AST: 10-40 U/L', 'U/L', 1, 1),
    ('RFT001', 'Renal Function Test', 'Tests for kidney function', 180.00, 'Creatinine: 0.7-1.3 mg/dL', 'mg/dL', 1, 1),
    ('BS001', 'Blood Sugar', 'Fasting and post-prandial blood glucose', 100.00, 'Fasting: 70-100 mg/dL', 'mg/dL', 1, 1),
    ('LIPID001', 'Lipid Profile', 'Cholesterol and triglyceride levels', 250.00, 'Total Cholesterol: <200 mg/dL', 'mg/dL', 1, 1);
END

PRINT '✅ Sample lab tests added successfully!'
GO
