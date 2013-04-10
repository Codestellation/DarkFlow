using System;

namespace Codestellation.DarkFlow
{
    /// <summary>
    /// Forces executor to persist task. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PersistentAttribute : Attribute
    {
         
    }
}