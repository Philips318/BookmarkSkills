---
name: what-context-needed
description: 'Ask Copilot what files it needs to see before answering a question'
---
你需要什么样的背景？

在回答我的问题之前，告诉我你需要看哪些文件。

##我的问题

{{问题}}

# #指令

1. 根据我的问题，列出你需要检查的文件
2. 解释为什么每个文件都是相关的
3. 注意在此对话中已经看到的所有文件
4. 确定你不确定的是什么

##输出格式```markdown
## Files I Need

### Must See (required for accurate answer)
- `path/to/file.ts` — [why needed]

### Should See (helpful for complete answer)
- `path/to/file.ts` — [why helpful]

### Already Have
- `path/to/file.ts` — [from earlier in conversation]

### Uncertainties
- [What I'm not sure about without seeing the code]
```
我提供这些文件后，我会再问我的问题。