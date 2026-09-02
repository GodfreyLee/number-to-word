# number-to-word

Web page that converts a numeric dollar amount into words, e.g. `123.45` →
`ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS`.

- [docs/APPROACH.md](docs/APPROACH.md) — design rationale, alternatives considered
- [docs/TESTPLAN.md](docs/TESTPLAN.md) — test plan (automated + manual)

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) (built and tested with .NET 10)

## Build

```
dotnet build
```

## Run

```
dotnet run --project src/NumberToWords.Web
```

Then open the URL printed in the console (typically `http://localhost:5000`)
in a browser, type an amount into the form, and click Convert.

Largest amount supported: `9,223,372,036,854,775,807.99` (`long.MaxValue`
dollars). Anything larger returns a `too large to convert` error — see
[docs/APPROACH.md](docs/APPROACH.md) for why.


## Test

```
dotnet test
```

Runs the xUnit suite in `tests/NumberToWords.Tests`, covering the conversion
algorithm (see [docs/TESTPLAN.md](docs/TESTPLAN.md) for the full case list).

## Project layout

```
src/NumberToWords.Web/          web app: minimal API + static HTML UI
  Program.cs                    HTTP route (/api/convert)
  NumberToWordsConverter.cs     the conversion algorithm
  wwwroot/index.html            the test UI
tests/NumberToWords.Tests/      xUnit tests for the converter
docs/                           approach + test plan documents
```

## Screenshots


