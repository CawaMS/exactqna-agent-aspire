targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for all deployed resources')
param location string

@description('Id of the user or app to assign application roles')
param principalId string = ''


var tags = {
  'azd-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}
module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
    location: location
    tags: tags
    principalId: principalId
  }
}

module aoai 'aoai/aoai.module.bicep' = {
  name: 'aoai'
  scope: rg
  params: {
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
  }
}
module aoai_roles 'aoai-roles/aoai-roles.module.bicep' = {
  name: 'aoai-roles'
  scope: rg
  params: {
    aoai_outputs_name: aoai.outputs.name
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalType: 'ServicePrincipal'
  }
}
module redis 'redis/redis.module.bicep' = {
  name: 'redis'
  scope: rg
  params: {
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
  }
}


output MANAGED_IDENTITY_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output MANAGED_IDENTITY_NAME string = resources.outputs.MANAGED_IDENTITY_NAME
output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = resources.outputs.AZURE_LOG_ANALYTICS_WORKSPACE_NAME
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = resources.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID
output AZURE_CONTAINER_REGISTRY_NAME string = resources.outputs.AZURE_CONTAINER_REGISTRY_NAME
output AZURE_CONTAINER_APPS_ENVIRONMENT_NAME string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_NAME
output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_ID
output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN
output AOAI_CONNECTIONSTRING string = aoai.outputs.connectionString
output AOAI_CHAT_MODEL_DEPLOYMENT_ID string = aoai.outputs.chatModelDeploymentId
output AOAI_EMBEDDING_MODEL_DEPLOYMENT_ID string = aoai.outputs.embeddingModelDeploymentId
output AOAI_CHAT_MODEL_DEPLOYMENT_NAME string = aoai.outputs.chatModelDeploymentName
output AOAI_KEY_KEYVAULT_SECRET_URI string = aoai.outputs.aoaiKeyKvSecret
output AOAI_NAME string = aoai.outputs.aoaiCustomSubDomainName
output REDIS_CONNECTIONSTRING string = redis.outputs.connectionString
output REDIS_ENDPOINT string = redis.outputs.redisEndpoint
