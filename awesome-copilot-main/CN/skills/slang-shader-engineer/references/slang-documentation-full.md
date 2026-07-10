#俚语语言文档-完整参考

来源：[官方俚语着色库]（https://github.com/shader-slang/slang“俚语着色库”）

##目录-[项目概况]（# Project - Overview）
- [Introduction and Why Use - Slang]（# Introduction -and- Why Use - Slang）
- [Getting Started]（# Getting - Started）
-[语言功能]（# Language - Features）
-[常规功能]（# Conventional - Features）
-[接口和泛型]（# Interfaces -and- Generics）
-[自动区分]（# Automatic - Differentiation）
-[模块和访问控制]（# Modules -and- Access - Control）
-[能力系统]（# Capabilities - System）
-[编译带有俚语的代码]（# Compiling - Code -with- Slang）
-[反射API]（#reflect - API）
-[编译目标]（# compile - Targets）
-[目标兼容性]（# Target - Compatibility）
-[命令行参考]（# Command - Line - Reference）
- [Building From Source]（# Building - From - Source）
——(FAQ)(#常见问题)

##项目概述俚语是一种实时着色语言，旨在提高GPU编程的开发效率，同时保持高性能。它扩展HLSL与现代编程语言的功能，并支持编译到多个目标平台，包括Direct3D， Vulkan， CUDA, Metal，和CPU。

关键好处:

-向后兼容大多数现有的hsl代码
-参数块用于有效的描述符表使用
-类型安全着色器专门化的接口和泛型
-机器学习应用的自动微分
-模块系统，更好的代码组织
-综合反射API
-跨平台编译到多个目标

介绍和为什么使用俚语

为什么要用俚语？俚语系统帮助实时图形开发人员编写更清晰、更易于维护的GPU代码，而不会牺牲运行时性能。俚语扩展了HLSL语言，从现代通用语言中精心挑选了一些特性，这些特性支持提高开发人员的生产力和代码质量。

俚语的一些好处包括：- **向后兼容性**：俚语是向后兼容大多数现有的hsl代码
- **参数块**：允许着色器参数按更新率分组，以利用Direct3D 12描述符表和Vulkan描述符集
**接口和泛型**：提供基于预处理器的着色器专门化的一流替代方案
- **自动区分**：大大简化了在着色器中基于学习的技术的实现
- **模块系统**：启用真正的单独编译和着色器代码的语义检查
- **多平台支持**：相同的编译器生成DX字节码，DXIL, SPIR-V, hsl， GLSL， CUDA等代码
- **健壮的反射API**：以一致的格式提供关于着色器参数的binding/offset/layout信息

俚语指的是谁？俚语的目标是成为实时图形开发人员关注代码质量、可移植性和性能的最佳语言。

####实时图形开发人员

俚语主要是为开发人员创建运行在end-user/client机器上的实时图形应用程序，如3D游戏和数字内容创建（DCC）工具。

####从业余爱好者到专业人士

对于业余开发者来说，俚语语言非常简单和熟悉，但是对于专业开发团队创建下一代游戏渲染器的需求来说，它可以扩展。

####多平台应用程序开发人员

俚语系统为多个操作系统构建，支持许多图形api，并与来自多个硬件供应商的gpu一起工作。这个项目是完全开源的。

####拥有现有HLSL投资的开发人员俚语的关键特性之一是它与现有hsl代码的高度兼容性。随着时间的推移，开发人员可以逐步采用俚语特性来提高代码库的质量。

目标和非目标

主要设计目标：

**性能**：使用俚语的好处不能以性能为代价
**生产力**：语言概念在大型代码库中培养了更高的开发人员生产力
- **可移植性**：支持各种硬件，图形api和操作系统
- **易于采用**：与现有代码兼容，熟悉其他语言的语法
**可预测性**：代码应该做它看起来要做的事，跨平台一致
- **有限的范围**：俚语是一种语言，编译器和模块-而不是引擎或框架

##开始

# # #的安装开始使用俚语最简单的方法是从GitHub存储库下载一个二进制版本。提取文件并在`/bin/windows-x64/release/`下找到`slangc.exe`。请注意，`slang.dll`和`slang-glslang.dll`必须位于同一目录下。

对于从源代码构建，请参见[从源代码构建]（#building-from-source）小节。

你的第一个俚语着色器

创建名为`hello-world.slang`的文件：```hlsl
// hello-world.slang
StructuredBuffer<float> buffer0;
StructuredBuffer<float> buffer1;
RWStructuredBuffer<float> result;

[shader("compute")]
[numthreads(1,1,1)]
void computeMain(uint3 threadId : SV_DispatchThreadID)
{
    uint index = threadId.x;
    result[index] = buffer0[index] + buffer1[index];
}
```
编译成SPIR-V：```bat
slangc hello-world.slang -target spirv -o hello-world.spv
```
编译成GLSL：```bat
slangc hello-world.slang -target glsl -o hello-world.glsl
```
##语言特性

导入声明

俚语引入`import`声明以实现更好的软件模块化：```hlsl
// foo.slang
float4 someFunc(float4 x) { return x; }

// bar.slang
import foo;
float4 someOtherFunc(float4 y) { return someFunc(y); }
```
关键细节:

—Import使用与`#include`相同的搜索路径搜索`.slang`文件
—同一文件的多个导入只处理一次
-没有自动命名空间（可能发生名称冲突）
—使用`__exported import`重新导出声明
-导入不像`#include`-没有预处理宏共享

显式参数块

俚语使用描述符tables/sets支持参数块的显式语法：```hlsl
struct ViewParams
{
    float3 cameraPos;
    float4x4 viewProj;
    TextureCube envMap;
}

ParameterBlock<ViewParams> gViewParams;
```
字段被分配给registers/bindings以支持分配到单个参数块中。

# # #接口

俚语支持声明用户定义的`struct`类型可以实现的`interface`：```hlsl
// Interface definition
struct LightSample { float3 intensity; float3 direction; };

interface ILight
{
    LightSample sample(float3 position);
}

// Implementation
struct PointLight : ILight
{
    float3 position;
    float3 intensity;
  
    LightSample sample(float3 hitPos)
    {
        float3 delta = hitPos - position;
        float distance = length(delta);
      
        LightSample sample;
        sample.direction = delta / distance;
        sample.intensity = intensity * falloff(distance);
        return sample;
    }
}
```
# # #泛型

俚语支持使用尖括号语法的泛型声明：```hlsl
float4 computeDiffuse<L : ILight>(float4 albedo, float3 P, float3 N, L light)
{
    LightSample sample = light.sample(P);
    float nDotL = max(0, dot(N, sample.direction));
    return albedo * nDotL;
}
```
####全局范围的通用参数

为了与现有的HLSL全局声明兼容：```hlsl
type_param M : IMaterial;
M gMaterial;
```
####关联类型

对于每个实现类型都需要自己选择的中间类型的情况：```hlsl
interface IMaterial
{
    associatedtype B : IBRDF;
    B evalPattern(float3 position, float2 uv);
}

struct MyCoolMaterial : IMaterial
{
    typedef DisneyBRDF B;
    B evalPattern(float3 position, float2 uv) { ... }
}
```
##常规功能

# # #类型

俚语支持传统的着色语言类型，包括标量、向量、矩阵、数组、结构、枚举和资源。

####标量类型

整数类型:

-`int8_t`,`int16_t`,`int`,`int64_t`-`uint8_t`,`uint16_t`,`uint`,`uint64_t`浮点类型:

-`half`（16位）
-`float`（32位）
-`double`（64位）

布尔类型：`bool`无效类型：`void`####矢量类型

向量类型：`vector<T,N>`，其中T为标量类型，N为2-4
方便名称：`float3`=`vector<float,3>`####矩阵类型

矩阵类型：`matrix<T,R,C>`，其中T为标量，R、C为2-4
方便名称：`float3x4`=`matrix<float,3,4>`####数组类型

数组类型`T[N]`表示T类型的N个元素的数组：```hlsl
int a[3];           // sized array
int a[] = {1,2,3};  // inferred size
void f(int b[]) {}  // unsized array parameter
```
数组有返回长度的`getCount()`方法。

####结构类型`struct`关键字结构类型：```hlsl
struct MyData
{
    int a;
    float b;
}
```
结构可以有用`__init`关键字定义的构造函数。

接口和泛型

# # #接口

接口定义了类型应该提供的方法和服务：```hlsl
interface IFoo
{
    int myMethod(float arg);
}

struct MyType : IFoo
{
    int myMethod(float arg)
    {
        return (int)arg + 1;
    }
}
```
####多接口一致性```hlsl
interface IBar { uint myMethod2(uint2 x); }

struct MyType : IFoo, IBar
{
    int myMethod(float arg) {...}
    uint myMethod2(uint2 x) {...}
}
```
####默认实现```hlsl
interface IFoo
{
    int getVal() { return 0; }  // default implementation
}

struct MyType : IFoo {}  // uses default

struct MyType2 : IFoo
{
    override int getVal() { return 1; }  // explicit override
}
```
# # #泛型

泛型方法消除共享逻辑的重复代码：```hlsl
int myGenericMethod<T>(T arg) where T : IFoo
{
    return arg.myMethod(1.0);
}

// Usage
MyType obj;
int a = myGenericMethod<MyType>(obj); // explicit type
int b = myGenericMethod(obj);         // type deduction
```
####通用值```hlsl
void g1<let n : int>() { ... }

enum MyEnum { A, B, C }
void g2<let e : MyEnum>() { ... }

void g3<let b : bool>() { ... }
```
####可选语法```hlsl
__generic<typename T>
int myGenericMethod(T arg) where T : IFoo { ... }

// Simplified syntax
int myGenericMethod<T:IFoo>(T arg) { ... }
```
####多重约束```hlsl
struct MyType<T, U>
    where T: IFoo, IBar
    where U : IBaz<T>
{
}
```
####可选的一致性```hlsl
int myGenericMethod<T>(T arg) where optional T: IFoo
{
    if (T is IFoo)
    {
        arg.myMethod(1.0); // OK in conformance check block
    }
}
```
支持的接口结构

# # # #属性```hlsl
interface IFoo
{
    property int count {get; set;}
}
```
####泛型方法```hlsl
interface IFoo
{
    int compute<T>(T val) where T : IBar;
}
```
####静态方法```hlsl
interface IFoo
{
    static int compute(int val);
}
```
##自动区分

俚语通过自动区分为可微分编程提供一流的支持。

关键特性

-`fwd_diff`和`bwd_diff`算子用于前向和后向模式导数传播
-`DifferentialPair<T>`类型用于传递带有输入的导数
-可微类型的`IDifferentiable`和`IDifferentiablePtrType`接口
-用户定义的衍生函数，通过`[ForwardDerivative]`和`[BackwardDerivative]`-兼容所有俚语功能：控制流，泛型，接口等。

数学背景

前向模式计算雅可比向量积：`<Df(x), v>`后向模式计算向量雅可比积：`<v^T, Df(x)>`前向模式示例```hlsl
[Differentiable]
float2 foo(float a, float b) 
{ 
    return float2(a * b * b, a * a);
}

void main()
{
    DifferentialPair<float> dp_a = diffPair(1.0, 1.0);  // value and derivative
    DifferentialPair<float> dp_b = diffPair(2.4, 0.0);
  
    DifferentialPair<float2> dp_output = fwd_diff(foo)(dp_a, dp_b);
  
    float2 output_p = dp_output.p;  // primal output
    float2 output_d = dp_output.d;  // derivative output
}
```
向后模式示例```hlsl
[Differentiable]
float2 foo(float a, float b) 
{ 
    return float2(a * b * b, a * a);
}

void main()
{
    DifferentialPair<float> dp_a = diffPair(1.0);
    DifferentialPair<float> dp_b = diffPair(2.4);
  
    float2 dL_doutput = float2(1.0, 0.0);  // output derivatives
  
    bwd_diff(foo)(dp_a, dp_b, dL_doutput);
  
    float dL_da = dp_a.d;  // computed input derivatives
    float dL_db = dp_b.d;
}
```
可微型系统

####内置可微分类型

—标量：`float`，`double`,`half`-Vectors/matrices的可微标量
—数组：如果`T`可导，则为`T[n]`-元组：`Tuple<each T>`，如果`T`是可微的

####用户定义的可微分类型```hlsl
struct MyType : IDifferentiable
{
    float x;
    float y;
}
```
`Differential`关联类型带有相应的导数（通常与原始类型相同）。

自定义衍生工具```hlsl
[Differentiable]
[ForwardDerivative(myForwardDerivative)]
[BackwardDerivative(myBackwardDerivative)]
float myFunction(float x) { ... }
```
模块和访问控制

定义一个模块

模块由一个或多个文件组成，其中主文件包含`module`声明：```hlsl
// scene.slang
module scene;

__include "scene-helpers";
```

```hlsl
// scene-helpers.slang
implementing scene;
// ...
```
####模块包含语义`__include`与`#include`不同：

1. 文件之间没有预处理器状态共享
2. 每个文件只包含一次
3. 通告包括允许的
4. 无论包含顺序如何，所有模块文件都可以访问所有其他实体

####模块参考语法

支持标识符和字符串字面语法：```hlsl
__include dir.file_name;           // translated to "dir/file-name.slang"
__include "dir/file-name.slang";
__include "dir/file-name";
```
导入模块```hlsl
// MyShader.slang
import YourLibrary;
```
导入规则:

—只能导入主模块文件
-同一模块的多个导入一次加载
-文件之间没有预处理器共享

访问控制

三个能见度级别：

# # # #`public`可访问的任何地方-不同的类型，文件，模块

# # # #`private`仅在同一类型中可见：```hlsl
struct MyType
{
    private int member;
  
    int f() { member = 5; }  // OK
  
    struct ChildType
    {
        int g(MyType t) { return t.member; }  // OK
    }
}

void outerFunc(MyType t)
{
    t.member = 2;  // Error - not visible
}
```
# # # #`internal`在同一模块中可见：```hlsl
// a.slang
module a;
public struct PS
{
    internal int internalMember;
    public int publicMember;
}
internal void f() { ... }

// m.slang  
module m;
import a;
void main()
{
    f();  // Error - f is internal to module a
    PS p;
    p.internalMember = 1;  // Error - not visible outside module a
    p.publicMember = 1;    // OK
}
```
默认可见性是`internal`（除非接口成员继承接口可见性）。

####验证规则

—签名中可见度高的实体不能暴露可见度低的实体
—成员的可见度不能高于父成员
—类型定义不能为`private`—接口要求不能为`private`旧模块兼容性

没有`module`声明、`__include`或可见性修饰符的模块被视为带有所有符号`public`的遗留模块。

##能力系统

功能系统有助于管理不同gpu、图形api和着色器阶段的硬件功能差异。

能力原子和需求

能力原子表示目标、阶段、扩展和特性：

-`GLSL_460`- GLSL 460目标
-`compute`-计算着色器阶段
-`_sm_6_7`- shader模型6.7功能
-`SPV_KHR_ray_tracing`- SPIR-V扩展
-`spvShaderClockKHR`- SPIR-V能力声明需求```hlsl
[require(spvShaderClockKHR)]
[require(glsl, GL_EXT_shader_realtime_clock)]
[require(hlsl_nvapi)]
uint2 getClock() {...}
```
这就产生了需求：```
(spvShaderClockKHR | glsl + GL_EXT_shader_realtime_clock | hlsl_nvapi)
```
冲突的能力

有些功能是互斥的：

-不同的代码生成目标（`hlsl`,`glsl`）
-不同的着色器阶段（`vertex`,`fragment`）

具有冲突原子的需求是不相容的。

父作用域要求

需求与父作用域合并：```hlsl
[require(glsl)]
[require(hlsl)]
struct MyType
{
    [require(hlsl, hlsl_nvapi)]
    [require(spirv)]
    static void method() { ... }  // requirement: glsl | hlsl + hlsl_nvapi | spirv
}
```
自动推理

俚语推断了`internal`/`private`函数的需求：```hlsl
void myFunc()
{
    if (getClock().x % 1000 == 0)
        discard;  // requires fragment stage
}
// Inferred: (spirv + SPV_KHR_shader_clock + spvShaderClockKHR + fragment | ...)
```
目标开关`__target_switch`引入析取：```hlsl
void myFunc()
{
    __target_switch
    {
    case spirv: ...;
    case hlsl: ...;
    }
}
// Requirement: (spirv | hlsl)
```
###能力别名

别名简化了跨平台需求：```hlsl
alias sm_6_6 = _sm_6_6
             | glsl_spirv_1_5 + sm_6_5 + GL_EXT_shader_atomic_int64 + atomicfloat2
             | spirv_1_5 + sm_6_5 + GL_EXT_shader_atomic_int64 + atomicfloat2 + SPV_EXT_descriptor_indexing
             | cuda
             | cpp;
```
用法:`[require(sm_6_6)]`# # #验证

-公共方法和接口方法需要显式的能力声明
-经过验证的功能不会使用超出声明要求的功能
-入口点功能推荐，但不是必需的

用俚语编译代码

# # #的概念

####源单元和翻译单元

源单元（files/strings）被分组为翻译单元。每个翻译单元在编译时产生一个单独的模块。

####入口点

入境点可通过以下途径确定：

1.`[shader(...)]`属性（推荐）
2. 显式的兼容性入口点选项

# # # #的目标

目标代表平台和功能：

-格式：SPIR-V， DXIL等。
-配置文件：D3D着色器模型5.1,GLSL 4.60等。
—可选功能：Vulkan扩展
-代码生成选项

# # # #布局

参数布局取决于：-模块和入口点一起使用
-参数排序
—针对目标的规则和约束

# # # #组成

组件类型（模块，入口点）可以组成复合材料，定义着色器代码的单元，以便一起使用。

# # # #链接

解析跨模块引用并为目标代码生成生成自包含的IR模块。

# # # #内核

入口点生成内核代码。相同的入口点可以为不同的目标和组合生成不同的核。`slangc`命令行编译

####简单示例```bat
slangc hello-world.slang -target spirv -o hello-world.spv
```
####源文件和翻译单元

—每个输入文件都是一个独立的源单元
-`.slang`文件分组到单个翻译单元
-每个`.hlsl`文件都有自己的翻译单元

####常用选项

-`-target <format>`：指定输出格式（spirv, dxil, hlsl, glsl, cuda, cpp）
—`-entry <name>`：指定入口点函数名
—`-profile <profile>`：指定目标配置文件
—`-o <file>`：指定输出文件
-`-D<name>[=<value>]`：定义预处理宏
—`-I<path>`：添加include搜索路径

####多目标

在一次调用中为多个目标编译：```bat
slangc shader.slang -target spirv -o shader.spv -target dxil -o shader.dxil
```
####参数绑定

俚语提供了跨目标的确定性参数绑定。生成的代码包括明确的绑定布局，以确保参数位置的一致性。

###使用编译API

对于需要运行时编译的应用程序：```cpp
// Create session
Slang::ComPtr<slang::IGlobalSession> globalSession;
slang::createGlobalSession(globalSession.writeRef());

slang::ComPtr<slang::ISession> session;
slang::SessionDesc sessionDesc;
sessionDesc.targetCount = 1;
slang::TargetDesc targetDesc;
targetDesc.format = SLANG_SPIRV;
targetDesc.profile = SLANG_PROFILE_GLSL_450;
sessionDesc.targets = &targetDesc;
globalSession->createSession(sessionDesc, session.writeRef());

// Load module
slang::ComPtr<slang::IModule> module;
session->loadModule("myModule", module.writeRef());

// Create entry point
slang::ComPtr<slang::IEntryPoint> entryPoint;
module->findEntryPointByName("main", entryPoint.writeRef());

// Compose program
slang::ComPtr<slang::IComponentType> program;
slang::IComponentType* components[] = { module, entryPoint };
session->createCompositeComponentType(components, 2, program.writeRef());

// Compile
slang::ComPtr<slang::IComponentType> linkedProgram;
program->link(linkedProgram.writeRef());

// Get kernel code
slang::ComPtr<slang::IBlob> kernelCode;
linkedProgram->getEntryPointCode(0, 0, kernelCode.writeRef());
```
##反射API

为反射编译```cpp
slang::IComponentType* program = ...;
slang::ProgramLayout* programLayout = program->getLayout(targetIndex);
```
类型和变量

# # # #变量`VariableReflection`表示变量声明：```cpp
void printVariable(slang::VariableReflection* variable)
{
    const char* name = variable->getName();
    slang::TypeReflection* type = variable->getType();
  
    print("name: "); printQuotedString(name);
    print("type: "); printType(type);
}
```
# # # #类型`TypeReflection`表示程序中的类型：```cpp
void printType(slang::TypeReflection* type)
{
    const char* name = type->getName();
    slang::TypeReflection::Kind kind = type->getKind();
  
    print("name: "); printQuotedString(name);
    print("kind: "); printTypeKind(kind);
  
    switch(type->getKind())
    {
    case slang::TypeReflection::Kind::Scalar:
        print("scalar type: ");
        printScalarType(type->getScalarType());
        break;
      
    case slang::TypeReflection::Kind::Struct:
        print("fields:");
        int fieldCount = type->getFieldCount();
        for (int f = 0; f < fieldCount; f++)
        {
            slang::VariableReflection* field = type->getFieldByIndex(f);
            printVariable(field);
        }
        break;
      
    case slang::TypeReflection::Kind::Array:
        print("element count: ");
        printPossiblyUnbounded(type->getElementCount());
        print("element type: ");
        printType(type->getElementType());
        break;
    }
}
```
参数布局

参数布局描述了如何将参数映射到特定于目标的资源：```cpp
void printParameterLayout(slang::ParameterLayout* parameterLayout)
{
    slang::VariableReflection* variable = parameterLayout->getVariable();
    printVariable(variable);
  
    // Print binding information
    int bindingRangeCount = parameterLayout->getBindingRangeCount();
    for (int r = 0; r < bindingRangeCount; r++)
    {
        slang::BindingRangeType rangeType = parameterLayout->getBindingRangeType(r);
        int rangeIndex = parameterLayout->getBindingRangeIndex(r);
        int rangeSpace = parameterLayout->getBindingRangeSpace(r);
      
        print("binding: ");
        printBindingRangeType(rangeType);
        printf(" index=%d space=%d", rangeIndex, rangeSpace);
    }
}
```
入口点布局

入口点布局提供关于变化的inputs/outputs及其特定于阶段的语义的信息：```cpp
void printEntryPointLayout(slang::EntryPointLayout* entryPointLayout)
{
    slang::Stage stage = entryPointLayout->getStage();
    print("stage: "); printStage(stage);
  
    // Print varying parameters
    int varyingCount = entryPointLayout->getVaryingParamCount();
    for (int v = 0; v < varyingCount; v++)
    {
        slang::VaryingParameterReflection* varying = 
            entryPointLayout->getVaryingParamByIndex(v);
        printVaryingParameter(varying);
    }
}
```
##编译目标

### Direct3D

D3D11使用DirectX字节码（DXBC）格式。支持栅格化和计算管道。

####光栅化管道阶段

-`vertex`(VS) -必选
-`hull`(HS) -可选镶嵌
-`domain`(DS) -可选镶嵌
—`geometry`（GS）—可选
-`fragment`/`pixel`(PS) -可选

####参数传递

每个阶段都有专用插槽：

- **常量缓冲区**:`b`寄存器，≤4KB统一数据
- **Shader资源视图(srv)**:`t`寄存器，只读资源
- **无序访问视图（无人机）**:`u`寄存器，读写资源
**Samplers**:`s`寄存器，纹理采样状态

### Direct3D

D3D12采用DXIL （DirectX Intermediate Language）格式。增加光线跟踪和网格着色器支持。

####额外的管道级

**网格着色器**（俚语中尚未支持）：-`amplification`-决定网格着色器调用`mesh`-生成网格的顶点和索引数据

**光线追踪管道**：

-`raygeneration`-跟踪射线，类似于计算
-`intersection`-自定义基元交集
-`anyhit`-候选命中acceptance/rejection-`closesthit`表示接受命中的进程
-`miss`-处理错过几何的光线
-`callable`-用户自定义子程序

####参数传递

使用根签名：

—**根常量**：小数据直接参数传递
—**描述符表**：资源的描述符组
- **根描述符**：直接描述符绑定

# # #凡尔康

使用SPIR-V中间表示。类似于D3D12的功能。

####主要特性

-描述符集而不是描述符表
-推常量而不是根常量
-广泛推广系统
-跨厂商标准化

#### vulkan特有的属性```hlsl
[[vk::binding(0, 1)]]
Texture2D myTexture;

[[vk::push_constant]]
cbuffer PushConstants
{
    float4 color;
}

[[vk::shader_record]]
cbuffer ShaderRecord
{
    uint shaderRecordID;
}
```
# # # CUDA

编译到CUDA c++或PTX。通过特定gpu的优化支持计算工作负载。

####主要特性

本地指针支持
-丰富的数学库
-合作小组
张量运算

# # # #的局限性

-没有图形流水线阶段
-有限的纹理操作
—不同的wave/warp型号

# # #金属

苹果的图形API和着色语言。

####主要特性

-参数缓冲区用于参数传递
-基于tile的渲染优化
-统一内存模型
-iOS/macOS支持

# # #CPU/C+ +

生成用于CPU执行的c++代码。

####主要特性

-主机端着色器执行
-调试和测试
-参考实现
—跨平台部署

# # # #的局限性

—无gpu相关特性
-限制并行执行
-不同的内存模型

##目标兼容性

针对不同目标的俚语特征的综合兼容性矩阵：###数据类型

|特性| D3D11 | D3D12 | Vulkan | CUDA | Metal | CPU || ----------- | ----- | ----- | ------ | ---- | ----- | --- |
|半类型|否|是|是|是|是|否|
|双类型|是|是|是|是|否|是|
|u/int8_t|否|否|是|是|是|是|
|u/int16_t|否|是|是|是|是|是|
|u/int64_t|否|是|是|是|是|是|

Shader特性

|特性| D3D11 | D3D12 | Vulkan | CUDA | Metal | CPU || --------------------- | ----- | ----- | ------- | ---- | ----- | --- |
| SM6.0 Wave intrinsic |否|是| Partial |是|否|否|
|光线追踪DXR 1.0 |否|是|是|否|否|否|
|网格着色器|否|是|是|否|是|否|
|镶嵌|是|是|否|否|否|否|
|图形管道|是|是|是|否|是|否|

资源特性

|特性| D3D11 | D3D12 | Vulkan | CUDA | Metal | CPU || ---------------------- | ----- | ----- | ------ | ------- | ----- | ------- |
|本机无绑定|否|否|否|是|否|是|
|缓冲区边界检查|是|是|是|有限|否|有限|
|单独采样|是|是|是|否|是|是|
|原子|是|是|是|是|是|是|

特定于平台的注释

####半型

- D3D12: StructuredBuffer包含一半的问题
—CUDA：需要cuda_fp16.h可用性

####整数类型

-D3D11/D3D12:8/16-bit类型需要特定的着色器模型和DXIL
- Vulkan：需要显式算术类型扩展

####波的本质

CUDA：初步支持合成WaveMask
—不同的硬件能力会影响可用性

####光线追踪

- Vulkan：使用shader记录代替本地根签名
- D3D12：完整的DXR1.0/1.1支持

####无限资源CUDA：原生支持纹理对象
-其他目标：需要大量的手工工作

命令行参考

###一般选项

# # # #`-D<name>[=<value>]`插入预处理器宏。如果未指定值，则定义空宏。

# # # #`-entry <name>`指定入口点函数名。如果指定了阶段，默认为`main`。允许多个条目。

# # # #`-o <file>`指定输出文件路径。

# # # #`-target <format>`指定编译目标格式：

-`spirv`- Vulkan的SPIR-V
-`dxil`- D3D12的DXIL
-`dxbc`- DXBC适用于D3D11
-`hlsl`- HLSL源输出
-`glsl`- GLSL源输出
-`cuda`- CUDA c++输出
-`cpp`- c++输出
-`metal`—金属输出

# # # #`-profile <profile>`指定目标profile/version：

-`glsl_450`，`glsl_460`- GLSL版本
-`sm_5_0`,`sm_6_0`，`sm_6_7`- Shader模型版本

# # # #`-stage <stage>`指定着色器阶段：-`vertex`,`fragment`,`compute`-`hull`,`domain`,`geometry`-`raygeneration`,`closesthit`,`miss`,`anyhit`,`intersection`,`callable`包括和模块选项

# # # #`-I<path>`添加目录以包含搜索路径。

# # # #`-r <module>`参考预编译模块。

优化选项

# # # #`-O<level>`设置优化级别：

-`-O0`-没有优化
-`-O1`-基本优化
-`-O2`-标准优化
-`-O3`-激进优化

# # # #`-g`生成调试信息。

# # # #`-line-directive-mode <mode>`控制线指令生成：

-`none`-没有行指令
-`default`-标准行指令
-`source-map`-源映射支持

特定于目标的选项

#### Vulkan期权

# # # #`-fvk-use-entrypoint-name`使用SPIR-V OpEntryPoint的入口点名称。

# # # #`-fvk-use-gl-layout`使用opengl风格的内存布局规则。

#### CUDA选项

# # # #`-cuda-sm <version>`指定CUDA计算能力（例如，7.0的`70`）。

#### CPU选项

# # # #`-fpic`生成与位置无关的代码。

###高级选项

# # # #`-capability <cap>`指定编译所需的功能。

# # # #`-matrix-layout-column-major`使用列主矩阵布局。

# # # #`-matrix-layout-row-major`使用行为主矩阵布局。

# # # #`-emit-ir`输出中间表示（. lang-module）。

# # # #`-load-stdlib-from <path>`从指定路径加载标准库。

# # # #`-no-stdlib`不要自动导入标准库。

##从源代码构建

# # #先决条件

要求:

- CMake（3.26优先，最低3.22）
支持c++ 17的c++编译器（GCC, Clang， MSVC）
- CMake兼容后端（Visual Studio, Ninja）
Python3（用于spirv-tools依赖）

可选的测试：

——CUDA
——OptiX
——NVAPI
- - - - - -后果
——X11

获取源代码```bash
git clone https://github.com/shader-slang/slang --recursive
```
###配置和构建

#### Ninja Build（所有平台）```bash
cmake --preset default
cmake --build --preset releaseWithDebugInfo
```
#### Visual Studio```bash
cmake --preset vs2022
cmake --build --preset releaseWithDebugInfo
```
可用的预设:

-`debug`-调试构建
-`release`-发布版本
-`releaseWithDebugInfo`-发布与调试信息

####完整工作流程```bash
cmake --workflow --preset release  # Configure, build, and package
```
WebAssembly Build

需要Emscripten SDK：```bash
# Install and activate Emscripten
git clone https://github.com/emscripten-core/emsdk.git
cd emsdk
./emsdk install latest
./emsdk activate latest

# Build generators for build platform
cmake --workflow --preset generators --fresh
mkdir generators  
cmake --install build --prefix generators --component generators

# Configure and build for WebAssembly
source ../emsdk/emsdk_env
emcmake cmake -DSLANG_GENERATORS_PATH=generators/bin --preset emscripten -G "Ninja"
cmake --build --preset emscripten --target slang-wasm
```
# # #测试```bash
build/Debug/bin/slang-test
```
有关全面的测试信息，请参阅俚语测试文档。

# # #的安装```bash
cmake --build . --target install
```
这将为`find_package`支持安装`SlangConfig.cmake`：```cmake
find_package(slang REQUIRED PATHS ${CMAKE_INSTALL_PREFIX} NO_DEFAULT_PATH)
target_link_libraries(yourLib PUBLIC slang::slang)
```
# # #交叉编译

对于交叉编译场景：

1. 用于构建平台的构建生成器
2. 使用目标平台工具链进行配置
3. 为目标平台构建

### CMake Options

关键配置选项：

|可选项|默认值|描述|| ------------------------- | ------- | -------------------------------- |
|`SLANG_ENABLE_TESTS`| ON |构建测试套件|
|`SLANG_ENABLE_EXAMPLES`| ON |构建示例程序|
|`SLANG_ENABLE_GFX`| ON |构建图形抽象层|
|`SLANG_ENABLE_SLANGRT`| ON |构建运行库|

# #常见问题解答

这个项目是怎么开始的？

俚语项目是从“尖塔”阴影语言研究项目衍生出来的。Slang从对生产性着色器编译语言的研究中吸取了经验教训，并将其应用于更容易采用且更适合生产使用的系统。

为什么使用俚语而不是其他的hlsl到glsl的翻译？虽然像gl俚语和hlsl2glslfork这样的工具对于基本的hlsl到glsl的翻译很有用，但俚语的目标是不同的。俚语的目标不是成为“另一个HLSL到glsl的翻译器”，而是创建一种着色语言和工具链，在现有HLSL的基础上提高开发人员的生产力，同时为现有HLSL代码提供合理的采用路径。

如果您只是在寻找hlsl到glsl的翻译，现有的工具可能会满足您的需求。如果您对具有更好的模块化、类型安全性和现代语言特性的高效着色语言感兴趣，那么俚语可能值得研究。

是什么让着色语言更高效？

俚语设计的关键研究：- **着色器组件：模块化和高性能着色器开发** -显示模块化着色器开发的好处
-一个快速探索着色器优化选择的系统** -演示了着色器优化的编译器技术
- **Spark：用于图形硬件的模块化可组合着色器** -可组合着色器系统的早期工作

核心生产力改进：

-模块化**：真正的独立编译和模块系统
- **类型安全**：接口和泛型取代容易出错的预处理器hack
—**反射**：跨目标的参数绑定信息一致
- **可移植性**：单一源编译到许多目标
- **现代功能**：自动区分，参数块等。

谁在使用俚语？

目前主要用户：- **NVIDIA Falcor** - NVIDIA研究院开发的实时渲染框架
- **各种游戏工作室** -采用下一代渲染器
- **研究机构** -图形和机器学习研究项目
- **独立开发者** -业余爱好和商业项目

该实现主要侧重于Falcor的需求，但被设计为广泛适用。

我们很快就会使用C/C++来制作着色器吗？

向文档化二进制中间语言（SPIR-V、DXIL）的迁移为语言创新创造了机会。虽然c++的着色器支持很有价值，但俚语解决了实时图形所特有的挑战，这些挑战无法通过c++自动改进：- **跨不同api的参数绑定复杂度**
-着色器阶段语义和验证
- **针对特定目标的优化**和功能
- **图形特定类型系统**（纹理，采样器等）
- **自动区分**基于学习的渲染

俚语是对c++着色器的补充，专注于特定领域的图形编程改进。

主要的限制是什么？

目前的限制包括：

- **与现有工具相比，有限的生态系统**
- **前沿状态** - API和语言的变化仍在发生
- **目标覆盖** -一些高级功能不可用于所有目标
- **文档** -仍在扩大高级主题的覆盖范围
-工具集成** - IDE支持改进，但不是普遍的

然而，随着项目的成熟，这些限制正在被积极地解决。语言和API有多稳定？

俚语在许多用例中都可以用于生产，但仍在不断发展：

- **核心语言功能**稳定
- **编译API**具有良好的版本稳定性
- **高级功能** （autodiff，能力）可能会看到改进
- **向后兼容性**优先考虑HLSL兼容性
- **支持增量采用** -可以逐步引入俚语特性

该项目遵循语义版本控制，并为破坏性更改提供迁移指导。

来源：[俚语着色器库文档]（https://github.com/shader-slang/slang/tree/master/docs）