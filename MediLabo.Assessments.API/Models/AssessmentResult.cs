using MediLabo.Common.Models;

namespace MediLabo.Assessments.API.Models
{
    public class AssessmentResult
    {
        public int PatientId { get; set; }
        public DiabetesRiskLevel RiskLevel { get; set; }
        public int TriggerCount { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
    }
}
