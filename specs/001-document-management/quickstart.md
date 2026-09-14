# Quickstart: Document Upload and Management

## Prerequisites

- .NET 9 SDK installed
- Local development environment with access to the ContosoDashboard repository
- SQL Server or the project’s configured database target for local database creation

## Setup

1. Open a terminal in the repository root.
2. Restore dependencies:
   - `dotnet restore`
3. Build the application:
   - `dotnet build`
4. Run the app:
   - `dotnet run --project ContosoDashboard/ContosoDashboard.csproj`

## Validation Scenarios

### 1. Upload a valid document

- Sign in with a seeded user account.
- Open the project or personal document area.
- Select a valid PDF or Office document under 25 MB.
- Enter a title, category, and optional project/context metadata.
- Submit the upload.

Expected outcome:
- A success message appears.
- The document appears in the user’s document list.
- Metadata is stored with the file and project association.

### 2. Reject an invalid upload

- Attempt to upload a file larger than 25 MB or with an unsupported extension.

Expected outcome:
- The submission is rejected.
- The user sees a clear validation error.
- No file is persisted in storage or metadata records.

### 3. Search and access with authorization

- Search by title, description, tags, uploader, or project.
- Confirm that only documents within that user’s valid scope are returned.

Expected outcome:
- Authorized documents are visible.
- Unauthorized documents are excluded from search results and download access.

### 4. Share and notify

- Share a document with another valid user or project member.

Expected outcome:
- The recipient sees the document in the shared list after the share is created.
- An in-app notification is generated.

### 5. Delete with permission

- Delete a document after confirming the action.

Expected outcome:
- The file and metadata are removed for authorized users.
- The action is recorded in the application’s audit trail or activity logs.

## Expected Result

The feature is considered valid when the upload, search, permission, and sharing flows work end-to-end without bypassing the project authorization model.
