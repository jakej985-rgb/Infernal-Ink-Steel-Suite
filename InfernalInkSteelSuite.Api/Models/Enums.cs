namespace InfernalInkSteelSuite.Api.Models;

public enum UserRole
{
    Admin,
    Manager,
    Artist,
    Piercer
}

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled,
    Blocked
}
