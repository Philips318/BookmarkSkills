# BDD Specifications

This directory contains Gherkin `.feature` files organized by plan slug.

```
.harness/specs/
├── {plan-slug}/
│   ├── task-1-short-name.feature
│   ├── task-2-short-name.feature
│   └── task-3-short-name.feature
└── README.md
```

## Purpose

Feature files serve as **both specification and executable acceptance tests**:
- The `@planner` agent writes them as behavioral specifications
- The `@executor` agent implements step definitions to make them pass
- The `@evaluator` agent runs them as the primary verification gate

## Rules

- Feature files are created by `@planner` and MUST NOT be modified by `@executor`
- Each feature file maps to exactly one task in the plan JSON
- Scenarios must be independent (no shared mutable state)
- Steps should use domain language, not implementation details

## BDD Framework

This project uses **Reqnroll** (the SpecFlow successor) for .NET BDD execution.
