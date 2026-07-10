#俚语参考

**来源：**[官方俚语着色库]（https://github.com/shader-slang/slang“俚语着色库”）
**官方文档：**[官方俚语着色器在线文档]（https://shader-slang.com/slang/user-guide/“俚语着色器在线文档”）
**游乐场：**[官方俚语Shader沙盒]（https://shader-slang.com/slang-playground）

---

##目录

1. (类型)(#类型)
2. [接口和泛型]（# Interfaces -and- Generics）
3. (自动分化)(# automatic-differentiation)
4. [模块和访问控制]（# Modules -and- Access - Control）
5. (功能系统)(# capabilities-system)
6. (编译API) (# compilation-api-c)
7. (反射API)(#反射API)
8. (编译目标)(# compilation-targets)
9. [目标兼容性矩阵]（# Target - Compatibility - Matrix）
10. [slangc命令行]（#slangc- Command）

---

# #类型# # #标量
—整数：`int8_t`、`int16_t`、`int`、`int64_t`、`uint8_t`、`uint16_t`、`uint`、`uint64_t`—浮点数：`half`（16位）、`float`（32位）、`double`（64位）
—其他：`bool`、`void`向量和矩阵
-`vector<T,N>`(N = 2-4)，方便：`float3`，`uint2`等。
-`matrix<T,R,C>`(R,C = 2-4)，方便：`float3x4`等。

# # #数组```hlsl
int a[3];            // fixed size
int a[] = {1,2,3};  // inferred size
void f(int b[]) {}  // unsized parameter
```
数组有`.getCount()`。

# # #结构```hlsl
struct MyData { int a; float b; }
// Custom constructor:
__init(int a_, float b_) { a = a_; b = b_; }
```
###参数块```hlsl
struct MaterialParams { float3 albedo; float metallic; float roughness; };
ParameterBlock<MaterialParams> gMaterial;
// Binds to a single descriptor table (D3D12) or descriptor set (Vulkan)
```
# # #进口```hlsl
import foo;                  // imports foo.slang
__exported import foo;       // re-exports foo's declarations
```
`import`≠`#include`—不共享预处理器，每个模块加载一次。

---

接口和泛型

接口定义和实现```hlsl
interface ILight
{
    LightSample sample(float3 position);
    static int  getCount();               // static method in interface
    property int id { get; set; }        // property in interface
    int compute<T>(T val) where T : IBar; // generic method in interface
}

struct PointLight : ILight
{
    float3 position;
    LightSample sample(float3 hitPos) { ... }
    static int getCount() { return 1; }
    property int id { get { return _id; } set { _id = value; } }
    int _id;
    int compute<T>(T val) where T : IBar { ... }
}
```
多重一致性```hlsl
struct MyType : IFoo, IBar { ... }
```
默认实现```hlsl
interface IFoo
{
    int getVal() { return 0; }  // default
}
struct MyType2 : IFoo
{
    override int getVal() { return 1; }
}
```
泛型方法和约束```hlsl
// Basic
float4 computeDiffuse<L : ILight>(float4 albedo, float3 P, float3 N, L light) { ... }

// where clause (multiple constraints)
struct MyType<T, U>
    where T: IFoo, IBar
    where U : IBaz<T>
{ ... }

// Simplified constraint syntax
int myMethod<T:IFoo>(T arg) { ... }

// Generic value parameters
void g<let n : int>() { ... }

// Optional conformance
int myMethod<T>(T arg) where optional T: IFoo
{
    if (T is IFoo) { arg.myMethod(1.0); }
}
```
关联类型```hlsl
interface IMaterial
{
    associatedtype B : IBRDF;
    B evalPattern(float3 pos, float2 uv);
}

struct MyCoolMaterial : IMaterial
{
    typedef DisneyBRDF B;
    B evalPattern(float3 pos, float2 uv) { ... }
}
```
全局范围的通用参数```hlsl
type_param M : IMaterial;
M gMaterial;
```
---

##自动区分

标记函数可微```hlsl
[Differentiable]
float2 foo(float a, float b) { return float2(a * b * b, a * a); }
```
###转发模式```hlsl
DifferentialPair<float> dp_a = diffPair(1.0, 1.0); // (value, derivative)
DifferentialPair<float> dp_b = diffPair(2.4, 0.0);
DifferentialPair<float2> out = fwd_diff(foo)(dp_a, dp_b);
float2 primal     = out.p;
float2 derivative = out.d;
```
###反向模式```hlsl
DifferentialPair<float> dp_a = diffPair(1.0);
DifferentialPair<float> dp_b = diffPair(2.4);
float2 dL_doutput = float2(1.0, 0.0);
bwd_diff(foo)(dp_a, dp_b, dL_doutput);
float dL_da = dp_a.d;
float dL_db = dp_b.d;
```
可微分类型
内置：`float`，`double`,`half`，vectors/matrices等，可微类型数组。```hlsl
struct MyType : IDifferentiable { float x; float y; }
```
自定义衍生工具```hlsl
[Differentiable]
[ForwardDerivative(myForwardDeriv)]
[BackwardDerivative(myBackwardDeriv)]
float myFunc(float x) { ... }
```
---

模块和访问控制

定义一个模块```hlsl
// scene.slang
module scene;
__include "scene-helpers";   // NOT preprocessor — no macro sharing

// scene-helpers.slang
implementing scene;
// all entities in module are mutually visible regardless of include order
```
模块包含语法```hlsl
__include dir.sub_file;           // → "dir/sub-file.slang"
__include "dir/sub-file.slang";
```
访问修饰符
|修改器|可见性||------------|-----------------------------------------|
|`public`|无处不在（其他文件，模块）|
|`internal`|只有相同的模块（大多数默认）|
|`private`|相同类型和嵌套类型|

规则:
-接口成员继承接口的可见性。
-遗留模块（没有`module`声明）将所有符号视为`public`。
—高可见性实体不能在其签名中暴露低可见性实体。

---

##能力系统

声明需求```hlsl
[require(spvShaderClockKHR)]
[require(glsl, GL_EXT_shader_realtime_clock)]
[require(hlsl_nvapi)]
uint2 getClock() { ... }
// Combined: (spvShaderClockKHR | glsl + GL_EXT_shader_realtime_clock | hlsl_nvapi)
```
目标开关```hlsl
void myFunc()
{
    __target_switch
    {
    case spirv: /* SPIR-V path */ break;
    case hlsl:  /* HLSL path  */ break;
    }
}
```
###能力别名```hlsl
// Use a named alias instead of spelling out the full disjunction:
[require(sm_6_6)]
void myFunc() { ... }
```
通用能力原子
-阶段：`vertex`，`fragment`,`compute`,`hull`,`domain`,`geometry`- api:`hlsl`,`glsl`,`spirv`,`cuda`,`cpp`-特性：`_sm_6_7`，`SPV_KHR_ray_tracing`,`spvShaderClockKHR`,`hlsl_nvapi`---

编译API （c++）```cpp
// 1. Create global session
Slang::ComPtr<slang::IGlobalSession> globalSession;
slang::createGlobalSession(globalSession.writeRef());

// 2. Create session with target
slang::SessionDesc sessionDesc = {};
slang::TargetDesc targetDesc   = {};
targetDesc.format   = SLANG_SPIRV;
targetDesc.profile  = SLANG_PROFILE_GLSL_450;
sessionDesc.targets      = &targetDesc;
sessionDesc.targetCount  = 1;
Slang::ComPtr<slang::ISession> session;
globalSession->createSession(sessionDesc, session.writeRef());

// 3. Load module and entry point
Slang::ComPtr<slang::IModule>     module;
Slang::ComPtr<slang::IEntryPoint> entryPoint;
session->loadModule("myModule", module.writeRef());
module->findEntryPointByName("main", entryPoint.writeRef());

// 4. Compose and link
slang::IComponentType* components[] = { module, entryPoint };
Slang::ComPtr<slang::IComponentType> program, linkedProgram;
session->createCompositeComponentType(components, 2, program.writeRef());
program->link(linkedProgram.writeRef());

// 5. Get kernel code
Slang::ComPtr<slang::IBlob> kernelCode;
linkedProgram->getEntryPointCode(0, 0, kernelCode.writeRef());
```
### CMake integration```cmake
find_package(slang REQUIRED PATHS ${CMAKE_INSTALL_PREFIX} NO_DEFAULT_PATH)
target_link_libraries(yourLib PUBLIC slang::slang)
```
---

##反射API```cpp
slang::ProgramLayout* layout = program->getLayout(targetIndex);

// Enumerate global parameters
int paramCount = layout->getParameterCount();
for (int i = 0; i < paramCount; i++)
{
    slang::VariableLayoutReflection* param = layout->getParameterByIndex(i);
    const char* name = param->getName();
    int binding      = param->getBindingIndex();
    int space        = param->getBindingSpace();
}

// Entry point layouts (stage, varying params)
slang::EntryPointLayout* ep = layout->getEntryPointByIndex(0);
slang::Stage stage = ep->getStage();
```
类型反射种类值：`Scalar`、`Vector`、`Matrix`、`Array`、`Struct`、`Resource`、`SamplerState`等。

---

##编译目标

d3d11 （dxbc）
阶段：`vertex`、`hull`、`domain`、`geometry`、`fragment`寄存器：`b`(cbuffers),`t`(srv),`u`（无人机），`s`（采样器）

### d3d12 （dxil）
增加：光线追踪（`raygeneration`,`closesthit`,`miss`,`anyhit`,`intersection`,`callable`）
根签名：根常量，描述符表，根描述符。

Vulkan （SPIR-V）
描述符集而不是表。推送常量而不是根常量。```hlsl
[[vk::binding(0, 1)]] Texture2D myTexture;
[[vk::push_constant]]  cbuffer PC { float4 color; };
[[vk::shader_record]]  cbuffer SR { uint id; };
```
# # # CUDA
-本地指针支持，合作组，张量操作。
-没有图形流水线阶段，有限的纹理操作。

# # #金属
-参数缓冲区，基于tile的优化，统一内存。
-没有双重类型。

# # #CPU/C+ +
-主机端执行调试和参考实现。
—无gpu相关特性。

---

目标兼容性矩阵

|特性| D3D11 | D3D12 | Vulkan | CUDA | Metal | CPU ||------------------------|:-----:|:-----:|:------:|:----:|:-----:|:---:|
★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
|`double`类型|✓|✓|✓|✓|·|✓|
✓|✓|✓|✓|
|`u/int16_t`|✓|✓|✓|✓|✓|✓|
|`u/int64_t`|·|✓|✓|✓|✓|✓|
|波本质（SM6） |·|✓|部分|·|·|·|
光线追踪|、|、|、|、|、|、|
|网格着色器|、|、|、|、|、|、|
镶嵌贴图|✓|✓|·|·|·|·|
|图形管道|✓|✓|✓|·|✓|·|
本机无绑定|、|、|、|、|、|、|
|原子|✓|✓|✓|✓|✓|✓|
|指针| / |你的目标是什么？你的目标是什么* *注:平台* *
镶嵌：在Vulkan上不可用（通过网格着色器使用镶嵌）。
-一半的D3D12:`StructuredBuffer<half>`的问题-避免。
- CUDA上的波特性：初步，使用合成的WaveMask。
- D3D上的8/16-bit整数：需要特定的着色器模型和DXIL标志。

---

命令行```bash
# Basic
slangc shader.slang -target spirv -o shader.spv
slangc shader.slang -target dxil  -o shader.dxil
slangc shader.slang -target glsl  -o shader.glsl
slangc shader.slang -target cuda  -o shader.cu
slangc shader.slang -target metal -o shader.metal
slangc shader.slang -target cpp   -o shader.cpp

# Multi-target
slangc shader.slang -target spirv -o shader.spv -target dxil -o shader.dxil

# Entry point and stage
slangc shader.slang -target spirv -entry mainCS -stage compute -o out.spv

# Profile
slangc shader.slang -target glsl  -profile glsl_460 -o out.glsl
slangc shader.slang -target dxil  -profile sm_6_7   -o out.dxil

# Matrix layout (important for row-major engines like xMath)
slangc shader.slang -target spirv -matrix-layout-row-major -o out.spv

# Optimization
slangc shader.slang -O0   # no optimization
slangc shader.slang -O2   # standard
slangc shader.slang -O3   # aggressive

# Debug info
slangc shader.slang -g -o out.spv

# Include paths and macros
slangc shader.slang -I./include -DENABLE_SHADOWS=1 -o out.spv

# Capabilities
slangc shader.slang -capability spvShaderClockKHR -target spirv -o out.spv

# Vulkan-specific
slangc shader.slang -target spirv -fvk-use-entrypoint-name -o out.spv
slangc shader.slang -target spirv -fvk-use-gl-layout       -o out.spv

# Precompiled modules
slangc shader.slang -r prebuilt.slang-module -target spirv -o out.spv

# Emit IR
slangc shader.slang -emit-ir -o shader.slang-module
```
优化关卡
b|标志|效果||------|-------------------------------------|
|`-O0`|无优化（调试）|
|`-O1`|基本优化|
|`-O2`|标准优化（默认）|
|`-O3`|激进（可能增加编译时间）|`-stage`的艺名`vertex`·`fragment`·`compute`·`hull`·`domain`·`geometry``raygeneration`·`closesthit`·`miss`·`anyhit`·`intersection`·`callable`