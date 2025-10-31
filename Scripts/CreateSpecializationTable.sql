-- Create TblSpecializations table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TblSpecializations' AND xtype='U')
BEGIN
    CREATE TABLE TblSpecializations (
        SpecializationId INT IDENTITY(1,1) PRIMARY KEY,
        SpecializationName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        DepartmentId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (DepartmentId) REFERENCES TblDepartments(DepartmentId)
    );

    -- Insert some sample specializations
    INSERT INTO TblSpecializations (SpecializationName, Description, DepartmentId, IsActive)
    SELECT 'General Medicine', 'General medical practice', d.DepartmentId, 1
    FROM TblDepartments d WHERE d.DepartmentName LIKE '%General%' OR d.DepartmentName LIKE '%Medicine%'
    
    UNION ALL
    
    SELECT 'Cardiology', 'Heart and cardiovascular system', d.DepartmentId, 1
    FROM TblDepartments d WHERE d.DepartmentName LIKE '%Cardio%' OR d.DepartmentName LIKE '%Heart%'
    
    UNION ALL
    
    SELECT 'Orthopedics', 'Bone and joint disorders', d.DepartmentId, 1
    FROM TblDepartments d WHERE d.DepartmentName LIKE '%Ortho%' OR d.DepartmentName LIKE '%Bone%'
    
    UNION ALL
    
    SELECT 'Pediatrics', 'Child healthcare', d.DepartmentId, 1
    FROM TblDepartments d WHERE d.DepartmentName LIKE '%Pediatric%' OR d.DepartmentName LIKE '%Child%'
    
    UNION ALL
    
    SELECT 'Dermatology', 'Skin disorders', d.DepartmentId, 1
    FROM TblDepartments d WHERE d.DepartmentName LIKE '%Dermat%' OR d.DepartmentName LIKE '%Skin%';

    -- If no matching departments found, insert with first available department
    IF NOT EXISTS (SELECT 1 FROM TblSpecializations)
    BEGIN
        DECLARE @FirstDeptId INT;
        SELECT TOP 1 @FirstDeptId = DepartmentId FROM TblDepartments WHERE IsActive = 1;
        
        IF @FirstDeptId IS NOT NULL
        BEGIN
            INSERT INTO TblSpecializations (SpecializationName, Description, DepartmentId, IsActive)
            VALUES 
                ('General Medicine', 'General medical practice', @FirstDeptId, 1),
                ('Internal Medicine', 'Internal organ disorders', @FirstDeptId, 1),
                ('Family Medicine', 'Comprehensive family healthcare', @FirstDeptId, 1);
        END
    END

    PRINT 'TblSpecializations table created successfully with sample data.';
END
ELSE
BEGIN
    PRINT 'TblSpecializations table already exists.';
END

-- Create stored procedures if they don't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllSpecializations')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[sp_GetAllSpecializations]
    AS
    BEGIN
        SELECT s.SpecializationId, s.SpecializationName, s.Description, s.DepartmentId, s.IsActive,
               s.CreatedDate, d.DepartmentName
        FROM TblSpecializations s
        INNER JOIN TblDepartments d ON s.DepartmentId = d.DepartmentId
        WHERE s.IsActive = 1
        ORDER BY s.SpecializationName;
    END
    ');
    PRINT 'sp_GetAllSpecializations procedure created.';
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetSpecializationsByDepartment')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[sp_GetSpecializationsByDepartment]
        @DepartmentId INT
    AS
    BEGIN
        SELECT s.SpecializationId, s.SpecializationName, s.Description, s.DepartmentId, s.IsActive,
               d.DepartmentName
        FROM TblSpecializations s
        INNER JOIN TblDepartments d ON s.DepartmentId = d.DepartmentId
        WHERE s.IsActive = 1 AND s.DepartmentId = @DepartmentId
        ORDER BY s.SpecializationName;
    END
    ');
    PRINT 'sp_GetSpecializationsByDepartment procedure created.';
END
