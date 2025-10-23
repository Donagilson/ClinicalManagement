# Appointment Scheduling - Complete Guide & Troubleshooting

## 🚀 Quick Start Guide

### Prerequisites
1. **Database Setup**: Run the provided database schema and sample data
2. **Application Running**: Start your Clinical Management System
3. **Login**: Use receptionist credentials (`receptionist` / `Recep@123`)

### Test Data Setup
Run the `test_appointment_scheduling.sql` script to ensure you have:
- ✅ Test patients (including Adithya P with phone 9207039181)
- ✅ Test doctors (Dr. John Smith, Dr. Sarah Wilson)
- ✅ Departments (Cardiology, Neurology, etc.)
- ✅ Sample appointments for testing

---

## 📋 Step-by-Step Appointment Scheduling

### 1. Search for Patient
1. Go to Receptionist Dashboard
2. Click **"Search Patient"** button
3. Enter patient ID `1` or phone `9207039181`
4. Click **"Search"**
5. Click **"Schedule"** button next to the patient

### 2. Schedule Appointment Form
The form should show:
```
✅ Patient Information Card (Name: Adithya P, Age: 30, etc.)
✅ Department Dropdown (Cardiology, Neurology, etc.)
✅ Doctor Dropdown (populated with available doctors)
✅ Date/Time fields
✅ Reason and Notes fields
✅ "Schedule Appointment" button
```

### 3. Fill the Form
1. **Department**: Select "Neurology" (optional - filters doctors)
2. **Doctor**: Select "Dr. Sarah Wilson (Brain & Nerve Specialist) - ₹1800"
3. **Date**: Select today's date or tomorrow
4. **Time**: Select "14:30" (2:30 PM)
5. **Duration**: Keep default "30 minutes"
6. **Reason**: Enter "Regular checkup"
7. Click **"Schedule Appointment"**

### 4. Success Confirmation
After successful scheduling, you should see:
- 🎉 **Success Page** with appointment details
- **Appointment Code** (e.g., APT00001)
- **Complete appointment summary**
- Options to **Print** or **Send SMS**

### 5. Verify in Today's Appointments
1. Go back to **Dashboard**
2. Check **"Today's Appointments"** section
3. Your appointment should appear in the list

---

## 🐛 Troubleshooting Common Issues

### Issue 1: No Doctors in Dropdown
**Problem**: Doctor dropdown shows "-- Select Doctor --" but no options

**Solutions**:
```sql
-- Check if doctors exist
SELECT d.DoctorId, u.FullName, d.Specialization
FROM TblDoctors d
INNER JOIN TblUsers u ON d.UserId = u.UserId
WHERE d.IsActive = 1 AND u.IsActive = 1;

-- If empty, run the test script to add sample doctors
```

**Code Check**: Verify `GetDoctorsByDepartment` method in `ReceptionistController`

### Issue 2: Form Submission Not Working
**Problem**: Clicking "Schedule Appointment" doesn't do anything

**Check**:
1. Browser console for JavaScript errors
2. Form validation messages
3. Required fields are filled
4. Network tab for HTTP requests

**Fix**: Ensure all required fields have values:
- DoctorId (not empty)
- AppointmentDate (valid date)
- AppointmentTime (valid time)

### Issue 3: Patient Not Found
**Problem**: Search returns "No patients found"

**Solutions**:
```sql
-- Add test patient
INSERT INTO TblPatients (FirstName, LastName, Gender, DateOfBirth, Phone, Email, Address, BloodGroup, EmergencyContact, IsActive)
VALUES ('Adithya', 'P', 'Female', '1993-01-01', '9207039181', 'adi@gmail.com', '123 Main Street', 'AB+', '9876543210', 1);

-- Search with wildcards
SELECT * FROM TblPatients WHERE Phone LIKE '%9207%' OR FirstName LIKE '%Adithya%';
```

### Issue 4: Today's Appointments Not Showing
**Problem**: Dashboard shows 0 appointments even after scheduling

**Check Database**:
```sql
-- Verify appointment was created
SELECT TOP 5 
    a.AppointmentId,
    'APT' + FORMAT(a.AppointmentId, '00000') AS Code,
    p.FirstName + ' ' + p.LastName AS Patient,
    u.FullName AS Doctor,
    a.AppointmentDate,
    a.AppointmentTime,
    a.Status
FROM TblAppointments a
INNER JOIN TblPatients p ON a.PatientId = p.PatientId
INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
INNER JOIN TblUsers u ON d.UserId = u.UserId
ORDER BY a.CreatedDate DESC;
```

**Fix**: Check `GetTodaysAppointmentsAsync()` method implementation

### Issue 5: Success Page Not Loading
**Problem**: After scheduling, redirects to dashboard instead of success page

**Check**:
1. `AppointmentSuccess` action exists in `ReceptionistController`
2. `AppointmentSuccess.cshtml` view exists
3. Database transaction completed successfully

---

## 🔧 Database Verification Queries

### Check System Status
```sql
-- 1. Verify basic setup
SELECT 'Users' AS Table_Name, COUNT(*) AS Count FROM TblUsers WHERE IsActive = 1
UNION ALL
SELECT 'Patients', COUNT(*) FROM TblPatients WHERE IsActive = 1
UNION ALL
SELECT 'Doctors', COUNT(*) FROM TblDoctors WHERE IsActive = 1
UNION ALL
SELECT 'Appointments', COUNT(*) FROM TblAppointments;

-- 2. Check today's appointments
SELECT 
    a.AppointmentId,
    p.FirstName + ' ' + p.LastName AS PatientName,
    u.FullName AS DoctorName,
    FORMAT(a.AppointmentTime, 'HH:mm') AS Time,
    a.Status
FROM TblAppointments a
INNER JOIN TblPatients p ON a.PatientId = p.PatientId
INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
INNER JOIN TblUsers u ON d.UserId = u.UserId
WHERE a.AppointmentDate = CAST(GETDATE() AS DATE)
ORDER BY a.AppointmentTime;

-- 3. Check available doctors
SELECT 
    d.DoctorId,
    u.FullName AS DoctorName,
    d.Specialization,
    dep.DepartmentName,
    d.ConsultationFee,
    FORMAT(d.AvailableFrom, 'HH:mm') + ' - ' + FORMAT(d.AvailableTo, 'HH:mm') AS Hours
FROM TblDoctors d
INNER JOIN TblUsers u ON d.UserId = u.UserId
INNER JOIN TblDepartments dep ON d.DepartmentId = dep.DepartmentId
WHERE d.IsActive = 1 AND u.IsActive = 1;
```

### Manual Test Appointment Creation
```sql
-- Create test appointment manually
DECLARE @PatientId INT = (SELECT TOP 1 PatientId FROM TblPatients WHERE Phone = '9207039181');
DECLARE @DoctorId INT = (SELECT TOP 1 DoctorId FROM TblDoctors WHERE IsActive = 1);
DECLARE @CreatedBy INT = (SELECT TOP 1 UserId FROM TblUsers WHERE UserName = 'receptionist');

INSERT INTO TblAppointments (PatientId, DoctorId, AppointmentDate, AppointmentTime, Status, Reason, CreatedBy, CreatedDate)
VALUES (@PatientId, @DoctorId, CAST(GETDATE() AS DATE), '15:30:00', 'Scheduled', 'Test appointment', @CreatedBy, GETDATE());

SELECT 'Appointment created with ID: ' + CAST(SCOPE_IDENTITY() AS VARCHAR(10));
```

---

## 🏥 Application Flow Diagram

```
1. Receptionist Dashboard
   ↓
2. Search Patient (by ID/Phone)
   ↓
3. Click "Schedule" button
   ↓
4. Schedule Appointment Form
   ├── Patient Info (auto-filled)
   ├── Select Department (optional)
   ├── Select Doctor (required)
   ├── Select Date/Time (required)
   ├── Enter Reason (optional)
   └── Click "Schedule Appointment"
   ↓
5. Server Processing
   ├── Validate form data
   ├── Check doctor availability
   ├── Create appointment record
   └── Generate appointment code
   ↓
6. Success Page
   ├── Display appointment details
   ├── Show appointment code
   ├── Print/SMS options
   └── "Back to Dashboard" link
   ↓
7. Dashboard (Updated)
   └── Today's Appointments (shows new appointment)
```

---

## 📱 Testing Scenarios

### Scenario 1: Happy Path
1. Search patient: `9207039181`
2. Select patient: `Adithya P`
3. Choose doctor: `Dr. Sarah Wilson`
4. Set date: Today
5. Set time: `14:30`
6. Enter reason: `Regular checkup`
7. Submit form
8. **Expected**: Success page with appointment code
9. **Verify**: Dashboard shows appointment in today's list

### Scenario 2: Doctor Availability
1. Follow Scenario 1 steps 1-4
2. Set time outside doctor's hours (e.g., `06:00`)
3. Submit form
4. **Expected**: Validation error about availability
5. **Fix**: Choose time between doctor's available hours

### Scenario 3: Duplicate Appointment
1. Create appointment for patient at specific time
2. Try to create another appointment for same patient, doctor, and time
3. **Expected**: System should prevent or warn about conflict

---

## 🔍 Debugging Tips

1. **Enable Detailed Logging**:
   - Check application logs for errors
   - Use browser developer tools (F12)
   - Check network requests and responses

2. **Database Monitoring**:
   - Use SQL Server Profiler to monitor queries
   - Check for constraint violations
   - Verify foreign key relationships

3. **Step-through Debugging**:
   - Set breakpoints in `ScheduleAppointment` POST method
   - Verify model binding and validation
   - Check repository method calls

4. **Frontend Debugging**:
   - Check jQuery console errors
   - Verify AJAX calls to `GetDoctorsByDepartment`
   - Test form validation JavaScript

---

## 📞 Support Checklist

If appointment scheduling still doesn't work:

- [ ] Database connection string is correct
- [ ] All required tables exist with proper schema
- [ ] Sample data is populated (run test script)
- [ ] Application builds without errors
- [ ] User has receptionist role and permissions
- [ ] JavaScript is enabled in browser
- [ ] Form validation is working
- [ ] Network requests are successful (200 OK)
- [ ] No console errors in browser
- [ ] Repository methods are implemented
- [ ] Service layer is properly wired

---

## 🎯 Expected Results

After successful setup and testing:

✅ **Patient Search**: Fast and accurate patient lookup
✅ **Doctor Selection**: Dropdown populated with available doctors  
✅ **Date/Time Validation**: Prevents past dates and invalid times
✅ **Appointment Creation**: Saves to database with unique ID
✅ **Success Confirmation**: Clear appointment details displayed
✅ **Dashboard Update**: New appointments appear in today's list
✅ **Print/SMS Ready**: Integration points for notifications

Your Clinical Management System should now have a fully functional appointment scheduling system! 🏥✨