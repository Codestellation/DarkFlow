namespace BookingService
{
    public class Booking
    {
        private bool _rejected;
        private string _message;
        private bool _accepted;

        public Booking(string person, BookPeriod period)
        {
            Person = person;
            Period = period;
        }

        public string Person { get; private set; }

        public BookPeriod Period { get; private set; }

        public bool Rejected
        {
            get { return _rejected; }
        }

        public string Message
        {
            get { return _message; }
        }

        public bool Accepted
        {
            get { return _accepted; }
        }

        public void Reject(string message)
        {
            _rejected = true;
            _message = message;
        }

        public void Accept()
        {
            _accepted = true;
        }
    }
}