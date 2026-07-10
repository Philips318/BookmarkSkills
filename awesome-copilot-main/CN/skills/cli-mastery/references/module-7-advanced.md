模块7：高级技术

1. **`@`文件提到** -总是给出精确的上下文，不要依赖AI查找文件
—`@src/auth.ts`—单个文件
-`@src/components/`-目录列表
-“修复@src/auth.ts匹配@tests/auth.test.ts”-多文件上下文

2. **`! shell bypass`** -`!git log --oneline -5`立即运行，没有AI开销

3. **`/research`** -使用GitHub搜索和web资源进行深入的研究调查

4. **`/resume`+`--continue`** -跨CLI启动的会话连续性

5. **`/compact`** -当上下文变大时压缩历史记录（自动设置为95%）
-先检查`/context`-最适合用于自然任务边界
警告信号：AI与之前的陈述相矛盾，代币使用率>80%

6. **`/context`** -可视化什么在消耗你的代币预算

7. **自定义指令优先级**（从高到低）：
-`CLAUDE.md`/`GEMINI.md`/`AGENTS.md`（git root + cwd）
-`.github/instructions/**/*.instructions.md`（路径特定！）   - `.github/copilot-instructions.md`
——`~/.copilot/copilot-instructions.md`-`COPILOT_CUSTOM_INSTRUCTIONS_DIRS`（通过env var添加目录）

8. * *于路径指令:* *
-`.github/instructions/backend.instructions.md`和`applyTo: "src/api/**"`-针对代码库的不同部分采用不同的编码标准

9. **LSP配置** -`~/.copilot/lsp-config.json`或`.github/lsp.json`10. **`/review`** -无需离开终端即可获得代码审查

11. **`--allow-all`/`--yolo`** -完全信任模式（负责任使用！）

12. **`Ctrl+T`** -观看AI思考（学习其推理模式）