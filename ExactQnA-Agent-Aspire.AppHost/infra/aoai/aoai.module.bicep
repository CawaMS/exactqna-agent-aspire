@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param keyVaultName string = 'mykv${uniqueString(resourceGroup().id)}'

param principalId string

resource aoai 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: take('aoai-${uniqueString(resourceGroup().id)}', 64)
  location: location
  kind: 'OpenAI'
  properties: {
    customSubDomainName: toLower(take(concat('aoai', uniqueString(resourceGroup().id)), 24))
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: false
  }
  sku: {
    name: 'S0'
  }
  tags: {
    'aspire-resource-name': 'aoai'
  }
}

resource chatModelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2025-04-01-preview' = {
  name: 'chatModelDeployment'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4.1'
      version: '2025-04-14'
    }
  }
  sku: {
    name: 'GlobalStandard'
    capacity: 8
  }
  parent: aoai
}

resource embeddingModelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  name: 'embeddingModelDeployment'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'text-embedding-ada-002'
      version: '2'
    }
  }
  sku: {
    name: 'Standard'
    capacity: 8
  }
  parent: aoai
  dependsOn: [
    chatModelDeployment
  ]
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    enabledForTemplateDeployment: true
    tenantId: tenant().tenantId
    accessPolicies: [
    ]
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
}

resource keyVaultSecret_AoaiKey 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'aoai-key'
  properties: {
    value: aoai.listKeys().key1
  }
}

var keyVaultSecretUserRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7')

resource keyVaultSecretUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVaultSecretUserRoleId)
  scope: keyVault
  properties: {
    principalId: principalId
    roleDefinitionId: keyVaultSecretUserRoleId
    principalType: 'ServicePrincipal'
  }
}

output connectionString string = aoai.properties.endpoint
output chatModelDeploymentId string = chatModelDeployment.name
output embeddingModelDeploymentId string = embeddingModelDeployment.name
output chatModelDeploymentName string = chatModelDeployment.name
output aoaiKeyKvSecret string = keyVaultSecret_AoaiKey.properties.secretUri
output name string = aoai.name
output aoaiCustomSubDomainName string = aoai.properties.customSubDomainName
output aoaiKey string = aoai.listKeys().key1
