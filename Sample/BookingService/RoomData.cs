using System.Collections.Generic;

namespace BookingService
{
    public class RoomData
    {
        private readonly int _number;
        private readonly List<Booking> _bookings;

        public RoomData(int number)
        {
            _number = number;
            _bookings = new List<Booking>();
        }

        public void TryBook(Booking booking)
        {
            var intersected = _bookings.Find(b => b.Period.Intersects(booking.Period));
            if (intersected == null)
            {
                _bookings.Add(booking);
                booking.Accept();
            }
            booking.Reject("Room already booked at this period.");
        }
    }
} 