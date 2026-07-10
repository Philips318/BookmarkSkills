---
name: TICS CSharp Standards
description: 'Use when creating or modifying C# files that will be validated by TICS or TIOBE. Apply the team rules for copyright headers, XML doc comments, abstract base constructors, one top-level type per file, namespace consistency, exception logging, and mirrored file updates.'
applyTo: "**/*.cs"
---

# TICS CSharp Standards

## Scope

- Prefer the narrowest behavior-preserving fix that resolves the reported rule.
- Default must-fix target for auto-generated or edited C# code: Philips C# Coding Standard 5.33 checked levels 1 through 5.
- Treat checked levels 6 and 7 as best-effort: fix when the issue is local and cheap. Use the `tics-standard` skill for full level 1-7 pre-delivery checks.
- Treat checked level 8 as best-effort when the issue is local and cheap to fix.
- Treat checked levels 9 and 10 as low priority unless they are explicitly reported for the changed code.
- If TICS or CI already reports a lower-priority rule on the touched code, fix it anyway.

## File Header (4@101)

- Every new `.cs` file must begin with the Philips copyright header block.
- Use the current year in the copyright line. Template:

```csharp
/*Copyright <YEAR> Philips Healthcare
* All rights are reserved. Reproduction or dissemination
* in whole or in part is prohibited without the prior written
* consent of the copyright holder.
*/
```

- When editing an existing file that already has a copyright header, do not modify the year or the header.
- When editing an existing file that is missing the header, add it at the top of the file.

## XML Documentation Comments

- Every `public` or `internal` class, struct, interface, or enum must have a `<summary>` XML doc comment.
- Every `public` or `internal` method, property, or event on a `public` or `internal` type must have a `<summary>` XML doc comment.
- When adding a new `public`/`internal` type or member, include the XML doc comment immediately.
- When editing an existing `public`/`internal` type or member that lacks XML doc comments, add them.
- Do not add XML doc comments to `private` or `protected` members unless explicitly requested.

## Generation Constraints

- Generate null-safe code. Do not dereference a value unless the current control flow proves it is non-null.
- Do not suppress compiler warnings in source code.
- Do not swallow exceptions silently. Every `catch` must log enough context to diagnose the failure or rethrow with added context.
- Avoid throwing exceptions from unexpected locations such as property accessors, event callbacks, finalizers, or similar utility members unless the API contract explicitly requires it.
- Keep `switch` statements exhaustive.
- Do not change a `for` loop variable inside the loop body.
- Do not lock on publicly reachable objects, `this`, `Type` instances, or strings.
- Keep fields `private` by default.
- Make new types `internal` by default unless broader visibility is required by an existing API.
- Use one top-level type per file and name the file after the main type.
- Do not hide inherited members with `new`. Override, rename, or refactor instead.
- Do not re-declare a visible name in a nested scope.
- Every abstract base class must declare an explicit `protected` constructor.
- Do not leave an overridable method as the direct implementation surface of a private or internal interface member. Prefer a non-virtual wrapper plus a protected core hook, explicit interface implementation, or a sealed declaring type.
- If you override `Equals`, also override `GetHashCode`.
- If you implement `==`, keep `Equals` consistent with it.
- If you implement any relational operator, implement the full relational set.
- If you implement `IComparable`, keep the comparison operators consistent with it.
- Do not modify operands inside overloaded operator implementations.
- Do not overload modifying operators on class types.
- Implement `IDisposable` when a type owns disposables, unmanaged resources, or event subscriptions.
- Avoid finalizers. If a finalizer exists, do not access reference-type members in it.
- Do not use a `using`-scoped variable outside its scope.
- Pair every event subscription with a corresponding unsubscribe.
- Null-check an event delegate before raising it. Do not use callback return values in events.
- Do not compare floating-point values with `==`, `!=`, or `Equals`. Use a tolerance-based comparison.
- Prefer named constants over magic numbers, except obvious literals that are already accepted by the local code style.
- Return stable interfaces for read-only collections.
- Prefer pattern matching over `as` when it improves safety and clarity.
- Add generic constraints when the code already relies on those capabilities.
- Follow the existing project namespace pattern. Reuse the nearest established namespace branch instead of inventing a new one.

## Repository Rules

- When the same behavior exists in mirrored folders, update both copies in the same change.

## Validation

- After editing, run focused validation on the touched files.
- Call out anything that still needs a TICS or CI rerun.