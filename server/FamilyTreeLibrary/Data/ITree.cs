using FamilyTreeLibrary.Models;
using MongoDB.Bson;

namespace FamilyTreeLibrary.Data
{
    public interface ITree : IEnumerable<Family> 
    {
        public long Count
        {
            get;
        }

        public int Height
        {
            get;
        }

        public string Name
        {
            get;
        }

        public void Add(ObjectId id, Family parent, Family child);

        public void Clear();

        public bool Contains(Family parent, Family child);

        public int Depth(Family element);

        public IEnumerable<Family> GetChildren(Family element);

        public Family GetParent(Family element);
        public ITree Subtree(Family element);

        public void Update(Family initial, Family final);
    }
}