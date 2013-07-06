using System;
using System.ServiceModel;
using BookingContract;
using Codestellation.DarkFlow;
using Codestellation.DarkFlow.Async;

namespace BookingService
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple, 
        InstanceContextMode = InstanceContextMode.Single)]
    public class BookingService : IBookingService
    {
        private readonly IExecutor _executor;
        private readonly BookRegister _register;

        public BookingService(IExecutor executor, BookRegister register)
        {
            _executor = executor;
            _register = register;
        }

        public IAsyncResult BeginBook(BookRequest request, AsyncCallback callback, object state)
        {
            var bookTask = new BookTask(request, _register);
            var asyncWrap = new AsyncTaskWrap<BookTask, Booking>(bookTask, x => x.Result, callback, state);
            _executor.Execute(asyncWrap);
            return asyncWrap;
        }

        public BookResult EndBook(IAsyncResult asyncResult)
        {
            var task = (AsyncTaskWrap<BookTask, Booking>)asyncResult;

            var result = new BookResult()
            {
                BookingStatus = task.Result.Accepted ? BookingStatus.Approved : BookingStatus.Rejected,
                Message = task.Result.Message
            };
            
            return result;
        }
    }
}