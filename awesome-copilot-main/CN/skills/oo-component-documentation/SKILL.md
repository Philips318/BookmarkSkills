---
name: oo-component-documentation
description: 'Create or update standardized object-oriented component documentation using a shared template plus mode-specific guidance for new and existing docs.'
---
# OO组件文档

为面向对象组件创建新的文档，或者通过分析当前实现来更新现有的组件文档文件。

##先确定模式

在写任何东西之前选择工作流程：1. 当用户提供现有文档标记文件、指向文档路径或显式要求刷新或修改现有文档时，使用**更新模式**。遵循[references/update-mode.md] (references/update-mode.md)。
2. 当用户提供源文件或文件夹、指向组件路径或要求从代码生成文档时，使用**create模式**。遵循[references/create-mode.md] (references/create-mode.md)。
3. 如果同时提供了代码和现有的文档文件，则将现有的文档文件作为输出目标，并使用当前的源代码作为事实的来源。
4. 如果请求不明确，则尽可能从路径类型推断模式：现有的Markdown文档文件意味着更新模式；source/component路径表示创建模式。

文档标准- DOC-001：遵循C4模型文档级别（上下文，容器，组件，代码）
- DOC-002：与Arc42软件架构文档模板保持一致
—DOC-003：符合IEEE 1016软件设计描述标准
DOC-004：使用敏捷文档原则（只需要足够的文档来增加价值）
- DOC-005：以开发人员和维护人员为主要受众

共享分析指导- ANA-001：确定主要组件边界，以及输入是否表示文件夹、文件或现有文档目标
检查源代码文件中的类结构、继承、组合和接口
- ANA-003：识别设计模式、架构决策和集成点
- ANA-004：记录或刷新公共api、接口、依赖关系和使用模式
—ANA-005：捕获方法参数、返回值、异步行为、异常和生命周期关注点
- ANA-006：评估性能、安全性、可靠性、可维护性和可扩展性特征
- ANA-007：推断数据流、协作模式以及与周围组件的关系
- ANA-008：保持文档以实施为基础；避免发明代码不支持的行为

共享输出需求-使用[assets/documentation-template.md]（assets/documentation-template.md）作为规范切片检查表和基线结构。
保持Markdown的输出具有清晰的标题层次结构，有用的表格，示例的代码块，以及需要可视化架构关系的美人鱼图。
-使示例和接口描述匹配当前实现，而不是通用占位符。
-只包括代码、项目结构、配置或明确说明的假设所支持的信息。
-当源覆盖不完整时，明确记录限制而不是猜测。

特定于语言的优化- lng-001: ** c# /。. NET** -async/await，依赖注入，配置，处置，选项模式
- LNG-002: **Java** - Spring框架，注释，异常处理，打包，依赖注入
- LNG-003: **TypeScript/JavaScript** -模块，异步模式，类型，npm依赖，运行时边界
- LNG-004: **Python** -包，虚拟环境，类型提示，测试，依赖管理

##错误处理- ERR-001：如果路径不存在，解释期望的路径，以及技能是否需要源路径或现有的文档文件
- ERR-002：如果没有找到相关的源文件，将差距记录下来，并建议下一步可能检查的位置
- ERR-003：如果无法从请求中推断出文档目标，则说明歧义并仅在无法推断时才要求缺失路径
- ERR-004：如果代码使用非标准的体系结构模式，记录自定义方法，而不是将其强制为通用模式
- ERR-005：如果来源访问不完整，继续使用现有证据，并明确指出任何不受支持的部分

# #工作流程1. 确定任务是创建模式还是更新模式。
2. 检查组件实现和任何相关文件，以了解其公共表面区域和内部结构。
3. 使用[assets/documentation-template.md]（assets/documentation-template.md）作为共享文档脚手架。
4. 在[references/create-mode.md]（references/create-mode.md）或[references/update-mode.md]（references/update-mode.md）中应用模式特定的规则。
5. 生成或修改文档，以便图表、示例、接口、依赖关系和质量属性反映当前的实现。

##完成标准

文档清楚地标识了组件的用途、体系结构、接口、实现细节、使用模式、质量属性和引用。
-前物质场对所选模式是准确的。
-示例和图表匹配实现。
-明确指出任何未知、差距或假设。