#验证

在部署之前，根据人类标签验证LLM判断。目标>80%协议。

# #要求

|需求|目标|| ----------- | ------ |
|测试集大小| 100+示例|
|平衡| ~50/50pass/fail|
|准确度| >80% |
|TPR/TNR| >70% |

# #指标

|公制|公式|使用时|| ------ | ------- | -------- |
| **准确度** | (TP+TN) /总|一般|
| **TPR（召回）** | TP / (TP+FN) |质量保证|
| **TNR（特异性）** | TN / (TN+FP) |安全临界|
| **科恩Kappa** |超越机会的协议|比较评估者|

##快速验证```python
from sklearn.metrics import classification_report, confusion_matrix, cohen_kappa_score

print(classification_report(human_labels, evaluator_predictions))
print(f"Kappa: {cohen_kappa_score(human_labels, evaluator_predictions):.3f}")

# Get TPR/TNR
cm = confusion_matrix(human_labels, evaluator_predictions)
tn, fp, fn, tp = cm.ravel()
tpr = tp / (tp + fn)
tnr = tn / (tn + fp)
```
黄金数据集结构```python
golden_example = {
    "input": "What is the capital of France?",
    "output": "Paris is the capital.",
    "ground_truth_label": "correct",
}
```
构建黄金数据集

1. 样品生产轨迹（错误、负反馈、边缘情况）
2. 平衡~50/50pass/fail3. 专家给每个例子贴上标签
4. 版本数据集（从不修改现有数据集）```python
# GOOD - create new version
golden_v2 = golden_v1 + [new_examples]

# BAD - never modify existing
golden_v1.append(new_example)
```
##警告标志

—全部通过或全部失败→toolenient/strict-随机结果→标准不明确
-TPR/TNR< 70%→需要改进

##重新验证何时

-提示模板更改
-判断模型的变化
-准则更改
——每月