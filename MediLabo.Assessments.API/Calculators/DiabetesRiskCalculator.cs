using MediLabo.Assessments.API.Models;
using MediLabo.Common.Models;

namespace MediLabo.Assessments.API.Calculators;

public class DiabetesRiskCalculator
{
    public DiabetesRiskLevel CalculateRisk(RiskInput input)
    {
        if (input.Age < 30)
        {
            return CalculateRiskForUnderThirty(input.Gender, input.TriggerCount);
        }

        return CalculateRiskForThirtyOrOver(input.TriggerCount);
    }

    private DiabetesRiskLevel CalculateRiskForUnderThirty(Gender gender, int triggerCount)
    {
        if (gender == Gender.Male)
        {
            if (triggerCount <= 2)
                return DiabetesRiskLevel.None;

            if (triggerCount <= 4)
                return DiabetesRiskLevel.InDanger;

            return DiabetesRiskLevel.EarlyOnset;
        }
        else
        {
            if (triggerCount <= 3)
                return DiabetesRiskLevel.None;

            if (triggerCount <= 6)
                return DiabetesRiskLevel.InDanger;

            return DiabetesRiskLevel.EarlyOnset;
        }
    }

    private DiabetesRiskLevel CalculateRiskForThirtyOrOver(int triggerCount)
    {
        if (triggerCount <= 1)
            return DiabetesRiskLevel.None;

        if (triggerCount <= 5)
            return DiabetesRiskLevel.Borderline;

        if (triggerCount <= 7)
            return DiabetesRiskLevel.InDanger;

        return DiabetesRiskLevel.EarlyOnset;
    }
}
