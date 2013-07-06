using BookingContract;
using Codestellation.DarkFlow;

namespace BookingService
{
    public class BookTask : ITask
    {
        private readonly BookRequest _request;
        private readonly BookRegister _register;
        
        private Booking _booking;

        public Booking Result
        {
            get { return _booking; }
        }

        public BookTask(BookRequest request, BookRegister register)
        {
            _request = request;
            _register = register;
        }

        public void Execute()
        {
            var period = new BookPeriod(_request.BeginAt, _request.EndAt);
            _booking = new Booking(_request.PersonName, period);
            _register.TryBook(_request.RoomNumber, _booking);
        }
    }
}