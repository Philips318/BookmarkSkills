# Azure通用模式（稳定）

此文件仅包含在Azure服务中重复的**近乎不可变的模式**。
此处不包括API版本、SKU和区域等动态信息→参见`azure-dynamic-sources.md`。

---

# # 1。网络隔离模式

私有端点3组件集

所有使用PE的服务都必须配置3个组件集：

1. **专用端点** -放置在pe子网中
2. **私有DNS区域** + **VNet Link** （`registrationEnabled: false`）
3. **DNS区域组** -与PE联动

>如果缺少任何一个，即使PE存在，DNS解析也会失败，导致连接失败。

PE子网必选设置```bicep
resource peSubnet 'Microsoft.Network/virtualNetworks/subnets' = {
  properties: {
    addressPrefix: peSubnetPrefix              // ← CIDR as parameter — prevent existing network conflicts
    privateEndpointNetworkPolicies: 'Disabled'  // ← Required. PE deployment fails without it
  }
}
```
### publicNetworkAccess模式

使用PE的服务必须包括：```bicep
properties: {
  publicNetworkAccess: 'Disabled'
  networkAcls: {
    defaultAction: 'Deny'
  }
}
```
---

# # 2。安全模式

密钥库```bicep
properties: {
  enableRbacAuthorization: true    // Do not use Access Policy method
  enableSoftDelete: true
  softDeleteRetentionInDays: 90
  enablePurgeProtection: true
}
```
###管理身份

当AI服务访问其他资源时：```bicep
identity: {
  type: 'SystemAssigned'  // or 'UserAssigned'
}
```
敏感信息

-使用`@secure()`装饰器
—禁止在`.bicepparam`文件中存储明文
—使用Key Vault引用

---

# # 3。命名约定（基于ca）```
rg-{project}-{env}          Resource Group
vnet-{project}-{env}        Virtual Network
st{project}{env}             Storage Account (no special characters, lowercase+numbers only)
kv-{project}-{env}           Key Vault
srch-{project}-{env}         AI Search
foundry-{project}-{env}      Cognitive Services (Foundry)
```
>防止名称冲突：建议使用`uniqueString(resourceGroup().id)`>“二头肌
> param storageName string = ‘st${uniqueString(resourceGroup().id)}’
> ' ' '

---

# # 4。二头肌模块结构```
<project>/
├── main.bicep              # Orchestration — module calls + parameter passing
├── main.bicepparam         # Environment-specific values (excluding sensitive info)
└── modules/
    ├── network.bicep           # VNet, Subnet
    ├── <service>.bicep         # Per-service modules
    ├── keyvault.bicep          # Key Vault
    └── private-endpoints.bicep # All PE + DNS Zone + VNet Link
```
依赖管理```bicep
// ✅ Correct: Implicit dependency via resource reference
resource project '...' = {
  properties: {
    parentId: foundry.id  // foundry reference → automatically deploys foundry first
  }
}

// ❌ Avoid: Explicit dependsOn (use only when necessary)
```
---

# # 5。PE肱二头肌通用模板```bicep
// ── Private Endpoint ──
resource pe 'Microsoft.Network/privateEndpoints@<fetch>' = {
  name: 'pe-${serviceName}'
  location: location
  properties: {
    subnet: { id: peSubnetId }
    privateLinkServiceConnections: [{
      name: 'pls-${serviceName}'
      properties: {
        privateLinkServiceId: serviceId
        groupIds: ['<groupId>']  // ← Varies by service. See service-gotchas.md
      }
    }]
  }
}

// ── Private DNS Zone ──
resource dnsZone 'Microsoft.Network/privateDnsZones@<fetch>' = {
  name: '<dnsZoneName>'  // ← Varies by service
  location: 'global'
}

// ── VNet Link ──
resource vnetLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@<fetch>' = {
  parent: dnsZone
  name: '${dnsZone.name}-link'
  location: 'global'
  properties: {
    virtualNetwork: { id: vnetId }
    registrationEnabled: false  // ← Must be false
  }
}

// ── DNS Zone Group ──
resource dnsGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@<fetch>' = {
  parent: pe
  name: 'default'
  properties: {
    privateDnsZoneConfigs: [{
      name: 'config'
      properties: { privateDnsZoneId: dnsZone.id }
    }]
  }
}
```
>`@<fetch>`：始终在部署前从MS Docs中验证最新的稳定API版本。