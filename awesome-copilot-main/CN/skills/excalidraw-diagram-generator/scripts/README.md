# excaldraw库工具

这个目录包含使用Excalidraw库的脚本。## split-excalidraw-library.py
将Excalidraw库文件（`*.excalidrawlib`）拆分为单个图标JSON文件，以便AI助手有效地使用令牌。

# # #先决条件

- Python 3.6或更高版本
-不需要额外的依赖（仅使用标准库）

# # #使用```bash
python split-excalidraw-library.py <path-to-library-directory>
```
###逐步工作流程

1. **创建库目录**：   ```bash
   mkdir -p skills/excalidraw-diagram-generator/libraries/aws-architecture-icons
   ```
2. **下载并放置库文件**：
—访问：https://libraries.excalidraw.com/—搜索“AWS Architecture Icons”，下载`.excalidrawlib`文件
—将其重命名为与目录名称匹配：`aws-architecture-icons.excalidrawlib`—放入步骤1中创建的目录

3. **运行脚本**：   ```bash
   python skills/excalidraw-diagram-generator/scripts/split-excalidraw-library.py skills/excalidraw-diagram-generator/libraries/aws-architecture-icons/
   ```
输出结构

脚本在库目录中创建如下结构：```
skills/excalidraw-diagram-generator/libraries/aws-architecture-icons/
  aws-architecture-icons.excalidrawlib  # Original file (kept)
  reference.md                          # Generated: Quick reference table
  icons/                                # Generated: Individual icon files
    API-Gateway.json
    CloudFront.json
    EC2.json
    S3.json
    ...
```
###脚本的作用

1. **读取**`.excalidrawlib`文件
2. **从`libraryItems`数组中提取**每个图标
3. **清理**图标名称以创建有效的文件名（空格→连字符，删除特殊字符）
4. **保存**每个图标作为一个单独的JSON文件在`icons/`目录
5. **生成**一个`reference.md`文件，其中包含一个映射图标名称到文件名的表

# # #好处

- **令牌效率**:AI可以首先读取轻量级的`reference.md`来查找相关图标，然后只加载所需的特定图标文件
- **组织**：图标以清晰的目录结构组织
- **扩展性**：用户可以添加多个库集并排

推荐的工作流程

1. 从https://libraries.excalidraw.com/下载所需的excaldraw库
2. 在每个库文件上运行此脚本
3. 将生成的文件夹移动到`../libraries/`4. 人工智能助手将使用`reference.md`文件来有效地定位和使用图标库源（示例-验证可用性）

在https://libraries.excalidraw.com/上发现的例子可能包括cloud/service图标集。
-可用性随时间变化在使用之前，请在站点上验证库的确切名称。
-该脚本可以与您提供的任何有效的`.excalidrawlib`文件一起工作。

# # #故障排除

**错误：文件未找到**
—检查文件路径是否正确
-确保文件的扩展名为`.excalidrawlib`**错误：库文件格式无效
—确保该文件是有效的Excalidraw库文件
—确认包含“`libraryItems`”数组

许可注意事项

当使用第三方图标库时：
- **AWS架构图标**：以AWS内容许可为准
- **GCP图标**：以谷歌的条款为准
- **其他图书馆**：检查每个图书馆的许可证

这个脚本供personal/organizational使用。拆分图标文件的再分发应遵守原始库的许可条款。## add-icon-to-diagram.py
将拆分Excalidraw库中的特定图标添加到现有的`.excalidraw`图中。该脚本处理坐标转换和ID冲突避免，并且可以选择在图标下添加标签。

# # #先决条件

- Python 3.6或更高版本
-一个图表文件（`.excalidraw`）
-拆分图标库目录（由`split-excalidraw-library.py`创建）

# # #使用```bash
python add-icon-to-diagram.py <diagram-path> <icon-name> <x> <y> [OPTIONS]
```
* * * *选项
—`--library-path PATH`：图标库目录的路径（默认：`aws-architecture-icons`）
-`--label TEXT`：在图标下方添加一个文本标签
——`--use-edit-suffix`：通过`.excalidraw.edit`编辑以避免编辑器覆盖问题（默认启用，通过`--no-use-edit-suffix`来禁用）

# # #的例子```bash
# Add EC2 icon at position (400, 300)
python add-icon-to-diagram.py diagram.excalidraw EC2 400 300

# Add VPC icon with label
python add-icon-to-diagram.py diagram.excalidraw VPC 200 150 --label "VPC"

# Safe edit mode is enabled by default (avoids editor overwrite issues)
# Use `--no-use-edit-suffix` to disable
python add-icon-to-diagram.py diagram.excalidraw EC2 500 300

# Add icon from another library
python add-icon-to-diagram.py diagram.excalidraw Compute-Engine 500 200 \
   --library-path libraries/gcp-icons --label "API Server"
```
###脚本的作用

1. **从库的`icons/`目录加载**图标JSON
2. **计算**图标的边界框
3. **将**所有坐标偏移到目标位置
4. **生成**唯一的id为所有元素和组
5. **将转换后的元素追加到图中
6. **（可选）**在图标下方添加标签

---## add-arrow.py
在现有`.excalidraw`图的两点之间添加一个直箭头。支持可选的标签和线条样式。

# # #先决条件

- Python 3.6或更高版本
-一个图表文件（`.excalidraw`）

# # #使用```bash
python add-arrow.py <diagram-path> <from-x> <from-y> <to-x> <to-y> [OPTIONS]
```
* * * *选项
-`--style {solid|dashed|dotted}`：线条样式（默认为`solid`）
-`--color HEX`：箭头颜色（默认为`#1e1e1e`）
-`--label TEXT`：在箭头上添加文本标签
——`--use-edit-suffix`：通过`.excalidraw.edit`编辑以避免编辑器覆盖问题（默认启用，通过`--no-use-edit-suffix`来禁用）

# # #的例子```bash
# Simple arrow
python add-arrow.py diagram.excalidraw 300 200 500 300

# Arrow with label
python add-arrow.py diagram.excalidraw 300 200 500 300 --label "HTTPS"

# Dashed arrow with custom color
python add-arrow.py diagram.excalidraw 400 350 600 400 --style dashed --color "#7950f2"

# Safe edit mode is enabled by default (avoids editor overwrite issues)
# Use `--no-use-edit-suffix` to disable
python add-arrow.py diagram.excalidraw 300 200 500 300
```
###脚本的作用

1. **从给定坐标创建**箭头元素
2. **（可选）**在箭头中点附近添加标签
3. **向图表追加**元素
4. **保存**更新后的文件