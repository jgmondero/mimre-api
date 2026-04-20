using Mimre.Domain.Enums;

namespace Mimre.Application.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    PlanTier PlanTier,
    long StorageUsedBytes);
