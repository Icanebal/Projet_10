using FluentAssertions;
using MediLabo.Assessments.API.Calculators;

namespace MediLabo.Assessments.Tests.Calculators
{
    [Trait("Category", "Unit")]
    public class AgeCalculatorTests
    {
        private readonly AgeCalculator _calculator;

        public AgeCalculatorTests()
        {
            _calculator = new AgeCalculator();
        }

        [Fact]
        public void CalculateAge_BirthdayIsToday_ReturnsCorrectAge()
        {
            var birthDate = DateTime.Today.AddYears(-30);

            var age = _calculator.CalculateAge(birthDate);

            age.Should().Be(30);
        }

        [Fact]
        public void CalculateAge_BirthdayWasYesterday_ReturnsCorrectAge()
        {
            var birthDate = DateTime.Today.AddYears(-30).AddDays(-1);

            var age = _calculator.CalculateAge(birthDate);

            age.Should().Be(30);
        }

        [Fact]
        public void CalculateAge_BirthdayIsTomorrow_ReturnsAgeMinusOne()
        {
            var birthDate = DateTime.Today.AddYears(-30).AddDays(1);

            var age = _calculator.CalculateAge(birthDate);

            age.Should().Be(29);
        }

        [Fact]
        public void CalculateAge_SixMonthsAgo_ReturnsZero()
        {
            var birthDate = DateTime.Today.AddMonths(-6);

            var age = _calculator.CalculateAge(birthDate);

            age.Should().Be(0);
        }

        [Fact]
        public void CalculateAge_VeryOldPerson_ReturnsCorrectAge()
        {
            var birthDate = DateTime.Today.AddYears(-105);

            var age = _calculator.CalculateAge(birthDate);

            age.Should().Be(105);
        }
    }
}
