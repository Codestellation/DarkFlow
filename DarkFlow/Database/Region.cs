using System;

namespace Codestellation.DarkFlow.Database
{
    public struct Region
    {
        private readonly string _name;

        private readonly bool _isValid;

        public Region(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name", "Region name should be not null not empty string.");
            }
            _name = string.Intern(name);
            _isValid = true;
        }

        public string Name
        {
            get
            {
                if (_isValid)
                {
                    return _name;
                }
                throw new InvalidOperationException("Name is not set.");
            }
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        public override string ToString()
        {
            return _name ?? "<undefined>";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Region && Equals((Region)obj);
        }

        public bool Equals(Region other)
        {
            return string.Equals(_name, other._name);
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