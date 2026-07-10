提交启发式分类

> **在工作流中的作用：**提交消息是一个*次要的*信号。代码差异
>总是首先被阅读，并被视为基本真理。使用这些启发式添加
>的意图和背景之上的差异已经显示-而不是取代它。
>当提交消息与diff相矛盾时，信任diff。

读取`git log`输出时，将每个提交映射到下面的一个类别。
遵循常规提交（https://www.conventionalcommits.org/）的Repos将
有明确的前缀——直接使用它们。对于自由格式的提交消息，请使用
启发式。

---

常规提交前缀→类别

|前缀|类别||---|---|
|`feat:`/`feat(scope):`| feat |
|`fix:`/`fix(scope):`|固定|
|`perf:`| perf |
|`refactor:`|重构|
|`docs:`| docs |
|`chore:`|家务|
|`test:`/`tests:`| test |
|`ci:`|家务|
|`build:`|家务|
|`style:`|家务|
|`revert:`|取决于还原的|
|`BREAKING CHANGE`在页脚或`!`后类型（例如`feat!:`） |打破|

---

自由提交消息启发式

* *打破:* *
-包含单词：*breaking*， *incompatible*, *remove*, *rename*, *drop support*
-短语模式：*不再*，*已删除*，*已删除*，*打破改变*

**壮举（新功能）：**
—以：*add*， * implementation *, *introduce*, *support*， *new*开头
-包含：*现在支持*，*能力到*，*现在可以*

* *修复:* *
—以：*fix*、*patch*、*resolve*、*correct*、*handle*开头
-包含：*bug*， *回归*，*崩溃*，*错误*，*错误*，*不正确*，*破碎** *性能:* *
-包含：*加速*，*更快*，*减少内存*，*优化*，*性能*

* *重构:* *
-包含：*重构*，*清理*，*重组*，*重组*，*简化*，*提取*

* *文档:* *
—包含：*docs*、*readme*、*comment*、*example*、*typo*

* *琐事:* *
-包含：*碰撞*，*升级依赖*，*更新深度*，*版本碰撞*，*ci*, *lint*

* *测试:* *
-包含：*测试*，*规格*，*覆盖*，*夹具*

---

分类合并提交

合并提交（例如，`Merge pull request #42`）通常是噪音。看看PR标题
或者合并中的提交。如果PR标题遵循Conventional Commits，那么就使用它。

---

##当你无法分辨

如果提交看起来像维护，默认为**PATCH**。如果升级为**次要**
没有提到任何新功能。只使用explicit升级到**MAJOR**
突破性变化的证据-不要猜测突破。

---将类别映射到Changelog部分

|类别|变更日志部分||---|---|
|`breaking`+新行为|改变|
|`breaking`+移除|移除|
|`feat`|增加|
|`fix`，`perf`|固定|
|`security`|安全|
|`refactor`,`docs`,`chore`，`test`|忽略（除非用户可见）|

**将先前的内部helper提取到
new public export→将其视为添加，而不是重构。