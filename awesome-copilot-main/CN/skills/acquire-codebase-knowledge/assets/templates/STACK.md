#技术栈

##核心部分（必选）

### 1)运行时总结

|区域|值|证据||------|-------|----------|
|主语言| [VALUE] | [FILE_PATH] |
| Runtime + version | [VALUE] | [FILE_PATH] |
|包管理器| [VALUE] | [FILE_PATH] |
|Module/buildsystem | [VALUE] | [FILE_PATH] |

### 2)生产框架和依赖

只列出影响较大的生产依赖项（框架、数据、传输、授权）。

|依赖关系|版本|系统角色|证据||------------|---------|----------------|----------|
| [name] | [version] | [role] | [file_path] |

3)开发工具链

|工具|用途|证据||------|---------|----------|
| [TOOL] | [LINT/FORMAT/TEST/BUILD] | [FILE_PATH] |

4)关键命令```bash
[install command]
[build command]
[test command]
[lint command]
```
环境和配置

-配置源：[LIST FILES]
-所需环境变量：[VAR_1]， [VAR_2], [TODO]
-Deployment/runtime约束：[简短说明]

6)证据- [path/to/manifest]
- [path/to/runtime-config]
- [path/to/build-or-ci-config]
##扩展节（可选）

只在需要复杂的仓库时添加：

-按类别进行完全依赖分类
—详细的compiler/runtime标志
-环境矩阵（dev/stage/prod）
-进程管理器和容器运行时详细信息