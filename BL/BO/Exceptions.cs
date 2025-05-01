namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, DO.DalDoesNotExistException innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, DO.DalAlreadyExistsException innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlFormatException : Exception
{
    public BlFormatException(string? message) : base(message) { }
    public BlFormatException(string message, DO.FormatException innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlNullReferenceException : Exception
{
    public BlNullReferenceException(string? message) : base(message) { }
    public BlNullReferenceException(string message, DO.NullReferenceException innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlXMLFileLoadCreateException : Exception
{
    public BlXMLFileLoadCreateException(string? message) : base(message) { }
    public BlXMLFileLoadCreateException(string message, DO.DalXMLFileLoadCreateException innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlNotImplementedException : Exception
{
    public BlNotImplementedException(string? message) : base(message) { }
}

[Serializable]
public class BlArgumentNullException : Exception
{
    public BlArgumentNullException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidTimeException : Exception
{
    public BlInvalidTimeException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidInputException : Exception
{
    public BlInvalidInputException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidOperationException : Exception
{
    public BlInvalidOperationException(string? message) : base(message) { }
}

[Serializable]
public class BlAuthorizationException : Exception
{
    public BlAuthorizationException(string? message) : base(message) { }
}
public class BlGeneralException : Exception
{
    public BlGeneralException(string message, Exception innerException)
        : base(message, innerException)
    {
    }


}


