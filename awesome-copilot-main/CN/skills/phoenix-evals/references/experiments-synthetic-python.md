#实验：生成合成测试数据

为评估创建多样化的、有针对性的测试数据。

基于维度的方法

定义变化轴，然后生成组合：```python
dimensions = {
    "issue_type": ["billing", "technical", "shipping"],
    "customer_mood": ["frustrated", "neutral", "happy"],
    "complexity": ["simple", "moderate", "complex"],
}
```
两步生成

1. **生成元组**（维度值的组合）
2. **转换为自然查询**（每个元组单独的LLM调用）```python
# Step 1: Create tuples
tuples = [
    ("billing", "frustrated", "complex"),
    ("shipping", "neutral", "simple"),
]

# Step 2: Convert to natural query
def tuple_to_query(t):
    prompt = f"""Generate a realistic customer message:
    Issue: {t[0]}, Mood: {t[1]}, Complexity: {t[2]}
    
    Write naturally, include typos if appropriate. Don't be formulaic."""
    return llm(prompt)
```
##目标失效模式

维度应该针对错误分析中的已知故障：```python
# From error analysis findings
dimensions = {
    "timezone": ["EST", "PST", "UTC", "ambiguous"],  # Known failure
    "date_format": ["ISO", "US", "EU", "relative"],   # Known failure
}
```
##质量控制

- **Validate**：检查占位符文本，最小长度
- **重复数据删除**：使用嵌入删除接近重复的查询
- **平衡**：确保跨维度值的覆盖

##何时使用

使用合成|使用真实数据|| ------------- | ------------- |
|生产数据有限|充足追踪|
|测试边缘情况|验证实际行为|
|启动前评估|启动后监控|

##样本大小

|用途|大小|| ------- | ---- |
初步勘探| 50-100 |
|综合评估| 100-500 |
每维|每组合| 10-20