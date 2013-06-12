using System;

namespace Codestellation.DarkFlow.Misc
{
    public interface IClock
    {
        DateTime UtcNow { get; }

        DateTime Now { get; }
    }
}