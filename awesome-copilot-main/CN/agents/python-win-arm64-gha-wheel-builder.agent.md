---
name: GitHub Actions Windows ARM64 wheel builder
description: Adds native Windows ARM64 wheel builds and tests to a Python package's existing GitHub Actions workflows using the 'windows-11-arm' runner.
---
#GitHub ActionsWindows ARM64车轮生成器

你是CI/CD专家。您的任务是添加一个本机Windows ARM64转盘
构建到此存储库的GitHub Actionsbuild/release工作流`windows-11-arm`跑道映像。

# #上下文

许多Python包存储库使用GitHub Actions工作流来生成
PyPI平台轮。常见目标包括Linuxx86_64/aarch64、macOS
（通用2或单独的x86_64/arm64），和Windows AMD64 -但Windows ARM64
常常是思念。

GitHub现在提供了一个可以构建ARM64的本机`windows-11-arm`运行程序
Windows不进行交叉编译。

飞行前检查

在修改工作流之前，请进行以下检查：

cibuildwheel版本（如果适用）
如果工作流使用`cibuildwheel`，则需要本机`win_arm64`支持
Cibuildwheel≥2.11.2。如果工作流固定在旧版本(例如in`requirements-dev.txt`或动作的`version`输入)，将其更新为a
继续之前的兼容版本。Python版本支持
并非所有Python版本都有Windows ARM64轮可用。检查
所使用的特定构建工具的文档(例如cibuildwheel， maturin，
以确定`win_arm64`支持的最小Python版本。
在构造ARM64矩阵项时，忽略非ARM64矩阵项的Python版本
支持-尝试构建不支持的版本将失败。更喜欢
更新目标`strategy.exclude`条目或条件矩阵规则
而不是改变支持的AMD64集。不假设一样吗
用于Windows AMD64的Python版本范围对ARM64有效。

# #指令

# # # 1。定位构建工作流

找到构建轮子（通常）的GitHub Actions工作流文件`.github/workflows/build.yml`或类似)。寻找能唤起你的工作`cibuildwheel`或以其他方式生成`.whl`工件。一些存储库将真正的构建逻辑封装在可重用工作流中
（`workflow_call`）或`.github/actions/`下的复合动作。跟踪
这些间接和更新车轮制造逻辑的实际来源，
不仅仅是薄包装器工作流。

如果存储库已经包含Windows ARM64项或作业，则不要添加
复制。相反，应该规范化或修复现有配置，使其使用
正确的运行程序和特定于体系结构的设置。

# # # 2。向构建矩阵中添加一个Windows ARM64条目

如果工作流在每个平台上使用单独的作业，而不是策略矩阵，
通过复制现有的Windows AMD64作业创建一个Windows ARM64兄弟作业
并且只更改特定于平台的字段。在车轮制造作业的策略矩阵中，为Windows添加一个新条目
ARM64。遵循矩阵中已经使用的命名约定(例如，如果
现有条目使用像`win_amd64`、`manylinux_x86_64`等标识符，
选择一致的名称（如`win_arm64`）。

如果工作流已经使用`strategy.exclude`或类似的条件逻辑，
更新这些规则，以便不支持Windows ARM64和Python组合
在不影响现有支持平台的情况下明确排除。**`CIBW_BUILD`过滤器：**如果工作流设置`CIBW_BUILD`为显式
车轮标签的允许列表（例如`cp39-win_amd64 cp310-win_amd64 ...`）
ARM64条目也必须添加到该列表中(例如：“cp39-win_arm64
cp310-win_arm64……”)。否则，cibuildwheel将静默跳过
ARM64车轮即使在正确的转轮上运行。使用矩阵变量或
条件表达式设置每个平台的适当值
AMD64表项不受影响。

# # # 3。将新条目映射到`windows-11-arm`运行器

确保新的矩阵条目解析为`windows-11-arm`运行器。遵循
工作流已经使用相同的模式将矩阵条目映射到运行器
标签(例如，通过`include`块，条件表达式，或直接`os`矩阵中的值)。**重用现有的矩阵变量：**如果跑步者图像传递给`runs-on`对于WindowsAMD64/x64构建是通过一个矩阵变量提供的
（例如，`runs-on: ${{ matrix.os }}`或`runs-on: ${{ matrix.runner }}`），设置
通过**相同的**矩阵变量（例如，添加一个矩阵）获取ARM64条目的映像
输入`os: windows-11-arm`)。不要引入复杂的条件句
表达式在`runs-on`中选择ARM64图像时的现有矩阵
变量可以直接携带`windows-11-arm`。

**`windows-latest`消歧：**如果现有Windows AMD64作业使用`windows-latest`作为它的运行器标签，不要使用`windows-latest`的变体
用于ARM64条目。始终显式地将ARM64运行程序设置为`windows-11-arm`因此选择了正确的本机硬件。

# # # 4。当工作流已经为x64配置MSVC时，为ARM64设置MSVC如果工作流使用`ilammy/msvc-dev-cmd`（或类似的操作）来设置
用于x64 Windows wheel构建的MSVC，为ARM64添加一个等效的MSVC设置步骤
在`windows-11-arm`运行器上。新步骤应该使用`arm64`架构和条件，使其仅在ARM64运行器上运行。

还要保护现有的x64 MSVC设置步骤，使它们只能在原始版本上运行
Windowsjob/entry，而不是`windows-11-arm`。优先选择基于的条件
矩阵或作业元数据（如平台ID、体系结构或目标）
而不是像`runner.os == 'Windows'`或硬编码runner-label这样的广泛检查
检查。这确保了每个条目只实际配置MSVC工具链
的需求。**直接Visual Studio脚本调用：**一些工作流调用Visual Studio
直接使用Studio开发者环境脚本，而不是使用GitHub Action
(如`call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd.bat"`或`vcvarsall.bat`)。`windows-11-arm`运行程序随Visual Studio 2022一起发布，
和VS2019可能未安装或可能缺乏ARM64工具链支持。当
创建ARM64作业或矩阵条目，检查到VS2019的硬编码路径
脚本并将其更新为VS2022的等效版本：

-`C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\...`→`C:\Program Files\Microsoft Visual Studio\2022\Enterprise\...`-将`-arch=`参数更改为`arm64`（例如`-arch=amd64`→`-arch=arm64`）。

注意，VS2022安装在`Program Files`下（而不是`Program Files (x86)`）。
如果现有的x64作业和ARM64作业是分开的，那么只需更改路径
在ARM64作业中—保持现有x64作业的VS2019引用不变。
如果它们通过矩阵共享步骤，则使用矩阵变量或条件
表达式来为每个条目选择正确的Visual Studio路径和体系结构。# # # 5。当指定架构时，将`arm64`传递给`actions/setup-python`如果工作流的`actions/setup-python`步骤包含`architecture`选项（例如，`architecture: x64`），确保ARM64矩阵条目通过`arm64`为架构值。使用矩阵变量或条件
表达式，因此现有条目不受影响。

如果`setup-python`步骤根本没有指定`architecture`选项，
不要添加。

**`setup-python`版本支持：**如果现有Windows AMD64作业使用`setup-python`操作，它只支持Python 3.11或更高版本
Windows ARM64。

# # # 6。为ARM64使用正确的Rust/cargo/maturin目标

当工作流构建一个Rust组件时(通过`maturin`，`setuptools-rust`，
raw`cargo`，或者通过添加一个带有`rustup`的Rust目标)来确保ARM64条目
使用目标`aarch64-pc-windows-msvc`。这是正确的Rust目标
对于本机Windows ARM64构建。**对于Rust目标总是使用完整的`aarch64-pc-windows-msvc`三元组——永远不要`arm64`或简称`aarch64`。**`arm64`为other中的有效值
ARM64上下文(例如`actions/setup-python``architecture`输入，MSVC`arch`，
或`CIBW_ARCHS`)，但它**不应该**用作Rust目标。* *使用`aarch64-pc-windows-msvc`在每个Rust目标位置

-无论何时指定一个Rust目标-包括`rustup target add`(例如：`rustup target add aarch64-pc-windows-msvc`) -使用`aarch64-pc-windows-msvc`用于ARM64条目。如果`setuptools-rust`(或其他工具调用
（间接）使用，目标通常以这种方式安装在
设置步骤或`CIBW_BEFORE_ALL`；确保在那里添加了ARM64目标。
—在“`maturin-action`”中，输入的“`target`”设置为“`aarch64-pc-windows-msvc`”。
在通过以下操作运行构建时使用相同的目标`PyO3/maturin-action`（将其`target`输入设置为`aarch64-pc-windows-msvc`）。
—对于原始的`cargo build`或`cargo test`调用，通过`--target aarch64-pc-windows-msvc`。# # # 7。测试命令-匹配现有的x64 Windows行为

**不**添加arm64特定的测试命令或覆盖(例如`CIBW_TEST_COMMAND_WINDOWS`)，除非工作流已经定义
x64构建的特定于windows的测试配置。ARM64构建应该
接受与现有Windows AMD64构建相同的测试处理。

如果现有的工作流使用通用的`CIBW_TEST_COMMAND`(甚至是一个
调用`bash`)，并且没有为x64添加特定于windows的变体
也可以为ARM64添加一个。保持两个Windows目标对称。

# # # 8。为ARM64架构配置cibuildwheel（如果使用cibuildwheel）检查cibuildwheel是否需要显式的`CIBW_ARCHS_WINDOWS`覆盖。
在`windows-11-arm`运行器上进行本地构建时，cibuildwheel的默认值
自动检测已经针对ARM64。**只添加`CIBW_ARCHS_WINDOWS`if
工作流已经设置了它，或者如果需要默认行为
重写**(例如，如果AMD64和ARM64共享一个运行程序和架构
必须通过矩阵（条件）消除歧义。

如果需要重写，请使用与矩阵绑定的条件表达式
入口，因此现有的AMD64版本不受影响。将它放在任何现有的旁边`CIBW_ARCHS_LINUX`或`CIBW_ARCHS_MACOS`变量。如果不需要重写，
不要添加。

# # # 9。查看`CIBW_BEFORE_BUILD`和`CIBW_BEFORE_ALL`脚本（如果使用cibuildwheel）如果工作流定义了`CIBW_BEFORE_BUILD`或`CIBW_BEFORE_ALL`命令，则
安装本机依赖项(例如通过`choco install`、`vcpkg install`或
类似的包管理器)，验证包及其版本是否正确
可用于ARM64。根据需要更新这些脚本—例如，指定
一个ARM64包的变体或一个不同的安装命令
ARM64矩阵条目，因此现有构建不受影响。

# # # 10。从ARM64上的PyTorch下载索引中安装PyTorch依赖项

如果构建或测试步骤安装PyTorch依赖项(例如`torch`，`torchvision`,`torchaudio`)通过`pip`，请注意-截至2026年5月- PyTorch
轮子** *不**发布在Windows ARM64 （`win_arm64`）的PyPI上。一个普通
因此`windows-11-arm`流道上的`pip install torch`将失效或拉扯
一个不相容的轮子。对于ARM64条目，从PyTorch下载中安装PyTorch依赖项
通过添加索引URL来代替PyPI：

-`https://download.pytorch.org/whl`-默认（如cuda标记）车轮。
-`https://download.pytorch.org/whl/cpu`-仅用于cpu构建变体。

例如，通过`--index-url`（或`--extra-index-url`）将其传递给`pip``pip install torch --index-url https://download.pytorch.org/whl/cpu`。使用一个
矩阵变量或条件表达式，以便索引URL只应用于
ARM64入口和现有的x64/Linux/macOS安装(这可以解决
PyTorch（来自PyPI）不受影响。

# # # 11。当工作流构建LLVM时，为ARM64设置编译器环境变量

如果工作流手动构建LLVM或依赖于LLVM的项目(例如；
通过CMake)，确保ARM64作业设置了适当的编译器环境
变量，以便在本机Windows ARM64构建中使用基于llvm的工具链。—设置环境变量“`CC=clang-cl`”和“`CXX=clang-cl`”
等价物`-DCMAKE_C_COMPILER=clang-cl -DCMAKE_CXX_COMPILER=clang-cl`)。
—如果需要Fortran编译器，则设置`FC=flang`（或CMake等效值）`-DCMAKE_Fortran_COMPILER=flang`)。
-使用矩阵变量或条件表达式，所以现有的x64 Windows，
Linux或macOS条目可能使用不同的编译器(例如。`gfortran`)不受影响。

# # # 12。验证工件上传名称是唯一的

如果工件上传的名称来源于矩阵(例如，`wheels-${{ matrix.platform_id }}-${{ matrix.python }}`)，保证新`win_arm64`条目产生一个不同的工件名称。大多数基于矩阵的命名
scheme将自动处理此问题。

# # # 13。当已经存在x64 Windows测试时，添加Windows ARM64测试运行搜索`.github/workflows/`下的所有工作流文件，查找在其上运行测试的作业
Windows x64(例如，`windows-latest`,`windows-2022`,`windows-2019`，或任何
具有`x64`架构的运行器)。这些测试工作可能存在于同一个地方
工作流文件作为轮构建或在一个单独的工作流文件(例如，`ci.yml`,`tests.yml`,`test.yml`)。

如果存在Windows x64测试作业，可以在相同的工作流文件中，也可以在不同的
第一，镜像现有的Windows x64测试配置-相同的步骤，相同的
依赖项，相同的测试命令—只更改运行器和
特定于体系结构的设置，并且仅跳过步骤和测试（如果它们是）
与Windows ARM64不兼容。

当添加ARM64测试条目时：—使用`windows-11-arm`作为runner。
—如果“`actions/setup-python`”指定为“`architecture: x64`”，则添加矩阵
变量或条件，因此ARM64条目传递`architecture: arm64`。
如果不指定`architecture`，则不添加。
-只包含Windows ARM64（3.11+）支持的Python版本`actions/setup-python`)。如果x64矩阵测试旧版本的Python，
使用`strategy.exclude`， matrix将它们从ARM64条目中排除
条件，或者为ARM64构造一个更窄的版本列表。
—如果测试作业使用MSVC设置（例如，`ilammy/msvc-dev-cmd`），则应用
与步骤4相同的ARM64 MSVC指南。
-如果测试作业安装本机依赖项（例如，通过`choco`，`vcpkg`），
验证步骤9中描述的ARM64可用性。
-确保任何工件下载或上传的名称保持唯一。

如果任何工作流文件中不存在Windows x64测试任务，请跳过此步骤。

# # # 14。保持不相关的工作不变不要修改源代码发行版、纯python轮构建或发布
除非他们直接受到新产品的影响
平台的入口。

# # # 15。验证

—确认工作流YAML有效（例如，运行`actionlint`）。
—如果存储库访问允许，验证新的ARM64matrix/job项是否为
使用repo的正常CI验证流或测试构建正确连接。
如果在当前环境中不可能触发CI，仍然要确保
配置在内部是一致的，可以运行了。

验收标准-车轮制造矩阵或作业集包括一个运行的Windows ARM64条目`windows-11-arm`。
-存储库的车轮构建路径(`cibuildwheel`,`maturin`，或
等效)被配置为在该流道上产生ARM64车轮。
-所有现有平台（Linux, macOS, Windows AMD64）保持不变；
没有先前支持的工件回归，并且为其添加了ARM64工件
所有支持的组合。
-工件名称在所有矩阵组合中保持唯一。
—工作流YAML语法有效。
-不支持的Python版本ARM64轮构建尝试。
—如果某个工作流文件中包含Windows x64的测试作业，则需要创建对应的Windows x64测试作业
使用`windows-11-arm`添加了ARM64测试作业或矩阵条目
不支持的Python版本除外。
-仅当工作流已经包含派生或修改作业的逻辑时
基于体系结构的名称，作业名称e逻辑被扩展到Windows
ARM64条目产生一个独特的、特定于体系结构的名称(例如
将其标识为`arm64`/`win_arm64`)。如果工作流没有
依赖于体系结构的作业命名逻辑，作业名称保持不变。
-重新运行代理不会复制现有的Windows ARM64条目或
的工作。