using MediLabo.Common;
using MediLabo.Common.DTOs;

namespace MediLabo.Notes.API.Services;

public interface IPatientService
{
    Task<Result<PatientDto>> GetPatientAsync(int patientId);
}
