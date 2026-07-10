{skill_fallback_guide}

你是一名质量工程师，继续一阶段一阶段地运行质量剧本。阶段1-3完成。

阅读这些文件来获取上下文：
1.quality/PROGRESS.md-运行元数据，阶段状态，BUG跟踪器
2.quality/REQUIREMENTS.md派生需求
3.quality/BUGS.md-第3阶段（代码审查）中发现的bug
4.SKILL.md-阅读第4阶段部分（“第4阶段：规格审核和分类”）。同时阅读references/spec_audit.md.解析SKILL.md和参考/目录通过上面记录的回退列表；不要假设任何单一的安装布局。

执行阶段4：规范审计+分流+第二层语义引用检查。A部分-规范审核：
运行每个quality/RUN_SPEC_AUDIT.md.产品的规格审计：
-个别审计师报告在quality/spec_audits/YYYY-MM-DD-auditor-N.md（每名审计师一份）
-分诊合成在quality/spec_audits/YYYY-MM-DD-triage.md-quality/spec_audits/triage_probes.sh的可执行分类探测
-回归测试和任何新规范审计错误的补丁
-更新BUGS.md和PROGRESS.mdBUG跟踪器与任何新的发现

第2层语义引用检查（v1.5.1）：
门的不变量#17 （schemas.md§10）需要三个议会成员
对每个Tier1/2REQ的citation_摘录进行投票。执行以下步骤：

1. 生成每个理事会成员的提示：     python3 -m bin.quality_playbook semantic-check plan .
将一个或多个提示文件写入
文件中每个成员的quality/council_semantic_check_prompts/<member>.txt
理事会名册(bin/council_config.py: claude-opus-4.7, gpt-5.4，
双子座- 2.5 - pro)。对于> - 15 Tier1/2req，提示符是分批划分的
5 （<member>-batch<N>.txt）。
如果不存在Tier1/2req (Spec Gap run)，则此步骤写入空quality/citation_semantic_check.json直接跳过步骤2-4。

2. 对于每个理事会成员的提示文件，将提示提供给该模型
（与运行Part A的花名册相同）并捕获其json数组响应quality/council_semantic_check_responses/<member>.json。如果
成员已批处理，将每批响应连接到单个
数组中的响应文件。每个条目必须有req_id， verdict
（支持|超过|不清楚），和推理。

3. 组装语义检查输出：     python3 -m bin.quality_playbook semantic-check assemble . \
       --member claude-opus-4.7 --response quality/council_semantic_check_responses/claude-opus-4.7.json \
       --member gpt-5.4         --response quality/council_semantic_check_responses/gpt-5.4.json \
       --member gemini-2.5-pro  --response quality/council_semantic_check_responses/gemini-2.5-pro.json
这将根据schemas.md§9写入quality/citation_semantic_check.json。

4. 验证输出文件是否存在。阶段6的门不变式17要求
它在每层1/2运行。

标记阶段4（规范审计+分类+语义检查）在PROGRESS.md中完成（使用复选框格式`- [x] Phase 4 - Spec Audit`—阶段5入口门查找确切的子字符串，如果找到表行或任何其他布局，将中止）。

重要提示：不要进入第5阶段（调解）。下一阶段将处理协调和TDD。