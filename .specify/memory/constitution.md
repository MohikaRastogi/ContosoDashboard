<!--
Sync Impact Report
- Version change: 0.0.0 -> 1.0.0
- Modified principles: N/A -> I. Security-by-Default; II. Offline-First Training Architecture; III. Evidence-Driven Delivery; IV. User Isolation & Authorization; V. Maintainability & Simplicity
- Added sections: Security & Data Handling; Development Workflow & Quality Gates
- Removed sections: none
- Deferred TODOs: none
-->

# ContosoDashboard Constitution

## Core Principles

### I. Security-by-Default
The project MUST treat security as a product requirement, not an afterthought. Authentication and authorization boundaries MUST be enforced before exposing any page, service method, or data query. Mock security features used for training MUST remain clearly labeled as non-production and MUST NOT be adopted as production identity or authorization mechanisms without a separate review and replacement plan.

This principle exists because the repository is explicitly a learning environment, but the behaviors it demonstrates must still reflect disciplined security patterns. A guardrail that is enforced at the UI, page, and service layers reduces accidental privilege escalation and teaches the correct default posture.

### II. Offline-First Training Architecture
The application MUST remain runnable without external cloud dependencies and MUST prioritize local, self-contained execution for training scenarios. Infrastructure boundaries MUST be designed so that local implementations can be swapped for cloud equivalents without changing business logic, and any file or persistence feature MUST document the local training assumptions and the migration path.

This principle preserves the training goals of the repository while keeping the architecture realistic and teachable. Local-first execution ensures availability in offline or restricted environments, while abstraction layers demonstrate the migration path used in production systems.

### III. Evidence-Driven Delivery
Every change to application behavior MUST be grounded in observable evidence: requirements, code review, tests, or direct verification. Feature work MUST be implemented in small, reviewable increments, and new behavior MUST be validated before it is considered complete. When the project lacks direct automated tests, the team MUST document the verification method explicitly and prefer the narrowest meaningful validation path.

This principle reduces guesswork and protects the learning purpose of the project. Evidence keeps changes inspectable, reproducible, and easier to reason about during training exercises.

### IV. User Isolation & Authorization
The application MUST enforce user-specific access rules so that one user cannot view or manipulate data outside their authorized scope. Project, task, and notification access MUST be validated against the current principal, role, and membership data. Any direct object reference, query, or route parameter that relies on caller trust MUST be rejected unless authorization checks succeed.

This principle exists because the training scenario intentionally demonstrates secure-by-construction access patterns. User isolation is a non-negotiable default for every dashboard feature and every data operation.

### V. Maintainability & Simplicity
The codebase MUST favor readable structure, clear naming, and explicit boundaries between data, domain behavior, and UI concerns. Features MUST not add unnecessary abstraction or complexity without a demonstrated need, and the project MUST preserve a clear separation between Models, Services, Data, and Pages so that future changes remain understandable to learners.

This principle keeps the project approachable and teachable. Simplicity increases maintainability, lowers onboarding friction, and makes the security and architecture lessons easier to follow.

## Security & Data Handling
The project is a training-focused application and MUST be treated as non-production software. It MUST NOT be deployed to production environments or used as a production security model without additional review, authentication hardening, and compliance controls. The mock authentication flow, sample users, and local-only persistence are intentionally simplified to support offline education.

The following constraints are mandatory:
- Authentication and authorization checks MUST exist at the page and service layers.
- User data MUST be isolated by identity, role, and project membership.
- Direct object references MUST be validated before access is granted.
- Local storage and data access patterns MUST remain self-contained and reproducible.
- Production-grade controls such as password hashing, MFA, enterprise identity providers, encryption, and audit logging MUST be treated as separate requirements for any real deployment.

## Development Workflow & Quality Gates
All work MUST follow a disciplined delivery flow that preserves clarity, testability, and secure defaults. Features MUST be scoped to the current learning objective, and changes that affect authorization, routing, or data access MUST be reviewed with the security implications explicitly considered.

The following workflow is required:
- Requirements or user stories MUST be understood before implementation begins.
- Changes MUST be kept small, reviewable, and aligned with the current task.
- Verification MUST happen before a feature is considered complete, including direct execution or automated checks where feasible.
- Security-sensitive changes MUST be reviewed for authorization, access boundaries, and user isolation issues.
- Documentation or comments MUST be updated when behavior, architecture, or training assumptions change.

## Governance
This constitution supersedes informal practices that conflict with it. Any project decision, feature, or architectural change that affects security, user isolation, or project constraints MUST be evaluated against these principles and either comply with them or document an exception with a clear rationale.

Amendments to this constitution require a written update to the governing document, a version bump that reflects the change, and review by the project maintainer or designated approver before the change is accepted. The versioning policy is semantic: MAJOR for backward-incompatible governance changes, MINOR for new principles or materially expanded guidance, and PATCH for clarifications or non-semantic wording improvements.

Compliance review is expected for all material changes. Reviewers MUST confirm that the change preserves the training purpose, does not weaken security posture, and remains aligned with the repository's local-first, educational scope.

**Version**: 1.0.0 | **Ratified**: 2026-09-15 | **Last Amended**: 2026-09-15
