@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param aoai_outputs_name string

param principalType string

param principalId string

resource aoai 'Microsoft.CognitiveServices/accounts@2024-10-01' existing = {
  name: aoai_outputs_name
}

resource aoai_CognitiveServicesOpenAIContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(aoai.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'a001fd3d-188f-4b5d-821b-7da978bf7442'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'a001fd3d-188f-4b5d-821b-7da978bf7442')
    principalType: principalType
  }
  scope: aoai
}