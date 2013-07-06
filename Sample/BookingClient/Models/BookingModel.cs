using System;

namespace BookingClient.Models
{
    public class BookingModel
    {
        public int RoomNumber { get; set; }

        public string PersonName { get; set; }

        public DateTime BeginAt { get; set; }

        public DateTime EndAt { get; set; }
    }
}