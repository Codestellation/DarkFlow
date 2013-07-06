using System;
using System.Runtime.Serialization;

namespace BookingContract
{
    [DataContract]
    public class BookRequest
    {
        [DataMember]
        public int RoomNumber { get; set; }
        
        [DataMember]
        public string PersonName { get; set; }

        [DataMember]
        public DateTime BeginAt { get; set; }

        [DataMember]
        public DateTime EndAt { get; set; }
    }
}
