using NumberToWords.Web;

namespace NumberToWords.Tests;

public class ConverterTests
{
    [Theory]
    [InlineData("123.45", "ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS")]
    [InlineData("0", "ZERO DOLLARS AND ZERO CENTS")]
    [InlineData("0.00", "ZERO DOLLARS AND ZERO CENTS")]
    [InlineData("1", "ONE DOLLAR")]
    [InlineData("0.01", "ONE CENT")]
    [InlineData("1.01", "ONE DOLLAR AND ONE CENT")]
    [InlineData("100", "ONE HUNDRED DOLLARS")]
    [InlineData("0.99", "NINETY-NINE CENTS")]
    [InlineData("1000000", "ONE MILLION DOLLARS")]
    [InlineData("1000000000", "ONE BILLION DOLLARS")]
    [InlineData("20.05", "TWENTY DOLLARS AND FIVE CENTS")]
    [InlineData(".45", "FORTY-FIVE CENTS")]
    [InlineData("1.005", "ONE DOLLAR AND ONE CENT")]
    [InlineData("1234567.89", "ONE MILLION TWO HUNDRED AND THIRTY-FOUR THOUSAND FIVE HUNDRED AND SIXTY-SEVEN DOLLARS AND EIGHTY-NINE CENTS")]
    [InlineData("-5", "NEGATIVE FIVE DOLLARS")]
    [InlineData("-123.45", "NEGATIVE ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS")]
    [InlineData("-0.01", "NEGATIVE ONE CENT")]
    [InlineData(" 123.45", "ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS")]
    [InlineData("123.45 ", "ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS")]
    [InlineData("  123.45  ", "ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS")]
    public void Convert_ReturnsExpectedWords(string input, string expected)
    {
        Assert.Equal(expected, NumberToWordsConverter.Convert(input).Words);
    }

    [Theory]
    [InlineData("123.45")]
    [InlineData("100")]
    [InlineData("0.99")]
    public void Convert_NoRoundingNoteWhenInputAlreadyHasAtMostTwoDecimals(string input)
    {
        Assert.Null(NumberToWordsConverter.Convert(input).Note);
    }

    [Theory]
    [InlineData("1.005", "Amount rounded to 1.01 before conversion.")]
    [InlineData("0.995", "Amount rounded to 1.00 before conversion.")]
    [InlineData("-1.005", "Amount rounded to -1.01 before conversion.")]
    public void Convert_AddsRoundingNoteWhenInputHasMoreThanTwoDecimals(string input, string expectedNote)
    {
        Assert.Equal(expectedNote, NumberToWordsConverter.Convert(input).Note);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("   ")]
    public void Convert_RejectsInvalidInput(string input)
    {
        Assert.Throws<ArgumentException>(() => NumberToWordsConverter.Convert(input));
    }

    [Theory]
    [InlineData("99999999999999999999999999999")] // exceeds decimal's range
    [InlineData("99999999999999999999")] // fits in decimal but exceeds long's range
    [InlineData("9223372036854775808")] // long.MaxValue + 1
    public void Convert_RejectsTooLargeInput(string input)
    {
        var ex = Assert.Throws<ArgumentException>(() => NumberToWordsConverter.Convert(input));
        Assert.Contains("too large", ex.Message);
        Assert.Contains(NumberToWordsConverter.MaxSupportedAmount, ex.Message);
    }

    [Fact]
    public void Convert_HandlesLargestSupportedValue()
    {
        var words = NumberToWordsConverter.Convert(long.MaxValue.ToString()).Words;

        Assert.StartsWith("NINE QUINTILLION", words);
        Assert.EndsWith("SEVEN DOLLARS", words);
    }
}
