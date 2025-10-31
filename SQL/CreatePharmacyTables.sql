-- =====================================================
-- PHARMACY MODULE DATABASE SETUP
-- =====================================================
-- Creates all tables and sample data for the pharmacy module
-- =====================================================

PRINT '🏥 Starting pharmacy module database setup...';
PRINT '';

-- Step 1: Create TblMedicines table
PRINT '1️⃣ Creating TblMedicines table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblMedicines')
BEGIN
    CREATE TABLE TblMedicines (
        MedicineId INT IDENTITY(1,1) PRIMARY KEY,
        MedicineName NVARCHAR(100) NOT NULL,
        GenericName NVARCHAR(100) NULL,
        MedicineCode NVARCHAR(20) NULL,
        Category NVARCHAR(50) NULL,
        Manufacturer NVARCHAR(100) NULL,
        Description NVARCHAR(MAX) NULL,
        UnitPrice DECIMAL(10,2) NOT NULL,
        Unit NVARCHAR(50) NULL,
        Strength NVARCHAR(50) NULL,
        Form NVARCHAR(50) NULL,
        RequiresPrescription BIT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedDate DATETIME NULL,
        UpdatedBy INT NULL
    );

    -- Add sample medicines
    INSERT INTO TblMedicines (MedicineName, GenericName, MedicineCode, Category, Manufacturer, Description, UnitPrice, Unit, Strength, Form, RequiresPrescription, IsActive, CreatedDate)
    VALUES
    ('Paracetamol', 'Acetaminophen', 'MED001', 'Analgesic', 'Pharma Corp', 'Pain relief and fever reducer', 5.00, 'mg', '500mg', 'Tablet', 0, 1, GETDATE()),
    ('Amoxicillin', 'Amoxicillin', 'MED002', 'Antibiotic', 'Medi Labs', 'Antibiotic for bacterial infections', 15.00, 'mg', '250mg', 'Capsule', 1, 1, GETDATE()),
    ('Ibuprofen', 'Ibuprofen', 'MED003', 'Anti-inflammatory', 'Health Pharma', 'Anti-inflammatory and pain relief', 8.00, 'mg', '400mg', 'Tablet', 0, 1, GETDATE()),
    ('Vitamin D3', 'Cholecalciferol', 'MED004', 'Vitamins', 'Wellness Labs', 'Vitamin D supplement', 12.00, 'IU', '1000IU', 'Capsule', 0, 1, GETDATE()),
    ('Omeprazole', 'Omeprazole', 'MED005', 'Gastrointestinal', 'Digestive Care', 'Proton pump inhibitor for acid reflux', 20.00, 'mg', '20mg', 'Capsule', 1, 1, GETDATE()),
    ('Cetirizine', 'Cetirizine', 'MED006', 'Antihistamine', 'Allergy Relief', 'Antihistamine for allergies', 7.50, 'mg', '10mg', 'Tablet', 0, 1, GETDATE()),
    ('Metformin', 'Metformin', 'MED007', 'Diabetes', 'Diabetic Care', 'Medication for type 2 diabetes', 10.00, 'mg', '500mg', 'Tablet', 1, 1, GETDATE()),
    ('Aspirin', 'Acetylsalicylic Acid', 'MED008', 'Cardiovascular', 'Heart Health', 'Blood thinner and pain relief', 6.00, 'mg', '75mg', 'Tablet', 1, 1, GETDATE());

    PRINT '   ✅ TblMedicines table created with 8 sample medicines!';
END
ELSE
    PRINT '   ✅ TblMedicines table already exists.';

-- Step 2: Create TblPharmacyInventory table
PRINT '';
PRINT '2️⃣ Creating TblPharmacyInventory table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblPharmacyInventory')
BEGIN
    CREATE TABLE TblPharmacyInventory (
        InventoryId INT IDENTITY(1,1) PRIMARY KEY,
        MedicineId INT NOT NULL,
        CurrentStock INT NOT NULL DEFAULT 0,
        MinStockLevel INT NOT NULL DEFAULT 0,
        MaxStockLevel INT NOT NULL DEFAULT 0,
        ExpiryDate DATETIME NULL,
        BatchNumber NVARCHAR(50) NULL,
        Supplier NVARCHAR(100) NULL,
        CostPrice DECIMAL(10,2) NOT NULL,
        SellingPrice DECIMAL(10,2) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedDate DATETIME NULL,
        UpdatedBy INT NULL,
        FOREIGN KEY (MedicineId) REFERENCES TblMedicines(MedicineId)
    );

    -- Add sample inventory
    INSERT INTO TblPharmacyInventory (MedicineId, CurrentStock, MinStockLevel, MaxStockLevel, ExpiryDate, BatchNumber, Supplier, CostPrice, SellingPrice, IsActive, CreatedDate)
    VALUES
    (1, 150, 20, 500, DATEADD(YEAR, 2, GETDATE()), 'B001', 'Pharma Corp', 3.50, 5.00, 1, GETDATE()),
    (2, 75, 15, 200, DATEADD(YEAR, 1, GETDATE()), 'B002', 'Medi Labs', 12.00, 15.00, 1, GETDATE()),
    (3, 200, 25, 400, DATEADD(YEAR, 3, GETDATE()), 'B003', 'Health Pharma', 6.00, 8.00, 1, GETDATE()),
    (4, 100, 10, 300, DATEADD(YEAR, 2, GETDATE()), 'B004', 'Wellness Labs', 9.00, 12.00, 1, GETDATE()),
    (5, 50, 10, 150, DATEADD(MONTH, 18, GETDATE()), 'B005', 'Digestive Care', 15.00, 20.00, 1, GETDATE()),
    (6, 120, 15, 250, DATEADD(YEAR, 2, GETDATE()), 'B006', 'Allergy Relief', 5.50, 7.50, 1, GETDATE()),
    (7, 80, 20, 200, DATEADD(YEAR, 1, GETDATE()), 'B007', 'Diabetic Care', 7.50, 10.00, 1, GETDATE()),
    (8, 60, 15, 180, DATEADD(YEAR, 2, GETDATE()), 'B008', 'Heart Health', 4.50, 6.00, 1, GETDATE());

    PRINT '   ✅ TblPharmacyInventory table created with 8 inventory items!';
END
ELSE
    PRINT '   ✅ TblPharmacyInventory table already exists.';

-- Step 3: Create TblDispensedMedications table
PRINT '';
PRINT '3️⃣ Creating TblDispensedMedications table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TblDispensedMedications')
BEGIN
    CREATE TABLE TblDispensedMedications (
        DispensedId INT IDENTITY(1,1) PRIMARY KEY,
        MedicineId INT NOT NULL,
        PatientId INT NOT NULL,
        InventoryId INT NOT NULL,
        PrescriptionId INT NULL,
        DoctorId INT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(10,2) NOT NULL,
        TotalAmount DECIMAL(10,2) NOT NULL,
        Instructions NVARCHAR(500) NULL,
        Dosage NVARCHAR(200) NULL,
        Frequency NVARCHAR(100) NULL,
        Duration NVARCHAR(100) NULL,
        DispensedBy INT NOT NULL,
        DispensedDate DATETIME NOT NULL DEFAULT GETDATE(),
        PaymentStatus NVARCHAR(50) NOT NULL DEFAULT 'Paid',
        PaymentMethod NVARCHAR(100) NULL,
        Notes NVARCHAR(MAX) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (MedicineId) REFERENCES TblMedicines(MedicineId),
        FOREIGN KEY (InventoryId) REFERENCES TblPharmacyInventory(InventoryId)
    );

    -- Add sample dispensed medications
    INSERT INTO TblDispensedMedications (MedicineId, PatientId, InventoryId, PrescriptionId, DoctorId, Quantity, UnitPrice, TotalAmount, Instructions, Dosage, Frequency, Duration, DispensedBy, DispensedDate, PaymentStatus, PaymentMethod, Notes, IsActive, CreatedDate)
    VALUES
    (1, 1, 1, 1, 1, 20, 5.00, 100.00, 'Take 1 tablet every 6 hours as needed for pain', '1 tablet', 'Every 6 hours', '5 days', 1, DATEADD(DAY, -1, GETDATE()), 'Paid', 'Cash', 'For headache relief', 1, GETDATE()),
    (2, 2, 2, 2, 2, 21, 15.00, 315.00, 'Take 1 capsule 3 times daily with food', '1 capsule', '3 times daily', '7 days', 1, DATEADD(HOUR, -2, GETDATE()), 'Paid', 'Card', 'For bacterial infection', 1, GETDATE()),
    (3, 3, 3, NULL, 1, 15, 8.00, 120.00, 'Take 1 tablet every 8 hours with food', '1 tablet', 'Every 8 hours', '5 days', 1, DATEADD(HOUR, -4, GETDATE()), 'Paid', 'Cash', 'Over-the-counter purchase', 1, GETDATE()),
    (4, 1, 4, NULL, NULL, 30, 12.00, 360.00, 'Take 1 capsule daily', '1 capsule', 'Daily', '30 days', 1, DATEADD(HOUR, -6, GETDATE()), 'Paid', 'Insurance', 'Vitamin supplement', 1, GETDATE()),
    (6, 4, 6, NULL, NULL, 10, 7.50, 75.00, 'Take 1 tablet daily as needed for allergies', '1 tablet', 'As needed', '10 days', 1, DATEADD(DAY, -2, GETDATE()), 'Paid', 'Cash', 'For seasonal allergies', 1, GETDATE());

    PRINT '   ✅ TblDispensedMedications table created with 5 sample records!';
END
ELSE
    PRINT '   ✅ TblDispensedMedications table already exists.';

-- Step 4: Create indexes for better performance
PRINT '';
PRINT '4️⃣ Creating indexes for performance...';
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblMedicines') AND name = 'IX_Medicines_Code')
    CREATE INDEX IX_Medicines_Code ON TblMedicines(MedicineCode);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblMedicines') AND name = 'IX_Medicines_Category')
    CREATE INDEX IX_Medicines_Category ON TblMedicines(Category);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblPharmacyInventory') AND name = 'IX_Inventory_MedicineId')
    CREATE INDEX IX_Inventory_MedicineId ON TblPharmacyInventory(MedicineId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblPharmacyInventory') AND name = 'IX_Inventory_StockLevel')
    CREATE INDEX IX_Inventory_StockLevel ON TblPharmacyInventory(CurrentStock, MinStockLevel);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblDispensedMedications') AND name = 'IX_Dispensed_PatientId')
    CREATE INDEX IX_Dispensed_PatientId ON TblDispensedMedications(PatientId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblDispensedMedications') AND name = 'IX_Dispensed_MedicineId')
    CREATE INDEX IX_Dispensed_MedicineId ON TblDispensedMedications(MedicineId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TblDispensedMedications') AND name = 'IX_Dispensed_Date')
    CREATE INDEX IX_Dispensed_Date ON TblDispensedMedications(DispensedDate);

PRINT '   ✅ All indexes created!';

-- Step 5: Update existing prescription tables if needed
PRINT '';
PRINT '5️⃣ Checking prescription tables...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblPrescriptionDetails' AND COLUMN_NAME = 'MedicineId')
BEGIN
    ALTER TABLE TblPrescriptionDetails ADD MedicineId INT NULL;
    PRINT '   ✅ MedicineId column added to TblPrescriptionDetails!';
END

-- Step 6: Create pharmacy-specific views for reporting
PRINT '';
PRINT '6️⃣ Creating pharmacy reporting views...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'ViewPharmacyStockStatus')
BEGIN
    EXEC('
    CREATE VIEW ViewPharmacyStockStatus AS
    SELECT
        m.MedicineId,
        m.MedicineName,
        m.MedicineCode,
        m.Category,
        i.CurrentStock,
        i.MinStockLevel,
        i.MaxStockLevel,
        i.SellingPrice,
        CASE
            WHEN i.CurrentStock <= i.MinStockLevel THEN ''Low Stock''
            WHEN i.ExpiryDate <= DATEADD(MONTH, 3, GETDATE()) THEN ''Expiring Soon''
            ELSE ''Normal''
        END as StockStatus
    FROM TblMedicines m
    LEFT JOIN TblPharmacyInventory i ON m.MedicineId = i.MedicineId
    WHERE m.IsActive = 1
    ');
    PRINT '   ✅ ViewPharmacyStockStatus created!';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'ViewDispensingSummary')
BEGIN
    EXEC('
    CREATE VIEW ViewDispensingSummary AS
    SELECT
        d.DispensedDate,
        m.MedicineName,
        m.MedicineCode,
        m.Category,
        d.Quantity,
        d.UnitPrice,
        d.TotalAmount,
        d.PatientId,
        d.PaymentMethod,
        d.PaymentStatus
    FROM TblDispensedMedications d
    INNER JOIN TblMedicines m ON d.MedicineId = m.MedicineId
    WHERE d.IsActive = 1
    ');
    PRINT '   ✅ ViewDispensingSummary created!';
END

PRINT '';
PRINT '🎉 =====================================================';
PRINT '🎉 PHARMACY MODULE SETUP COMPLETED!';
PRINT '🎉 =====================================================';
PRINT '';
PRINT '✅ TblMedicines table with 8 medicines';
PRINT '✅ TblPharmacyInventory table with stock levels';
PRINT '✅ TblDispensedMedications table with dispensing history';
PRINT '✅ All foreign key relationships established';
PRINT '✅ Performance indexes created';
PRINT '✅ Reporting views created';
PRINT '✅ Sample data included for testing';
PRINT '';
PRINT '🚀 Your pharmacy module is now fully functional!';
PRINT '';
PRINT 'Available features:';
PRINT '• Medicine management (CRUD operations)';
PRINT '• Inventory tracking with low stock alerts';
PRINT '• Medication dispensing with prescription support';
PRINT '• Revenue tracking and reporting';
PRINT '• Stock level monitoring';
PRINT '• Expiry date tracking';
PRINT '';
PRINT '🎊 Ready to use! Start your application and visit /Pharmacy 🎊';
PRINT '=====================================================';
