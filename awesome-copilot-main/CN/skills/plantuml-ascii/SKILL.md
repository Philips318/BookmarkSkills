---
name: plantuml-ascii
description: "Generate ASCII art diagrams using PlantUML text mode. Use when user asks to create ASCII diagrams, text-based diagrams, terminal-friendly diagrams, or mentions plantuml ascii, text diagram, ascii art diagram. Supports: Converting PlantUML diagrams to ASCII art, Creating sequence diagrams, class diagrams, flowcharts in ASCII format, Generating Unicode-enhanced ASCII art with -utxt flag"
license: MIT
allowed-tools: Bash, Write, Read
---
# PlantUML ASCII艺术图表生成器

# #概述

使用PlantUML创建基于文本的ASCII艺术图表。非常适合终端环境中的文档、自述文件、电子邮件或任何不适合图形化图表的场景。

什么是PlantUML ASCII艺术？

PlantUML可以生成纯文本（ASCII艺术）而不是图像的图表。这是有用的：

—基于终端的工作流
—不支持镜像的Gitcommits/PRs-需要版本控制的文档
-图形工具不可用的环境

# #安装```bash
# macOS
brew install plantuml

# Linux (varies by distro)
sudo apt-get install plantuml  # Ubuntu/Debian
sudo yum install plantuml      # RHEL/CentOS

# Or download JAR directly
wget https://github.com/plantuml/plantuml/releases/download/v1.2024.0/plantuml-1.2024.0.jar
```
##输出格式

|标志位|格式|描述|| ------- | ------------- | ------------------------------------ |
|`-txt`| ASCII |纯ASCII字符|
|`-utxt`| Unicode ASCII |增强了画框字符|

基本工作流程

# # # 1。创建PlantUML图文件```plantuml
@startuml
participant Bob
actor Alice

Bob -> Alice : hello
Alice -> Bob : Is it ok?
@enduml
```
# # # 2。生成ASCII图像```bash
# Standard ASCII output
plantuml -txt diagram.puml

# Unicode-enhanced output (better looking)
plantuml -utxt diagram.puml

# Using JAR directly
java -jar plantuml.jar -txt diagram.puml
java -jar plantuml.jar -utxt diagram.puml
```
# # # 3。视图输出

输出保存为`diagram.atxt`（ASCII）或`diagram.utxt`（Unicode）。

支持的图表类型

序列图```plantuml
@startuml
actor User
participant "Web App" as App
database "Database" as DB

User -> App : Login Request
App -> DB : Validate Credentials
DB --> App : User Data
App --> User : Auth Token
@enduml
```
类图```plantuml
@startuml
class User {
  +id: int
  +name: string
  +email: string
  +login(): bool
}

class Order {
  +id: int
  +total: float
  +items: List
  +calculateTotal(): float
}

User "1" -- "*" Order : places
@enduml
```
活动图```plantuml
@startuml
start
:Initialize;
if (Is Valid?) then (yes)
  :Process Data;
  :Save Result;
else (no)
  :Log Error;
  stop
endif
:Complete;
stop
@enduml
```
状态图```plantuml
@startuml
[*] --> Idle
Idle --> Processing : start
Processing --> Success : complete
Processing --> Error : fail
Success --> [*]
Error --> Idle : retry
@enduml
```
组件图```plantuml
@startuml
[Client] as client
[API Gateway] as gateway
[Service A] as svcA
[Service B] as svcB
[Database] as db

client --> gateway
gateway --> svcA
gateway --> svcB
svcA --> db
svcB --> db
@enduml
```
用例图```plantuml
@startuml
actor "User" as user
actor "Admin" as admin

rectangle "System" {
  user -- (Login)
  user -- (View Profile)
  user -- (Update Settings)
  admin -- (Manage Users)
  admin -- (Configure System)
}
@enduml
```
部署图```plantuml
@startuml
actor "User" as user
node "Load Balancer" as lb
node "Web Server 1" as ws1
node "Web Server 2" as ws2
database "Primary DB" as db1
database "Replica DB" as db2

user --> lb
lb --> ws1
lb --> ws2
ws1 --> db1
ws2 --> db1
db1 --> db2 : replicate
@enduml
```
##命令行选项```bash
# Specify output directory
plantuml -txt -o ./output diagram.puml

# Process all files in directory
plantuml -txt ./diagrams/

# Include dot files (hidden files)
plantuml -txt -includeDot diagrams/

# Verbose output
plantuml -txt -v diagram.puml

# Specify charset
plantuml -txt -charset UTF-8 diagram.puml
```
Ant任务集成```xml
<target name="generate-ascii">
  <plantuml dir="./src" format="txt" />
</target>

<target name="generate-unicode-ascii">
  <plantuml dir="./src" format="utxt" />
</target>
```
更好的ASCII图提示

1. **保持简单**：复杂的图表不能很好地在ASCII中呈现
2. **短标签**：长文本破坏ASCII对齐
3. **使用Unicode (`-utxt`)**：更好的视觉质量与框绘制字符
4. **共享前测试**：固定宽度字体终端验证
5. **考虑备选方案**：对于复杂的图表，使用Mermaid.js或graphviz

输出比较示例

**标准ASCII码(`-txt`)**：```
     ,---.          ,---.
     |Bob|          |Alice|
     `---'          `---'
      |   hello      |
      |------------->|
      |              |
      |  Is it ok?   |
      |<-------------|
      |              |
```
**Unicode ASCII (`-utxt`)**：```
┌─────┐        ┌─────┐
│ Bob │        │Alice│
└─────┘        └─────┘
  │   hello      │
  │─────────────>│
  │              │
  │  Is it ok?   │
  │<─────────────│
  │              │
```
##快速参考```bash
# Create sequence diagram in ASCII
cat > seq.puml << 'EOF'
@startuml
Alice -> Bob: Request
Bob --> Alice: Response
@enduml
EOF

plantuml -txt seq.puml
cat seq.atxt

# Create with Unicode
plantuml -utxt seq.puml
cat seq.utxt
```
# #故障排除

**问题**:Unicode字符乱码

- **解决方案**：确保终端支持UTF-8，字体正确

**问题**：图表看起来不对齐

解决方案：使用定宽字体（Courier, Monaco, Consolas）

**问题**：没有找到命令

- **解决方案**：安装PlantUML或直接使用Java JAR

**问题**：未创建输出文件

—**解决方案**：检查文件权限，确保PlantUML有写权限