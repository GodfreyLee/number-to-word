# Test Plan

## Unit tests

`tests/NumberToWords.Tests/ConverterTests.cs`, xUnit, one `[Theory]` driving
`NumberToWordsConverter.Convert(string)`. Run with `dotnet test`.

| # | Input | Expected output | Why this case |
|---|-------|------------------|----------------|
| 1 | `123.45` | `ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS` | Sample Answer |
| 2 | `0` | `ZERO DOLLARS AND ZERO CENTS` | Zero on both sides |
| 3 | `0.00` | `ZERO DOLLARS AND ZERO CENTS` | Trailing-zero decimal parses the same as `0` |
| 4 | `1` | `ONE DOLLAR` | Singular DOLLAR, cents half omitted since it's zero |
| 5 | `0.01` | `ONE CENT` | Singular CENT, dollars half omitted since it's zero |
| 6 | `1.01` | `ONE DOLLAR AND ONE CENT` | Singular on both sides at once |
| 7 | `100` | `ONE HUNDRED DOLLARS` | Whole dollar amount, no cents to mention |
| 8 | `0.99` | `NINETY-NINE CENTS` | Cents-only, hyphenated compound |
| 9 | `1000` | `ONE THOUSAND DOLLARS` | THOUSAND scale-word boundary |
| 10 | `1000000` | `ONE MILLION DOLLARS` | Scale-word boundary (thousand → million) |
| 11 | `1000000000` | `ONE BILLION DOLLARS` | Next scale-word boundary |
| 12 | `20.05` | `TWENTY DOLLARS AND FIVE CENTS` | Tens with zero ones digit |
| 13 | `.45` | `FORTY-FIVE CENTS` | No leading digit before the decimal point |
| 14 | `1.000` | `ONE DOLLAR` | Extra trailing zeros on decimal side parse cleanly to zero cents |
| 15 | `1.005` | rounds to `1.01` (documented rounding rule) | Sub-cent precision must round, not truncate or throw |
| 16 | `0.001` | `ZERO DOLLARS AND ZERO CENTS` | Whole amount is zero — both halves shown, nothing else to say |
| 17 | `0.995` | `ONE DOLLAR` | Rounds up across the dollar boundary — must not produce ZERO DOLLARS AND ONE HUNDRED CENTS |
| 18 | `-5` | `NEGATIVE FIVE DOLLARS` | Negative amounts convert (refund/debit), not rejected |
| 19 | `-123.45` | `NEGATIVE ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS` | Negative with cents |
| 20 | `abc` | throws (rejected as invalid) | Non-numeric input must not crash the server |
| 21 | `` (empty) | throws (rejected as invalid) | Empty input handled same as invalid |

## Manual UI test steps

Run the app (`dotnet build`, `dotnet run --project src/NumberToWords.Web`), open
`http://localhost:5152`, and for each row below type the input into the
form, click Convert, and check the displayed result:

1. Happy flow: type `123.45` → sample output.
2. Empty submit: click Convert with the field blank → see a clear on-page
   error, not a blank screen or unhandled exception.
3. Non-numeric: type `abc` → see the same kind of error message.
4. Negative: type `-5` → see `NEGATIVE FIVE DOLLARS`.
5. Large number: type `1234567.89` → check the words are grouped correctly
   across MILLION/THOUSAND.
6. Decimal-only: type `.45` → confirm it's treated as `0.45`.
7. Keyboard: confirm pressing Enter in the field submits the form (not just
   clicking the button).

## Out of scope

Load/performance testing, cross-browser matrix, and accessibility audit
beyond basic semantic HTML (label tied to input, button is a real
`<button>`) — none of these are called for by the exercise's scale (a single
internal test page).