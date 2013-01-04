using System;

namespace Codestellation.DarkFlow.Scheduling
{
    public interface ITimer : IDisposable
    {
        void CallbackAt(DateTimeOffset startAt);

        Action<DateTimeOffset> Callback { get; set; }
    }
}