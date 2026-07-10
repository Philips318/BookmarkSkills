---
name: phoenix-evals
description: Build and run evaluators for AI/LLM applications using Phoenix.
license: Apache-2.0
compatibility: Requires Phoenix server. Python skills need phoenix and openai packages; TypeScript skills need @arizeai/phoenix-client.
metadata:
  author: oss@arize.com
  version: "1.0.0"
  languages: "Python, TypeScript"
---
#凤凰评估

为AI/LLM应用程序构建评估器。代码优先，LLM为细微差别，对人类进行验证。

##快速参考

|任务|文件|| ---- | ----- |
|设置| [Setup -python](references/setup-python.md), [Setup -typescript](references/setup-typescript.md) |
|决定如何评估|[评估器-概述](references/evaluators-overview.md) |
|选择判断模型| [fundamentals-model-selection](references/fundamentals-model-selection.md) |
使用预构建的评估器|[评估器-预构建](references/evaluators-pre-built.md) |
|构建代码评估器| [evaluators-code-python](references/evaluators-code-python.md), [evaluators-code-typescript](references/evaluators-code-typescript.md) |
构建LLM求值器| [evaluators-llm-python](references/evaluators-llm-python.md), [evaluators-llm-typescript](references/evaluators-llm-typescript.md), [evaluators-custom-templates](references/evaluators-custom-templates.md) |
|批量计算DataFrame | [evaluate- DataFrame -python](references/evaluate-dataframe-python.md) |
|运行实验|[实验-运行-python](references/experiments-running-python.md)，[实验-运行-typescript](references/experiments-running-typescript.md) |
|创建数据集|[实验-数据集-python](references/experiments-datasets-python.md)，[实验-数据集-typescript](references/experiments-datasets-typescript.md) |
|生成合成数据| [experiments-synthetic-python](references/experiments-synthetic-python.md), [experiments-synthetic-typescript](references/experiments-synthetic-typescript.md) |
|验证[validation](references/validation.md), [validation-evaluators-python](references/validation-evaluators-python.md), [validation-evaluators-typescript](references/validation-evaluators-typescript.md) |
| [observe- Sample -python](references/observe-sampling-python.md), [observe- Sample -typescript](references/observe-sampling-typescript.md) |
|分析错误| [error-analysis](references/error-analysis.md), [error-analysis-multi-turn](references/error-analysis-multi-turn.md), [axial-coding](references/axial-coding.md) |
| RAG values | [evaluators-rag](references/evaluators-rag.md) |
避免常见错误| [common- errors -python](references/common-mistakes-python.md), [fundamentals-anti-patterns](references/fundamentals-anti-patterns.md) |
|生产|[生产-概述](references/production-overview.md)，[生产-护栏](references/production-guardrails.md)，[生产-连续](references/production-continuous.md) |# #工作流程

* *开始新鲜:* *
[观察-跟踪-设置]（references/observe-tracing-setup.md）→[错误分析]（references/error-analysis.md）→[轴向编码]（references/axial-coding.md）→[评估器-概述]（references/evaluators-overview.md）

* *建筑评估者:* *
[基本原理]（references/fundamentals.md）→[常见错误-python]（references/common-mistakes-python.md）→求值器-{code|llm}-{python|typescript}→验证-求值器-{python|typescript}

* *破布系统:* *
[evaluators-rag]（references/evaluators-rag.md）→evaluators-code-*（检索）→evaluators-llm-*（忠实）

* *: * *
[生产-概述]（references/production-overview.md）→[生产-护栏]（references/production-guardrails.md）→[生产-连续]（references/production-continuous.md）

##参考类别

|前缀|描述信息|| ------ | ----------- |
|类型，分数，反模式|
|`observe-*`|跟踪，采样|
|`error-analysis-*`|查找失败|
|`axial-coding-*`|故障分类|
|`evaluators-*`|代码，LLM， RAG评估器|
|`experiments-*`|数据集，运行实验|
|`validation-*`|验证评估器对人类标签的准确性|
|`production-*`|CI/CD，监控|

##关键原则

|原理|动作|| --------- | ------ |
|首先进行错误分析|不能自动执行没有观察到的|
自定义>通用|从失败中构建|
|代码首先|确定在LLM |之前
|验证判断| >80%TPR/TNR|
|二进制>李克特|Pass/fail，不是1-5 |