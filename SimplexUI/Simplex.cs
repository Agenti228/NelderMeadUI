namespace SimplexUI
{
    public readonly struct Simplex
    {
        public readonly Point GetBest => _points[0];
        public readonly Point GetSecondBest => _points[1];
        public readonly Point GetWorst => _points[^1];
        /// <summary>
        /// <see cref="_points"/> MUST be sorted before calculating center
        /// </summary>
        public readonly Point GetCenter 
        {
            get
            {
                var center = new Point(new double[_dimention], _function);

                for (int i = 0; i < _points.Length - 1; i++)
                {
                    center += _points[i];
                }

                return center / (_points.Length - 1);
            } 
        }

        private readonly Settings _settings = new();
        private readonly Func<double[], double> _function;
        public readonly Point[] _points;
        private readonly int _dimention;
        
        public Simplex(Settings settings, double[][] points, Func<double[], double> function)
        {
            _settings = settings;
            _points = new Point[points.Length];
            _dimention = _points.Length - 1;
            _function = function;

            for (int i = 0; i < points.Length; i++)
            {
                _points[i] = new Point(points[i], _function);
            }
        }

        public Simplex(Settings settings, int dimention, double[] startingCoordinates, double edgeLength, Func<double[], double> function)
        {
            _settings = settings;
            _dimention = dimention;
            _points = new Point[_dimention + 1];
            _function = function;

            double[] x0 = [10];
            double[] x1 = [x0[0] + 2];

            _points[0] = new Point(startingCoordinates, function);
            for (int i = 1; i < _points.Length; i++)
            {
                var point = new Point(startingCoordinates, _function);
                point[i - 1] += edgeLength;

                _points[i] = point;
            }
        }

        public readonly void Iteration()
        {
            SortPoints();

            Point reflected = Reflect(GetWorst, GetCenter);

            if (reflected < GetBest)
            {
                Point expanded = Expand(reflected, GetCenter);

                if (expanded < reflected)
                {
                    ReplaceWorst(expanded);
                }
                else
                {
                    ReplaceWorst(reflected);
                }
            }
            else if (reflected < GetSecondBest)
            {
                ReplaceWorst(reflected);
            }
            else
            {
                Point contracted;

                if (reflected < GetWorst)
                {
                    contracted = ContractOutside(reflected, GetCenter);

                    if (contracted < reflected)
                    {
                        ReplaceWorst(contracted);
                    }
                    else
                    {
                        ReduceSimplex();
                    }
                }
                else
                {
                    contracted = ContractInside(GetWorst, GetCenter);

                    if (contracted < GetWorst)
                    {
                        ReplaceWorst(contracted);
                    }
                    else
                    {
                        ReduceSimplex();
                    }
                }
            }
        }

        public readonly void SortPoints()
        {
            Array.Sort(_points);
        }

        public readonly Point Reflect(Point reflectedPoint, Point reflectionPoint)
        {
            return reflectionPoint + (reflectionPoint - reflectedPoint) * _settings.Reflection;
        }

        public readonly Point Expand(Point expandedPoint, Point expantionPoint)
        {
            return expantionPoint + (expandedPoint - expantionPoint) * _settings.Stretching;
        }

        public readonly Point ContractInside(Point contractedPoint, Point contractionPoint)
        {
            return contractionPoint + (contractedPoint - contractionPoint) * _settings.Compression;
        }

        public readonly Point ContractOutside(Point contractedPoint, Point contractionPoint)
        {
            return contractionPoint - (contractionPoint - contractedPoint) * _settings.Compression;
        }

        public readonly void ReplaceWorst(Point newPoint)
        {
            _points[^1] = (Point)newPoint.Clone();
        }

        public readonly void ReduceSimplex()
        {
            var best = (Point)_points[0].Clone();
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i] = best + (_points[i] - best) * _settings.Reduction;
            }
        }

        public readonly Point[] ClonePoints()
        {
            var points = new Point[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                points[i] = (Point)_points[i].Clone();
            }
            return points;
        }
    }
}