# Research: Document Upload and Management

## Decision

The feature will use a secure local file storage pattern outside the web root, with metadata stored in the relational database and all access decisions enforced through the existing role and project membership model.

## Rationale

- This matches the repository’s training-first, offline-only architecture.
- Storing files outside `wwwroot` prevents direct user browsing and reduces path traversal risk.
- The existing dashboard already authenticates users and manages project access, so document permissions can be enforced without introducing a separate security system.
- A storage abstraction keeps the domain logic stable while allowing a future Azure-backed implementation without rewriting page or service code.

## Alternatives Considered

1. Store files under `wwwroot`
   - Rejected because it exposes content to direct web access and weakens security posture.

2. Store document metadata only and upload file references without local persistence
   - Rejected because the feature requires safe, auditable, and reusable document storage within the application workflow.

3. Use a cloud-only document service immediately
   - Rejected because the repository explicitly requires offline-first behavior and local training compatibility.

## Research Findings

### Storage Pattern

- Use a dedicated application-managed directory such as `AppData/uploads`.
- Generate a GUID-based storage key before database inserts to avoid orphaned records and path collisions.
- Keep the actual stored path separate from any user-controlled filename.

### Access Model

- Document visibility must align with the current project and role model.
- Users can access documents only when they are the uploader, a project member, or a recipient of a share.
- Manager and admin rights remain validated through current identity claims and project membership records.

### Data Model Direction

- Use integer document IDs to stay consistent with the repository’s existing `User` and `Project` key patterns.
- Keep category as a human-readable text field to keep the feature simple and easy to configure within the current application.
- Keep sharing as a separate relationship entity so permissions remain auditable and inspectable.

### Validation Rules

- Require title and category on create.
- Accept only supported file types and enforce the 25 MB file-size cap.
- Return a clear user-facing status for validation failures and upload outcomes.
- Preserve access decisions for both project-associated and personal documents.
