namespace MediLabo.Assessments.API.Calculators;

public class AgeCalculator
{
    public int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;

        int age = today.Year - birthDate.Year;

        if (today.Month < birthDate.Month ||
            (today.Month == birthDate.Month && today.Day < birthDate.Day))
        {
            age--;
        }

        return age;
    }
}