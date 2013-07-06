using System.Runtime.Serialization;

namespace BookingContract
{
    [DataContract]
    public class BookResult
    {
        [DataMember]
        public BookingStatus BookingStatus { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}