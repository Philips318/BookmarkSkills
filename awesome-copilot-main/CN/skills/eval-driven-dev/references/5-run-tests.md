#步骤5：运行`pixie test`和修复机械问题

**为什么此步骤**：运行`pixie test`并修复QA组件中的机械问题-数据集格式问题，可运行的实现错误和自定义评估器错误-直到每个条目产生实际分数。这一步不是评估结果质量或修复应用程序本身。

---

# # 5。运行测试```bash
uv run pixie test
```
对于带有每个案例分数和求值器推理的详细输出：```bash
uv run pixie test -v
```
`pixie test`在运行测试之前自动加载`.env`文件。

评估工具：

1. 从数据集的`runnable`字段解析`Runnable`类
2. 调用`Runnable.create()`来构造一个实例，然后调用`setup()`一次
3. 运行所有数据集条目**并发**（最多4个并行）：
a.从条目中读取`input_data`和`eval_input`b.用`eval_input`数据填充包装输入注册表
c.初始化捕获注册表
d.在Pydantic模型中验证`input_data`，并调用`Runnable.run(args)`E.`wrap(purpose="input")`调用在应用程序返回注册表值，而不是调用外部服务
F.`wrap(purpose="output"/"state")`调用捕获数据进行评估
g.从捕获的数据构建`Evaluable`h.运行评估器
4. 调用`Runnable.teardown()`一次因为条目是并发运行的，所以Runnable的`run()`方法必须是并发安全的。如果您看到`sqlite3.OperationalError`、`"database is locked"`或类似的错误，请将`Semaphore(1)`添加到您的Runnable中（请参阅参考资料第2步中的并发部分）。

# # 5 b。只修复机械问题

这一步严格意义上是修复您在前面步骤中构建的内容——数据集、可运行程序和任何自定义的求值器。您正在修复阻止管道运行的机械问题，而不是评估或改进应用程序的输出质量。

**什么算机械问题**（修复这些）：

|错误|原因|修复|| ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
|`WrapRegistryMissError: name='<key>'`|数据集条目缺少`eval_input`项，其中`name`与应用程序的`wrap(purpose="input", name="<key>")`期望的`name`|在每个受影响的条目|中添加缺失的`{"name": "<key>", "value": ...}`到`eval_input`|`WrapTypeMismatchError`|反序列化类型与应用程序期望的不匹配|修复数据集|中的值
|`runnable`路径或类名错误，或类未实现`Runnable`协议|修复数据集中的`filepath:ClassName`；确保类具有`create()`和`run()`方法|
|导入错误|runnable/evaluator模块路径或语法错误|修复引用文件|
|`ModuleNotFoundError: pixie_qa`|`pixie_qa/`目录丢失`__init__.py`|运行`pixie init`重建|
|`TypeError: ... is not callable`|求值器名称指向不可调用的属性|求值器必须是函数、类或可调用的实例|
|`sqlite3.OperationalError`|并发`run()`调用共享一个SQLite连接|将`asyncio.Semaphore(1)`添加到Runnable（参见步骤2并发部分）|
|自定义求值器崩溃|自定义求值器实现中的Bug |修复求值器代码|**什么不是机械问题**（不要在这里修复这些）：

-应用程序产生wrong/low-quality输出→这是应用程序的行为，在步骤6中分析
-评估者得分低→这是一个质量信号，在步骤6中分析
- LLM调用在应用程序内失败→在步骤6中报告，不要嘲笑或工作
-评估器得分在运行之间波动→正常的LLM不确定性，而不是错误

迭代—修复错误，重新运行，修复下一个错误—直到`pixie test`运行完成，并为所有条目提供真实的求值器分数。

# #输出

在`pixie test`成功完成后，结果存储在每个条目的目录结构中：```
{PIXIE_ROOT}/results/<test_id>/
  meta.json                           # test run metadata
  dataset-{idx}/
    metadata.json                     # dataset name, path, runnable
    entry-{idx}/
      config.json                     # evaluators, description, expectation
      eval-input.jsonl                # input data fed to evaluators
      eval-output.jsonl               # output data captured from app
      evaluations.jsonl               # evaluation results (scored + pending)
      trace.jsonl                     # LLM call traces (if captured)
```
在控制台输出中打印`<test_id>`。您将在步骤6中引用该目录。

---

b> **如果在运行测试时遇到意外错误**（参数名称错误、导入失败、API不匹配），请在猜测修复之前读取`wrap-api.md`、`evaluators.md`或`testing-api.md`以获取权威API引用。