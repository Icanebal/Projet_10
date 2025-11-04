using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MediLabo.Common;
using MediLabo.Common.DTOs;
using MediLabo.Common.Models;

namespace MediLabo.Assessments.API.Tests.Integration;

// Tests d'intégration pour l'API Assessments
// Teste le endpoint /api/assessments/diabetes/{patientId} de bout en bout
[Trait("Category", "Integration")]
public class AssessmentsApiIntegrationTests : IClassFixture<AssessmentsWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly AssessmentsWebApplicationFactory _factory;

    public AssessmentsApiIntegrationTests(AssessmentsWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ConfigureTestData();
        _client = factory.CreateClient();
    }

    // Test Patient 1: TestNone (Femme, 58 ans, 0 termes) → None
    [Fact]
    public async Task GetDiabetesRisk_Patient1TestNone_ReturnsNone()
    {
        // Act
        var response = await _client.GetAsync("/api/assessments/diabetes/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<DiabetesRiskResponse>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value!.RiskLevel.Should().Be(DiabetesRiskLevel.None,
            "Femme de 58 ans avec 0 termes déclencheurs devrait avoir un risque 'None'");
    }

    // Test Patient 2: TestBorderline (Homme, 79 ans, 2 termes) → Borderline
    [Fact]
    public async Task GetDiabetesRisk_Patient2TestBorderline_ReturnsBorderline()
    {
        // Act
        var response = await _client.GetAsync("/api/assessments/diabetes/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<DiabetesRiskResponse>>();
        result!.IsSuccess.Should().BeTrue();
        result.Value!.RiskLevel.Should().Be(DiabetesRiskLevel.Borderline,
            "Homme de 79 ans avec 2 termes déclencheurs devrait avoir un risque 'Borderline'");
    }

    // Test Patient 3: TestInDanger (Homme, 20 ans, 3 termes) → InDanger
    [Fact]
    public async Task GetDiabetesRisk_Patient3TestInDanger_ReturnsInDanger()
    {
        // Act
        var response = await _client.GetAsync("/api/assessments/diabetes/3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<DiabetesRiskResponse>>();
        result!.IsSuccess.Should().BeTrue();
        result.Value!.RiskLevel.Should().Be(DiabetesRiskLevel.InDanger,
            "Homme de 20 ans avec 3 termes déclencheurs devrait avoir un risque 'InDanger'");
    }

    // Test Patient 4: TestEarlyOnset (Femme, 22 ans, 7+ termes) → EarlyOnset
    [Fact]
    public async Task GetDiabetesRisk_Patient4TestEarlyOnset_ReturnsEarlyOnset()
    {
        // Act
        var response = await _client.GetAsync("/api/assessments/diabetes/4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<DiabetesRiskResponse>>();
        result!.IsSuccess.Should().BeTrue();
        result.Value!.RiskLevel.Should().Be(DiabetesRiskLevel.EarlyOnset,
            "Femme de 22 ans avec 7+ termes déclencheurs devrait avoir un risque 'EarlyOnset'");
    }

    // Test global: Tous les patients retournent le bon risque
    [Fact]
    public async Task GetDiabetesRisk_AllTestPatients_ReturnCorrectRiskLevels()
    {
        // Arrange
        var testCases = new Dictionary<int, DiabetesRiskLevel>
        {
            { 1, DiabetesRiskLevel.None },
            { 2, DiabetesRiskLevel.Borderline },
            { 3, DiabetesRiskLevel.InDanger },
            { 4, DiabetesRiskLevel.EarlyOnset }
        };

        // Act & Assert
        foreach (var (patientId, expectedRisk) in testCases)
        {
            var response = await _client.GetAsync($"/api/assessments/diabetes/{patientId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<Result<DiabetesRiskResponse>>();
            result!.Value!.RiskLevel.Should().Be(expectedRisk,
                $"Patient {patientId} devrait avoir le risque {expectedRisk}");
        }
    }

    // Test: Patient inexistant
    [Fact]
    public async Task GetDiabetesRisk_NonExistentPatient_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/assessments/diabetes/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
