# Approach

## What this is

A web page where a user types a numeric amount (e.g. `123.45`) and a
web-server routine converts it to words (e.g. `ONE HUNDRED AND TWENTY-THREE
DOLLARS AND FORTY-FIVE CENTS`).

## Architecture: one minimal API project + one static HTML page

The whole app is a single conversion function behind a single HTTP endpoint,
called from a single form. That maps to:

- **ASP.NET Core Minimal API** (`Program.cs`) — one route,
  `GET /api/convert?amount=...`, returning JSON.
- **One static `wwwroot/index.html`** — a text input, a button, and ~20
  lines of vanilla JavaScript calling `fetch()`.

**Rejected: MVC / Razor Pages.** MVC's controller/view/routing conventions
exist to manage many endpoints and server-rendered views. There is one
endpoint here and the "view" is a static form — the convention overhead buys
nothing.

**Rejected: a SPA framework (React/Angular/Vue).** A form with one field and
one button doesn't need component state management, virtual DOM, or a build
pipeline. Plain HTML + `fetch()` is the same UI with zero build step and
nothing else to install.

**Rejected: splitting the converter into its own class-library project.**
A class library earns its keep when something *else* also consumes it. Here
the only consumer is the web project and the only other reader is the test
project, which can reference the web project directly. A third project
would be structure with no second use case.

## The conversion algorithm

Implemented from scratch in `NumberToWordsConverter.cs` as a pure static
function (`string Convert(string input)`), using only lookup arrays for the
words 0-19 and the tens (20, 30, ... 90), plus scale words (thousand,
million, billion). No parsing library, no globalization/culture-info number
formatting, no NuGet package — per the exercise's constraint against using
existing libraries for the solution itself.

**Rejected: `CultureInfo`/`NumberFormatInfo`-based spelling, or a
`System.Globalization` trick.** .NET has no built-in "number to words"
API, but even partial tricks (e.g. formatting through a culture that spells
numbers) would be exactly the kind of "let a library do the solution" the
brief asks candidates to avoid, and wouldn't produce the "DOLLARS AND CENTS"
phrasing anyway.

**Design choices in the algorithm itself:**
- The integer part is split into groups of three digits (ones/tens/hundreds
  triplets), each converted independently and joined with its scale word
  (THOUSAND, MILLION, ...) — the standard way to keep the table sizes small
  (only need words for 0-99 and the hundred/scale words) instead of a table
  per magnitude.
- "AND" is inserted between a hundreds digit and the remaining tens/ones
  within a group (`ONE HUNDRED AND TWENTY-THREE`), matching the sample
  output exactly; it is not inserted between higher-order groups.
- Cents are computed as `round((amount - floor(amount)) * 100)` using
  `decimal` arithmetic (not `double`) to avoid binary floating-point
  rounding error on money values.
- Singular/plural (`DOLLAR` vs `DOLLARS`, `CENT` vs `CENTS`) is handled for
  the amount-equals-1 case on each side independently, since $1.00 and
  $0.01 both need singular nouns but for different halves of the string.
- All output is uppercase, matching the sample.

## Input handling / validation

The amount arrives as a string from the query parameter, parsed with
`decimal.Parse(..., CultureInfo.InvariantCulture)`. Invalid text or a
negative amount raises an `ArgumentException`, which the endpoint turns into
an HTTP 400 with an error message — validated at the boundary (user input),
per the rule that input validation shouldn't be skipped even under a
"simplest approach" mandate.

## What was deliberately left out

No database, authentication, logging framework, or configuration system —
nothing in the brief calls for persisted state, multiple users, or
environment-specific config. Adding any of those would be solving problems
the exercise doesn't have.
