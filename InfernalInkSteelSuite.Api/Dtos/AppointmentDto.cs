using InfernalInkSteelSuite.Api.Models;

namespace InfernalInkSteelSuite.Api.Dtos;

public record AppointmentDto(
    int Id,
    int ClientId,
    int ArtistId,
    DateTime StartTime,
    DateTime EndTime,
    string ServiceType,
    string ServiceCategory,
    string Status,
    decimal? QuotedPrice,
    decimal? FinalPrice,
    string? Notes,
    ClientDto? Client,
    string? ArtistName
);
