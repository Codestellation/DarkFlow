using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ConcurrentQueue<TaskEnvelope> _queue;
        private readonly ISerializer _serializer;
        private readonly IDatabase _dataBase;
        private int _loaded;

        public TaskRepository(ISerializer serializer,  IDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }
            _loaded = -1;
            _queue = new ConcurrentQueue<TaskEnvelope>();
            _serializer = serializer;
            _dataBase = database;
        }

        public virtual void Add(ITask task)
        {
            var envelope = new TaskEnvelope(task);
            _queue.Enqueue(envelope);
        }

        public virtual void Add(IPersistentTask task, Region region)
        {
            var state = _serializer.Serialize(task);
            var id = _dataBase.Persist(region, state);
            var envelope = new TaskEnvelope(task,id);
            _queue.Enqueue(envelope);
        }

        public virtual ITask TakeNext()
        {
            var loaded = Interlocked.Increment(ref _loaded);
            
            if (loaded == 0)
            {
                foreach (var task in TasksInDatabase())
                {
                    _queue.Enqueue(task);
                }
            }

            TaskEnvelope result;
            _queue.TryDequeue(out result);

            if (result == null) return null;

            if (result.Persisted)
            {
                _dataBase.Remove(result.Id);
            }
            return result.Task;
        }

        private IEnumerable<TaskEnvelope> TasksInDatabase()
        {
            foreach (var serializedEnvelope  in _dataBase.GetAll())
            {
                var task = _serializer.Deserialize(serializedEnvelope.Value);
                yield return new TaskEnvelope(task, serializedEnvelope.Key);
                
            }
        }

        private class TaskEnvelope
        {
            public TaskEnvelope(ITask task)
            {
                Task = task;
                Persisted = false;
            }

            public TaskEnvelope(IPersistentTask task, Identifier id)
            {
                Task = task;
                Id = id;
                Persisted = true;
            }

            public bool Persisted { get; set; }

            public Identifier Id { get; set; }

            public ITask Task { get; set; }
        }
    }
}