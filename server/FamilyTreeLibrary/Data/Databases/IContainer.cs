using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Data.Databases
{
    public interface IContainer<T>
    {
        public T this[Guid id]
        {
            get;
        }

        public void Remove(T item);

        public void UpdateOrCreate(T item);
    }
}