After a thorough analysis of the codebase, I have identified numerous critical and currently untested code paths. The following summary details the most important areas that would benefit from robust test coverage, organized by project.

### Project: `InfernalInkSteelSuite.Domain`

*   **`Client.cs`**: The `FullName` property contains logic for constructing a client's full name, including handling an optional middle name. Tests should be written to verify that the name is formatted correctly in all cases (with, without, and with an empty middle name).
*   **`User.cs`**: The `IsAdmin()` method is a critical security-related function. Tests should confirm that it correctly identifies admin users (case-insensitively) and returns `false` for all other roles.

### Project: `InfernalInkSteelSuite.Repositories`

*   **`AppointmentRepository.cs`**: The `Add` and `Update` methods contain a business rule that prevents scheduling appointments in the past. This validation should be tested to ensure it throws the expected `ArgumentException`.
*   **`ClientRepository.cs`**: The `GetClientIdByName` method's case-insensitive name matching is a key piece of logic that should be thoroughly tested.
*   **`DocumentRepository.cs`**: The `GetDocuments` method contains role-based filtering and a `truncated` out parameter. This logic should be tested to ensure that it returns the correct documents for each role and correctly indicates when the results have been truncated.
*   **`UserRepository.cs`**: This repository is security-sensitive and requires extensive testing. The `HashPassword`, `AddUser`, `UpdateUser`, and `CheckPassword` methods all contain critical logic that should be validated to ensure data integrity and security.

### Project: `InfernalInkSteelSuite.Data`

*   **`DatabaseManager.cs`**: The `InitializeDatabase` method, which orchestrates the creation and migration of the database, is a prime candidate for integration testing. The `EnsureDefaultUserExists` and `MigrateUserDateFormats` methods are particularly important to test to ensure the database is always in a consistent state.

### Project: `Infernal-Ink-Steel-Suite.manager` (ViewModels)

*   **`AddEditClientViewModel.cs`**: The `Save` method's conditional logic for inserting or updating a client should be tested to ensure the correct repository method is called.
*   **`ClientViewModel.cs` and `DocumentsViewModel.cs`**: The filtering logic in these ViewModels should be tested to ensure it is case-insensitive and correctly handles empty search strings.
*   **`SettingsViewModel.cs`**: The constructor's role-based logic for displaying settings tabs is a critical feature that should be tested to ensure users only see the tabs they are authorized to access.
*   **`StatsViewModel.cs`**: The `LoadData` method's income calculation, including the fallback to an hourly rate, is a critical financial calculation that should be rigorously tested. The `ShouldCountAppointment` method's logic for excluding certain appointment statuses should also be tested.
