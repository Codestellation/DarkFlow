namespace Codestellation.DarkFlow.Execution
{
    public interface ISerializer
    {
        string Serialize(ITask task);

        ITask Deserialize(string serializedData);
    }
}