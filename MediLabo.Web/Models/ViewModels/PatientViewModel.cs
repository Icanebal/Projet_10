using MediLabo.Common.Models;

namespace MediLabo.Web.Models.ViewModels;

public class PatientViewModel
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public DateTime BirthDate { get; init; }
    public int GenderId { get; init; }
    public required string GenderName { get; init; }
    public string? Address { get; init; }
    public string? Phone { get; init; }

    public DiabetesRiskLevel? DiabetesRisk { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }
    }

    public string FormattedBirthDate => BirthDate.ToString("dd/MM/yyyy");

    public string DiabetesRiskLabel => DiabetesRisk switch
    {
        DiabetesRiskLevel.None => "Aucun",
        DiabetesRiskLevel.Borderline => "Risque limité",
        DiabetesRiskLevel.InDanger => "Danger",
        DiabetesRiskLevel.EarlyOnset => "Apparition précoce",
        _ => "Non évalué"
    };

    public string DiabetesRiskBadgeClass => DiabetesRisk switch
    {
        DiabetesRiskLevel.None => "badge bg-success",
        DiabetesRiskLevel.Borderline => "badge bg-warning text-dark",
        DiabetesRiskLevel.InDanger => "badge bg-orange text-white",
        DiabetesRiskLevel.EarlyOnset => "badge bg-danger",
        _ => "badge bg-secondary"
    };
}
