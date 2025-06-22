using FamilyTreeLibrary.Enumerators;

namespace FamilyTreeLibrary.Data.Models
{
    public readonly struct HierarchialCoordinate : IComparable<HierarchialCoordinate>, ICopyable<HierarchialCoordinate>, IEquatable<HierarchialCoordinate>
    {
        private readonly int[] coordinate;

        public HierarchialCoordinate()
        {
            coordinate = [];
        }

        public HierarchialCoordinate(int[] coordinate)
        {
            this.coordinate = coordinate;
        }

        public readonly HierarchialCoordinate Child
        {
            get
            {
                int[] childCoordinate = new int[coordinate.Length + 1];
                Array.Copy(coordinate, childCoordinate, coordinate.Length);
                childCoordinate[^1] = 1;
                return new(childCoordinate);
            }
        }

        public readonly HierarchialCoordinate? Parent
        {
            get
            {
                if (coordinate.Length == 0)
                {
                    return null;
                }
                int[] parentCoordinate = new int[coordinate.Length - 1];
                Array.Copy(coordinate, parentCoordinate, parentCoordinate.Length);
                return new(parentCoordinate);
            }
        }

        public readonly HierarchialCoordinate? Sibling
        {
            get
            {
                if (coordinate.Length == 0)
                {
                    return null;
                }
                int[] siblingCoordinate = new int[coordinate.Length];
                Array.Copy(coordinate, siblingCoordinate, coordinate.Length - 1);
                siblingCoordinate[^1] = coordinate[^1] + 1;
                return new(siblingCoordinate);
            }
        }

        public int CompareTo(HierarchialCoordinate other)
        {
            bool instanceIsRoot = coordinate.Length == 0;
            bool otherIsRoot = other.coordinate.Length == 0;
            if (instanceIsRoot && otherIsRoot)
            {
                return 0;
            }
            else if (instanceIsRoot && !otherIsRoot)
            {
                return -1;
            }
            else if (!instanceIsRoot && otherIsRoot)
            {
                return 1;
            }
            for (int i = 0; i < coordinate.Length && i < other.coordinate.Length; i++)
            {
                if (coordinate[i] != other.coordinate[i])
                {
                    return coordinate[i] - other.coordinate[i];
                }
            }
            return coordinate.Length - other.coordinate.Length;
        }

        public HierarchialCoordinate Copy()
        {
            return new(coordinate);
        }

        public override bool Equals(object? obj)
        {
            return obj is HierarchialCoordinate other && Equals(other);
        }

        public bool Equals(HierarchialCoordinate other)
        {
            return CompareTo(other) == 0;
        }

        public readonly override int GetHashCode()
        {
            IEnumerator<int> primeNumberEnumerator = new PrimeNumberEnumerator();
            int sum = 0;
            foreach (int p in coordinate)
            {
                sum += primeNumberEnumerator.Current * p;
                primeNumberEnumerator.Dispose();
            }
            return sum;
        }

        public readonly override string ToString()
        {
            string dotDelimitedCoordinate = string.Join('.', coordinate);
            return coordinate.Length > 0 ? $"[{dotDelimitedCoordinate}]" : dotDelimitedCoordinate;
        }

        public static bool operator ==(HierarchialCoordinate left, HierarchialCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HierarchialCoordinate left, HierarchialCoordinate right)
        {
            return !left.Equals(right);
        }

        public static bool operator<(HierarchialCoordinate left, HierarchialCoordinate right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator>(HierarchialCoordinate left, HierarchialCoordinate right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator<=(HierarchialCoordinate left, HierarchialCoordinate right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator>=(HierarchialCoordinate left, HierarchialCoordinate right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static HierarchialCoordinate operator++(HierarchialCoordinate coordinate)
        {
            return coordinate.Child;
        }
        public static HierarchialCoordinate operator--(HierarchialCoordinate coordinate)
        {
            return coordinate.Parent ?? throw new InvalidOperationException("Nothing comes before the root.");
        }
    }
}