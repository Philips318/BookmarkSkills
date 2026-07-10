#自学系统

在指定的lessons/memories目录下维护项目学习工件（例如，`.github/SecurityLessons`和`.github/SecurityMemories`）。

##何时创建

# # #的教训

创建一个课程，当：

-扫描产生假阳性，需要手动校正
-发现类别，跨步类别或缺陷类型在第一次通过时被遗漏，并被自我批评循环捕获
-发现工具或方法的局限性
-特定于语言的规则失效
—无法解析SCA依赖项

# # #内存

当：

发现架构决策、安全约定或技术堆栈细节
—确定依赖关系管理模式、特定于域的威胁模式或威胁参与者配置文件
-发现项目编码约定、框架习惯或已知的误报模式
任何特定于代码库的知识对于将来扫描相同的代码库都是有用的##课程模板```markdown
# Security Lesson: <short-title>

## Metadata

- CreatedAt: <date>
- Status: active | deprecated
- Supersedes: <previous lesson if any>

## Context

- Triggering scan/task:
- Component analyzed:

## Issue

- What went wrong or was missed:
- Expected behavior:
- Actual behavior:

## Root Cause

- Why was this missed or incorrect:

## Resolution

- How it was corrected:

## Preventive Guidance

- How to avoid this in future scans:
```
##内存模板```markdown
# Security Memory: <short-title>

## Metadata

- CreatedAt: <date>
- Status: active | deprecated
- Supersedes: <previous memory if any>

## Context

- Triggering scan/task:
- Scope/system:

## Key Fact

- What was discovered:
- Why it matters for security analysis:

## Reuse Guidance

- When to apply this knowledge:
- Related components:
```
##治理规则

1. **删除检查**：在创建新课程或记忆之前，搜索现有文件以查找类似内容。更新现有记录，而不是创建重复记录。
2. **冲突解决**：如果新的证据与现有的活动lesson/memory冲突，将旧的证据标记为`deprecated`，并使用`Supersedes`引用创建更新版本。
3. **在扫描开始时重用**：在每次分析开始时，检查lessons/memories目录以查找适用的上下文。在开始分析之前应用相关指导。