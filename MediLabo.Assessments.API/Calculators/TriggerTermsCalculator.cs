using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MediLabo.Assessments.API.Calculators
{
    public class TriggerTermsCalculator
    {
        private static readonly List<string> TriggerTerms = new()
    {
        "Hemoglobine A1C",
        "Microalbumine",
        "Taille",
        "Poids",
        "Fume",
        "Anormal",
        "Cholesterol",
        "Vertige",
        "Rechute",
        "Reaction",
        "Anticorps"
    };

        public int CountTriggers(IEnumerable<string> notes)
        {
            if (notes == null || !notes.Any())
            {
                return 0;
            }

            var triggerSet = new HashSet<string>();
            var allNotesText = string.Join(" ", notes);
            var normalizedText = NormalizeText(allNotesText);

            foreach (var trigger in TriggerTerms)
            {
                var normalizedTrigger = NormalizeText(trigger);
                var pattern = $@"\b{Regex.Escape(normalizedTrigger)}";


                if (Regex.IsMatch(normalizedText, pattern, RegexOptions.IgnoreCase))
                {
                    triggerSet.Add(normalizedTrigger);
                }
            }

            return triggerSet.Count;
        }

        private string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }
            text = text.ToLowerInvariant();

            text = RemoveAccents(text);

            return text;
        }

        private string RemoveAccents(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var character in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
