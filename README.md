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

### Happy Flow
<img width="551" height="178" alt="Screenshot 2026-09-02 at 7 07 41 pm" src="https://github.com/user-attachments/assets/b0bae867-ce6a-422a-975e-b1d08193776e" />

<img width="569" height="258" alt="Screenshot 2026-09-02 at 7 07 57 pm" src="https://github.com/user-attachments/assets/db89883a-2ce0-4405-a0ad-d3122856fe25" />

### Empty submit
<img width="565" height="201" alt="Screenshot 2026-09-02 at 7 11 37 pm" src="https://github.com/user-attachments/assets/65af64e9-d8d1-460f-8f89-d5d30e0f7f88" />

### Non-numeric
<img width="553" height="236" alt="Screenshot 2026-09-02 at 7 11 55 pm" src="https://github.com/user-attachments/assets/fb758367-1aa6-4955-b9f4-85b38545413d" />

### Negative
<img width="566" height="252" alt="Screenshot 2026-09-02 at 7 12 11 pm" src="https://github.com/user-attachments/assets/d9fd7476-f3f1-4e2b-be79-654c9e9ee10b" />

### Decimal only
<img width="543" height="227" alt="Screenshot 2026-09-02 at 7 12 23 pm" src="https://github.com/user-attachments/assets/a856625f-9145-42af-b107-0d43f0506f80" />

### Limit hint
<img width="545" height="242" alt="Screenshot 2026-09-02 at 7 12 52 pm" src="https://github.com/user-attachments/assets/a7153d35-179f-4453-a870-8d664d44820b" />

### Rounding 
<img width="545" height="266" alt="Screenshot 2026-09-02 at 7 08 15 pm" src="https://github.com/user-attachments/assets/4716eae0-b512-4775-92a4-aa1a63a3dc1a" />

### Test
<img width="454" height="244" alt="Screenshot 2026-09-02 at 7 08 49 pm" src="https://github.com/user-attachments/assets/44b167a9-d5a1-4335-b59f-05fa83da0fcb" />




