using System.Diagnostics;

namespace Codestellation.DarkFlow.Misc
{
    public static class Contract
    {
        [Conditional("DEBUG")]
        public static void Require(bool value, string message)
        {
            if(value) return;
            throw new ContractException(message);
        }
    }
}