using System;
using System.Collections.Generic;

namespace Health.Application.Common
{
    public class PrescriptionPdfMedicineItem
    {
        public string Name { get; set; } = string.Empty;
        public string Dose { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class PrescriptionPdfModel
    {
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public List<PrescriptionPdfMedicineItem> Medicines { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
    }
}
