using System.Collections;
namespace FamilyTreeLibrary.Enumerators
{
    public class PrimeNumberEnumerator : IEnumerator<int>
    {
        private int num = 2;

        public int Current
        {
            get
            {
                return num;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return num;
            }
        }

        public void Dispose()
        {
            num++;
            int n = 2;
            while (n < num)
            {
                if (num % n == 0)
                {
                    num++;
                    n = 2;
                }
                else
                {
                    n++;
                }
            }
        }

        public bool MoveNext()
        {
            return num < int.MaxValue;
        }

        public void Reset()
        {
            num = 2;
        }
    }
}