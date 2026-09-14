# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-management/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Confirm the document feature fits the repository’s existing model, data, and authorization patterns before implementation.

- [X] T001 Review existing application patterns and document feature boundaries in `ContosoDashboard/Program.cs`, `ContosoDashboard/Data/ApplicationDbContext.cs`, and `ContosoDashboard/Services/ProjectService.cs`
- [X] T002 Create the feature implementation folders and confirm repository conventions for document models, services, and pages in `ContosoDashboard/Models/`, `ContosoDashboard/Services/`, and `ContosoDashboard/Pages/`
- [X] T003 [P] Extend the database context for document records and access metadata in `ContosoDashboard/Data/ApplicationDbContext.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the document storage, validation, and authorization foundation that all user stories depend on.

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel.

- [X] T004 Add the `Document` and `DocumentShare` entity models with the required constraints from `specs/001-document-management/data-model.md` in `ContosoDashboard/Models/Document.cs` and `ContosoDashboard/Models/DocumentShare.cs`
- [X] T005 Implement the local file storage abstraction and validation workflow required by the contract in `ContosoDashboard/Services/IDocumentStorageService.cs` and `ContosoDashboard/Services/LocalDocumentStorageService.cs`
- [X] T006 Build the document service layer for creation, access checks, search, and lifecycle operations in `ContosoDashboard/Services/DocumentService.cs`
- [X] T007 Register the document services and storage dependency injection in `ContosoDashboard/Program.cs`
- [X] T008 [P] Add authorization and safety checks around file paths and project membership in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Services/ProjectService.cs`

---

## Phase 3: User Story 1 - Upload and categorize work documents (Priority: P1) 🎯 MVP

**Goal**: Allow employees to upload valid work documents with required metadata and project context while rejecting invalid files.

**Independent Test**: A signed-in employee can upload a valid file with a title and category, see the document in the correct list, and receive a clear validation message when the file fails checks.

### Implementation for User Story 1

- [X] T009 [P] [US1] Create the document upload form and validation state in `ContosoDashboard/Pages/Documents.razor`
- [X] T010 [US1] Implement create-document validation for required title/category, supported types, and 25 MB size limit in `ContosoDashboard/Services/DocumentService.cs`
- [X] T011 [US1] Persist uploaded files and metadata after successful validation in `ContosoDashboard/Services/LocalDocumentStorageService.cs` and `ContosoDashboard/Data/ApplicationDbContext.cs`
- [X] T012 [US1] Add success and error messaging for upload outcomes in `ContosoDashboard/Pages/Documents.razor`
- [X] T013 [US1] Surface the user’s uploaded documents with project association in the project or personal document list in `ContosoDashboard/Pages/ProjectDetails.razor` and `ContosoDashboard/Pages/Index.razor`

**Checkpoint**: At this point, User Story 1 should be fully functional and independently testable.

---

## Phase 4: User Story 2 - Find and access project documents safely (Priority: P2)

**Goal**: Let users search, filter, and access only the documents permitted by project membership or sharing rules.

**Independent Test**: A user can search by title, metadata, uploader, or project and only sees documents they are allowed to access.

### Implementation for User Story 2

- [ ] T014 [P] [US2] Add document search and filter queries in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T015 [US2] Build the authorized document list and preview/download actions in `ContosoDashboard/Pages/Documents.razor`
- [ ] T016 [US2] Enforce access checks for project documents and shared files in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Services/ProjectService.cs`
- [ ] T017 [US2] Add permission-aware results and empty-state handling in `ContosoDashboard/Pages/ProjectDetails.razor`

**Checkpoint**: At this point, User Stories 1 and 2 should both function independently.

---

## Phase 5: User Story 3 - Manage document lifecycle and sharing (Priority: P3)

**Goal**: Support metadata updates, replacement, sharing, and deletion while preserving auditability.

**Independent Test**: A user with valid ownership or manager rights can update metadata, share a document, and delete or replace it without bypassing authorization.

### Implementation for User Story 3

- [ ] T018 [P] [US3] Add share, revocation, and notification workflow support in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Services/NotificationService.cs`
- [ ] T019 [US3] Implement metadata update and file replacement flows in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T020 [US3] Add delete confirmation, audit logging, and removal of related access state in `ContosoDashboard/Pages/Documents.razor` and `ContosoDashboard/Data/ApplicationDbContext.cs`
- [ ] T021 [US3] Expose document actions in the UI for owner and manager roles in `ContosoDashboard/Pages/ProjectDetails.razor` and `ContosoDashboard/Pages/Index.razor`

**Checkpoint**: All user stories should now be independently functional.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize validation, security, and documentation after the feature stories are complete.

- [ ] T022 [P] Validate the upload, search, access control, sharing, and delete flows against `specs/001-document-management/quickstart.md`
- [ ] T023 [P] Review and harden document storage and authorization security in `ContosoDashboard/Program.cs`, `ContosoDashboard/Services/DocumentService.cs`, and `ContosoDashboard/Services/LocalDocumentStorageService.cs`
- [ ] T024 Perform final cleanup and document feature summary updates in `README.md` and `StakeholderDocs/document-upload-and-management-feature.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion and blocks all user stories.
- **User Stories (Phase 3-5)**: All depend on Foundational completion.
- **Polish (Phase 6)**: Depends on all desired user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Phase 2 and has no dependency on other stories.
- **User Story 2 (P2)**: Can start after Phase 2 and relies on the same shared document foundation.
- **User Story 3 (P3)**: Can start after Phase 2 and may integrate with US1 and US2 but should remain independently testable.

### Parallel Opportunities

- Setup tasks can be worked in parallel when the team is ready.
- Foundational tasks T004-T008 can run in parallel across model, storage, and service work.
- Story 1, Story 2, and Story 3 can be developed in parallel after the foundation is complete.
- The validation and documentation polish tasks in Phase 6 are parallelizable once user stories are complete.

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational.
3. Complete Phase 3: User Story 1.
4. Validate upload, category, and rejection scenarios independently.
5. Stop and confirm the MVP is ready before adding story 2 and story 3 work.

### Incremental Delivery

1. Complete Setup + Foundational.
2. Implement User Story 1 and validate the upload flow.
3. Implement User Story 2 and validate search/access permission rules.
4. Implement User Story 3 and validate share, replace, and delete flows.
5. Finish with the cross-cutting polish tasks.

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together.
2. Developer A focuses on User Story 1 upload and categorization.
3. Developer B focuses on User Story 2 search and authorization.
4. Developer C focuses on User Story 3 sharing and lifecycle management.
5. Final polish and validation happen after all stories are integrated.

---

## Notes

- [P] tasks indicate different files or independent workstreams.
- Each user story remains independently completable and testable.
- The task set reflects the repository’s local-first, role-based, project-centered design.
- Verification should occur using `dotnet build` and the quickstart scenarios in `specs/001-document-management/quickstart.md` before closing the feature.
