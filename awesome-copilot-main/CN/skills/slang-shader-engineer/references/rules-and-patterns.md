俚语着色器-规则，模式和例子

# # DOs-当可移植性或逐渐采用问题时保持HLSL兼容性。
-使用模块和导入来分离可重用的数学，材料，照明，实用程序和舞台逻辑。
-使用接口和泛型，而不是预处理繁重的专门化。
-使用泛型约束来保持有意的专门化和更清晰的诊断。
-使用`ParameterBlock<T>`设计按更新率组织资源和常量。
-将参数块设计连接到D3D12描述表和Vulkan描述集期望。
-使阶段输入和输出明确且语义清晰。
—根据内存压力、占用率和同步需求来选择计算工作组的大小。
-在依赖平台特定功能时，使用功能或明确的目标假设。
-当一个特性是目标限制的（指针，波操作，特定于后端的调试支持）时调用。
-保持数据布局，垫Rix约定、惯用手性和坐标空间转换都是明确的。
-当涉及主机端绑定或布局生成时，使用反射感知设计。
-在所有示例中提供编译目标、入口点和预期绑定。
-在重写着色器接口或资源布局之前询问现有的引擎约定。
-当交叉编译和调试是工作流程的一部分时，保留可读的生成代码期望。
-为所有着色器代码输出使用标记为`slang`的围栏代码块。
-包括一个简短的绑定总结或主机端假设与每个生成的着色器。
-对于复杂的着色器，将辅助逻辑与入口点分开。# #不该做的事不要发明没有文档的俚语语法、属性或资源规则。
不要像对待`import`那样对待`#include`，也不要假设宏可以跨模块共享。
不要假设所有的后端都支持相同的特性、指针行为、波操作、衍生或调试工具。
-不要硬编码特定于平台的假设而不调用它们。
-当接口或泛型更适合时，不要使用预处理器作为默认的专门化机制。
在没有检查主机端API和反射流的情况下，不要假设参数块布局或绑定约定。
如果精度、布局、ABI或主机互操作依赖于精确类型，不要到处使用隐式类型。
-不要在可移植代码中使用指针，除非目标集明确支持它们（仅限SPIR-V， c++， CUDA）。
-不要以为自动diff，光线追踪或高级功能是可以接受的，因为俚语支持他们。
不要在没有解释影响的情况下改变阶段语义、描述符布局或缓冲区打包规则。
-不要盲目优化-说明目标是更低的带宽，更少的障碍，更少的分歧，更好的缓存局域性，更高的占用，或更少的指令。
-当请求明显也需要主机集成细节时，不要只提供着色器代码。
不要隐藏不确定性——如果缺少细节，就问出来。---

当开发者不知道这些内容时，询问他们

当下列问题对正确性有重大影响时，提出重点的后续问题：- **目标后端** - D3D12， Vulkan, Metal, SPIR-V， GLSL， CUDA， CPU或多目标
-顶点，像素，计算，船体，域，光线追踪阶段等。
- **入口点名称** -是否必须适合现有的引擎接口。
- **坐标约定** -手性，夹距，矩阵填充，row/column-major.-描述符布局，参数块使用，反射工作流。
- **缓冲布局** -纹理格式，对齐，精度要求。
**性能目标** -吞吐量，延迟，寄存器压力，占用，编译大小。
- **硬件层/供应商约束**。
- **HLSL兼容性要求** -代码必须保持HLSL兼容吗？
- ** c++主机结构** -必须着色器匹配现有的c++数据结构或引擎绑定路径？
- **高级功能可用性** -是自动diff，光线追踪，或波操作这个项目允许什么？>只请求所需的最小缺失信息-不要预先加载用户长问卷。

---

##输出格式要求

生成新的俚语代码时：```slang
// Target: Vulkan / SPIR-V
// Stage: Vertex + Fragment
// Entry points: mainVS, mainPS
// Bindings: set=0 MaterialParams, set=1 PerFrame

module MyMaterial;

import CommonMath;

struct MaterialParams { ... };
ParameterBlock<MaterialParams> gMaterial;

[shader("vertex")]
VSOut mainVS(VSIn v) { ... }

[shader("fragment")]
float4 mainPS(VSOut v) : SV_Target { ... }
```
在审查或重构现有代码时：
1. 首先识别“正确性”风险。
2. 然后是可移植性问题。
3. 然后是**性能**问题。
4. 然后提供修改后的代码和增量解释。

---

模块结构模式

小项目（单个文件）```slang
// shader.slang — all-in-one; acceptable for prototypes
[shader("compute")]
[numthreads(64,1,1)]
void main(uint3 id : SV_DispatchThreadID) { ... }
```
中等项目（分域模块）```
shaders/
├── common/
│   ├── math.slang        — vector/matrix utilities
│   └── sampling.slang    — random/importance sampling
├── materials/
│   ├── brdf.slang        — BRDF interface + implementations
│   └── material.slang    — IMaterial, ParameterBlock setup
├── lighting/
│   └── light.slang       — ILight, PointLight, DirectionalLight
└── passes/
    ├── gbuffer.slang     — G-buffer write pass
    └── deferred.slang    — deferred shading pass
```
###参数块组织更新频率```slang
// Updated once per frame
struct PerFrameParams { float4x4 view; float4x4 proj; float time; };
ParameterBlock<PerFrameParams> gPerFrame;

// Updated per draw call
struct PerObjectParams { float4x4 model; };
ParameterBlock<PerObjectParams> gPerObject;

// Updated per material change
struct MaterialParams { float3 albedo; float metallic; float roughness; };
ParameterBlock<MaterialParams> gMaterial;
```
---

计算着色检查表

-[]线程组大小匹配目标的预期GPU占用率。
—[]共享内存占用率在硬件限制范围内（一般为48kb ~ 64kb）。
-[]内存访问模式最小化银行冲突和最大化合并。
- []`GroupMemoryBarrierWithGroupSync()`的正确位置-在and/or之前，在共享内存写入之后。
-[]诱导发散的分支最小化或移动到内部循环之外。
-[]调度尺寸和线程ID索引对1D/2D/3D数据正确。

---

交叉编译检查表

-[]所使用的特性在所有需要的目标后端都是可用的。
-[]指针使用仅保护为SPIR-V/C++/CUDA。
- []Wave/subgroupops是能力门控。
-[]矩阵布局假设是明确的（`-matrix-layout-row-major`/`-matrix-layout-column-major`）。
- [] Debug printf在不被普遍支持的情况下封装在目标守卫中。
-[]入口点语义跨目标保持一致。---

##示例提示技能处理良好为PBR写一个带有法线映射和参数块的俚语顶点和碎片着色器。
生成一个俚语船体和区域着色器对，用于自适应镶嵌，具有抗裂边缘因素。
“重构这个俚语计算着色器以减少共享内存库冲突。”
-“为渲染器创建一个俚语模块布局，有单独的材质、照明和实用模块。”
解释如何在没有预处理宏的灯光系统中使用俚语接口和泛型。
给定这个c++渲染密码和这个俚语着色器，找到绑定，布局或语义不匹配。
-“展示如何为spil - v编译这个俚语着色器，并从c++反映其参数布局。”
“写一个跨目标的俚语计算着色器，明确地标记后端敏感假设。”
查看这个俚语模块结构，告诉我是否使用了导入、泛型或参数块orrectly。”
-“解释`var`、`let`、泛型、相关类型和生产中的功能的实际使用注意事项。”
“设计一个反射感知的俚语+ c++工作流程，用于加载，编译和绑定计算着色器。”
-“展示如何构建一个俚语包用于多目标编译DXIL， SPIR-V和Metal。”---

c++和引擎集成说明

当任务触及引擎或主机代码时：

在对布局、反射、资源绑定或运行时调度做出假设之前，检查用户的代码库。
-使用语义符号工具检查c++类、枚举、编译路径、渲染通道和描述符设置。
-在更改着色器接口之前，检查俚语输出是如何编译，加载，反射，缓存和绑定在主机应用程序中的。
-对于c++集成问题，偏好精确的符号查找和用法查询，而不是原始文本搜索。
-总是更喜欢反射友好和引擎友好的界面，而不是聪明的仅着色器抽象。

make集成代码片段```cmake
find_package(slang REQUIRED PATHS ${CMAKE_INSTALL_PREFIX} NO_DEFAULT_PATH)
target_link_libraries(yourLib PUBLIC slang::slang)
```
俚语编译目标（俚语命令行）```bash
# SPIR-V for Vulkan
slangc shader.slang -target spirv -o shader.spv

# DXIL for D3D12
slangc shader.slang -target dxil -o shader.dxil

# GLSL
slangc shader.slang -target glsl -o shader.glsl

# CUDA
slangc shader.slang -target cuda -o shader.cu

# Row-major matrices (important for xMath-style engines)
slangc shader.slang -target spirv -matrix-layout-row-major -o shader.spv
```