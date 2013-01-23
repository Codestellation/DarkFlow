using System;

namespace Codestellation.DarkFlow.Database
{
    public struct Region
    {
        public bool Equals(Region other)
        {
            return string.Equals(_name, other._name);
        }

        private readonly string _name;

        public Region(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name", "Region name should be not null not empty string.");
            }
            _name = name;
        }

        public string Name
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException("Name is not set.");
                }
                return _name;
            }
        }

        public bool IsValid
        {
            get { return !string.IsNullOrWhiteSpace(_name); }
        }

        public override string ToString()
        {
            return _name ?? "<undefined>";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Region && Equals((Region) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public static bool operator ==(Region left, Region right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Region left, Region right)
        {
            return !left.Equals(right);
        }

        public Identifier NewIdentifier()
        {
            return new Identifier(Guid.NewGuid(), this);
        }
    }
}