using FamilyTreeLibrary.Infrastructure.Resource;

namespace FamilyTreeLibrary.Data.Models
{
    public readonly struct FamilyTreeGraphCredentials(FamilyTreeVault vault)
    {
        private readonly string neo4jAuth = vault["FamilyTreeGraphCredentials"].AsString;

        public string Username
        {
            get
            {
                return neo4jAuth.Split('/')[0];
            }
        }

        public string Password
        {
            get
            {
                return neo4jAuth.Split('/')[1];
            }
        }
    }
}