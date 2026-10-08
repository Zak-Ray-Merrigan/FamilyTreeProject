targetScope = 'resourceGroup'

param cosmosAccountName string = 'vfm-cosmos-main'
param personContainerName string = 'person'
param familyDynamicContainerName string = 'familyDynamic'

resource familyCosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2026-03-15' = {
  kind: 'GlobalDocumentDB'
  location: resourceGroup().location
  name: cosmosAccountName
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 240
        backupRetentionIntervalInHours: 8
        backupStorageRedundancy: 'Geo'
      }
    }
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    createMode: 'Default'
    databaseAccountOfferType: 'Standard'
    disableLocalAuth: true
    enableAnalyticalStorage: false
    enableAutomaticFailover: false
    enableBurstCapacity: false
    enableCassandraConnector: false
    enableFreeTier: false
    enableMultipleWriteLocations: false
    enablePartitionMerge: false
    enablePerRegionPerPartitionAutoscale: false
    enablePriorityBasedExecution: false
    enforceHierarchicalPartitionKeyIdLastLevel: false
    ipRules: []
    isVirtualNetworkFilterEnabled: false
    locations: [
      {
        failoverPriority: 0
        isZoneRedundant: false
        locationName: resourceGroup().location
      }
    ]
    minimalTlsVersion: 'Tls12'
    networkAclBypass: 'None'
    networkAclBypassResourceIds: []
    publicNetworkAccess: 'Enabled'
    virtualNetworkRules: []
  }
}

resource familyNoSQLCosmos 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2026-03-15' = {
  name: 'vfm'
  parent: familyCosmosAccount
  properties: {
    options: {}
    resource: {
      id: 'vfm'
    }
  }
}

resource personContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2026-03-15' = {
  name: personContainerName
  location: resourceGroup().location
  parent: familyNoSQLCosmos
  properties: {
    resource: {
      defaultTtl: -1
      id: personContainerName
      indexingPolicy: {
        automatic: true
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
        includedPaths: [
          {
            path: '/*'
          }
        ]
        indexingMode: 'consistent'
      }
      partitionKey: {
        kind: 'Hash'
        paths: [
          '/birthFamilyName'
        ]
        version: 2
      }
    }
  }
}

resource familyDynamicContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2026-03-15' = {
  name: familyDynamicContainerName
  parent: familyNoSQLCosmos
  location: resourceGroup().location
  properties: {
    resource: {
      defaultTtl: -1
      id: familyDynamicContainerName
      indexingPolicy: {
        automatic: true
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
        includedPaths: [
          {
            path: '/*'
          }
        ]
        indexingMode: 'consistent'
      }
      partitionKey: {
        kind: 'Hash'
        paths: [
          '/id'
        ]
        version: 2
      }
    }
  }
}
