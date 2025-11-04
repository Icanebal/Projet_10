using MediLabo.Assessments.API.Models;
using MediLabo.Assessments.API.Services;
using MediLabo.Common;
using MediLabo.Common.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MediLabo.Assessments.API.Tests.Integration;

public class AssessmentsWebApplicationFactory : WebApplicationFactory<MediLabo.Assessments.API.Controllers.AssessmentsController>
{
    public Mock<IPatientAndNotesService> MockPatientAndNotesService { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

            services.AddScoped<IPatientAndNotesService>(_ => MockPatientAndNotesService.Object);
        });

        builder.UseEnvironment("Test");
    }

    public void ConfigureTestData()
    {
        MockPatientAndNotesService
            .Setup(s => s.GetPatientAsync(1))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(new PatientAssessmentDto
            {
                Id = 1,
                BirthDate = new DateTime(1966, 12, 31),
                GenderId = 2
            }));

        MockPatientAndNotesService
            .Setup(s => s.GetPatientNotesAsync(1))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(new List<NoteDto>
            {
                new() { Id = "1", PatientId = 1, Content = "Patient en bonne santé générale.", CreatedAt = DateTime.UtcNow }
            }));

        MockPatientAndNotesService
            .Setup(s => s.GetPatientAsync(2))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(new PatientAssessmentDto
            {
                Id = 2,
                BirthDate = new DateTime(1945, 6, 24),
                GenderId = 1
            }));

        MockPatientAndNotesService
            .Setup(s => s.GetPatientNotesAsync(2))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(new List<NoteDto>
            {
                new() { Id = "2", PatientId = 2, Content = "Patient fumeur depuis 20 ans. Hémoglobine A1C légèrement élevée.", CreatedAt = DateTime.UtcNow }
            }));

        MockPatientAndNotesService
            .Setup(s => s.GetPatientAsync(3))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(new PatientAssessmentDto
            {
                Id = 3,
                BirthDate = new DateTime(2004, 6, 18),
                GenderId = 1 
            }));

        MockPatientAndNotesService
            .Setup(s => s.GetPatientNotesAsync(3))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(new List<NoteDto>
            {
                new() { Id = "3", PatientId = 3, Content = "Patient fumeur. Taille et poids à surveiller attentivement.", CreatedAt = DateTime.UtcNow }
            }));

        MockPatientAndNotesService
            .Setup(s => s.GetPatientAsync(4))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(new PatientAssessmentDto
            {
                Id = 4,
                BirthDate = new DateTime(2002, 6, 28),
                GenderId = 2
            }));

        MockPatientAndNotesService
            .Setup(s => s.GetPatientNotesAsync(4))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(new List<NoteDto>
            {
                new() { Id = "4a", PatientId = 4, Content = "Anticorps élevés. Réaction anormale.", CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new() { Id = "4b", PatientId = 4, Content = "Fumeuse. Hémoglobine A1C très élevée.", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new() { Id = "4c", PatientId = 4, Content = "Taille et poids normaux. Cholestérol anormal. Vertiges.", CreatedAt = DateTime.UtcNow }
            }));

        MockPatientAndNotesService
            .Setup(s => s.GetPatientAsync(999))
            .ReturnsAsync(Result<PatientAssessmentDto>.Failure("Patient not found"));
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        Microsoft.Extensions.Options.IOptionsMonitor<AuthenticationSchemeOptions> options,
        Microsoft.Extensions.Logging.ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "test-user"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "Test User")
        };

        var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestScheme");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}