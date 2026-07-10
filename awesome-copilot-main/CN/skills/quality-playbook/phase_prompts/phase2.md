{skill_fallback_guide}

你是一名质量工程师，继续一阶段一阶段地运行质量剧本。第一阶段（勘探）已经完成。

阅读这些文件来获取上下文：
1.quality/EXPLORATION.md—您的第一阶段发现（需求、风险、体系结构）
2.quality/PROGRESS.md-运行元数据和阶段状态
3.SKILL.md-阅读阶段2部分（从“阶段2：生成质量手册”到“检查点：在工件生成后更新PROGRESS.md”部分）。还要阅读该节中引用的参考文件。通过上面的备份列表解析SKILL.md和参考文件；不要假设任何单一的安装布局（`.github/skills/`、`.claude/skills/quality-playbook/`、`.cursor/skills/quality-playbook/`、`.continue/skills/quality-playbook/`或root）。**当将REQ假设从EXPLORATION.md转录到`quality/REQUIREMENTS.md`和`quality/requirements_manifest.json`时，源假设上存在的每个`- Pattern: <value>`字段必须出现在两个输出文件的相应REQ上。模式值为`whitelist | parity | compensation`。阶段1的笛卡尔UC规则（确认清单项6）要求对两个UC门匹配的每个REQ进行模式标记；阶段2一定不能默默地放弃这些标签。如果假设缺乏Pattern，但您认为它应该有一个（使用`UC-N.a`/`UC-N.b`后缀发出的每个站点UCs，多文件`References`表示并行结构），请在第2阶段添加Pattern -不要省略该字段。第5阶段基数门不能强制覆盖它不知道是模式标记的REQ；无声省略是一个文档化的v1.4.5回归向量。执行阶段2：生成所有质量工件。使用EXPLORATION.md中的探索结果作为源代码—不要从头开始重新探索代码库。生成:
-quality/QUALITY.md（质量构成）
-quality/CONTRACTS.md-quality/REQUIREMENTS.md（带有来自EXPLORATION.md的REQ-NNN和UC-NN标识符）- quality/COVERAGE_MATRIX.md
-功能测试（quality/test_functional.*）
-quality/RUN_CODE_REVIEW.md（代码审查协议）
-quality/RUN_INTEGRATION_TESTS.md（集成测试协议）
-quality/RUN_SPEC_AUDIT.md（规范审核协议）
-quality/RUN_TDD_TESTS.md（TDD验证协议）
-quality/COMPLETENESS_REPORT.md（基线，无判决）
-如果存在dispatch/enumeration合约：quality/mechanical/带有verify.sh和提取工件。立即运行verify.sh并保存收据。

更新PROGRESS.md：标记阶段2完成（使用复选框格式`- [x] Phase 2 - Generate`-不要切换到表格），更新工件清单。

重要：不要进入第三阶段（代码审查）。您的工作只是生成工件。下一阶段将执行您生成的评审协议。