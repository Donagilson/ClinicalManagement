-- =====================================================
-- Sample Data Script
-- =====================================================
-- Run this after QuickSetup.sql to add test data
-- =====================================================

PRINT 'Adding sample data...';

-- Sample Lab Tests
INSERT INTO TblLabTests (TestCode, TestName, Description, Price, NormalRange, Unit, IsActive)
VALUES
('CBC001', 'Complete Blood Count', 'Complete blood analysis including RBC, WBC, platelets', 150.00, 'RBC: 4.2-5.4 million/μL, WBC: 4,000-11,000/μL', 'cells/μL', 1),
('LFT001', 'Liver Function Test', 'Tests for liver enzymes and function', 200.00, 'ALT: 7-56 U/L, AST: 10-40 U/L', 'U/L', 1),
('RFT001', 'Renal Function Test', 'Tests for kidney function', 180.00, 'Creatinine: 0.7-1.3 mg/dL', 'mg/dL', 1),
('BS001', 'Blood Sugar', 'Fasting and post-prandial blood glucose', 100.00, 'Fasting: 70-100 mg/dL', 'mg/dL', 1),
('LIPID001', 'Lipid Profile', 'Cholesterol and triglyceride levels', 250.00, 'Total Cholesterol: <200 mg/dL', 'mg/dL', 1);

-- Sample Medical Note (adjust IDs based on your existing data)
-- Uncomment and modify these based on your actual Patient, Doctor, and User IDs
/*
INSERT INTO TblMedicalNotes (PatientId, DoctorId, Title, Notes, Diagnosis, Prescription, LabTests, NoteType, Priority, CreatedBy)
VALUES
(1, 1, 'Initial Consultation', 'Patient presented with fever and headache. Vital signs stable.', 'Viral Fever',
'Paracetamol 500mg three times daily for 3 days', 'Complete Blood Count', 'Consultation', 'Normal', 1);
*/

PRINT 'Sample data added successfully!';
PRINT 'You can now test the prescription and lab test workflows.';
