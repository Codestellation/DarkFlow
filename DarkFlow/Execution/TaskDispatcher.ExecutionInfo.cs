using System.Threading;
using System.Threading.Tasks;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
  public partial class TaskDispatcher
  {
      private class ExecutionInfo
      {
          //TODO I guess more sophisticated sync could be involved.
          public volatile Task TplTask;
          public volatile ExecutionEnvelope CurrentTask;
          private int _ownedCellIndex;
          private const int Free = -1;

          public ExecutionInfo()
          {
              _ownedCellIndex = Free;
          }

          public int OwnedCellIndex
          {
              get { return _ownedCellIndex; }
          }

          public void WaitTaskFinished()
          {
              if (TplTask != null)
              {
                  TplTask.Wait();
              }
          }

          public void Release()
          {
              Contract.Require(TplTask != null, "TplTask != null");
              Contract.Require(_ownedCellIndex > Free, "_ownedCellIndex > Free");

              CurrentTask = null;
              TplTask = null;
              _ownedCellIndex = Free;
          }

          public void TakeFreeCell(ExecutionInfo[] executionInfos)
          {
              Contract.Require(CurrentTask == null, "CurrentTask == null");
              Contract.Require(TplTask == null, "TplTask == null");
              Contract.Require(_ownedCellIndex == Free, "_ownedCellIndex == Free");

              for (int index = 0; index < executionInfos.Length; index++)
              {
                  var cell = executionInfos[index];

                  var cellAlreadyOwned = cell != null;
                  //First check to skip unnecessary CAS operation.
                  if (cellAlreadyOwned) continue;

                  var originalValue = Interlocked.CompareExchange(ref cell, this, null);

                  //Second check for interlocked result;
                  cellAlreadyOwned = originalValue != null;

                  if (cellAlreadyOwned) continue;
                  _ownedCellIndex = index;
                  break;
              }
          }
      }
  }
}