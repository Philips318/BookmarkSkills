---
name: react-audit-grep-patterns
description: 'Provides the complete, verified grep scan command library for auditing React codebases before a React 18.3.1 or React 19 upgrade. Use this skill whenever running a migration audit - for both the react18-auditor and react19-auditor agents. Contains every grep pattern needed to find deprecated APIs, removed APIs, unsafe lifecycle methods, batching vulnerabilities, test file issues, dependency conflicts, and React 19 specific removals. Always use this skill when writing audit scan commands - do not rely on memory for grep syntax, especially for the multi-line async setState patterns which require context flags.'
---
# React审计Grep模式

用于React 18.3.1和React 19迁移审计的完整扫描命令库。

# #使用

阅读你的目标的相关部分：
- **`references/react18-scans.md`** -所有扫描React16/17→18.3.1审计
- **`references/react19-scans.md`** -所有扫描React 18→19审计
**`references/test-scans.md`** -测试文件特定扫描（由两个审计器使用）
- **`references/dep-scans.md`** -依赖和对等冲突扫描

在所有扫描中使用的基本模式```bash
# Standard flags used throughout:
# -r = recursive
# -n = show line numbers
# -l = show filenames only (for counting affected files)
# --include="*.js" --include="*.jsx" = JS/JSX files only
# | grep -v "\.test\.\|\.spec\.\|__tests__" = exclude test files
# | grep -v "node_modules" = safety (usually handled by not scanning node_modules)
# 2>/dev/null = suppress "no files found" errors

# Source files only (exclude tests):
SRC_FLAGS='--include="*.js" --include="*.jsx"'
EXCLUDE_TESTS='grep -v "\.test\.\|\.spec\.\|__tests__"'

# Test files only:
TEST_FLAGS='--include="*.test.js" --include="*.test.jsx" --include="*.spec.js" --include="*.spec.jsx"'
```
