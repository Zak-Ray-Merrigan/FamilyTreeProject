targetScope = 'subscription'

resource familyTreeProject 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: 'family-tree-project-group'
  location: 'centralus'
}
