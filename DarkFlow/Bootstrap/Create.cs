namespace Codestellation.DarkFlow.Bootstrap
{
    public static class Create
    {
        public static QueuedExecutorBuilder QueuedExecutor()
        {
            return new QueuedExecutorBuilder();
        }

        public static LimitedConcurrencyBuilder LimitedConcurrencyExecutor()
        {
            return new LimitedConcurrencyBuilder();
        }

        public static int Threads(this int self)
        {
            return self;
        }
    }
}