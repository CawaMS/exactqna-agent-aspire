@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource aoai 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: take('aoai-${uniqueString(resourceGroup().id)}', 64)
  location: location
  kind: 'OpenAI'
  properties: {
    customSubDomainName: toLower(take(concat('aoai', uniqueString(resourceGroup().id)), 24))
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: true
  }
  sku: {
    name: 'S0'
  }
  tags: {
    'aspire-resource-name': 'aoai'
  }
}

resource chatModelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  name: 'chatModelDeployment'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4.1'
      version: '2025-04-14'
    }
  }
  sku: {
    name: 'Standard'
    capacity: 8
  }
  parent: aoai
}

resource embeddingModelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  name: 'embeddingModelDeployment'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'text-embedding-3-large'
      version: '1'
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

output connectionString string = 'Endpoint=${aoai.properties.endpoint}'

output name string = aoai.name