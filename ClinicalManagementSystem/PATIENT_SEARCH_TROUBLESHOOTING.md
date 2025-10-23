# Patient Search Troubleshooting Guide

## 🔍 Overview
This guide helps troubleshoot the patient search functionality in the Clinical Management System when searching by Patient ID or Phone Number is not working.

## 🚨 Current Issues & Solutions

### Issue 1: Search Modal Not Opening
**Problem**: Clicking "Search Patient" button doesn't open the modal

**Solutions**:
1. Check browser console for JavaScript errors (F12 → Console tab)
2. Verify Bootstrap is loaded properly
3. Ensure jQuery is available

**Quick Test**:
```javascript
// Open browser console and run:
$('#searchPatientModal').modal('show');
```

### Issue 2: Search Button Not Responding
**Problem**: Modal opens but clicking "Search" does nothing

**Debug Steps**:
1. Open browser console (F12)
2. Click "Search Patient" to open modal
3. Enter a test search term (e.g., "1" or "9207039181")
4. Click Search button
5. Check console for debug messages:
   - "Search button clicked"
   - "Search term: [your input]"
   - "Making AJAX request to: [URL]"
   - "AJAX response received: [response]"

### Issue 3: AJAX Request Failing
**Problem**: Search request fails with error

**Debug Info**: Check console for:
- "AJAX request failed: [status] [error]"
- "XHR response: [server response]"

**Common Fixes**:
1. **Authentication Issue**: Make sure you're logged in as receptionist
2. **Route Issue**: Verify controller action exists
3. **Database Issue**: Check database connection

### Issue 4: No Search Results
**Problem**: Search completes but shows "No patients found"

**Verification Steps**:
1. **Check Database Data**:
   ```sql
   -- Run in SQL Server Management Studio
   SELECT TOP 5 PatientId, FirstName, LastName, Phone, IsActive 
   FROM TblPatients 
   WHERE IsActive = 1
   ORDER BY PatientId DESC;
   ```

2. **Test with Known Data**:
   - Patient ID: `1` (if exists)
   - Phone: `9207039181` (test patient)

3. **Create Test Patient**:
   ```sql
   INSERT INTO TblPatients (FirstName, LastName, Gender, DateOfBirth, Phone, Email, Address, BloodGroup, EmergencyContact, IsActive)
   VALUES ('Test', 'Patient', 'Male', '1990-01-01', '1234567890', 'test@test.com', 'Test Address', 'O+', '0987654321', 1);
   ```

## 🛠️ Step-by-Step Testing

### Test 1: Modal Functionality
1. Go to receptionist dashboard
2. Click "Search Patient" button
3. ✅ **Expected**: Modal opens with search input field
4. ❌ **If fails**: Check JavaScript console for errors

### Test 2: Search Input
1. In search modal, enter "1" (patient ID)
2. Click "Search" button
3. ✅ **Expected**: Loading spinner appears, then results or "no results" message
4. ❌ **If fails**: Check console for debug messages

### Test 3: Phone Search
1. In search modal, enter "9207039181" (phone number)
2. Click "Search" button
3. ✅ **Expected**: Shows patient "Adithya P" if exists
4. ❌ **If fails**: Check if patient exists in database

### Test 4: Multiple Results
1. Create multiple patients with same phone number
2. Search by that phone number
3. ✅ **Expected**: Shows all patients with that phone number
4. ❌ **If fails**: Check repository method implementation

## 🔧 Manual Database Testing

### Check Patient Data
```sql
-- Verify patients exist
SELECT COUNT(*) as TotalPatients FROM TblPatients WHERE IsActive = 1;

-- Check specific test patient
SELECT * FROM TblPatients WHERE Phone = '9207039181' AND IsActive = 1;

-- Check by ID
SELECT * FROM TblPatients WHERE PatientId = 1 AND IsActive = 1;
```

### Create Test Data
```sql
-- Insert test patient if none exist
IF NOT EXISTS (SELECT 1 FROM TblPatients WHERE Phone = '9207039181')
BEGIN
    INSERT INTO TblPatients (FirstName, LastName, Gender, DateOfBirth, Phone, Email, Address, BloodGroup, EmergencyContact, IsActive)
    VALUES ('Adithya', 'P', 'Female', '1993-01-01', '9207039181', 'adi@gmail.com', '123 Main Street', 'AB+', '9876543210', 1);
END
```

## 🐛 Common JavaScript Issues

### Console Error: "$ is not defined"
**Fix**: jQuery not loaded
```html
<!-- Ensure this is in _Layout.cshtml -->
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
```

### Console Error: "modal is not a function"
**Fix**: Bootstrap JS not loaded
```html
<!-- Ensure this is in _Layout.cshtml -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
```

### Console Error: "POST 404 Not Found"
**Fix**: Controller action missing or wrong route
- Verify `SearchPatientByIdOrPhone` method exists in `ReceptionistController`
- Check method has `[HttpPost]` attribute

## 📱 Expected Behavior

### Successful Search Flow
1. **Click "Search Patient"** → Modal opens
2. **Enter search term** → Input accepted
3. **Click "Search"** → Loading spinner shows
4. **Results appear** → Patient cards displayed with:
   - Patient name
   - Phone number
   - Age and gender
   - "View Details" and "Schedule" buttons

### Search Results Format
```
┌─────────────────────────────────┐
│ Adithya P                  ID: 1│
│ 📞 Phone: 9207039181           │
│ 👤 Gender: Female | 🎂 Age: 30  │
│ 📧 Email: adi@gmail.com        │
│ [View Details] [Schedule]       │
└─────────────────────────────────┘
```

## 🚀 Quick Fixes

### Enable Debug Mode
Add to `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning"
    }
  }
}
```

### Test Direct API Call
Open browser console and test:
```javascript
// Test search API directly
$.post('/Receptionist/SearchPatientByIdOrPhone', { searchTerm: '1' })
  .done(function(data) { console.log('Success:', data); })
  .fail(function(xhr) { console.log('Error:', xhr.responseText); });
```

### Reset Search Modal
If modal gets stuck:
```javascript
// Run in browser console
$('#searchPatientModal').modal('hide');
$('#searchResults').html('');
$('#searchTerm').val('');
```

## 📞 Support Checklist

If search still doesn't work, verify:
- [ ] Database connection string is correct
- [ ] TblPatients table exists and has data
- [ ] Patient records have IsActive = 1
- [ ] User is logged in with receptionist role
- [ ] JavaScript console shows no errors
- [ ] Network tab shows AJAX requests are sent
- [ ] Server returns 200 OK response
- [ ] SearchPatientByIdOrPhone method exists in controller
- [ ] Bootstrap and jQuery are loaded

## 🔍 Browser Developer Tools Guide

1. **Open Dev Tools**: Press F12
2. **Console Tab**: See JavaScript errors and debug messages
3. **Network Tab**: Monitor AJAX requests
   - Look for POST to `/Receptionist/SearchPatientByIdOrPhone`
   - Check response status and content
4. **Elements Tab**: Inspect HTML structure of modal

---

**Note**: If you still experience issues after following this guide, the problem might be in the database connection, authentication, or server-side code. Check the application logs and ensure all dependencies are properly configured.