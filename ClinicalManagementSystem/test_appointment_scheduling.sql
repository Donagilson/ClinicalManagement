-- Clinical Management System - Test Appointment Scheduling
-- This script helps verify that appointment scheduling is working correctly

USE ClinicalManagementDB;
GO

-- 1. Check if basic data exists
PRINT '=== CHECKING BASIC DATA ==='

-- Check Roles
SELECT 'Roles:' AS DataType, RoleId, RoleName FROM TblRoles WHERE IsActive = 1;

-- Check Users
SELECT 'Users:' AS DataType, UserId, UserName, FullName, RoleName
FROM TblUsers u
INNER JOIN TblRoles r ON u.RoleId = r.RoleId
WHERE u.IsActive = 1;

-- Check Departments
SELECT 'Departments:' AS DataType, DepartmentId, DepartmentName FROM TblDepartments WHERE IsActive = 1;

-- Check if we have any doctors
SELECT 'Doctors:' AS DataType, d.DoctorId, u.FullName AS DoctorName, d.Specialization, dep.DepartmentName
FROM TblDoctors d
INNER JOIN TblUsers u ON d.UserId = u.UserId
INNER JOIN TblDepartments dep ON d.DepartmentId = dep.DepartmentId
WHERE d.IsActive = 1 AND u.IsActive = 1;

-- Check if we have any patients
SELECT 'Patients:' AS DataType, COUNT(*) AS PatientCount FROM TblPatients WHERE IsActive = 1;

PRINT '=== ADDING TEST DATA IF MISSING ==='

-- Add test doctor users if they don't exist
IF NOT EXISTS (SELECT 1 FROM TblUsers WHERE UserName = 'dr.smith')
BEGIN
    INSERT INTO TblUsers (UserName, UserPassword, FullName, Email, Phone, RoleId, IsActive)
    VALUES ('dr.smith', 'Doctor@123', 'Dr. John Smith', 'dr.smith@clinic.com', '+1234567890', 3, 1);
    PRINT 'Added Dr. John Smith';
END

IF NOT EXISTS (SELECT 1 FROM TblUsers WHERE UserName = 'dr.wilson')
BEGIN
    INSERT INTO TblUsers (UserName, UserPassword, FullName, Email, Phone, RoleId, IsActive)
    VALUES ('dr.wilson', 'Doctor@123', 'Dr. Sarah Wilson', 'dr.wilson@clinic.com', '+1234567891', 3, 1);
    PRINT 'Added Dr. Sarah Wilson';
END

-- Add doctors to TblDoctors if they don't exist
DECLARE @DrSmithUserId INT = (SELECT UserId FROM TblUsers WHERE UserName = 'dr.smith');
DECLARE @DrWilsonUserId INT = (SELECT UserId FROM TblUsers WHERE UserName = 'dr.wilson');

IF @DrSmithUserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblDoctors WHERE UserId = @DrSmithUserId)
BEGIN
    INSERT INTO TblDoctors (UserId, Specialization, Qualification, Experience, DepartmentId, ConsultationFee, AvailableFrom, AvailableTo, IsActive)
    VALUES (@DrSmithUserId, 'Heart Specialist', 'MBBS, MD Cardiology', 10, 1, 1500.00, '09:00', '17:00', 1);
    PRINT 'Added Dr. Smith to Cardiology department';
END

IF @DrWilsonUserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblDoctors WHERE UserId = @DrWilsonUserId)
BEGIN
    INSERT INTO TblDoctors (UserId, Specialization, Qualification, Experience, DepartmentId, ConsultationFee, AvailableFrom, AvailableTo, IsActive)
    VALUES (@DrWilsonUserId, 'Brain & Nerve Specialist', 'MBBS, MD Neurology', 8, 2, 1800.00, '10:00', '18:00', 1);
    PRINT 'Added Dr. Wilson to Neurology department';
END

-- Add test patient if doesn't exist
IF NOT EXISTS (SELECT 1 FROM TblPatients WHERE Phone = '9207039181')
BEGIN
    INSERT INTO TblPatients (FirstName, LastName, Gender, DateOfBirth, Phone, Email, Address, BloodGroup, EmergencyContact, IsActive)
    VALUES ('Adithya', 'P', 'Female', '1993-01-01', '9207039181', 'adi@gmail.com', '123 Main Street', 'AB+', '9876543210', 1);
    PRINT 'Added test patient: Adithya P';
END

PRINT '=== CURRENT DATA SUMMARY ==='

-- Show available doctors for appointment scheduling
SELECT
    'Available Doctors:' AS Info,
    d.DoctorId,
    u.FullName AS DoctorName,
    d.Specialization,
    dep.DepartmentName,
    d.ConsultationFee,
    CONVERT(VARCHAR(5), d.AvailableFrom, 108) AS AvailableFrom,
    CONVERT(VARCHAR(5), d.AvailableTo, 108) AS AvailableTo
FROM TblDoctors d
INNER JOIN TblUsers u ON d.UserId = u.UserId
INNER JOIN TblDepartments dep ON d.DepartmentId = dep.DepartmentId
WHERE d.IsActive = 1 AND u.IsActive = 1
ORDER BY dep.DepartmentName, u.FullName;

-- Show test patients
SELECT
    'Test Patients:' AS Info,
    PatientId,
    FirstName + ' ' + LastName AS FullName,
    Phone,
    Email,
    Gender,
    DATEDIFF(YEAR, DateOfBirth, GETDATE()) AS Age
FROM TblPatients
WHERE IsActive = 1 AND Phone LIKE '%920%'
ORDER BY FirstName;

PRINT '=== TESTING APPOINTMENT CREATION ==='

-- Create a test appointment for today if it doesn't exist
DECLARE @TestPatientId INT = (SELECT TOP 1 PatientId FROM TblPatients WHERE Phone = '9207039181' AND IsActive = 1);
DECLARE @TestDoctorId INT = (SELECT TOP 1 DoctorId FROM TblDoctors WHERE IsActive = 1);
DECLARE @TestReceptionistId INT = (SELECT TOP 1 UserId FROM TblUsers u INNER JOIN TblRoles r ON u.RoleId = r.RoleId WHERE r.RoleName = 'Receptionist' AND u.IsActive = 1);

IF @TestPatientId IS NOT NULL AND @TestDoctorId IS NOT NULL AND @TestReceptionistId IS NOT NULL
BEGIN
    -- Check if appointment already exists for today
    IF NOT EXISTS (
        SELECT 1 FROM TblAppointments
        WHERE PatientId = @TestPatientId
        AND DoctorId = @TestDoctorId
        AND AppointmentDate = CAST(GETDATE() AS DATE)
    )
    BEGIN
        INSERT INTO TblAppointments (
            PatientId, DoctorId, AppointmentDate, AppointmentTime,
            Status, Reason, Notes, CreatedBy, CreatedDate
        )
        VALUES (
            @TestPatientId, @TestDoctorId, CAST(GETDATE() AS DATE), '14:30:00',
            'Scheduled', 'Regular checkup', 'Test appointment created by script', @TestReceptionistId, GETDATE()
        );

        PRINT 'Created test appointment for today at 2:30 PM';
    END
    ELSE
    BEGIN
        PRINT 'Test appointment already exists for today';
    END
END
ELSE
BEGIN
    PRINT 'Cannot create test appointment - missing required data:';
    IF @TestPatientId IS NULL PRINT '  - No test patient found';
    IF @TestDoctorId IS NULL PRINT '  - No doctor found';
    IF @TestReceptionistId IS NULL PRINT '  - No receptionist user found';
END

PRINT '=== TODAY''S APPOINTMENTS ==='

-- Show today's appointments (this is what the dashboard should display)
SELECT
    a.AppointmentId,
    'APT' + FORMAT(a.AppointmentId, '00000') AS AppointmentCode,
    p.FirstName + ' ' + p.LastName AS PatientName,
    p.Phone AS PatientPhone,
    u.FullName AS DoctorName,
    d.Specialization,
    dep.DepartmentName,
    a.AppointmentDate,
    CONVERT(VARCHAR(5), a.AppointmentTime, 108) AS AppointmentTime,
    a.Status,
    a.Reason,
    d.ConsultationFee
FROM TblAppointments a
INNER JOIN TblPatients p ON a.PatientId = p.PatientId
INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
INNER JOIN TblUsers u ON d.UserId = u.UserId
INNER JOIN TblDepartments dep ON d.DepartmentId = dep.DepartmentId
WHERE a.AppointmentDate = CAST(GETDATE() AS DATE)
ORDER BY a.AppointmentTime;

PRINT '=== TESTING STORED PROCEDURES ==='

-- Test the stored procedure for getting today's appointments
EXEC sp_GetAppointmentsByDate @AppointmentDate = NULL; -- This should default to today

-- Test getting available doctors
EXEC sp_GetAvailableDoctors;

-- Test getting doctors by department (Cardiology)
EXEC sp_GetAvailableDoctors @DepartmentId = 1;

PRINT '=== APPOINTMENT STATISTICS ==='

-- Show appointment statistics
SELECT
    'Total Appointments:' AS Metric,
    COUNT(*) AS Count
FROM TblAppointments;

SELECT
    'Today''s Appointments:' AS Metric,
    COUNT(*) AS Count
FROM TblAppointments
WHERE AppointmentDate = CAST(GETDATE() AS DATE);

SELECT
    'Appointments by Status:' AS Metric,
    Status,
    COUNT(*) AS Count
FROM TblAppointments
GROUP BY Status
ORDER BY Count DESC;

SELECT
    'Appointments by Doctor:' AS Metric,
    u.FullName AS DoctorName,
    COUNT(a.AppointmentId) AS AppointmentCount
FROM TblAppointments a
INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
INNER JOIN TblUsers u ON d.UserId = u.UserId
WHERE a.AppointmentDate >= DATEADD(DAY, -7, GETDATE()) -- Last 7 days
GROUP BY u.FullName
ORDER BY AppointmentCount DESC;

PRINT '=== VALIDATION CHECKS ==='

-- Check for data integrity issues
SELECT 'Data Integrity Issues:' AS CheckType;

-- Check for appointments without valid patients
SELECT 'Appointments with invalid patients:' AS Issue, COUNT(*) AS Count
FROM TblAppointments a
LEFT JOIN TblPatients p ON a.PatientId = p.PatientId
WHERE p.PatientId IS NULL OR p.IsActive = 0;

-- Check for appointments without valid doctors
SELECT 'Appointments with invalid doctors:' AS Issue, COUNT(*) AS Count
FROM TblAppointments a
LEFT JOIN TblDoctors d ON a.DoctorId = d.DoctorId
WHERE d.DoctorId IS NULL OR d.IsActive = 0;

-- Check for doctors without departments
SELECT 'Doctors without departments:' AS Issue, COUNT(*) AS Count
FROM TblDoctors d
LEFT JOIN TblDepartments dep ON d.DepartmentId = dep.DepartmentId
WHERE dep.DepartmentId IS NULL OR dep.IsActive = 0;

-- Check for doctors without user accounts
SELECT 'Doctors without user accounts:' AS Issue, COUNT(*) AS Count
FROM TblDoctors d
LEFT JOIN TblUsers u ON d.UserId = u.UserId
WHERE u.UserId IS NULL OR u.IsActive = 0;

PRINT '=== TEST SCRIPT COMPLETED ==='
PRINT 'You can now test appointment scheduling in the web application'
PRINT 'Use phone number: 9207039181 to find the test patient'
PRINT 'Available doctors should appear in the dropdown'
PRINT 'Today''s appointments will show in the dashboard'

-- Show final summary
SELECT
    'SUMMARY' AS Section,
    (SELECT COUNT(*) FROM TblPatients WHERE IsActive = 1) AS ActivePatients,
    (SELECT COUNT(*) FROM TblDoctors WHERE IsActive = 1) AS ActiveDoctors,
    (SELECT COUNT(*) FROM TblAppointments WHERE AppointmentDate = CAST(GETDATE() AS DATE)) AS TodaysAppointments,
    (SELECT COUNT(*) FROM TblUsers WHERE IsActive = 1) AS ActiveUsers;
