using MediLabo.Assessments.API.Models;
using MediLabo.Common;
using MediLabo.Common.DTOs;

namespace MediLabo.Assessments.API.Services;

public interface IPatientAndNotesService
{
    Task<Result<PatientAssessmentDto>> GetPatientAsync(int patientId);
    Task<Result<IEnumerable<NoteDto>>> GetPatientNotesAsync(int patientId);
}
