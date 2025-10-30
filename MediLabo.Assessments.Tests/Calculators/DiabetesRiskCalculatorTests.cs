using FluentAssertions;
using MediLabo.Assessments.API.Calculators;
using MediLabo.Assessments.API.Models;
using MediLabo.Common.Models;

namespace MediLabo.Assessments.Tests.Calculators
{
    [Trait("Category", "Unit")]
    public class DiabetesRiskCalculatorTests
    {
        private readonly DiabetesRiskCalculator _calculator;

        public DiabetesRiskCalculatorTests()
        {
            _calculator = new DiabetesRiskCalculator();
        }

        [Fact]
        public void CalculateRisk_Person30OrOlder_NoTriggers_ReturnsNone()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 35,
                Gender = Gender.Male,
                TriggerCount = 1
            };

            // Act
            var result = _calculator.CalculateRisk(input);

            // Assert
            result.Should().Be(DiabetesRiskLevel.None);
        }

        [Fact]
        public void CalculateRisk_Person30OrOlder_TwoTriggers_ReturnsBorderline()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 40,
                Gender = Gender.Male,
                TriggerCount = 2
            };

            // Act
            var result = _calculator.CalculateRisk(input);

            // Assert
            result.Should().Be(DiabetesRiskLevel.Borderline);
        }

        [Fact]
        public void CalculateRisk_Person30OrOlder_FiveTriggers_ReturnsBorderline()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 40,
                Gender = Gender.Male,
                TriggerCount = 5
            };

            // Act
            var result = _calculator.CalculateRisk(input);

            // Assert
            result.Should().Be(DiabetesRiskLevel.Borderline);
        }

        [Fact]
        public void CalculateRisk_Person30OrOlder_SevenTriggers_ReturnsInDanger()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 45,
                Gender = Gender.Female,
                TriggerCount = 7
            };

            // Act
            var result = _calculator.CalculateRisk(input);

            // Assert
            result.Should().Be(DiabetesRiskLevel.InDanger);
        }

        [Fact]
        public void CalculateRisk_Person30OrOlder_EightOrMoreTriggers_ReturnsEarlyOnset()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 50,
                Gender = Gender.Female,
                TriggerCount = 8
            };

            // Act
            var result = _calculator.CalculateRisk(input);

            // Assert
            result.Should().Be(DiabetesRiskLevel.EarlyOnset);

        }

        [Fact]
        public void CalculateRisk_MaleUnder30_TwoTriggers_ReturnsNone()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 25,
                Gender = Gender.Male,
                TriggerCount = 2
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.None);
        }

        [Fact]
        public void CalculateRisk_MaleUnder30_ThreeTriggers_ReturnsInDanger()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 28,
                Gender = Gender.Male,
                TriggerCount = 3
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.InDanger);
        }

        [Fact]
        public void CalculateRisk_MaleUnder30_FourTriggers_ReturnsInDanger()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 22,
                Gender = Gender.Male,
                TriggerCount = 4
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.InDanger);
        }

        [Fact]
        public void CalculateRisk_MaleUnder30_FiveTriggers_ReturnsEarlyOnset()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 29,
                Gender = Gender.Male,
                TriggerCount = 5
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.EarlyOnset);
        }

        [Fact]
        public void CalculateRisk_MaleUnder30_EightTriggers_ReturnsEarlyOnset()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 27,
                Gender = Gender.Male,
                TriggerCount = 8
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.EarlyOnset);
        }
   
    [Fact]
        public void CalculateRisk_FemaleUnder30_ThreeTriggers_ReturnsNone()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 24,
                Gender = Gender.Female,
                TriggerCount = 3
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.None);
        }

        [Fact]
        public void CalculateRisk_FemaleUnder30_FourTriggers_ReturnsInDanger()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 26,
                Gender = Gender.Female,
                TriggerCount = 4
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.InDanger);
        }

        [Fact]
        public void CalculateRisk_FemaleUnder30_FiveTriggers_ReturnsInDanger()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 21,
                Gender = Gender.Female,
                TriggerCount = 5
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.InDanger);
        }

        [Fact]
        public void CalculateRisk_FemaleUnder30_SixTriggers_ReturnsInDanger()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 29,
                Gender = Gender.Female,
                TriggerCount = 6
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.InDanger);
        }

        [Fact]
        public void CalculateRisk_FemaleUnder30_SevenTriggers_ReturnsEarlyOnset()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 23,
                Gender = Gender.Female,
                TriggerCount = 7
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.EarlyOnset);
        }

        [Fact]
        public void CalculateRisk_FemaleUnder30_TenTriggers_ReturnsEarlyOnset()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 28,
                Gender = Gender.Female,
                TriggerCount = 10
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.EarlyOnset);
        }

        [Fact]
        public void CalculateRisk_PersonExactly30_FourTriggers_ReturnsBorderline()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 30,
                Gender = Gender.Male,
                TriggerCount = 4
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.Borderline);
        }

        [Fact]
        public void CalculateRisk_PersonExactly29_FourTriggers_Male_ReturnsInDanger()
        {
            // Arrange
            var input = new RiskInput
            {
                Age = 29,
                Gender = Gender.Male,
                TriggerCount = 4
            };
            // Act
            var result = _calculator.CalculateRisk(input);
            // Assert
            result.Should().Be(DiabetesRiskLevel.InDanger);
        }
    }
}
