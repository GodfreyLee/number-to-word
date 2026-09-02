# Approach

## What this is

A web page where the user types a number (e.g. `123.45`) and the app
converts it to words (e.g. `ONE HUNDRED AND TWENTY-THREE DOLLARS AND
FORTY-FIVE CENTS`).

## How the conversion works

The converter is in `NumberToWordsConverter.cs` as a single static function
(`string Convert(string input)`). It uses lookup arrays for the words 0–19,
the tens (20, 30, ... 90), and scale words (thousand, million, billion).

**Core Logic (Adopted):**
- The number is split into groups of three digits. Each group is converted
  separately and combined with its scale word (THOUSAND, MILLION, etc.).
  This keeps the lookup tables small — you only need words for 0–99 plus
  the scale words.
- "AND" goes between the hundreds and the remaining digits within a group
  (e.g. `ONE HUNDRED AND TWENTY-THREE`), matching the expected output.
- Cents are calculated using `decimal` arithmetic (not `double`) to avoid
  rounding errors with money values.
- The amount is rounded to 2 decimal places first (half-away-from-zero,
  e.g. `1.005` → `1.01`), *then* split into dollars and cents. Rounding
  before splitting means a value like `0.995` correctly carries into the
  dollar (`ONE DOLLAR`) instead of producing an invalid `ZERO DOLLARS AND
  ONE HUNDRED CENTS`.
- Whichever half is zero is left out of the sentence: `100` reads as
  `ONE HUNDRED DOLLARS`, not `...AND ZERO CENTS`, and `0.99` reads as
  `NINETY-NINE CENTS`, not `ZERO DOLLARS AND...`. The one exception is the
  amount being zero altogether (`0` → `ZERO DOLLARS AND ZERO CENTS`) — there
  has to be something left to say.
- Singular and plural are handled separately for dollars and cents, since
  `$1.00` and `$0.01` each need a singular noun but in different halves.
- All output is uppercase.

## Banned Logic

**A library or NuGet package** — packages like `Humanizer` convert numbers
to words in one line: `1234.ToWords()`.
- I would do it in a production app, but this not allowed to do so.

**One lookup table** — just list every number would ever need:
`1 → "ONE"`, `2 → "TWO"`, up to `999 → "NINE HUNDRED AND NINETY-NINE"`.
- Works for small ranges but doesn't scale — covering millions and billions
  would need millions of entries.
- Waste of memories

**String manipulation** — work with the number as a string of digits
and use pattern matching to pick the right words.
- Tends to produce tangled code with lots of `if` branches for edge cases
  (e.g. `14` is "FOURTEEN", not "TEN-FOUR").
- The lookup-table approach handles teens more cleanly by listing them
  separately.


## Architecture: one API + one HTML page

The app has two parts:

- **ASP.NET Core Minimal API** (`Program.cs`) — one route,
  `GET /api/convert?amount=...`, that returns JSON.
- **One static `wwwroot/index.html`** — a text input, a button, and ~20
  lines of JavaScript that calls `fetch()`.

**Why not MVC or Razor Pages?** MVC is designed for apps with many pages
and server-rendered views. This app has one endpoint and one static form,
so MVC's extra conventions would just add complexity for no benefit.

**Why not frontendframework like React/Angular/Vue?** A single form with one field and one button
doesn't need a JavaScript framework. Plain HTML with `fetch()` does the
same thing without a build step or extra dependencies.

**Why not a separate class library?** A separate library makes sense when
multiple projects share the same code. Here only the web project uses the
converter, and the test project can reference the web project directly.
Adding a third project would be extra structure with no real benefit.



## Input validation

The amount comes in as a string from the URL, parsed with
`decimal.Parse(..., CultureInfo.InvariantCulture)`. If the input isn't a
valid number, the app returns an HTTP 400 error with an explanation.

Negative amounts are *not* rejected — a currency amount can legitimately be
negative (a refund, a debit), so `-123.45` converts using its absolute value
with a `NEGATIVE ` prefix: `NEGATIVE ONE HUNDRED AND TWENTY-THREE DOLLARS
AND FORTY-FIVE CENTS`.

## What was left out

No database, login system, logging, security or configuration etc.