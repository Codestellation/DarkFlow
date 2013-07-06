using System;
using System.ServiceModel;
using BookingContract;

namespace BookingService
{
    [ServiceContract(Namespace = "DarkFlow.Samples", Name = "BookingService")]
    public interface IBookingService
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBook(BookRequest request, AsyncCallback callback, object state);

        BookResult EndBook(IAsyncResult asyncResult);
    }
}