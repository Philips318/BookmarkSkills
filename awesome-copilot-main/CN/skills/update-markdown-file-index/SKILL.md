---
name: update-markdown-file-index
description: 'Update a markdown file section with an index/table of files from a specified folder.'
---
更新Markdown文件索引

使用`${input:folder}`文件夹中的index/table文件更新markdown文件`${file}`。

# #过程

1. **扫描**：读取目标标记文件`${file}`以了解现有结构
2. **发现**：列出指定文件夹`${input:folder}`下匹配模式`${input:pattern}`的所有文件
3. **Analyze**：确定是否存在要更新或创建新结构的现有table/index节
4. **结构**：根据文件类型和现有内容生成合适的table/list格式
5. **更新**：用文件索引替换现有节或添加新节
6. **Validate**：确保markdown语法有效且格式一致

##文件分析

对于每个发现的文件，提取：- **Name**：文件名根据上下文有或没有扩展名
- **类型**：文件扩展名和类别（例如，`.md`,`.js`,`.py`）
- **描述**：第一行注释，头，或推断的目的
- **大小**：供参考的文件大小（可选）
- **修改**：最后修改日期（可选）

表结构选项

根据文件类型和现有内容选择格式：

选项1：简单列表```markdown
## Files in ${folder}

- [filename.ext](path/to/filename.ext) - Description
- [filename2.ext](path/to/filename2.ext) - Description
```
选项2：详细表

|文件|类型|描述||------|------|-------------|
|(文件名。（path/to/filename.ext） |扩展|描述|
(filename2 |。（path/to/filename2.ext） |扩展|描述|

选项3：分类章节

按type/category对文件进行分组，并使用单独的节或子表。

##更新策略

-🔄**更新现有**：如果table/index节存在，替换内容，同时保留结构
-➕**添加新的**：如果没有现有的节，创建新的节使用最适合的格式
-📋**保存**：保持现有的降价格式，标题水平和文档流
—🔗**Links**：存储库内的文件链接使用相对路径

##分段标识

查找具有以下模式的现有部分：

-标题包括：“索引”、“文件”、“内容”、“目录”、“清单”
—包含文件相关列的表
—带有文件链接的列表
-标记文件索引部分的HTML注释

# #要求-保留现有的降价结构和格式
—文件链接使用相对路径
—包括可用的文件描述
—默认按字母顺序排序
—处理文件名中的特殊字符
-验证所有生成的降价语法