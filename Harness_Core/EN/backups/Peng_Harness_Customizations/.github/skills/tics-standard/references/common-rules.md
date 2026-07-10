# Common TICS Rules

This reference captures the TICS patterns that have already appeared in this repository and should be checked first when editing C# code.

## Philips C# Priority Policy

- Source of truth: Philips C# Coding Standard 5.33 in CSViewer, filtered on `Status = CHECKED`.
- Auto-applied instruction must-fix range: checked levels 1 through 5.
- Skill (pre-delivery) must-fix range: checked levels 1 through 7.
- Best-effort range: checked level 8 when the issue is local to the touched slice and the fix is cheap.
- Low-priority range: checked levels 9 and 10 unless they are explicitly reported by TICS or CI for the changed code.
- Reference summary: [philips-csharp-checked-level1-7.md](./philips-csharp-checked-level1-7.md)

## 20@480 Seal Methods That Satisfy Private Interfaces

- Do not leave an overridable method as the direct implementation surface of a private or internal interface contract.
- Prefer one of these fixes:
  - Replace the overridable entry point with a non-virtual wrapper and move extensibility into a protected core hook.
  - Implement the interface member explicitly.
  - Seal the declaring type if extensibility is not needed.
- Avoid patterns such as an `internal virtual` method that directly satisfies a private interface member.

## 7@105 Define a Protected Constructor on Abstract Base Classes

- Every abstract base class should declare an explicit `protected` constructor.
- Keep the constructor empty if no initialization is required.

## 7@107 Keep One Top-Level Type Per File

- Prefer one top-level type per source file.
- If a helper type is tightly coupled to one owner and not reused elsewhere, prefer a nested type.
- Otherwise move the helper to its own file.

## 3@109 Use a Consistent Namespace Pattern

- Reuse the owning project's existing root namespace.
- Follow the nearest sibling namespace rather than inventing a new namespace branch.
- Avoid broad namespace renames unless the task explicitly asks for them.

## Exception Handling

- Do not swallow exceptions silently.
- In a `catch`, either log enough context to diagnose the failure or add context and rethrow.
- If a catch is intentionally non-fatal, keep the log message specific to the operation that failed.

## Repository Mirror Rule

- When a behavior exists in mirrored code paths, update both copies in the same change.
- In this repository, common mirrored pairs include:
  - `MIA_Dev` and `MIA_Splot`
  - `MIPPP` and `MIPPP_Dev` for mirrored config or resource changes
- Validate all mirrored files touched by the change.

## Validation Pattern

- After the first substantive edit, run the narrowest available validation on the touched files.
- Prefer file-scoped errors, then a narrow build or test, then a structural grep if the rule is purely structural.
- A TICS rerun is still required before calling the change fully compliant.

## 4@101 File Header (Copyright Block)

- Every `.cs` file must begin with the Philips copyright header.
- Use the current year when creating a new file. Do not modify the year in existing headers.
- Template:

```csharp
/*Copyright <YEAR> Philips Healthcare
* All rights are reserved. Reproduction or dissemination
* in whole or in part is prohibited without the prior written
* consent of the copyright holder.
*/
```

- When editing a file that is missing the header, add it at the very top before any `using` or `namespace`.

## XML Documentation Comments

- Every `public` or `internal` class, struct, interface, or enum must have a `<summary>` XML doc comment.
- Every `public` or `internal` method, property, or event on a `public`/`internal` type must have a `<summary>` XML doc comment.
- When creating a new type or member, include the comment immediately.
- When editing an existing type or member that lacks comments, add them.
- Do not add XML doc comments to `private` or `protected` members unless explicitly requested.
