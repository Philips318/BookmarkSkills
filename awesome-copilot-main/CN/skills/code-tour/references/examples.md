#真实世界的代码示例

当您想了解真正的repos如何使用CodeTour特性时，请参考此文件。
每个示例都来自公共GitHub repo，并带有`.tour`文件的直接链接。

---

##microsoft/codetour-贡献者方向

**游览文件：**https://github.com/microsoft/codetour/blob/main/.tours/intro.tour**角色：**新贡献者
**步骤：** ~5·**深度：**标配

**优点：**
-带有嵌入式SVG架构图的介绍步骤（描述中包含原始GitHub URL）
-每一步丰富的标记与表情符号部分标题（`### 🎥 Tour Player`）
-内嵌跨文件链接的描述：`[Gutter decorator](./src/player/decorator.ts)`-使用顶级的`description`字段作为游览本身的副标题

**复制技巧：**在描述中嵌入图像和交叉链接，使其独立。```json
{
  "file": "src/player/index.ts",
  "line": 436,
  "description": "### 🎥 Tour Player\n\nThe CodeTour player ...\n\n![Architecture](https://raw.githubusercontent.com/.../overview.svg)\n\nSee also: [Gutter decorator](./src/player/decorator.ts)"
}
```
---

##a11yproject/a11yproject.com-新贡献者加入

**游览文件：**https://github.com/a11yproject/a11yproject.com/blob/main/.tours/code-tour.tour**角色：**外部贡献者
**步骤：** 26·**深度：**深

**优点：**
-几乎完全`directory`步骤-面向每个`src/`子目录，而不会丢失文件
-对话，初学者友好的语气贯穿始终
-在开始步骤上添加`selection`，以突出显示`package.json`中的确切条目
-以真诚的感谢和行动呼吁结束

**复制技巧：**使用目录步骤作为入门指南的骨架-它们教授结构而不需要作者解释每个文件。```json
{
  "directory": "src/_data",
  "description": "This folder contains the **data files** for the site. Think of them as a lightweight database — YAML files that power the resource listings, posts index, and nav."
}
```
---

##github/codespaces-codeql-技术上最完整的例子

**游览文件：**https://github.com/github/codespaces-codeql/blob/main/.tours/codeql-tutorial.tour角色：安全工程师/概念学习者
**步骤：** 12·**深度：**标准

**优点：**
-`isPrimary: true`-当代码空间打开时自动启动
-`commands`数组在漫游中运行真正的VS Code命令：当读取器到达该步骤时，漫游实际上执行`codeQL.runQuery``view`属性用于切换侧边栏面板（`"view": "codeQLDatabases"`）
-弹性匹配用`pattern`代替`line`:`"pattern": "import tutorial.*"`-`selection`在查询文件中突出显示精确的`select`子句

**这是`commands`、`view`和`pattern`的规范参考```json
{
  "file": "tutorial.ql",
  "pattern": "import tutorial.*",
  "view": "codeQLDatabases",
  "commands": ["codeQL.setDefaultTourDatabase", "codeQL.runQuery"],
  "title": "Run your first query",
  "description": "Click the **▶ Run** button above. The results appear in the CodeQL Query Results panel."
}
```
---

##github/codespaces-learn-with-me-最小交互式教程

**游览文件：**https://github.com/github/codespaces-learn-with-me/blob/main/.tours/main.tour**角色：**完全是新手
**步骤：** 4·**深度：**快速

**优点：**
-只有4个步骤-证明了quick/vibecoder人物角色少即是多
-`isPrimary: true`为自动启动
-每一步都告诉读者做一些事情（编辑字符串，改变颜色）-而不仅仅是阅读
-以一个有形的结果结束：“你的页面是活的”

**复制技巧：**对于quick/vibecoder旅游，无情地剪切。驱动行动的四个步骤胜过解释一切的十二个步骤。

---

##blackgirlbytes/copilot-todo-list- 28步互动教程

**游览文件：**https://github.com/blackgirlbytes/copilot-todo-list/blob/main/.tours/main.tour**角色：**概念学习者/实践教程
**步骤：** 28·**深度：**深**优点：**
-使用**内容检查点步骤**（没有`file`键）作为进度里程碑：“检查您的页面！🎉”和“试试吧！
—描述中的终端内联命令：`>> npm install uuid; npm install styled-components`. xml
-每个文件步骤显示了用户应该接受的确切代码，在一个标记代码围栏，所以他们知道预期的输出

**复制技巧：**检查点步骤（只有内容，里程碑式的标题）打破漫长的旅程，给读者一种进步的感觉。```json
{
  "title": "Check out your page! 🎉",
  "description": "Open the **Simple Browser** tab to see your to-do list. You should see all three tasks rendering from your data array.\n\nOnce you're happy with it, continue to add interactivity."
}
```
---

##lucasjellema/cloudnative-on-oci-2021-多游建筑系列

* *旅游文件:* *- https://github.com/lucasjellema/cloudnative-on-oci-2021/blob/main/.tours/function-tweet-retriever.tour
- https://github.com/lucasjellema/cloudnative-on-oci-2021/blob/main/.tours/oci-and-infrastructure-as-code.tour
- https://github.com/lucasjellema/cloudnative-on-oci-2021/blob/main/.tours/build-and-deployment-pipeline-function-tweet-retriever.tour
角色：**平台工程师/架构师
**步骤：** 12个/次·**深度：**标准

**优点：**
-三个独立的旅行三个独立的关注点（功能代码，IaC，CI/CD管道）-每个独立的，但通过`nextTour`链接
-`selection`坐标在地形文件中大量使用，其中一个块（不是一条线）是点
-步骤包括内联官方OCI文档的降价链接
-设计通过`vscode.dev/github.com/...`不克隆浏览

**复制技巧：**对于复杂的系统，每层写一个tour，然后用`nextTour`链接它们。不要试图在一次旅行中涵盖基础设施+应用程序代码+CI/CD。

---

##SeleniumHQ/selenium- Monorepo构建系统登录

* *旅游文件:* *
-`.tours/bazel.tour`- Bazel工作空间和构建目标方向
-`.tours/building-and-testing-the-python-bindings.tour`- Python绑定BUILD。巴泽尔预排

角色：外部贡献者（构建系统焦点）
**步骤：** ~10次**优点：**
-目标是一个不明显的切入点-不是产品代码，而是构建系统
-证明“贡献者入行”之旅不必从`main()`开始-他们从这个特定的repo中任何令人困惑的地方开始
-用于大型、成熟的OSS项目

---

技术快速参考

|特性|何时使用|实际示例||---------|-------------|-------------|
|`isPrimary: true`|当repo打开时自动启动游览（codesace, vcode .dev） | codespaces-learn-with-me, codespaces-codeql |
|`commands: [...]`|当阅读器到达此步骤时，运行VS Code命令| codespaces-codeql (`codeQL.runQuery`) |
|`view: "terminal"`|在这一步切换VS Codesidebar/panel| codesspaces -codeql (`codeQLDatabases`) |
|`pattern: "regex"`|按行内容匹配，而不是数字-用于易失性文件| codesspaces -codeql |
|`selection: {start, end}`|突出显示块（函数体，配置部分，类型def） | a11yproject, oci-2021，代码空间-codeql |
|`directory: "path/"`|定向到一个文件夹，不读取每个文件| a11yproject， codesspaces -codeql |
|`uri: "https://..."`|链接到PR， issue， RFC， ADR，外部文档|任何PR审查游|
|`nextTour: "Title"`|连锁旅游系列| oci-2021（3部分系列）|
|检查点步骤（仅限内容）|长交互式旅行中的进展里程碑| copilot-todo-list |
|`>> command`in description |终端内联命令链接在VS Code| copilot-todo-list |
|架构图，截图|microsoft/codetour|---

##在GitHub上发现更多真实的旅游

**搜索GitHub上的所有`.tour`文件：**https://github.com/search?q=path%3A**%2F*.tour+&type=code
这个搜索返回提交到公共GitHub仓库的每个`.tour`文件。用它来：
-在同一个language/framework中找到与你正在工作的相同的repos指南
-研究其他作者如何处理相同的人物角色或步骤类型
-查看如何在野外使用特定字段（`commands`,`selection`,`pattern`）

通过语言或关键字过滤来缩小搜索结果——例如在查询中添加`language:TypeScript`或`fastapi`。

---

##进一步阅读

- **DEV社区-“用CodeTour装载你的代码库”**:https://dev.to/tobiastimm/onboard-your-codebase-with-codetour-2jc8- **程序员博客-“使用CodeTour更快地启动新项目”**:https://coder.com/blog/onboard-to-new-projects-faster-with-codetour- **微软技术社区-教育家开发者博客**:https://techcommunity.microsoft.com/blog/educatordeveloperblog/codetour-vscode-extension-allows-you-to-produce-interactive-guides-assessments-a/1274297- **AMIS技术博客- vcode .dev + CodeTour**:https://technology.amis.nl/software-development/visual-studio-code-the-code-tours-extension-for-in-context-and-interactive-readme/- **CodeTour GitHub主题**:https://github.com/topics/codetour