namespace AccountOperationUtilities.Interfaces;
public interface IRecord
{
    string Name { get; }
    string OwnerId { get; }
    string Path { get; }
}
