using System;

namespace BookingService
{
    public struct BookPeriod
    {
        public readonly DateTime BeginAt;
        public readonly DateTime EndAt;

        public BookPeriod(DateTime beginAt, DateTime endAt)
        {
            BeginAt = beginAt.Date;
            EndAt = endAt.Date;
        }

        public bool Intersects(BookPeriod other)
        {
            return other.BeginAt <= BeginAt && BeginAt < other.EndAt ||
                   other.BeginAt < EndAt && EndAt <= other.EndAt;
        }

        public bool Equals(BookPeriod other)
        {
            return BeginAt.Equals(other.BeginAt) && EndAt.Equals(other.EndAt);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BookPeriod && Equals((BookPeriod) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (BeginAt.GetHashCode()*397) ^ EndAt.GetHashCode();
            }
        }

        public static bool operator ==(BookPeriod left, BookPeriod right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BookPeriod left, BookPeriod right)
        {
            return !left.Equals(right);
        }
    }
}