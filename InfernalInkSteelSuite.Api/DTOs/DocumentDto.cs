namespace InfernalInkSteelSuite.Api.DTOs;

public record DocumentDto(
    int Id,
    int ClientId,
    int UploadedByUserId,
    string Title,
    string FilePath,
    DateTime CreatedAt
);
