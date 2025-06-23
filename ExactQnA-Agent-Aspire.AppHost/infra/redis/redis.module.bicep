@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param principalId string

resource redis 'Microsoft.Cache/redisEnterprise@2025-04-01' = {
  name: take('redis-${uniqueString(resourceGroup().id)}', 63)
  location: location
  tags: {
    'aspire-resource-name': 'redis'
  }
  sku: {
      name: 'Balanced_B5'
  }
  identity: {
    type:'SystemAssigned'
  }
  properties: {
          minimumTlsVersion: '1.2'
  }
}

resource redisDatabase 'Microsoft.Cache/redisEnterprise/databases@2025-04-01' = {
  name: 'default'
  parent: redis
  properties:{
    clientProtocol: 'Encrypted'
    port: 10000
    clusteringPolicy: 'EnterpriseCluster'
    modules: [
      {
        name: 'RediSearch'
      }
      {
        name: 'RedisJSON'
      }
    ]
    evictionPolicy: 'NoEviction'
    persistence:{
      aofEnabled: false 
      rdbEnabled: false
    }
  }
}


resource redisAccessPolicyAssignmentName 'Microsoft.Cache/redisEnterprise/databases/accessPolicyAssignments@2024-09-01-preview' = {
  name: take('cachecontributor${uniqueString(resourceGroup().id)}', 24)
  parent: redisDatabase
  properties: {
    accessPolicyName: 'default'
    user: {
      objectId: principalId
      }
    }
  }

output connectionString string = '${redis.properties.hostName}:10000,ssl=true'

output name string = redis.name

output redisEndpoint string = '${redis.properties.hostName}:10000'

