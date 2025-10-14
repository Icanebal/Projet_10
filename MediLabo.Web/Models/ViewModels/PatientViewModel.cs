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
}
