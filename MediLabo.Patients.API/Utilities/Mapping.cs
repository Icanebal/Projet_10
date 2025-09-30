using MediLabo.Patients.API.Models.DTOs;
using MediLabo.Patients.API.Models.Entities;

namespace MediLabo.Patients.API.Utilities
{
    // Classe statique pour centraliser tous les mappings entre entités et DTOs
    public static class Mapping
    {
        // Mapping manuel Entity vers DTO pour les réponses API
        public static PatientDto MapToDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                BirthDate = patient.BirthDate,
                Gender = patient.Gender,
                Address = patient.Address,
                Phone = patient.Phone
            };
        }

        // Mapping manuel CreateDTO vers Entity pour la création
        public static Patient MapToEntity(CreatePatientDto createDto)
        {
            return new Patient
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                BirthDate = createDto.BirthDate,
                Gender = createDto.Gender,
                Address = createDto.Address,
                Phone = createDto.Phone
            };
        }

        // Mapping pour mettre à jour une entité existante avec les données du DTO
        public static void MapUpdateToEntity(CreatePatientDto updateDto, Patient existingPatient)
        {
            existingPatient.FirstName = updateDto.FirstName;
            existingPatient.LastName = updateDto.LastName;
            existingPatient.BirthDate = updateDto.BirthDate;
            existingPatient.Gender = updateDto.Gender;
            existingPatient.Address = updateDto.Address;
            existingPatient.Phone = updateDto.Phone;
        }

        // Extension pour mapper une collection d'entités vers DTOs
        public static IEnumerable<PatientDto> MapToDtoCollection(IEnumerable<Patient> patients)
        {
            return patients.Select(MapToDto);
        }
    }
}
