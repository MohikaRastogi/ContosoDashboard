# Data Model: Document Upload and Management

## Overview

This feature introduces document metadata and access records on top of the application’s existing user, project, and notification structures. The data model remains intentionally simple and aligned with the repository’s training-first design.

## Entities

### Document

| Field | Type | Constraints | Notes |
|---|---|---|---|
| DocumentId | int | PK, required | Matches existing integer-based entity patterns |
| Title | string | Required, max 255 | User-entered document title |
| Description | string | Optional, max 2000 | Searchable metadata |
| Category | string | Required | Text-based value such as Project Documents or Personal Files |
| UploaderId | int | Required, FK to User | Identifies the owner of the document |
| ProjectId | int? | Optional, FK to Project | Associated project context |
| StoredFileName | string | Required, max 255 | Safe generated filename without user-controlled path data |
| StoredFilePath | string | Required, max 1024 | Application-managed relative path |
| FileSizeBytes | int | Required | Size of uploaded file |
| ContentType | string | Required, max 255 | MIME type such as application/pdf |
| UploadedUtc | DateTime | Required | Audit timestamp |
| IsDeleted | bool | Default false | Supports soft delete or cleanup workflow if needed |

**Relationships**
- One `User` can upload many `Document` records.
- One `Project` can contain many `Document` records.
- One `Document` can be shared to many `DocumentShare` relationships.

### DocumentShare

| Field | Type | Constraints | Notes |
|---|---|---|---|
| ShareId | int | PK, required | Unique share record |
| DocumentId | int | Required, FK to Document | Target document |
| SharedByUserId | int | Required, FK to User | Document owner or manager |
| SharedWithUserId | int | Required, FK to User | Recipient |
| SharedUtc | DateTime | Required | When the share was created |
| NotificationSent | bool | Default false | Tracks whether access was notified |
| IsActive | bool | Default true | Supports revocation or expiration logic |

**Relationships**
- One `Document` can have many shares.
- One `User` can share many documents and receive many shares.

### Project

Existing entity used as access boundary. A document may be associated with a project to support project-based visibility and audit patterns.

### User

Existing entity used for uploader, owner, recipient, and access validation.

## Validation Rules

- A document must include a title and category before it can be saved.
- File type must match the supported allowlist.
- File size must be less than or equal to 25 MB.
- The application must generate a safe storage key and must not trust a user-defined filename.
- Access is only granted when the user is authorized for the relevant project or share relationship.

## Lifecycle Notes

- Upload creates metadata and the storage record.
- Search and listing filter by the user’s access scope.
- Sharing adds a `DocumentShare` record and triggers an in-app notification.
- Deletion removes the document record and its related access references when deletion is authorized.

## State Considerations

This feature does not require a complex state machine. The practical lifecycle is:

1. Draft/validation pending
2. Uploaded and visible to authorized users
3. Shared to additional users
4. Deleted or replaced with a newer version

The system should preserve auditability for upload, share, update, and delete operations without introducing an overly complex workflow model.
