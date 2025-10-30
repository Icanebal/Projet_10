using FluentAssertions;
using MediLabo.Assessments.API.Calculators;

namespace MediLabo.Assessments.Tests.Calculators
{
    [Trait("Category", "Unit")]
    public class TriggerTermsCalculatorTests
    {
        private readonly TriggerTermsCalculator _analyzer;

        public TriggerTermsCalculatorTests()
        {
            _analyzer = new TriggerTermsCalculator();
        }

        [Fact]
        public void CountTriggers_EmptyNotes_ReturnsZero()
        {
            // Arrange
            var notes = new List<string>();

            // Act
            var count = _analyzer.CountTriggers(notes);

            // Assert
            count.Should().Be(0);
        }

        [Fact]
        public void CountTriggers_NotesWithNoTriggers_ReturnsZero()
        {
            // Arrange
            var notes = new List<string>
            {
                "Le patient est en bonne santé.",
                "Aucun signe de diabète."
            };
            // Act
            var count = _analyzer.CountTriggers(notes);
            // Assert
            count.Should().Be(0);
        }

        [Fact]
        public void CountTriggers_NotesWithOneTriggers_ReturnsOne()
        {
            // Arrange
            var notes = new List<string>
            {
                "Le patient est redevenu fumeur.",
            };
            // Act
            var count = _analyzer.CountTriggers(notes);
            // Assert
            count.Should().Be(1);
        }

        [Fact]
        public void CountTriggers_NoteWithMultipleDifferentTriggers_ReturnsCorrectCount()
        {
            // Arrange
            var notes = new List<string>
            {
                "Le patient est fumeur et présente un taux de cholestérol élevé avec des vertiges."
            };
            // Act
            var count = _analyzer.CountTriggers(notes);
            // Assert
            count.Should().Be(3);
        }

        [Fact]
        public void CountTriggers_SameTriggerMultipleTimes_CountsOneOccurrence()
        {
            // Arrange
            var notes = new List<string>
            {
                "Le patient fumeur a été fumeur pendant 10 ans."
            };
            // Act
            var count = _analyzer.CountTriggers(notes);
            // Assert
            count.Should().Be(1);
        }

        [Fact]
        public void CountTriggers_MultipleNotesWithTriggers_ReturnsTotalCount()
        {
            // Arrange
            var notes = new List<string>
            {
                "Le patient est fumeur.",
                "Poids normal.",
                "Présence de cholestérol et vertiges."
            };
            // Act
            var count = _analyzer.CountTriggers(notes);
            // Assert
            count.Should().Be(4);
        }

        [Fact]
        public void CountTriggers_CaseInsensitive_CountsAllVariations()
        {
            // Arrange
            var notes = new List<string>
            {
                "Le patient est FUMEUR et le cholestérol est élevé. RÉACTION au traitement."
            };
            // Act
            var count = _analyzer.CountTriggers(notes);
            // Assert
            count.Should().Be(3);
        }

        [Fact]
        public void CountTriggers_AccentInsensitive_CountsAllVariations()
        {
            // Arrange
            var notes = new List<string>
            {
                "Réaction allergique et CHOLESTEROL."
            };
            // Act
            var count = _analyzer.CountTriggers(notes);
            // Assert
            count.Should().Be(2);
        }

        [Fact]
        public void CountTriggers_TriggerWithSpace_CountsCorrectly()
        {
            // Arrange
            var notes = new List<string>
            {
                "Taux d'Hémoglobine A1C élevé."
            };
            // Act
            var count = _analyzer.CountTriggers(notes);
            // Assert
            count.Should().Be(1);
        }

        [Fact]
        public void CountTriggers_ComplexCase_ReturnsCorrectCount()
        {
            // Arrange
            var notes = new List<string>
            {
                "Le patient FUMEUR présente du Cholestérol.",
                "Poids anormal, vertiges et réaction.",
                "Microalbumine détectée. Taille normale."
            };
            // Act
            var count = _analyzer.CountTriggers(notes);
            // Assert
            count.Should().Be(8);
        }
    }
}
