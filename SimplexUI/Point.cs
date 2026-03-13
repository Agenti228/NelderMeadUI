namespace SimplexUI
{
    public readonly struct Point : IComparable<Point>, ICloneable
    {
        public readonly double[] Coordinates { get; }
        public readonly Func<double[], double> Function { get; }
        public double Value { get; }

        public Point(double[] coordinates, Func<double[], double> function)
        {
            Coordinates = coordinates;
            Function = function;
            Value = Function(coordinates);
        }

        /// <summary>
        /// redo this
        /// </summary>
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// redo this
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
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

        /// <summary>
        /// redo function equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <exception cref="Exception"></exception>
        private static void CheckForException(Point left, Point right)
        {
            if (left.Function != right.Function)
            {
                throw new Exception("left and right part have different functions");
            }
            else if (left.Coordinates.Length != right.Coordinates.Length)
            {
                throw new Exception("left and right part have different dimentions");
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
        }

        public PointF ToPointF()
        {
            if (Coordinates.Length > 2 || Coordinates.Length == 0) throw new ArgumentException();
            return new PointF((float)Coordinates[0], (float)Coordinates[1]);
        }
    }
}