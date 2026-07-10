# phase_prompts /

质量手册的外化阶段提示体。

v1.5.4 F-1 （Bootstrap_Findings 2026-04-30）从`bin/run_playbook.py`的内联字符串模板，所以两者都执行
模式- ui -上下文技能-直接(一个编码代理走过SKILL.md内联)和cli自动化运行程序驱动(' python -m
Bin.run_playbook ') -从同一个真理来源阅读。
如果没有外化，这两种模式就会漂移；有了它，编辑到a
阶段提示登陆一次，双方都受益。

##文件布局

-`phase1.md`…`phase6.md`-每个管道阶段一个文件。加载`bin/run_playbook.py::_load_phase_prompt`。
-`single_pass.md`-遗留的单提示调用(用于
作业者希望LLM能够直列驱动所有六个阶段
而不是通过每个阶段的编排器)。
-`iteration.md`-迭代策略提示符(间隙，未过滤，
奇偶校验，对抗性-参见`bin/run_playbook.py::next_strategy`)。

##替换约定大多数文件都是纯文字标记-加载器返回它们
不变。三个文件使用`str.format()`替换命名
占位符:

-`phase1.md`-`{seed_instruction}`(跳过阶段0/0b前奏时`--no-seeds`)和`{role_taxonomy}`(从`bin.role_map.ROLE_DESCRIPTIONS`)。
—`single_pass.md`—`{skill_fallback_guide}`和`{seed_instruction}`。
—`iteration.md`—`{skill_fallback_guide}`和`{strategy}`。

在通过`.format()`， JSON大括号和其他
文字`{`/`}`字符必须加倍（`{{`/`}}`）
Python的格式字符串转义规则。纯文字文件则不然
需要任何逃避。

编辑纪律

当您更改一个阶段提示时，加载器会选择新的内容
在下一次调用时，没有缓存层要使其失效。这个
测试套件在`bin/tests/test_phase_prompts_externalized.py`装载机的合同;如果添加新的替换变量，请扩展
这些测试。