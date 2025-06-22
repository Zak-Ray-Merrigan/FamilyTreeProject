using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary
{
    public interface ICopyable<T>
    {
        public T Copy();
    }
}