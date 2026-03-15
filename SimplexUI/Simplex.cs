namespace SimplexUI
{
    public struct Simplex
    {
        /// <summary>
        /// Simplex points MUST be sorted before calling this
        /// </summary>
        public readonly EvaluateableVector GetBestInSorted
        {
            get
            {
                if (!_pointsSorted)
                {
                    throw new Exception("Points array MUST be sorted");
                }

                return _points[0];
            }
        }
        /// <summary>
        /// <inheritdoc cref="GetBestInSorted"/>
        /// </summary>
        public readonly EvaluateableVector GetSecondBestInSorted
        {
            get
            {
                if (!_pointsSorted)
                {
                    throw new Exception("Points array is not sorted");
                }

                return _points[1];
            }
        }
        /// <summary>
        /// <inheritdoc cref="GetBestInSorted"/>
        /// </summary>
        public readonly EvaluateableVector GetWorstInSorted
        {
            get
            {
                if (!_pointsSorted)
                {
                    throw new Exception("Points array is not sorted");
                }

                return _points[^1];
            }
        }
        /// <summary>
        /// <inheritdoc cref="GetBestInSorted"/>
        /// </summary>
        public readonly EvaluateableVector GetCenterInSorted 
        {
            get
            {
                if (!_pointsSorted)
                {
                    throw new Exception("Points array is not sorted");
                }

                var center = new EvaluateableVector(new double[_initialConditions.SimplexDimention - 1], _initialConditions.VectorFunction);

                for (int i = 0; i < _points.Length - 1; i++)
                {
                    center += _points[i];
                }

                return center / (_points.Length - 1);
            } 
        }

        private readonly Settings _settings = new();
        private readonly InitialConditions _initialConditions = new();
        private readonly EvaluateableVector[] _points;
        private bool _pointsSorted = false;

        public Simplex(Settings settings, InitialConditions initialConditions)
        {
            _settings = settings;
            _initialConditions = initialConditions;

            _points = new EvaluateableVector[initialConditions.SimplexDimention];
            Array.Copy(initialConditions.InitialVectors, _points, _points.Length);
        }

        public void Iteration()
        {
            SortPoints();

            EvaluateableVector reflected = Reflect(GetWorstInSorted, GetCenterInSorted);

            if (reflected < GetBestInSorted)
            {
                EvaluateableVector expanded = Expand(reflected, GetCenterInSorted);

                if (expanded < reflected)
                {
                    ReplaceWorst(expanded);
                }
                else
                {
                    ReplaceWorst(reflected);
                }
            }
            else if (reflected < GetSecondBestInSorted)
            {
                ReplaceWorst(reflected);
            }
            else
            {
                EvaluateableVector contracted;

                if (reflected < GetWorstInSorted)
                {
                    contracted = ContractOutside(reflected, GetCenterInSorted);

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
                    contracted = ContractInside(GetWorstInSorted, GetCenterInSorted);

                    if (contracted < GetWorstInSorted)
                    {
                        ReplaceWorst(contracted);
                    }
                    else
                    {
                        ReduceSimplex();
                    }
                }
            }

            _pointsSorted = false;
        }

        public void SortPoints()
        {
            Array.Sort(_points);
            _pointsSorted = true;
        }

        public readonly EvaluateableVector Reflect(EvaluateableVector reflectedPoint, EvaluateableVector reflectionPoint)
        {
            return reflectionPoint + (reflectionPoint - reflectedPoint) * _settings.Reflection;
        }

        public readonly EvaluateableVector Expand(EvaluateableVector expandedPoint, EvaluateableVector expantionPoint)
        {
            return expantionPoint + (expandedPoint - expantionPoint) * _settings.Stretching;
        }

        public readonly EvaluateableVector ContractInside(EvaluateableVector contractedPoint, EvaluateableVector contractionPoint)
        {
            return contractionPoint + (contractedPoint - contractionPoint) * _settings.Compression;
        }

        public readonly EvaluateableVector ContractOutside(EvaluateableVector contractedPoint, EvaluateableVector contractionPoint)
        {
            return contractionPoint - (contractionPoint - contractedPoint) * _settings.Compression;
        }

        public readonly void ReplaceWorst(EvaluateableVector newPoint)
        {
            _points[^1] = (EvaluateableVector)newPoint.Clone();
        }

        public readonly void ReduceSimplex()
        {
            var best = (EvaluateableVector)GetBestInSorted.Clone();
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i] = best + (_points[i] - best) * _settings.Reduction;
            }
        }

        public readonly EvaluateableVector[] ClonePoints()
        {
            var points = new EvaluateableVector[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                points[i] = (EvaluateableVector)_points[i].Clone();
            }
            return points;
        }
    }
}