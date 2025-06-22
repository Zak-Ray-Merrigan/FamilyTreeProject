using FamilyTreeLibrary.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;

namespace FamilyTreeLibrary.Data.Enumerators
{
    public class FamilyEnumerator : IEnumerator<Family>
    {
        private readonly FamilyNode root;
        private readonly IMongoCollection<BsonDocument> mongoCollection;
        private FamilyNode current;

        private Stack<Queue<FamilyNode>> familyNodeCollection;

        public FamilyEnumerator(FamilyNode root, IMongoCollection<BsonDocument> mongoCollection)
        {
            this.root = root;
            this.mongoCollection = mongoCollection;
            Reset();
        }

        public Family Current
        {
            get
            {
                return current.Element;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            familyNodeCollection = null;
            current = null;
        }

        public bool MoveNext()
        {
            if (familyNodeCollection.Count == 0)
            {
                return false;
            }
            Queue<FamilyNode> families = familyNodeCollection.Pop();
            current = families.Dequeue();
            if (families.Any())
            {
                familyNodeCollection.Push(families);
            }
            Queue<FamilyNode> children = new();
            IEnumerable<FamilyNode> childNodes = DataUtils.GetChildrenOf(current, mongoCollection);
            foreach (FamilyNode child in childNodes)
            {
                children.Enqueue(child);
            }
            if (children.Any())
            {
                familyNodeCollection.Push(children);
            }
            return true;
        }

        public void Reset()
        {
            familyNodeCollection = new();
            Queue<FamilyNode> initial = new();
            if (root is not null)
            {
                initial.Enqueue(root);
                familyNodeCollection.Push(initial);
            }
        }
    }
}