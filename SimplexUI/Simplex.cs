using SimplexUI.Exceptions;

namespace SimplexUI
{
    public struct Simplex : ICloneable
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

                return Vectors[0];
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

                return Vectors[1];
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

                return Vectors[^1];
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

                var center = new EvaluateableVector(new double[InitialConditions.SimplexDimention - 1], InitialConditions.VectorFunction);

                for (int i = 0; i < Vectors.Length - 1; i++)
                {
                    center += Vectors[i];
                }

                return center / (Vectors.Length - 1);
            } 
        }

        public Settings Settings { get; }
        public InitialConditions InitialConditions { get; }
        public EvaluateableVector[] Vectors { get; }
        private bool _vectorsSorted = false;

        public Simplex(Settings settings, InitialConditions initialConditions)
        {
            Settings = settings;
            InitialConditions = initialConditions;

            Vectors = new EvaluateableVector[initialConditions.SimplexDimention];
            Array.Copy(initialConditions.InitialVectors, Vectors, Vectors.Length);
        }

        /// <summary>
        /// Simplex points MUST be sorted before calling this
        /// </summary>
        /// <exception cref="UnsortedArrayException"></exception>
        public void IterationOnSorted()
        {
            if (!_vectorsSorted)
            {
                throw new UnsortedArrayException("Can't iterate on unsorted array");
            }

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

        public void SortVectors()
        {
            Array.Sort(Vectors);
            _vectorsSorted = true;
        }

        public readonly EvaluateableVector Reflect(EvaluateableVector reflectedPoint, EvaluateableVector reflectionPoint)
        {
            return reflectionPoint + (reflectionPoint - reflectedPoint) * Settings.Reflection;
        }

        public readonly EvaluateableVector Expand(EvaluateableVector expandedPoint, EvaluateableVector expantionPoint)
        {
            return expantionPoint + (expandedPoint - expantionPoint) * Settings.Stretching;
        }

        public readonly EvaluateableVector ContractInside(EvaluateableVector contractedPoint, EvaluateableVector contractionPoint)
        {
            return contractionPoint + (contractedPoint - contractionPoint) * Settings.Compression;
        }

        public readonly EvaluateableVector ContractOutside(EvaluateableVector contractedPoint, EvaluateableVector contractionPoint)
        {
            return contractionPoint - (contractionPoint - contractedPoint) * Settings.Compression;
        }

        public readonly void ReplaceWorst(EvaluateableVector newPoint)
        {
            Vectors[^1] = (EvaluateableVector)newPoint.Clone();
        }

        public readonly void ReduceSimplex()
        {
            var best = (EvaluateableVector)GetBestInSorted.Clone();
            for (int i = 0; i < Vectors.Length; i++)
            {
                Vectors[i] = best + (Vectors[i] - best) * Settings.Reduction;
            }
        }

        public readonly EvaluateableVector[] ClonePoints()
        {
            var points = new EvaluateableVector[Vectors.Length];
            for (int i = 0; i < Vectors.Length; i++)
            {
                points[i] = (EvaluateableVector)Vectors[i].Clone();
            }
            return points;
        }

        public readonly object Clone()
        {
            var simplex = new Simplex(Settings, InitialConditions);
            Array.Copy(Vectors, simplex.Vectors, Vectors.Length);

            return simplex;
        }
    }
}