namespace ForaFin.Api.Exceptions;

public class ValidationException(IEnumerable<string> validationErrors) : Exception;