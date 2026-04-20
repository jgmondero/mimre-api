namespace Mimre.Domain.Exceptions;

public class StorageQuotaExceededException()
    : DomainException("Storage quota exceeded for this plan.");
