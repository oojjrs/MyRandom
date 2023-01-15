using System;

namespace Assets.Sources.Scripts
{
    public partial struct Hex
    {
        public override bool Equals(object obj)
        {
            return obj is Hex hex &&
                   Form == hex.Form &&
                   Q == hex.Q &&
                   R == hex.R;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Form, Q, R);
        }

        public override string ToString()
        {
            return Q + "_" + R;
        }

        public static bool operator ==(Hex left, Hex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Hex left, Hex right)
        {
            return !(left == right);
        }
    }
}
