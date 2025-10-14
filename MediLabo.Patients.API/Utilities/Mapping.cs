using MediLabo.Patients.API.Models.DTOs;
using MediLabo.Patients.API.Models.Entities;

namespace MediLabo.Patients.API.Utilities
{
    public static class Mapping
    {
        public static PatientDto MapToDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                BirthDate = patient.BirthDate,
                GenderId = patient.GenderId,
                GenderName = patient.Gender.Name,
                Address = patient.Address,
                Phone = patient.Phone
            };
        }

        public static Patient MapToEntity(CreatePatientDto createDto)
        {
            return new Patient
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                BirthDate = createDto.BirthDate,
                GenderId = createDto.GenderId,
                Address = createDto.Address,
                Phone = createDto.Phone
            };
        }

        public static void MapUpdateToEntity(CreatePatientDto updateDto, Patient existingPatient)
        {
            existingPatient.FirstName = updateDto.FirstName;
            existingPatient.LastName = updateDto.LastName;
            existingPatient.BirthDate = updateDto.BirthDate;
            existingPatient.GenderId = updateDto.GenderId;
            existingPatient.Address = updateDto.Address;
            existingPatient.Phone = updateDto.Phone;
        }

        public static IEnumerable<PatientDto> MapToDtoCollection(IEnumerable<Patient> patients)
        {
            return patients.Select(MapToDto);
        }

        public static GenderDto MapToGenderDto(Gender gender)
        {
            return new GenderDto
            {
                Id = gender.Id,
                Name = gender.Name
            };
        }
        public static IEnumerable<GenderDto> MapToGenderDtoCollection(IEnumerable<Gender> genders)
        {
            return genders.Select(MapToGenderDto);
        }
    }
}
