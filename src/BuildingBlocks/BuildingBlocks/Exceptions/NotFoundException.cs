namespace BuildingBlocks.Exceptions;
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key) : base($"Entity {key} - {name} was not found.")
    {
        
    }
}
