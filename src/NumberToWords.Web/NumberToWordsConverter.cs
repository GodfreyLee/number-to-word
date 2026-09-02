using System.Globalization;
using System.Text.RegularExpressions;

namespace NumberToWords.Web;

public readonly record struct ConversionResult(string Words, string? Note);

public static partial class NumberToWordsConverter
{
    [GeneratedRegex(@"^[+-]?\d+(\.\d+)?$")]
    private static partial Regex NumericPattern();

    public const string MaxSupportedAmount = "9,223,372,036,854,775,807.99";

    private static readonly string[] Ones =
    {
        "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE",
        "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN",
        "SEVENTEEN", "EIGHTEEN", "NINETEEN"
    };

    private static readonly string[] Tens =
    {
        "", "", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY"
    };

    // ponytail: covers up to long.MaxValue (quintillions); no known requirement to go higher.
    private static readonly string[] Scales =
    {
        "", "THOUSAND", "MILLION", "BILLION", "TRILLION", "QUADRILLION", "QUINTILLION"
    };

    public static ConversionResult Convert(string input)
    {
        if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            if (NumericPattern().IsMatch(input.Trim()))
            {
                throw new ArgumentException($"'{input}' is too large to convert. Maximum supported amount is {MaxSupportedAmount}.");
            }

            throw new ArgumentException($"'{input}' is not a valid number.");
        }

        var isNegative = value < 0;
        var absValue = Math.Abs(value);
        var roundedValue = decimal.Round(absValue, 2, MidpointRounding.AwayFromZero);
        var wasRounded = absValue != roundedValue;
        value = roundedValue;

        long dollars;
        try
        {
            dollars = (long)decimal.Truncate(value);
        }
        catch (OverflowException)
        {
            throw new ArgumentException($"'{input}' is too large to convert. Maximum supported amount is {MaxSupportedAmount}.");
        }

        var cents = (int)decimal.Round((value - dollars) * 100, 0, MidpointRounding.AwayFromZero);

        var dollarUnit = dollars == 1 ? "DOLLAR" : "DOLLARS";
        var centUnit = cents == 1 ? "CENT" : "CENTS";
        var sign = isNegative ? "NEGATIVE " : "";

        // ponytail: omit whichever half is zero, unless the whole amount is zero (nothing left to say otherwise).
        var showDollars = dollars != 0 || cents == 0;
        var showCents = cents != 0 || dollars == 0;

        var dollarPart = $"{ConvertWholeNumber(dollars)} {dollarUnit}";
        var centPart = $"{ConvertUnderThousand(cents)} {centUnit}";

        var amountWords = (showDollars, showCents) switch
        {
            (true, true) => $"{dollarPart} AND {centPart}",
            (true, false) => dollarPart,
            _ => centPart
        };

        var words = $"{sign}{amountWords}";
        var note = wasRounded
            ? $"Amount rounded to {(isNegative ? "-" : "")}{dollars}.{cents:D2} before conversion."
            : null;

        return new ConversionResult(words, note);
    }

    private static string ConvertWholeNumber(long number)
    {
        if (number == 0)
        {
            return Ones[0];
        }

        var groups = new List<string>();
        var scaleIndex = 0;

        while (number > 0)
        {
            var group = (int)(number % 1000);
            number /= 1000;

            if (group > 0)
            {
                var scale = Scales[scaleIndex];
                var groupWords = ConvertUnderThousand(group);
                groups.Insert(0, scale.Length == 0 ? groupWords : $"{groupWords} {scale}");
            }

            scaleIndex++;
        }

        return string.Join(" ", groups);
    }

    private static string ConvertUnderThousand(int number)
    {
        if (number < 20)
        {
            return Ones[number];
        }

        if (number < 100)
        {
            var tens = Tens[number / 10];
            var ones = number % 10;
            return ones == 0 ? tens : $"{tens}-{Ones[ones]}";
        }

        var hundredsWords = $"{Ones[number / 100]} HUNDRED";
        var remainder = number % 100;

        return remainder == 0 ? hundredsWords : $"{hundredsWords} AND {ConvertUnderThousand(remainder)}";
    }
}
