using System;

namespace Codestellation.DarkFlow.Misc
{
    public interface IClock
    {
        DateTimeOffset Now { get; }
    }
}