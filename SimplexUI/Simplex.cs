using SimplexUI.Exceptions;

namespace SimplexUI
{
    public struct Simplex
    {
        /// <summary>
        /// Simplex points MUST be sorted before calling this
        /// </summary>
        /// <exception cref="UnsortedArrayException"></exception>
        public readonly EvaluateableVector GetBestInSorted
        {
            get
            {
                if (!_vectorsSorted)
                {
                    throw new UnsortedArrayException("Can't get best point in unsorted array");
                }

                return _vectors[0];
            }
        }
        /// <summary>
        /// <inheritdoc cref="GetBestInSorted"/>
        /// </summary>
        /// <exception cref="UnsortedArrayException"></exception>
        public readonly EvaluateableVector GetSecondBestInSorted
        {
            get
            {
                if (!_vectorsSorted)
                {
                    throw new UnsortedArrayException("Can't get second best point in unsorted array");
                }

                return _vectors[1];
            }
        }
        /// <summary>
        /// <inheritdoc cref="GetBestInSorted"/>
        /// </summary>
        /// <exception cref="UnsortedArrayException"></exception>
        public readonly EvaluateableVector GetWorstInSorted
        {
            get
            {
                if (!_vectorsSorted)
                {
                    throw new UnsortedArrayException("Can't get worst point in unsorted array");
                }

                return _vectors[^1];
            }
        }
        /// <summary>
        /// <inheritdoc cref="GetBestInSorted"/>
        /// </summary>
        /// <exception cref="UnsortedArrayException"></exception>
        public readonly EvaluateableVector GetCenterInSorted 
        {
            get
            {
                if (!_vectorsSorted)
                {
                    throw new UnsortedArrayException("Can't get center point in unsorted array");
                }

                var center = new EvaluateableVector(new double[_initialConditions.SimplexDimention - 1], _initialConditions.VectorFunction);

                for (int i = 0; i < _vectors.Length - 1; i++)
                {
                    center += _vectors[i];
                }

                return center / (_vectors.Length - 1);
            } 
        }

        private readonly Settings _settings = new();
        private readonly InitialConditions _initialConditions = new();
        private readonly EvaluateableVector[] _vectors;
        private bool _vectorsSorted = false;

        public Simplex(Settings settings, InitialConditions initialConditions)
        {
            _settings = settings;
            _initialConditions = initialConditions;

            _vectors = new EvaluateableVector[initialConditions.SimplexDimention];
            Array.Copy(initialConditions.InitialVectors, _vectors, _vectors.Length);
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

            _vectorsSorted = false;
        }

        public void SortPoints()
        {
            Array.Sort(_vectors);
            _vectorsSorted = true;
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
            _vectors[^1] = (EvaluateableVector)newPoint.Clone();
        }

        public readonly void ReduceSimplex()
        {
            var best = (EvaluateableVector)GetBestInSorted.Clone();
            for (int i = 0; i < _vectors.Length; i++)
            {
                _vectors[i] = best + (_vectors[i] - best) * _settings.Reduction;
            }
        }

        public readonly EvaluateableVector[] ClonePoints()
        {
            var points = new EvaluateableVector[_vectors.Length];
            for (int i = 0; i < _vectors.Length; i++)
            {
                points[i] = (EvaluateableVector)_vectors[i].Clone();
            }
            return points;
        }
    }
}