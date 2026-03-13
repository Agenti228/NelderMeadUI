namespace SimplexUI
{
    public readonly struct Point : IComparable<Point>, ICloneable
    {
        public readonly double[] Coordinates { get; }
        public readonly Func<double[], double> Function { get; }
        public double Value { get; }

        public Point(double[] coordinates, Func<double[], double> function)
        {
            Coordinates = new double[coordinates.Length];
            Array.Copy(coordinates, Coordinates, Coordinates.Length);

            Function = function;
            Value = Function(Coordinates);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Point other)
            {
                return false;
            }

            if (Coordinates.Length != other.Coordinates.Length)
            {
                return false;
            }

            return Coordinates.SequenceEqual(other.Coordinates);
        }

        public override int GetHashCode()
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

        public int CompareTo(Point other)
        {
            return Value.CompareTo(other.Value);
        }

        public override readonly string ToString()
        {
            string coordinates = string.Empty;

            for (int i = 0; i < Coordinates.Length; i++)
            {
                coordinates += $"{Coordinates[i]} ";
            }

            return $"{coordinates} {Value}";
        }
        
        public object Clone()
        {
            return new Point(Coordinates, Function);
        }

        private static void CheckForException(Point left, Point right)
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

        public static Point operator +(Point left, Point right)
        {
            CheckForException(left, right);

            double[] coordinates = new double[left.Coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = left.Coordinates[i] + right.Coordinates[i];
            }

            return new Point(coordinates, left.Function);
        }

        public static Point operator -(Point left, Point right)
        {
            CheckForException(left, right);

            double[] coordinates = new double[left.Coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = left.Coordinates[i] - right.Coordinates[i];
            }

            return new Point(coordinates, left.Function);
        }

        public static Point operator *(Point left, double right)
        {
            double[] coordinates = new double[left.Coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = left.Coordinates[i] * right;
            }

            return new Point(coordinates, left.Function);
        }

        public static Point operator /(Point left, double right)
        {
            double[] coordinates = new double[left.Coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = left.Coordinates[i] / right;
            }

            return new Point(coordinates, left.Function);
        }

        public static bool operator <(Point left, Point right)
        {
            CheckForException(left, right);

            return left.Value < right.Value;
        }

        public static bool operator <=(Point left, Point right)
        {
            CheckForException(left, right);

            return left.Value <= right.Value;
        }

        public static bool operator >(Point left, Point right)
        {
            CheckForException(left, right);

            return left.Value > right.Value;
        }

        public static bool operator >=(Point left, Point right)
        {
            CheckForException(left, right);

            return left.Value >= right.Value;
        }

        public static bool operator ==(Point left, Point right)
        {
            CheckForException(left, right);

            return left.Value == right.Value;
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left.Value == right.Value);
        }

        public double this[int index]
        {
            get
            {
                return Coordinates[index];
            }
            set
            {
                Coordinates[index] = value;
            }
        }
    }
}