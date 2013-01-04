namespace Codestellation.DarkFlow.Execution
{
    public interface ISerializer
    {
        string Serialize(IPersistentTask task);

        IPersistentTask Deserialize(string serializedEnvelope);
    }
}