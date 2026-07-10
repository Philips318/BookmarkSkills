---
name: sponsor-finder
description: Find which of a GitHub repository's dependencies are sponsorable via GitHub Sponsors. Uses deps.dev API for dependency resolution across npm, PyPI, Cargo, Go, RubyGems, Maven, and NuGet. Checks npm funding metadata, FUNDING.yml files, and web search. Verifies every link. Shows direct and transitive dependencies with OSSF Scorecard health data. Invoke with /sponsor followed by a GitHub owner/repo (e.g. "/sponsor expressjs/express").
---
#赞助商查找器

发现支持项目依赖项背后的开源维护者的机会。接受GitHub`owner/repo`（例如`/sponsor expressjs/express`），使用deep .dev API进行依赖项解析和项目健康数据，并生成友好的赞助报告，涵盖直接和传递依赖项。

你的工作流程

当用户输入`/sponsor {owner/repo}`或提供`owner/repo`格式的存储库时：1. **解析输入** -提取`owner`和`repo`。
2. **检测生态系统** -获取清单以确定包名+版本。
3. **获得完整的依赖树** - deep .dev`GetDependencies`（一次调用）。
4. **解决repos** - deep .dev`GetVersion`为每个深度→`relatedProjects`给GitHub的repo。
5. **获取项目健康** - depth .dev`GetProject`获取唯一的repos→OSSF记分卡。
6. **找到资金链接** - npm`funding`字段，FUNDING.yml，网页搜索回退。
7. **验证每个链接** -获取每个URL，以确认它是活的。
8. **分组和报告** -按资助目的地，按影响排序。

---

##步骤1：检测生态系统和包装

使用`get_file_contents`从目标仓库中获取清单。确定生态系统并提取包名+最新版本：

|文件|生态系统| |的包名|的版本|------|-----------|-------------------|--------------|
|`package.json`| NPM |`name`字段|`version`字段|
|`requirements.txt`| PYPI |包名列表|使用最新（省略deep .dev调用中的版本）|
|`pyproject.toml`| PYPI |`[project.dependencies]`|使用最新的|
|`Cargo.toml`| CARGO |`[package] name`|`[package] version`|
|`go.mod`| GO |`module`path | from GO国防部|
|`Gemfile`| RUBYGEMS | gem名称|使用最新的|
|`pom.xml`| MAVEN |`groupId:artifactId`|`version`|

---

##第二步：获取完整的依赖树（deep .dev）

这是关键的一步。**使用`web_fetch`来调用deep .dev API：```
https://api.deps.dev/v3/systems/{ECOSYSTEM}/packages/{PACKAGE}/versions/{VERSION}:dependencies
```
例如:```
https://api.deps.dev/v3/systems/npm/packages/express/versions/5.2.1:dependencies
```
这将返回一个`nodes`数组，其中每个节点具有：
—`versionKey.name`—包名
-`versionKey.version`-已解析版本
-`relation`-`"SELF"`、`"DIRECT"`、`"INDIRECT"`**这个调用给你整个依赖树**——包括直接的和传递的——以及精确解析的版本。不需要解析lockfiles。

### URL编码
包含特殊字符的包名必须使用百分比编码：
-`@colors/colors`→`%40colors%2Fcolors`—将`@`编码为`%40`，`/`编码为`%2F`对于没有根包的repos
如果repo没有发布包（例如，它是一个应用程序而不是一个库），则直接读取`package.json`依赖项并为每个依赖项调用deep .dev`GetVersion`。

---

##步骤3：解析每个依赖到GitHub的Repo （deep .dev）

对于树中的每个依赖项，调用depth .dev`GetVersion`：```
https://api.deps.dev/v3/systems/{ECOSYSTEM}/packages/{NAME}/versions/{VERSION}
```
从回复中，摘录如下：
- **`relatedProjects`**→查找`relationType: "SOURCE_REPO"`→`projectKey.id`给出`github.com/{owner}/{repo}`**`links`**→查找`label: "SOURCE_REPO"`→`url`字段

这适用于所有生态系统——npm、PyPI、Cargo、Go、RubyGems、Maven、NuGet——具有相同的字段结构。

效率规则
-每次处理**10个批次**。
-重复数据删除-多个包可以映射到相同的仓库。
-跳过深度，没有找到GitHub项目（计数为“无法解决”）。

---

##步骤4：获取项目运行状况数据（depth .dev）

对于每个唯一的GitHub repo，调用depth .dev`GetProject`：```
https://api.deps.dev/v3/projects/github.com%2F{owner}%2F{repo}
```
从回复中，摘录如下：
- **`scorecard.checks`**→查找`"Maintained"`检查→`score`（0-10）
- **`starsCount`** -人气指标
- **`license`** -工程许可证
- **`openIssuesCount`** -活动指示器

使用已维护分数标记项目运行状况：
-比分7-10→⭐积极维护
-得分4-6→⚠️部分维护
- 0-3→💤可能没有维护

效率规则
-只获取**唯一的repos**（不是每个包）。
-每次处理**10个批次**。
—此步骤是可选的—如果速率有限，请跳过，并在输出中注明。

---

##第五步：寻找融资链接

对于每个唯一的GitHub回购，按顺序使用三个来源检查资金信息：

### 5a: NPM`funding`字段（仅限NPM生态系统）
在`https://registry.npmjs.org/{package-name}/latest`上使用`web_fetch`，并检查`funding`字段：
- **字符串：**`"https://github.com/sponsors/sindresorhus"`→用作URL
**对象：**`{"type": "opencollective", "url": "https://opencollective.com/express"}`→使用`url`—**数组：**收集所有url### 5b:`.github/FUNDING.yml`（回购级，然后是组织级回退）

**步骤5b-i -每个回购检查：**
使用`get_file_contents`获取`{owner}/{repo}`路径`.github/FUNDING.yml`。

**步骤5b-ii -Org/user-level回退：**
如果5b-i返回404（在repo本身中没有FUNDING.yml），请检查所有者的默认社区运行状况repo：
使用`get_file_contents`获取`{owner}/.github`路径`FUNDING.yml`。

GitHub支持[默认社区健康文件]（https://docs.github.com/en/communities/setting-up-your-project-for-healthy-contributions/creating-a-default-community-health-file）约定：user/org级别的`.github`存储库为所有没有自己的仓库提供默认值。例如，`isaacs/.github/FUNDING.yml`适用于所有`isaacs/*`的仓库。

只查找每个唯一的`{owner}/.github`回购**一次** -重用该所有者下的所有回购的结果。一次批量处理**10个业主**。

解析YAML （5b-i和5b-ii相同）：
-`github: [username]`→`https://github.com/sponsors/{username}`-`open_collective: slug`→`https://opencollective.com/{slug}`-`ko_fi: username`→`https://ko-fi.com/{username}`-`patreon: username`→`https://patreon.com/{username}`-`tidelift: platform/package`→`https://tidelift.com/subscription/pkg/{platform-package}`-`custom: [urls]`→按原样使用### 5c: Web搜索回退
对于**前10个未资助的依赖**（根据可传递依赖的数量），使用`web_search`：```
"{package name}" github sponsors OR open collective OR funding
```
跳过已知由公司维护的包（React/Meta、TypeScript/Microsoft、@types/DefinitelyTyped）。

效率规则
- **检查所有深度的5a和5b。**只对顶级无资金项目使用5c。
-跳过非npm生态系统的npm注册表调用。
-重复删除-每个重复只检查一次。
- **一个`{owner}/.github`检查每个唯一的所有者** -重用所有他们的repos的结果。
-一次以**10个所有者的批量处理组织级查找**。

---

##步骤6：验证每个链接（关键）

**在包含任何资金链接之前，请验证它是否存在

在每个资助URL上使用`web_fetch`：
- **有效页面**→✅包含
- **404 /“未找到”/“未注册”**→❌排除
- **重定向到有效的页面**→✅包括最终的URL

以**5个批量进行验证**。不要提供未经验证的链接。

---

##步骤7：输出报告

输出规则**在数据收集过程中尽量减少中间输出。**不要宣布每批（“7批中的第3批……”，“现在正在检查资金……”）。而不是:
-在开始每个主要阶段时显示一个简短的状态行（例如，“解决67个依赖关系…”，“检查资金链接…”）
- **在生成报告前收集所有数据。**不要滴入部分表。
-在最后将最终报告输出为单个内聚块。

报告模板```
## 💜 Sponsor Finder Report

**Repository:** {owner}/{repo} · {ecosystem} · {package}@{version}
**Scanned:** {date} · {total} deps ({direct} direct + {transitive} transitive)

---

### 🎯 Ways to Give Back

Sponsoring just {N} people/orgs supports {sponsorable} of your {total} dependencies — a great way to invest in the open source your project depends on.

1. **💜 @{user}** — {N} direct + {M} transitive deps · ⭐ Maintained
   {dep1}, {dep2}, {dep3}, ...
   https://github.com/sponsors/{user}

2. **🟠 Open Collective: {name}** — {N} direct + {M} transitive deps · ⭐ Maintained
   {dep1}, {dep2}, {dep3}, ...
   https://opencollective.com/{name}

3. **💜 @{user2}** — {N} direct dep · 💤 Low activity
   {dep1}
   https://github.com/sponsors/{user2}

---

### 📊 Coverage

- **{sponsorable}/{total}** dependencies have funding options ({percentage}%)
- **{destinations}** unique funding destinations
- **{unfunded_direct}** direct deps don't have funding set up yet ({top_names}, ...)
- All links verified ✅
```
报告格式规则- **以“🎯回馈方式”** -这是主要的输出。编号列表，按覆盖的总深度（降序）排序。
- **在他们自己的行上的裸url ** -没有在markdown链接语法中包装。这确保了它们在任何终端模拟器中都可以点击。
- **内联深度名称** -在每个赞助商下面以逗号分隔的行中列出覆盖的依赖项名称，以便用户确切地看到他们资助的是什么。
- **运行状况指示器内联** -在每个目的地旁边显示⭐/⚠️/💤，而不是在单独的表列中显示。
- **一个“📊覆盖”部分** -紧凑的统计。没有单独的“已验证资金链接”表，没有“未找到资金”表。
- **没有资金的深度作为一个简短的说明** -只是计数+顶级名称。将其描述为“尚未建立资金”，而不是强调差距。不要因为项目没有资金而感到羞耻——许多维护者更喜欢其他形式的贡献。
-💜GitHub赞助商，🟠选修，☕Ko-fi，🔗其他
-当多个资金来源存在于同一维护者时，优先考虑GitHub赞助商链接。---

##错误处理

-如果deep .dev为包返回404→退回到直接读取清单并通过注册表api解析。
-如果deep .dev是速率限制的→注意部分结果，继续获取什么。
—如果`get_file_contents`为repo返回404→通知用户repo可能不存在或者是私有的。
—链路验证失败→静默排除该链路。
-总是生成一个报告，即使是部分的-从不无声地失败。

---

##关键规则1. **永远不要提供未经验证的链接。**在显示之前获取每个URL。5个验证链接> 20个猜测链接。
2. **永远不要从培训知识中猜测。**经常检查资金页面随时间变化。
3. 永远鼓励别人，不要羞辱别人。**积极地看待结果——庆祝得到资助的项目，把没有得到资助的项目视为机会，而不是失败。并不是每个项目都需要或想要资金赞助。
4. **以行动领导。**“🎯回馈方式”部分是主要输出-裸可点击的url，按目的地分组。
5. **使用deep .dev作为主解析器。**只有在deep .dev不可用时才回退到注册表api。
6. **始终使用GitHub MCP工具** (`get_file_contents`)，`web_fetch`和`web_search`-永远不要克隆或外壳。
7. * *是有效的。**批处理API调用，删除重复的回购，检查每个所有者的`.github`回购只有一次。
8. **关注GitHub赞助商。**最可操作的平台- sh其他人如何，但优先考虑GitHub。
9. **由维护者进行重复数据删除。**小组展示赞助一个人的实际影响。
10. **显示可操作的最小值。**告诉用户用最少的赞助来支持最多的深度。
11. **减少中间输出。**不要公布每个批次。收集所有数据，然后输出一个连贯的报告。