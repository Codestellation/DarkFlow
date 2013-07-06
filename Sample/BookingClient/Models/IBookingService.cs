using System.ServiceModel;
using BookingContract;

namespace BookingClient.Models
{
    [ServiceContract(Namespace = "DarkFlow.Samples", Name = "BookingService")]
    public interface IBookingService
    {
        [OperationContract]
        BookResult Book(BookRequest request);
    }
}