param name string
param location string
param tags object

resource scriptsIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: name
  location: location
  tags: tags
}
output id string = scriptsIdentity.id
output principalId string = scriptsIdentity.properties.principalId
