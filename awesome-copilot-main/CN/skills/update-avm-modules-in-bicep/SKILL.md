---
name: update-avm-modules-in-bicep
description: 'Update Azure Verified Modules (AVM) to latest versions in Bicep files.'
---
更新二头肌文件中的Azure验证模块

更新Bicep文件`${file}`以使用最新的Azure验证模块（AVM）版本。将进度更新限制为非破坏性更改。不要输出除最终输出表和摘要以外的信息。

# #过程1. **扫描**：从`${file}`中提取AVM模块和当前版本
1. **Identify**：列出使用`#search`工具匹配`avm/res/{service}/{resource}`所使用的所有唯一AVM模块
1. **检查**：使用`#fetch`工具从MCR:`https://mcr.microsoft.com/v2/bicep/avm/res/{service}/{resource}/tags/list`获取每个AVM模块的最新版本
1. **比较**：解析语义版本以识别需要更新的AVM模块
1. **Review**：对于突破性的更改，使用`#fetch`工具从：`https://github.com/Azure/bicep-registry-modules/tree/main/avm/res/{service}/{resource}`获取文档
1. **更新**：使用`#editFiles`工具进行版本更新和参数更改
1. **验证**：使用`#runCommands`工具运行`bicep lint`和`bicep build`以确保合规。
1. **输出**：以表格格式总结更改，下面是更新摘要。

##工具使用

总是使用工具`#search`，`#searchResults`,`#fetch`,`#editFiles`,`#runCommands`,`#todos`。避免编写代码来执行任务。

打破改变政策

⚠️**暂停审批**如果更新涉及：-不兼容的参数更改
-Security/compliance修改
-行为改变

##输出格式

仅在表格中显示带有图标的结果：```markdown
| Module | Current | Latest | Status | Action | Docs |
|--------|---------|--------|--------|--------|------|
| avm/res/compute/vm | 0.1.0 | 0.2.0 | 🔄 | Updated | [📖](link) |
| avm/res/storage/account | 0.3.0 | 0.3.0 | ✅ | Current | [📖](link) |

### Summary of Updates

Describe updates made, any manual reviews needed or issues encountered.
```
# #图标

-🔄更新
-✅当前
-⚠️需要人工审查
-❌失败
-📖文档

# #要求

-使用MCR标签API仅用于版本发现
-解析JSON标签数组和排序的语义版本
-维护肱二头肌文件的有效性和符合性