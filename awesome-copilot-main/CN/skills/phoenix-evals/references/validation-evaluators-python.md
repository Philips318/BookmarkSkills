#验证求值器（Python）

根据人工标记的示例验证LLM评估器。目标>80%TPR/TNR/Accuracy.##计算指标```python
from sklearn.metrics import classification_report, confusion_matrix

print(classification_report(human_labels, evaluator_predictions))

cm = confusion_matrix(human_labels, evaluator_predictions)
tn, fp, fn, tp = cm.ravel()
tpr = tp / (tp + fn)
tnr = tn / (tn + fp)
print(f"TPR: {tpr:.2f}, TNR: {tnr:.2f}")
```
正确的生产估算```python
def correct_estimate(observed, tpr, tnr):
    """Adjust observed pass rate using known TPR/TNR."""
    return (observed - (1 - tnr)) / (tpr - (1 - tnr))
```
##发现错误分类```python
# False Positives: Evaluator pass, human fail
fp_mask = (evaluator_predictions == 1) & (human_labels == 0)
false_positives = dataset[fp_mask]

# False Negatives: Evaluator fail, human pass
fn_mask = (evaluator_predictions == 0) & (human_labels == 1)
false_negatives = dataset[fn_mask]
```
##红旗

—TPR或TNR < 70%
- TPR与TNR差距较大
- Kappa < 0.6