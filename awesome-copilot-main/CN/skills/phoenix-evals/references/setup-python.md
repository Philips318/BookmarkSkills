#设置：Python

凤凰评估和实验所需的包。

# #安装```bash
# Core Phoenix package (includes client, evals, otel)
pip install arize-phoenix

# Or install individual packages
pip install arize-phoenix-client   # Phoenix client only
pip install arize-phoenix-evals    # Evaluation utilities
pip install arize-phoenix-otel     # OpenTelemetry integration
```
LLM提供商

对于LLM-as-judge评估器，安装提供商的SDK：```bash
pip install openai      # OpenAI
pip install anthropic   # Anthropic
pip install google-generativeai  # Google
```
##验证（可选）```bash
pip install scikit-learn  # For TPR/TNR metrics
```
##快速验证```python
from phoenix.client import Client
from phoenix.evals import LLM, ClassificationEvaluator
from phoenix.otel import register

# All imports should work
print("Phoenix Python setup complete")
```
##键导入（eval 2.0）```python
from phoenix.client import Client
from phoenix.evals import (
    ClassificationEvaluator,      # LLM classification evaluator (preferred)
    LLM,                          # Provider-agnostic LLM wrapper
    async_evaluate_dataframe,     # Batch evaluate a DataFrame (preferred, async)
    evaluate_dataframe,           # Batch evaluate a DataFrame (sync)
    create_evaluator,             # Decorator for code-based evaluators
    create_classifier,            # Factory for LLM classification evaluators
    bind_evaluator,               # Map column names to evaluator params
    Score,                        # Score dataclass
)
from phoenix.evals.utils import to_annotation_dataframe  # Format results for Phoenix annotations
```
** **:`ClassificationEvaluator`优于`create_classifier`（更多parameters/customization）。
**优先选择**:`async_evaluate_dataframe`而不是`evaluate_dataframe`（LLM评估的吞吐量更好）。

**不要使用**旧版本1.0导入：`OpenAIModel`，`AnthropicModel`,`run_evals`,`llm_classify`。