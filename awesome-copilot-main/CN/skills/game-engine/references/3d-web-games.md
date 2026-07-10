# 3D网页游戏

在web上构建3D游戏的综合参考，涵盖基础理论，主要框架，着色器编程，碰撞检测和沉浸式WebXR体验。

来源：[MDN Web Docs—-游戏技术：3D Web]（https://developer.mozilla.org/en-US/docs/Games/Techniques/3D_on_the_web）

---

## 3D理论和基础

在使用任何框架之前，理解3D渲染背后的核心概念是必不可少的。

坐标系统

WebGL使用右手坐标系：

- ** x轴**——指向右边
- ** y轴**——指向上方
- ** z轴** -指向屏幕外的观众

所有3D对象都相对于这个坐标系统定位。

顶点，边缘，面和网格**顶点**——由`(x, y, z)`定义的3D空间中的一个点，具有额外的属性：颜色（RGBA，值0.0-1.0），法线（顶点面对的方向，用于照明）和纹理坐标。
- **Edge**——连接两个顶点的线。
**面**——由边（例如，连接三个顶点的三角形）包围的平面。
- **几何**——由顶点、边缘和面构建的结构形状。
- **材质**——表面外观，结合颜色、纹理、粗糙度、金属质感等。
- **网格** -几何与材料相结合，以产生可渲染的3D对象。

渲染管道

流水线将3D对象转换为屏幕上的2D像素，主要分为四个阶段：

* * 1。顶点处理* *

将单个顶点数据合并为原语（三角形，直线，点）并应用转换：-模型转换-物体在世界空间中的位置和方向。
- **视图变换**——虚拟摄像机的位置和方向。
- **投影变换** -定义相机的视野（FOV），宽高比，近平面和远平面。
- **视口转换**——将结果映射到屏幕视口。

* * 2。光栅化* *

将3D原语转换为与像素网格对齐的2D片段。

* * 3。片段处理* *

使用纹理和光照确定每个片段的最终颜色：- **纹理**:2D图像映射到3D表面。单个纹理元素称为“texels”。纹理包裹在几何图形周围重复图像；当显示分辨率与纹理分辨率不同时，纹理过滤处理缩小和放大。
- **照明（Phong模型）**：四种类型的光交互——**漫射**（像太阳一样的远距离定向光），**镜面**（像手电筒一样的点光源高光），**环境**（恒定的全局照明），和**发射**（物体本身发出的光）。

* * 4。输出合并* *

将3D片段转换为最终的2D像素网格。屏幕外和遮挡的物体被剔除以提高效率。

# # #相机

相机定义了什么是可见的：

- **Position**——在3D空间中的位置。
- **方向**——摄像机指向的位置。
- **方向**——围绕观察轴旋转。

实用技巧- WebGL中的大小和位置值是无单位的；你决定它们是代表毫米、米、英尺还是其他任何东西。
-在深入代码之前，从概念上理解管道；顶点和片段处理阶段通过着色器可编程。
-每个框架（Three.js,Babylon.js, A-Frame, PlayCanvas）都抽象了这个管道，但基本原理保持不变。

---

# #框架### Three.js
Three.js是网络上最流行的3D引擎之一。它提供了一个基于WebGL的高级API，以及一个由插件、示例和社区支持组成的大型生态系统。

# # # #设置```html
<!doctype html>
<html lang="en-GB">
  <head>
    <meta charset="utf-8" />
    <title>Three.js Demo</title>
    <style>
      html, body, canvas {
        margin: 0;
        padding: 0;
        width: 100%;
        height: 100%;
        font-size: 0;
      }
    </style>
  </head>
  <body>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/three.js/r79/three.min.js"></script>
    <script>
      const WIDTH = window.innerWidth;
      const HEIGHT = window.innerHeight;
      /* all code goes here */
    </script>
  </body>
</html>
```
或者通过npm安装：```bash
npm install --save three
npm install --save-dev vite
npx vite
```
####核心组件

**Renderer**——在浏览器中显示场景：```javascript
const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(WIDTH, HEIGHT);
renderer.setClearColor(0xdddddd, 1);
document.body.appendChild(renderer.domElement);
```
**场景**——所有3D物体、灯光和摄像机的容器：```javascript
const scene = new THREE.Scene();
```
**Camera**——定义视点（PerspectiveCamera是最常见的）：```javascript
const camera = new THREE.PerspectiveCamera(70, WIDTH / HEIGHT);
camera.position.z = 50;
scene.add(camera);
```
参数：视场（度），纵横比。其他相机类型包括正交和立方体。

####几何，材料和网格```javascript
// Geometry defines the shape
const boxGeometry = new THREE.BoxGeometry(10, 10, 10);
const torusGeometry = new THREE.TorusGeometry(7, 1, 16, 32);
const dodecahedronGeometry = new THREE.DodecahedronGeometry(7);

// Material defines the surface appearance
const basicMaterial = new THREE.MeshBasicMaterial({ color: 0x0095dd });   // No lighting
const phongMaterial = new THREE.MeshPhongMaterial({ color: 0xff9500 });   // Glossy
const lambertMaterial = new THREE.MeshLambertMaterial({ color: 0xeaeff2 }); // Matte

// Mesh combines geometry + material
const cube = new THREE.Mesh(boxGeometry, basicMaterial);
cube.position.set(-25, 0, 0);
cube.rotation.set(0.4, 0.2, 0);
scene.add(cube);
```
# # # #照明```javascript
const light = new THREE.PointLight(0xffffff);
light.position.set(-10, 15, 50);
scene.add(light);
```
其他类型的光：环境，定向，半球，斑点。

注意：`MeshBasicMaterial`不响应照明。使用`MeshPhongMaterial`或`MeshLambertMaterial`照明表面。

####动画循环```javascript
let t = 0;
function render() {
  t += 0.01;
  requestAnimationFrame(render);

  cube.rotation.y += 0.01;                          // continuous rotation
  torus.scale.y = Math.abs(Math.sin(t));             // pulsing scale
  dodecahedron.position.y = -7 * Math.sin(t * 2);   // bobbing position

  renderer.render(scene, camera);
}
render();
```
####实用贴士

-使用`Math.sin()`动画比例时使用`Math.abs()`，以避免负比例值。
-渲染循环使用`requestAnimationFrame`平滑，浏览器优化的帧更新。
-参考[Three.js文档]（https://threejs.org/docs/）获得完整的API。

---### Babylon.js
Babylon.js是一个全功能的3D引擎，具有内置的数学库、物理支持和广泛的文档。

# # # #设置```html
<script src="https://cdn.babylonjs.com/v7.34.1/babylon.js"></script>
<canvas id="render-canvas"></canvas>
```
####引擎、场景和渲染循环```javascript
const canvas = document.getElementById("render-canvas");
const engine = new BABYLON.Engine(canvas);

const scene = new BABYLON.Scene(engine);
scene.clearColor = new BABYLON.Color3(0.8, 0.8, 0.8);

function renderLoop() {
  scene.render();
}
engine.runRenderLoop(renderLoop);
```
####相机和灯光```javascript
const camera = new BABYLON.FreeCamera("camera", new BABYLON.Vector3(0, 0, -10), scene);
const light = new BABYLON.PointLight("light", new BABYLON.Vector3(10, 10, 0), scene);
```
####创建网格```javascript
const box = BABYLON.Mesh.CreateBox("box", 2, scene);       // name, size, scene
const torus = BABYLON.Mesh.CreateTorus("torus", 2, 0.5, 15, scene); // name, diameter, thickness, tessellation, scene
const cylinder = BABYLON.Mesh.CreateCylinder("cylinder", 2, 2, 2, 12, 1, scene);
// name, height, topDiameter, bottomDiameter, tessellation, heightSubdivisions, scene
```
# # # #的材料```javascript
const boxMaterial = new BABYLON.StandardMaterial("material", scene);
boxMaterial.emissiveColor = new BABYLON.Color3(0, 0.58, 0.86);
box.material = boxMaterial;
```
####变换和动画```javascript
box.position.x = 5;
box.rotation.x = -0.2;
box.scaling.x = 1.5;

// Animation inside render loop
let t = 0;
function renderLoop() {
  scene.render();
  t -= 0.01;
  box.rotation.y = t * 2;
  torus.scaling.z = Math.abs(Math.sin(t * 2)) + 0.5;
  cylinder.position.y = Math.sin(t * 3);
}
engine.runRenderLoop(renderLoop);
```
####实用贴士

—`BABYLON`全局对象包含所有框架函数。
-`BABYLON.Vector3`和`BABYLON.Color3`广泛用于定位和着色。Babylon.js包括一个内置的数学库，用于向量，颜色和矩阵。
-参考[Babylon.js文档]（https://doc.babylonjs.com/）的高级功能，如物理，粒子，和后处理。

---

# # #的尖顶

A-Frame是Mozilla的声明式、基于html的框架，用于在web上构建VR/AR体验。它使用实体组件系统，并在底层运行在WebGL上。

# # # #设置```html
<!doctype html>
<html lang="en-US">
  <head>
    <meta charset="utf-8" />
    <title>A-Frame Demo</title>
    <script src="https://aframe.io/releases/1.6.0/aframe.min.js"></script>
    <style>
      body { margin: 0; padding: 0; width: 100%; height: 100%; font-size: 0; }
    </style>
  </head>
  <body>
    <a-scene>
      <!-- entities go here -->
    </a-scene>
  </body>
</html>
```
`<a-scene>`元素是根容器。a帧自动包括默认相机，照明和输入控制。

####原语和实体```html
<!-- Built-in primitive shapes -->
<a-box position="0 1 -3" rotation="0 10 0" color="#4CC3D9"></a-box>
<a-sky color="#DDDDDD"></a-sky>

<!-- Generic entity with explicit geometry and material -->
<a-entity
  geometry="primitive: torus; radius: 1; radiusTubular: 0.1; segmentsTubular: 12;"
  material="color: #EAEFF2; roughness: 0.1; metalness: 0.5;"
  rotation="10 0 0"
  position="-3 1 0">
</a-entity>
```
####用JavaScript创建实体```javascript
const scene = document.querySelector("a-scene");
const cylinder = document.createElement("a-cylinder");
cylinder.setAttribute("color", "#FF9500");
cylinder.setAttribute("height", "2");
cylinder.setAttribute("radius", "0.75");
cylinder.setAttribute("position", "3 1 0");
scene.appendChild(cylinder);
```
####相机和灯光```html
<a-camera position="0 1 4" cursor-visible="true" cursor-color="#0095DD" cursor-opacity="0.5">
</a-camera>

<a-light type="directional" color="white" intensity="0.5" position="-1 1 2"></a-light>
<a-light type="ambient" color="white"></a-light>
```
默认控制：WASD键用于移动，鼠标用于环顾四周。右下角出现VR模式按钮。

# # # #动画

通过HTML属性的声明式动画：```html
<a-box
  color="#0095DD"
  rotation="20 40 0"
  position="0 1 0"
  animation="property: rotation; from: 20 0 0; to: 20 360 0;
    dir: alternate; loop: true; dur: 4000; easing: easeInOutQuad;">
</a-box>
```
动画属性：`property`（动画属性），`from`/`to`（start/end值），`dir`（交替或正常），`loop`（布尔值），`dur`（毫秒），`easing`（缓动函数）。

通过JavaScript的动态动画：```javascript
let t = 0;
function render() {
  t += 0.01;
  requestAnimationFrame(render);
  cylinder.setAttribute("position", `3 ${Math.sin(t * 2) + 1} 0`);
}
render();
```
####实用贴士

- A-Frame是理想的快速VR/AR原型使用熟悉的HTML语法。
-实体-组件架构使其可扩展；社区插件添加物理，手柄控制，和更多。
-使用`<a-sky>`为背景颜色或360度的图像。
- A-Frame支持桌面，移动（iOS/Android）和VR头显（Meta Quest, HTC Vive）。

---

# # # PlayCanvas

PlayCanvas是一个WebGL游戏引擎，有两个工作流选项：

1. **引擎方法**——将PlayCanvas JavaScript库直接包含在HTML和从头编写的代码中。
2. **编辑器方法**——使用在线拖放视觉编辑器进行场景构图。

####主要特性

—实体-组件系统架构
内置物理引擎由[ammo.js]（https://github.com/kripken/ammo.js/）驱动
-碰撞检测
-音频支持
-输入处理（键盘，鼠标，触摸，手柄）
-Resource/asset管理

####实用贴士- PlayCanvas非常适合团队游戏开发，这要归功于其实时协作的在线编辑器。
-仅引擎的方法是轻量级的，可以嵌入到任何网页。
-查阅[PlayCanvas开发者文档]（https://developer.playcanvas.com/）有关实体，组件，相机，灯光，材料和动画的教程。

---

GLSL着色器

GLSL （OpenGL着色语言）是一种直接在GPU上运行的类c语言，可以对渲染管道的顶点和片段处理阶段进行自定义控制。

什么是着色器

着色器是在GPU而不是CPU上执行的小程序。它们是强类型的，并且严重依赖于向量和矩阵数学。有两种类型与WebGL相关：- **顶点着色器** -每个顶点运行一次，将3D位置转换为屏幕坐标。
- **碎片着色器**（像素着色器）-每像素运行一次，确定最终的RGBA颜色。

顶点着色器

顶点着色器的工作是设置`gl_Position`，这是一个内置的GLSL变量，用于存储顶点的转换位置：```glsl
void main() {
  gl_Position = projectionMatrix * modelViewMatrix * vec4(position.x, position.y, position.z, 1.0);
}
```
-`projectionMatrix`——处理透视或正交投影（由Three.js提供）。`modelViewMatrix`——组合模型和视图转换（由Three.js提供）。
-`vec4(x, y, z, w)`—四分量矢量；`w`默认为1.0的位置顶点。

你可以直接操作顶点：```glsl
void main() {
  gl_Position = projectionMatrix * modelViewMatrix * vec4(position.x + 10.0, position.y, position.z + 5.0, 1.0);
}
```
片段着色器

片段着色器的工作是设置`gl_FragColor`，这是一个内置的GLSL变量，用于保存RGBA颜色：```glsl
void main() {
  gl_FragColor = vec4(0.0, 0.58, 0.86, 1.0);
}
```
RGBA组件是0.0到1.0之间的浮点数。Alpha 0.0是完全透明的；1.0是完全不透明的。

在HTML和Three.js中使用着色器

在带有自定义类型属性的脚本标签中嵌入着色器源：```html
<script id="vertexShader" type="x-shader/x-vertex">
  void main() {
    gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
  }
</script>

<script id="fragmentShader" type="x-shader/x-fragment">
  void main() {
    gl_FragColor = vec4(0.0, 0.58, 0.86, 1.0);
  }
</script>
```
用`ShaderMaterial`应用它们：```javascript
const shaderMaterial = new THREE.ShaderMaterial({
  vertexShader: document.getElementById("vertexShader").textContent,
  fragmentShader: document.getElementById("fragmentShader").textContent,
});

const cube = new THREE.Mesh(boxGeometry, shaderMaterial);
```
着色器管道

1. 顶点着色器**处理每个顶点并输出`gl_Position`。
2. 光栅化**将3D坐标映射到2D屏幕像素。
3. **碎片着色器**处理每个像素并输出`gl_FragColor`。

关键概念

**制服**——从JavaScript传递到着色器的值，在单个绘制调用中所有vertices/fragments的值是恒定的（例如，光的位置，时间）。
- **属性**——传递给顶点着色器的每个顶点数据（例如，位置，法线，UV坐标）。
- **变化** -值从顶点着色器传递到片段着色器，在表面上插值。

实用技巧-着色器在GPU上运行，并从CPU卸载计算，这对实时性能至关重要。
-Three.js，Babylon.js和其他框架抽象了许多着色器设置；纯WebGL需要更多的样板文件。
- [ShaderToy]（https://www.shadertoy.com/）是一个很好的着色器示例和灵感资源。
- GLSL要求明确的类型声明；对于浮点数，总是使用`1.0`而不是`1`。

---

##碰撞检测

碰撞检测决定了3D物体何时相交，这是游戏物理、交互和玩法逻辑的基础。

轴对齐边界框（AABB）

AABB将对象封装在与坐标轴对齐的非旋转矩形框中。它是最快的常见碰撞测试，因为它只使用逻辑比较（不使用三角函数）。**限制**:aabb不随对象旋转。对于旋转实体，要么每帧调整边界框的大小，要么使用边界球体。

#### Point vs. AABB

通过测试所有三个轴来检查一个点是否位于一个框内：```javascript
function isPointInsideAABB(point, box) {
  return (
    point.x >= box.minX &&
    point.x <= box.maxX &&
    point.y >= box.minY &&
    point.y <= box.maxY &&
    point.z >= box.minZ &&
    point.z <= box.maxZ
  );
}
```
#### AABB对AABB

检查两个框是否在所有三个轴上重叠：```javascript
function intersect(a, b) {
  return (
    a.minX <= b.maxX &&
    a.maxX >= b.minX &&
    a.minY <= b.maxY &&
    a.maxY >= b.minY &&
    a.minZ <= b.maxZ &&
    a.maxZ >= b.minZ
  );
}
```
###边界球体

边界球体对旋转是不变的（无论物体如何旋转，球体都保持不变），这使它们成为旋转实体的理想选择。然而，它们不适合非球形，并导致更多的误报。

####点与球

检查点到球体中心的距离是否小于半径：```javascript
function isPointInsideSphere(point, sphere) {
  const distance = Math.sqrt(
    (point.x - sphere.x) ** 2 +
    (point.y - sphere.y) ** 2 +
    (point.z - sphere.z) ** 2
  );
  return distance < sphere.radius;
}
```
**性能优化**：通过比较距离的平方来避免平方根：```javascript
const distanceSqr =
  (point.x - sphere.x) ** 2 +
  (point.y - sphere.y) ** 2 +
  (point.z - sphere.z) ** 2;
return distanceSqr < sphere.radius * sphere.radius;
```
#### Sphere vs. Sphere

检查中心之间的距离是否小于半径之和：```javascript
function intersect(sphere, other) {
  const distance = Math.sqrt(
    (sphere.x - other.x) ** 2 +
    (sphere.y - other.y) ** 2 +
    (sphere.z - other.z) ** 2
  );
  return distance < sphere.radius + other.radius;
}
```
#### Sphere vs. AABB

用夹紧的方法在AABB上找到离球体中心最近的点，然后检查距离：```javascript
function intersect(sphere, box) {
  const x = Math.max(box.minX, Math.min(sphere.x, box.maxX));
  const y = Math.max(box.minY, Math.min(sphere.y, box.maxY));
  const z = Math.max(box.minZ, Math.min(sphere.z, box.maxZ));

  const distance = Math.sqrt(
    (x - sphere.x) ** 2 +
    (y - sphere.y) ** 2 +
    (z - sphere.z) ** 2
  );

  return distance < sphere.radius;
}
```
碰撞检测与Three.jsThree.js提供内置的`Box3`和`Sphere`对象，以及用于边界体碰撞检测的可视化助手。

####创建绑定卷```javascript
// Box3 from an object (recommended -- accounts for transforms and children)
const knotBBox = new THREE.Box3(new THREE.Vector3(), new THREE.Vector3());
knotBBox.setFromObject(knot);

// Sphere from geometry
const knotBSphere = new THREE.Sphere(
  knot.position,
  knot.geometry.boundingSphere.radius
);
```
**重要**:`setFromObject()`表示位置、旋转、比例和子网格。几何图形的`boundingBox`属性没有。

####交叉测试```javascript
// Point inside box or sphere
knotBBox.containsPoint(point);
knotBSphere.containsPoint(point);

// Box vs. box
knotBBox.intersectsBox(otherBox);

// Sphere vs. sphere
knotBSphere.intersectsSphere(otherSphere);
```
注意：`containsBox()`检查一个框是否完全包含另一个框，这与`intersectsBox()`不同。

#### Sphere vs. Box3（自定义补丁）Three.js本身不提供球对盒测试。手动添加：```javascript
THREE.Sphere.__closest = new THREE.Vector3();
THREE.Sphere.prototype.intersectsBox = function (box) {
  THREE.Sphere.__closest.set(this.center.x, this.center.y, this.center.z);
  THREE.Sphere.__closest.clamp(box.min, box.max);
  const distance = this.center.distanceToSquared(THREE.Sphere.__closest);
  return distance < this.radius * this.radius;
};
```
#### BoxHelper的可视化调试`BoxHelper`在任何网格周围创建一个可见的线框边界框并简化更新：```javascript
const knotBoxHelper = new THREE.BoxHelper(knot, 0x00ff00);
scene.add(knotBoxHelper);

// After moving or rotating the mesh, update the helper
knot.position.set(-3, 2, 1);
knot.rotation.x = -Math.PI / 4;
knotBoxHelper.update();

// Convert to Box3 for intersection tests
const box3 = new THREE.Box3();
box3.setFromObject(knotBoxHelper);
box3.intersectsBox(otherBox3);
```
BoxHelper的优点：使用`update()`自动调整大小，包括子网格，提供可视化调试。限制：只有盒子体积（没有球体辅助）。

物理引擎

对于更复杂的碰撞检测和响应，请使用物理引擎：

**Cannon.js**——JavaScript的开源3D物理引擎。
- **ammo.js**——子弹物理库的JavaScript端口（由PlayCanvas使用）。

物理引擎创建一个附着在视觉网格上的“物理体”，具有速度、位置、旋转和扭矩等属性。物理形状（盒子、球体、凸壳）用于碰撞计算。

实用技巧-使用aabb轴对齐，非旋转对象-他们是最快的选择。
-使用边界球旋转对象-球体是不变的旋转。
-对于复杂的形状，考虑复合边界体积（多个原语组合）。
-避免`Math.sqrt()`在紧环；比较距离的平方。
-对于制作游戏，集成物理引擎，而不是从头开始编写碰撞检测。

---

# # WebXR

WebXR是用于在浏览器中构建虚拟现实（VR）和增强现实（AR）体验的现代web API。它取代了已弃用的WebVR API。

WebXR是什么

WebXR设备API提供对XR硬件（耳机、控制器）的访问，并支持立体渲染。它捕获的实时数据包括：-耳机的位置和方向
-控制器的位置、方向、速度和加速度
—从XR控制器输入事件

支持的设备

- Meta任务
-阀门指数
- PlayStation VR （PSVR2）
-任何具有webxr兼容浏览器的设备

核心概念

每个WebXR体验都需要两个条件：

1. **实时位置数据** -应用程序在3D空间中持续接收耳机和控制器位置。
2. **实时立体渲染** -应用程序渲染两个略微偏移视图（每只眼睛一个）到耳机的显示器。

框架支持

所有主要的3D web框架都支持WebXR：- **A-Frame**——内置VR模式按钮；声明式的基于html的场景在VR中自动工作。
**Three.js**——通过`renderer.xr`提供WebXR集成。参见[Three.jsVR文档]（https://threejs.org/docs/#manual/en/introduction/How-to-create-VR-content）。
- **Babylon.js**——通过XR体验助手内置WebXR支持。

相关api

- **手柄API**——用于非xr控制器输入（手柄，操纵杆）。
- **设备方向API**——用于检测移动设备上的设备旋转。

设计原则

-优先考虑沉浸感，而不是原始图像质量或游戏玩法复杂性。
-用户必须觉得他们是“体验的一部分”。
-在高，稳定的帧率下渲染的基本形状在VR中比在不稳定帧率下渲染的详细图形更引人注目。
-实验是必不可少的；经常在实际硬件上进行测试。

实用技巧-从快速VR原型的a - frame开始-它的声明性HTML方法可以让您在几分钟内完成工作VR场景。
-使用Three.js或Babylon.js当你需要更多的控制渲染和性能。
-总是在真实的耳机上测试；体验与桌面预览有很大的不同。
-保持稳定，高帧率（72-90+ FPS），以防止晕动病。
—参考[MDN WebXR设备API]（https://developer.mozilla.org/en-US/docs/Web/API/WebXR_Device_API）获取完整的API参考。