-- Create Medical Notes Table
CREATE TABLE TblMedicalNotes (
    MedicalNoteId INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    AppointmentId INT NULL,
    Title NVARCHAR(200) NOT NULL,
    Notes NVARCHAR(MAX) NOT NULL,
    Diagnosis NVARCHAR(MAX) NULL,
    Prescription NVARCHAR(MAX) NULL,
    FollowUpInstructions NVARCHAR(MAX) NULL,
    NextAppointmentDate DATETIME NULL,
    NoteType NVARCHAR(50) NOT NULL DEFAULT 'Consultation',
    Priority NVARCHAR(20) NOT NULL DEFAULT 'Normal',
    LabTests NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedBy INT NULL,
    UpdatedDate DATETIME NULL,
    
    -- Foreign Key Constraints (uncomment if tables exist)
    -- CONSTRAINT FK_MedicalNotes_Patient FOREIGN KEY (PatientId) REFERENCES TblPatients(PatientId),
    -- CONSTRAINT FK_MedicalNotes_Doctor FOREIGN KEY (DoctorId) REFERENCES TblDoctors(DoctorId),
    -- CONSTRAINT FK_MedicalNotes_Appointment FOREIGN KEY (AppointmentId) REFERENCES TblAppointments(AppointmentId),
    -- CONSTRAINT FK_MedicalNotes_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES TblUsers(UserId),
    -- CONSTRAINT FK_MedicalNotes_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES TblUsers(UserId)
);

-- Create indexes for better performance
CREATE INDEX IX_MedicalNotes_PatientId ON TblMedicalNotes(PatientId);
CREATE INDEX IX_MedicalNotes_DoctorId ON TblMedicalNotes(DoctorId);
CREATE INDEX IX_MedicalNotes_AppointmentId ON TblMedicalNotes(AppointmentId);
CREATE INDEX IX_MedicalNotes_CreatedDate ON TblMedicalNotes(CreatedDate);
CREATE INDEX IX_MedicalNotes_IsActive ON TblMedicalNotes(IsActive);

-- Sample data (optional)
-- INSERT INTO TblMedicalNotes (PatientId, DoctorId, Title, Notes, NoteType, Priority, CreatedBy)
-- VALUES (1, 1, 'Initial Consultation', 'Patient presented with fever and headache. Vital signs stable.', 'Consultation', 'Normal', 1);

PRINT 'TblMedicalNotes table created successfully!';
