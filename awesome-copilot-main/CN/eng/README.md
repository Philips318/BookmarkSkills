#贡献者报告（维护者）🚧

此目录包含用于维护存储库的构建脚本和实用程序。

##构建脚本### `update-readme.mjs`
从存储库内容（代理、提示、说明、技能、钩子、集合）生成主要的README.md和文档文件。### `generate-marketplace.mjs`
自动从`plugins/`文件夹中的所有插件目录生成`.github/plugin/marketplace.json`。GitHub CopilotCLI使用这个文件从这个存储库中发现和安装插件。

**原理：**
—扫描`plugins/`中的所有目录
-读取每个插件的`.github/plugin/plugin.json`元数据
-生成一个统一的`marketplace.json`与所有可用的插件
—作为`npm run build`的一部分自动运行

**手动运行：**```bash
npm run plugin:generate-marketplace
```

### `generate-website-data.mjs`
从存储库内容为网站生成JSON数据文件。

##贡献者工具

-`contributor-report.mjs`-为丢失的贡献者（包括共享的助手）生成合并pr的降价报告。
-`add-missing-contributors.mjs`-按需维护脚本，用于自动向`.all-contributorsrc`添加缺失的贡献者（从合并的PR文件推断贡献类型，然后运行all-contributor CLI）。

##维护人员注意事项

-报告按需生成并输出到`reports/contributor-report.md`供人工审阅。
-报告输出有意做到最少：一个受影响pr的列表和一个添加缺失贡献者的命令。
-该存储库需要完整的git历史以进行准确的分析。在CI中设置为`fetch-depth: 0`。
-链接：[所有贡献者的CLI文档]（https://allcontributors.org/docs/en/cli）

按需脚本（非CI）

这些是维护工具。它们有意只按需提供（但可以稍后连接到CI中）。### `add-missing-contributors.mjs`
-目的：检测缺失的贡献者，从他们合并的PR文件中推断贡献类型，并运行`npx all-contributors add ...`更新`.all-contributorsrc`。
——要求:	- GitHub CLI (`gh`) available (used to query merged PRs).
	- `.all-contributorsrc` exists.
	- Auth token set to avoid the anonymous GitHub rate limits:
		- Set `GITHUB_TOKEN` (preferred), or `GH_TOKEN` for the `gh` CLI.
		- If you use `PRIVATE_TOKEN` locally, `contributor-report.mjs` will map it to `GITHUB_TOKEN`.
##安全关机

-`contributor-report.mjs`从文件早期的`eng/utils/graceful-shutdown.mjs`调用`setupGracefulShutdown('script-name')`，以附加signal/exception处理程序。

测试和维护

辅助函数具有小的、确定性的行为，并包含JSDoc注释。
-`contributor-report.mjs`中的`getMissingContributors`函数是用于检测`all-contributors check`输出中缺失贡献者的单一真相来源。