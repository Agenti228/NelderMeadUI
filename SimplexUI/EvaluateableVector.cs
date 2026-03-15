namespace SimplexUI
{
    public struct EvaluateableVector : IComparable<EvaluateableVector>, ICloneable
    {
        public readonly double[] Coordinates { get; }
        public readonly Func<double[], double> Function { get; }
        public double Value { get; set; }

        public EvaluateableVector(double[] coordinates, Func<double[], double> function)
        {
            Coordinates = new double[coordinates.Length];
            Array.Copy(coordinates, Coordinates, Coordinates.Length);

            Function = function;
            Value = Function(Coordinates);
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj is not EvaluateableVector other)
            {
                return false;
            }

            if (Coordinates.Length != other.Coordinates.Length)
            {
                return false;
            }

            return Coordinates.SequenceEqual(other.Coordinates);
        }

        public override readonly int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (double coordinate in Coordinates)
                {
                    hash = hash * 31 + coordinate.GetHashCode();
                }

                return hash;
            }
        }

        public readonly int CompareTo(EvaluateableVector other)
        {
            return Value.CompareTo(other.Value);
        }

        public override readonly string ToString()
        {
            string coordinates = string.Empty;

            for (int i = 0; i < Coordinates.Length; i++)
            {
                coordinates += $"{Coordinates[i]}";

                if (i < Coordinates.Length - 1)
                {
                    coordinates += ", ";
                }
            }

            return $"[{coordinates}] {Value}";
        }
        
        public readonly object Clone()
        {
            return new EvaluateableVector(Coordinates, Function);
        }

        private static void CheckForException(EvaluateableVector left, EvaluateableVector right)
        {
            if (left.Function != right.Function)
            {
                throw new Exception("Left and right part have different pointers to functions");
            }
            else if (left.Coordinates.Length != right.Coordinates.Length)
            {
                throw new Exception("Left and right part have different dimentions");
            }
        }

        public static EvaluateableVector operator +(EvaluateableVector left, EvaluateableVector right)
        {
            CheckForException(left, right);

            double[] coordinates = new double[left.Coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = left.Coordinates[i] + right.Coordinates[i];
            }

            return new EvaluateableVector(coordinates, left.Function);
        }

        public static EvaluateableVector operator -(EvaluateableVector left, EvaluateableVector right)
        {
            CheckForException(left, right);

            double[] coordinates = new double[left.Coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = left.Coordinates[i] - right.Coordinates[i];
            }

            return new EvaluateableVector(coordinates, left.Function);
        }

        public static EvaluateableVector operator *(EvaluateableVector left, double multiplyer)
        {
            double[] coordinates = new double[left.Coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = left.Coordinates[i] * multiplyer;
            }

            return new EvaluateableVector(coordinates, left.Function);
        }

        public static EvaluateableVector operator /(EvaluateableVector left, double dividor)
        {
            double[] coordinates = new double[left.Coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = left.Coordinates[i] / dividor;
            }

            return new EvaluateableVector(coordinates, left.Function);
        }

        public static bool operator <(EvaluateableVector left, EvaluateableVector right)
        {
            CheckForException(left, right);

            return left.Value < right.Value;
        }

        public static bool operator <=(EvaluateableVector left, EvaluateableVector right)
        {
            CheckForException(left, right);

            return left.Value <= right.Value;
        }

        public static bool operator >(EvaluateableVector left, EvaluateableVector right)
        {
            CheckForException(left, right);

            return left.Value > right.Value;
        }

        public static bool operator >=(EvaluateableVector left, EvaluateableVector right)
        {
            CheckForException(left, right);

            return left.Value >= right.Value;
        }

        public static bool operator ==(EvaluateableVector left, EvaluateableVector right)
        {
            CheckForException(left, right);

            return left.Value == right.Value;
        }

        public static bool operator !=(EvaluateableVector left, EvaluateableVector right)
        {
            return !(left.Value == right.Value);
        }

        public double this[int index]
        {
            readonly get
            {
                return Coordinates[index];
            }
            set
            {
                Coordinates[index] = value;
                Value = Function(Coordinates);
            }
        }
    }
}