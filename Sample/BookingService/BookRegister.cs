using System.Collections.Generic;
using System.Linq;

namespace BookingService
{
    public class BookRegister
    {
        private Dictionary<int, RoomData> _rooms ;

        public BookRegister()
        {
            _rooms = Enumerable.Range(1, 1000).ToDictionary(x => x, x => new RoomData(x));
        }

        public void TryBook(int roomNumber, Booking booking)
        {
            RoomData room;
            if (_rooms.TryGetValue(roomNumber, out room))
            {
                room.TryBook(booking);
            }
            else
            {
                booking.Reject("No such room.");    
            }
        }
    }
}
