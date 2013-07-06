using System.ServiceModel;
using System.Web.Mvc;
using BookingClient.Models;
using BookingContract;

namespace BookingClient.Controllers
{
    public class BookingController : Controller
    {
        [HttpGet]
        public ViewResult Book()
        {
            ViewBag.Title = "Book a room";
            return View(new BookingModel());
        }

        [HttpPost]
        public ViewResult Book(BookingModel model)
        {
            var request = new BookRequest
            {
                RoomNumber = model.RoomNumber,
                PersonName = model.PersonName,
                BeginAt = model.BeginAt,
                EndAt = model.EndAt
            };

            var binding = new NetTcpBinding();
            var endpointAddress = new EndpointAddress("net.tcp://localhost:9090");

            var channelFactory = new ChannelFactory<IBookingService>(binding, endpointAddress);

            
            var channel = channelFactory.CreateChannel(endpointAddress);

            var result = channel.Book(request);
            if (result.BookingStatus == BookingStatus.Approved)
            {
                @ViewBag.Title = "Booking completed";
                return View("Success", model);
            }
            
            @ViewBag.Title = "Booking failed";
            @ViewBag.Message = result.Message;

            return View("Failed");
        }
    }
}