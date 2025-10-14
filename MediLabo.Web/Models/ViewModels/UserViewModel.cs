namespace MediLabo.Web.Models.ViewModels;
public class UserViewModel
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string FullName => $"{FirstName} {LastName}";
    public List<string> Roles { get; set; } = new();
}
