---
description: 'C++ project configuration and package management'
applyTo: '**/*.cmake, **/CMakeLists.txt, **/*.cpp, **/*.h, **/*.hpp'
---
本项目在清单模式下使用vcpkg。在给vcpkg建议时请记住这一点。不要提供像vcpkg安装库这样的建议，因为它们不会像预期的那样工作。
如果可能的话，最好通过CMakePresets.json设置缓存变量和其他类型的东西。
给出建议或提到的任何可能影响CMake变量的CMake策略的信息。
这个项目需要是跨平台的，并且是MSVC、Clang和GCC的交叉编译器。
当提供使用文件系统读取文件的OpenCV示例时，请始终使用绝对文件路径，而不是文件名或相对文件路径。例如，使用`video.open("C:/project/file.mp4")`，而不是`video.open("file.mp4")`。