using System;

namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskFactory
    {
        ITask Create(TaskData taskData);

        TaskData GetTaskData(ITask task);
    }

    public class TaskData
    {
        public string TaskType { get; set; }

        public PropertyValue[] Properties { get; set; }
    }

    public class PropertyValue
    {
        public string Name;

        public object Value;

        public Type ValueType;

        protected bool Equals(PropertyValue other)
        {
            return string.Equals(Name, other.Name) && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PropertyValue) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode()*397) ^ Value.GetHashCode();
            }
        }
    }
}