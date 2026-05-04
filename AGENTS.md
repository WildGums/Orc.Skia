# Orc.Skia

Orc.Skia is a library that provides a DPI-aware Skia canvas for XAML platforms (WPF). It wraps SkiaSharp to expose a rendering surface that integrates cleanly with WPF's visual tree and composition pipeline.

The library includes:

- `SkiaCanvas` — A WPF `Canvas`-derived control that renders via a `WriteableBitmap` backed by SkiaSharp.
- `LottieCanvas` — A `SkiaCanvas` specialization that plays Lottie animations using SkiaSharp.Skottie.
- `SkiaElement` — A lightweight UIElement-based rendering component.

---

## Critical Rules (Read First)

These rules are **non-negotiable**. Violating them causes broken builds, crashes, or downstream breakage.

### 1. Never Edit Generated Files

Files matching `*.generated.cs` are auto-generated.

- **NEVER** manually edit these files

### 2. ABI / API Stability

This project maintains a stable public API. Breaking changes break downstream consumers.

| Allowed | Never |
|---------|-------|
| Add new overloads | Modify existing signatures |
| Add new methods | Remove public APIs |
| Add new classes | Change return types |

### 3. Tests Are Mandatory

**Building alone is NOT sufficient.** Run tests before claiming completion (see [Commands](#commands)).

### 4. Branch Protection (COMPLIANCE REQUIRED)

**Direct commits to protected branches are a policy violation.**

| Repository | Protected Branches |
|------------|-------------------|
| Orc.Skia | `master` |
| Orc.Skia | `develop` |

**Required workflow:**

1. **Create a feature branch FIRST** — Use naming convention: `feature/issue-NNNN-description`
2. **Make all commits on the feature branch** — Never commit directly to protected branches
3. **Submit a Pull Request** — Changes must be reviewed by a human before merging

```bash
# CORRECT — Always create a feature branch first
git checkout -b feature/issue-1234-fix-description

# NEVER DO THIS — Policy violation
git checkout develop && git commit  # FORBIDDEN

# NEVER DO THIS — Policy violation
git checkout master && git commit   # FORBIDDEN
```

---

## Commands

Single source of truth for all commands:

| Task | Command |
|------|---------|
| **Build** | `dotnet cake --target=build` |
| **Test** | `dotnet cake --target=test` |
| **Build and test** | `dotnet cake --target=buildandtest` |

---

## Architecture & Directories

### Solution Layout

```
src/
  Orc.Skia/            # Main library — WPF Skia canvas controls
  Orc.Skia.Tests/      # NUnit test project
  Orc.Skia.Example/    # Example application
deployment/            # Build / deployment scripts (Cake)
design/                # Design-time assets
```

### Key Source Files

| File | Purpose |
|------|---------|
| `src/Orc.Skia/SkiaCanvas.cs` | Core WPF Canvas rendering via SkiaSharp |
| `src/Orc.Skia/LottieCanvas.cs` | Lottie animation playback canvas |
| `src/Orc.Skia/SkiaElement.cs` | Lightweight SkiaSharp UIElement |
| `src/Orc.Skia/Interfaces/` | Public interfaces (e.g. `ISkiaElement`) |
| `src/Orc.Skia/EventArgs/` | Custom event argument types |
| `src/Orc.Skia/Extensions/` | Extension methods |

### Directory Guide

| Directory | Editable? | Notes |
|-----------|-----------|-------|
| `*.generated.cs` | No | Leave as-is |
| `deployment/` | No | Deployment / build scripts |
| `src/Orc.Skia/` | Yes | Main library source |
| `src/Orc.Skia.Tests/` | Yes | Tests |

---

## Writing Code

### Target Frameworks

The library targets Windows-only frameworks:

- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

All code must remain WPF-compatible. Do not introduce cross-platform APIs that break on Windows-only targets.

### Dependencies

Key runtime dependencies:

| Package | Purpose |
|---------|---------|
| `Catel.MVVM` | MVVM infrastructure, IoC, logging |
| `SkiaSharp.Views.WPF` | Skia rendering for WPF |
| `SkiaSharp.Skottie` | Lottie animation support |

Build-time / code-gen only (private assets):

| Package | Purpose |
|---------|---------|
| `Catel.Fody` | Compile-time IL weaving |
| `Catel.SourceGenerators` | Source generators |
| `Fody` | IL weaving framework |
| `MethodTimer.Fody` | Method timing weaving |
| `Obsolete.Fody` | Obsolete API IL weaving |

### Anti-Patterns (Never Do This)

| Anti-Pattern | Why |
|-------------|-----|
| Modifying method signatures | ABI breaking |
| Manual edits to `*.generated.cs` | Overwritten on regenerate |
| Using default parameters in public APIs | ABI breaking |
| **Skipping failing tests** | **Unacceptable — tests must pass** |
| Adding cross-platform code | Library is Windows/WPF only |

---

## Testing & Debugging

### Running Tests

```bash
dotnet cake --target=test
```

### Tests MUST Pass

> **NON-NEGOTIABLE:** Tests must PASS before claiming completion.
>
> - Do NOT skip failing tests
> - Do NOT claim completion if tests fail
> - Do NOT use `SkipException` to work around failures

### Writing Tests

1. Use NUnit to write tests
2. Place tests in `src/Orc.Skia.Tests/`
3. Use PascalCase words separated by underscores for test method names (e.g. `Feature_Does_Work`)

```csharp
[Test]
public void Feature_Does_Work()
{
    var result = 47 - 5;

    Assert.That(result, Is.EqualTo(42));
}
```

### Public API Approval

The test project contains a `PublicApiFacts` class that verifies the public API has not changed unexpectedly. If you intentionally add or change a public API:

1. Run the tests once — the test will fail and generate a new `.received.txt` file.
2. Review the diff between `*.verified.txt` and `*.received.txt`.
3. If the change is intentional, overwrite the `*.verified.txt` file with the new content.
4. Commit the updated `*.verified.txt` file alongside your code changes.

### Debugging Methodology

1. **Establish baseline** — What's the known-good state?
2. **One change at a time** — Verify each change before proceeding
3. **Track changes in a table** — Log what you changed and the result
4. **Platform differences are signals** — If X works and Y fails, the difference IS the answer
5. **Revert if worse** — Don't pile fixes on top of failures

---

## Further Reading

| Topic | Link |
|-------|------|
| Repository | https://github.com/WildGums/Orc.Skia |
| Documentation portal | http://opensource.wildgums.com |
| SkiaSharp | https://github.com/mono/SkiaSharp |
| Catel framework | https://github.com/Catel/Catel |
| Contributing guide | [CONTRIBUTING.md](CONTRIBUTING.md) |
