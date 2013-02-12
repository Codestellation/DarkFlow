using System;

namespace Codestellation.DarkFlow.Misc
{
    [Serializable]
    public class ContractException : Exception
    {
        public ContractException(string message) : base(message)
        {
            
        }
    }
}