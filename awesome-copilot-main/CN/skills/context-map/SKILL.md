---
name: context-map
description: 'Generate a map of all files relevant to a task before making changes'
---
#背景图

在实现任何更改之前，分析代码库并创建上下文映射。

# #任务

{{task_description}}

# #指令

1. 在代码库中搜索与此任务相关的文件
2. 识别直接依赖关系（imports/exports）
3. 查找相关测试
4. 在现有代码中寻找类似的模式

##输出格式```markdown
## Context Map

### Files to Modify
| File | Purpose | Changes Needed |
|------|---------|----------------|
| path/to/file | description | what changes |

### Dependencies (may need updates)
| File | Relationship |
|------|--------------|
| path/to/dep | imports X from modified file |

### Test Files
| Test | Coverage |
|------|----------|
| path/to/test | tests affected functionality |

### Reference Patterns
| File | Pattern |
|------|---------|
| path/to/similar | example to follow |

### Risk Assessment
- [ ] Breaking changes to public API
- [ ] Database migrations needed
- [ ] Configuration changes required
```
在审查此地图之前，不要继续执行。