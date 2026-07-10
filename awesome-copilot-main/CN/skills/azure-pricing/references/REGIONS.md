# Azure区域名称引用

Azure零售价格API需要小写的`armRegionName`值，不带空格。使用此表将公共区域名称映射到它们的API值。

##区域映射

|显示名称| armRegionName ||-------------|---------------|
|美国东部|`eastus`|
|东US 2 |`eastus2`|
|美国中部|`centralus`|
|美国中北部|`northcentralus`|
美国中南部|`southcentralus`|
|美国中西部|`westcentralus`|
|美国西部|`westus`|
|美国西部2 |`westus2`|
|美国西部3 |`westus3`|
|加拿大中央|`canadacentral`|
|加拿大东部|`canadaeast`|
巴西南部|`brazilsouth`|
|北欧|`northeurope`|
|西欧|`westeurope`|
|英国南部|`uksouth`|
|英国西部|`ukwest`|
|法国中央|`francecentral`|
|法国南部|`francesouth`|
|德国西部中部|`germanywestcentral`|
|德国北部|`germanynorth`|
|瑞士北部|`switzerlandnorth`|
|瑞士西部|`switzerlandwest`|
|挪威东部|`norwayeast`|
|挪威西部|`norwaywest`|
|瑞典中央|`swedencentral`|
|意大利北部|`italynorth`|
|波兰中央|`polandcentral`|
|西班牙中央|`spaincentral`|
|东亚|`eastasia`|
|东南亚|`southeastasia`|
|日本东部|`japaneast`|
|日本西部|`japanwest`|
|澳大利亚东部|`australiaeast`|
|澳大利亚东南|`australiasoutheast`|
|澳大利亚中部|`australiacentral`|
|朝鲜中央|`koreacentral`|
|韩国|`koreasouth`|
|印度中部|`centralindia`|
|印度南部|`southindia`|
|西印度|`westindia`|
|阿联酋北部|`uaenorth`|
|阿联酋中央|`uaecentral`|
|南非北部|`southafricanorth`|
|南非西部|`southafricawest`|
|卡塔尔中央|`qatarcentral`|##转换规则

1. 删除所有空格
2. 转换为小写
3. 例子:
-“East US”→`eastus`-“西欧”→`westeurope`-“东南亚”→`southeastasia`-“美国中南部”→`southcentralus`##常用别名

用户可以非正式地引用区域。将这些映射到正确的`armRegionName`：

|用户说|映射到||-----------|---------|
|“美国东部”，“弗吉尼亚”|`eastus`|
|“美国西部”，“加州”|`westus`|
|“Europe”，“EU”|`westeurope`（默认值）|
|“UK”，“London”|`uksouth`|
|“亚洲”、“新加坡”|`southeastasia`|
|“Japan”，“Tokyo”|`japaneast`|
|“Australia”，“Sydney”|`australiaeast`|
|“印度”，“孟买”|`centralindia`|
|“Korea”，“Seoul”|`koreacentral`|
|“巴西”，“<s:1>圣保罗”|`brazilsouth`|
|“Canada”，“Toronto”|`canadacentral`|
|“德国”，“法兰克福”|`germanywestcentral`|
|“France”，“Paris”|`francecentral`|
|“瑞典”，“斯德哥尔摩”|`swedencentral`|