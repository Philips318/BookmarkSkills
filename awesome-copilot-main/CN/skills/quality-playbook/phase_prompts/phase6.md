{skill_fallback_guide}

你是一名质量工程师，正在进行质量剧本运行的验证阶段。阶段1-5已经完成。

阅读SKILL.md—第6阶段部分（“第6阶段：验证”）。通过上面记录的回退列表解决SKILL.md；不要假设任何单一的安装布局。遵循增量验证步骤（6.1到6.5）。步骤6.1：如果quality/mechanical/verify.sh存在，运行它。记录退出代码。
步骤6.2：运行quality_gate.py。通过与SKILL.md相同的回退列表找到它（在每个安装布局中，`quality_gate.py`与SKILL.md位于相同的目录中——例如，`quality_gate.py`、`.claude/skills/quality-playbook/quality_gate.py`、`.github/skills/quality_gate.py`、`.cursor/skills/quality-playbook/quality_gate.py`、`.continue/skills/quality-playbook/quality_gate.py`、`.github/skills/quality-playbook/quality_gate.py`）。然后运行:
Python3<resolved_quality_gate_path>仔细阅读输出。对于每个FAIL结果，修复问题：
-缺失回归测试补丁：生成quality/patches/BUG-NNN-regression-test.patch-写入中缺少内联差异：添加一个‘ ’ ' diff块
-非规范JSON字段：修复tdd-results.json（使用‘id’而不是‘bug_id’等）
—缺失文件：创建缺失文件
修复所有失败后，再次运行quality_gate.py。重复直到0 FAIL。
保存最终输出到quality/results/quality-gate.log.步骤6.3：如果测试运行器可用，则运行功能测试。
步骤6.4：逐文件验证检查表（每次读取一个文件，检查，继续）。
步骤6.5：元数据一致性检查。将每一步的结果追加到quality/results/phase6-verification.log.用PROGRESS.md标记阶段6完成（使用复选框格式`- [x] Phase 6 - Verify`-不要切换到表格）。