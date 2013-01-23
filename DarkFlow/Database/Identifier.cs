using System;

namespace Codestellation.DarkFlow.Database
{
    public struct Identifier
    {
        private readonly Guid _id;

        private readonly Region _region;
        
        private readonly string _asString;

        public Identifier(Guid id, Region region)
        {
            if (Guid.Empty.Equals(id))
            {
                throw new ArgumentException("Id should be not empty Guid.", "id");
            }

            if(string.IsNullOrWhiteSpace(region.Name))
            {
                throw new ArgumentException("Region should be initialized with not null not empty value", "region");
            }

            _id = id;
            _region = region;
            _asString = string.Format("{0}.{1}", _region, _id);
        }

        public Guid Id
        {
            get { return _id; }
        }

        public Region Region
        {
            get { return _region; }
        }

        public bool IsValid
        {
            get { return !Guid.Empty.Equals(_id) && _region.IsValid; }
        }

        public override string ToString()
        {
            return _asString;
        }

        public bool Equals(Identifier other)
        {
            return _id.Equals(other._id) && _region.Equals(other._region);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Identifier && Equals((Identifier) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_id.GetHashCode()*397) ^ _region.GetHashCode();
            }
        }

        public static bool operator ==(Identifier left, Identifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Identifier left, Identifier right)
        {
            return !left.Equals(right);
        }

        public static Identifier Parse(string identifier)
        {
            var tokens = identifier.Split('.');
            var region = new Region(tokens[0]);
            var id = Guid.Parse(tokens[1]);
            return new Identifier(id, region);
        }
    }
}