namespace VerticalSliceArchitectureTemplate.Domain;

public class InvalidMoveException : Exception
{
    public InvalidMoveException(string message)
        : base(message) { }
}
