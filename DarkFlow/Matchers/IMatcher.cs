namespace Codestellation.DarkFlow.Matchers
{
    public interface IMatcher
    {
        MatchResult TryMatch(ITask task);
    }
}