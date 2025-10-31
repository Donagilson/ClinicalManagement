-- Create Prescriptions Table
CREATE TABLE TblPrescriptions (
    PrescriptionId INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentId INT NULL,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    Diagnosis NVARCHAR(MAX) NULL,
    PrescriptionDate DATETIME NOT NULL DEFAULT GETDATE(),
    Notes NVARCHAR(MAX) NULL,

    -- Foreign Key Constraints (uncomment when referenced tables exist)
    -- CONSTRAINT FK_Prescriptions_Appointment FOREIGN KEY (AppointmentId) REFERENCES TblAppointments(AppointmentId),
    -- CONSTRAINT FK_Prescriptions_Patient FOREIGN KEY (PatientId) REFERENCES TblPatients(PatientId),
    -- CONSTRAINT FK_Prescriptions_Doctor FOREIGN KEY (DoctorId) REFERENCES TblDoctors(DoctorId)
);

-- Create Prescription Details Table
CREATE TABLE TblPrescriptionDetails (
    PrescriptionDetailId INT IDENTITY(1,1) PRIMARY KEY,
    PrescriptionId INT NOT NULL,
    MedicineId INT NOT NULL,
    Dosage NVARCHAR(100) NULL,
    Frequency NVARCHAR(100) NULL,
    Duration NVARCHAR(100) NULL,
    Instructions NVARCHAR(500) NULL,
    Quantity INT NOT NULL,

    -- Foreign Key Constraints (uncomment when referenced tables exist)
    -- CONSTRAINT FK_PrescriptionDetails_Prescription FOREIGN KEY (PrescriptionId) REFERENCES TblPrescriptions(PrescriptionId),
    -- CONSTRAINT FK_PrescriptionDetails_Medicine FOREIGN KEY (MedicineId) REFERENCES TblMedicines(MedicineId)
);

-- Create indexes for better performance
CREATE INDEX IX_Prescriptions_PatientId ON TblPrescriptions(PatientId);
CREATE INDEX IX_Prescriptions_DoctorId ON TblPrescriptions(DoctorId);
CREATE INDEX IX_Prescriptions_AppointmentId ON TblPrescriptions(AppointmentId);
CREATE INDEX IX_PrescriptionDetails_PrescriptionId ON TblPrescriptionDetails(PrescriptionId);
CREATE INDEX IX_PrescriptionDetails_MedicineId ON TblPrescriptionDetails(MedicineId);

PRINT 'Prescription tables created successfully!';
