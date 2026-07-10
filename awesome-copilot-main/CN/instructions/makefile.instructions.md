---
description: "Best practices for authoring GNU Make Makefiles"
applyTo: "**/Makefile, **/makefile, **/*.mk, **/GNUmakefile"
---
# Makefile开发说明

编写干净、可维护和可移植的GNU Make makefile文件的说明。这些指令基于[GNU Make手册]（https://www.gnu.org/software/make/manual/）。

##一般原则

遵循GNU Make约定，编写清晰、可维护的Make文件
-使用描述性的目标名称，清楚地表明其目的
—保留默认目标（第一个目标）作为最常见的构建操作
在编写规则和食谱时，优先考虑可读性而不是简洁性
—添加注释来解释复杂的规则、变量或不明显的行为

命名约定-命名你的makefile为`Makefile`（建议为可见性）或`makefile`—`GNUmakefile`仅用于与其他make实现不兼容的GNU make特性
—使用标准变量名：`objects`、`OBJECTS`、`objs`、`OBJS`、`obj`或`OBJ`用于对象文件列表
-内置变量名使用大写（例如，`CC`,`CFLAGS`,`LDFLAGS`）
-使用反映其动作的描述性目标名称（例如，`clean`,`install`,`test`）

##文件结构

-将默认目标（主要构建目标）作为makefile中的第一条规则
—将相关目标逻辑分组
-在makefile的顶部定义变量，然后再定义规则
—使用`.PHONY`声明不代表文件的目标
-构造makefile：变量，然后规则，然后伪目标```makefile
# Variables
CC = gcc
CFLAGS = -Wall -g
objects = main.o utils.o

# Default goal
all: program

# Rules
program: $(objects)
	$(CC) -o program $(objects)

%.o: %.c
	$(CC) $(CFLAGS) -c $< -o $@

# Phony targets
.PHONY: clean all
clean:
	rm -f program $(objects)
```
变量和代换

—使用变量避免重复，提高可维护性
-使用`:=`（简单展开）定义变量，用于立即求值，`=`用于递归展开
—使用“`?=`”设置可覆盖的默认值
—使用`+=`追加到已有的变量
-引用变量`$(VARIABLE)`而不是`$VARIABLE`（除非单个字符）
-在食谱中使用自动变量（`$@`,`$<`,`$^`,`$?`,`$*`），使规则更通用```makefile
# Simple expansion (evaluates immediately)
CC := gcc

# Recursive expansion (evaluates when used)
CFLAGS = -Wall $(EXTRA_FLAGS)

# Conditional assignment
PREFIX ?= /usr/local

# Append to variable
CFLAGS += -g
```
规则和先决条件

-明确区分目标、先决条件和配方
-对标准编译使用隐式规则（例如，`.c`到`.o`）
-按逻辑顺序列出先决条件（正常先决条件在仅顺序之前）
-对不应该触发重建的目录和依赖项使用仅订单先决条件（在`|`之后）
-包括所有实际依赖，以确保正确的重建
-避免目标之间的循环依赖
-请记住，像`$^`这样的自动变量省略了仅订购的先决条件，因此如果需要，请显式引用它们

下面的示例显示了将对象编译到`obj/`目录中的模式规则。目录本身被列为仅顺序的先决条件，因此在编译之前创建它，但在其时间戳更改时不会强制重新编译。```makefile
# Normal prerequisites
program: main.o utils.o
	$(CC) -o $@ $^

# Order-only prerequisites (directory creation)
obj/%.o: %.c | obj
	$(CC) $(CFLAGS) -c $< -o $@

obj:
	mkdir -p obj
```
##食谱和命令

-除非更改了`.RECIPEPREFIX`，否则每个配方行以**制表符**开始（不是空格）
—使用`@`前缀抑制命令回显
-使用`-`前缀来忽略特定命令的错误（谨慎使用）
—相关命令必须同时执行时，请与`&&`或`;`合并在同一行
-保持食谱的可读性；使用反斜杠将长命令跨多行分隔
-在需要的时候使用shell条件和循环```makefile
# Silent command
clean:
	@echo "Cleaning up..."
	@rm -f $(objects)

# Ignore errors
.PHONY: clean-all
clean-all:
	-rm -rf build/
	-rm -rf dist/

# Multi-line recipe with proper continuation
install: program
	install -d $(PREFIX)/bin && \
		install -m 755 program $(PREFIX)/bin
```
##虚假目标

-始终使用`.PHONY`声明伪目标，以避免与同名文件冲突
-使用虚假目标的行动，如`clean`，`install`,`test`,`all`-将虚假的目标声明放在它们的规则定义附近或makefile的末尾```makefile
.PHONY: all clean test install

all: program

clean:
	rm -f program $(objects)

test: program
	./run-tests.sh

install: program
	install -m 755 program $(PREFIX)/bin
```
模式规则和隐式规则

—使用模式规则（`%.o: %.c`）进行通用转换
-在适当的时候利用内置的隐式规则（GNU Make知道如何将`.c`编译为`.o`）
-覆盖隐式规则变量（如`CC`，`CFLAGS`），而不是重写规则
—只有当内置规则不足时才定义自定义模式规则```makefile
# Use built-in implicit rules by setting variables
CC = gcc
CFLAGS = -Wall -O2

# Custom pattern rule for special cases
%.pdf: %.md
	pandoc $< -o $@
```
分割长行

-使用反斜杠-换行符（`\`）分隔长行以提高可读性
-注意，在非配方上下文中，反斜杠-换行符被转换为单个空格
-在recipes中，反斜杠-换行符保留shell的行延续
—避免在反斜杠后面尾随空格

不添加空白的分割

如果需要在不添加空白的情况下分割行，可以使用一种特殊技术：插入`$ `（美元-space），后跟反斜杠-换行符。`$ `指的是一个具有单空格名称的变量，该变量不存在并展开为零，可以有效地连接行而不插入空格。```makefile
# Concatenate strings without adding whitespace
# The following creates the value "oneword"
var := one$ \
       word

# This is equivalent to:
# var := oneword
```

```makefile
# Variable definition split across lines
sources = main.c \
          utils.c \
          parser.c \
          handler.c

# Recipe with long command
build: $(objects)
	$(CC) -o program $(objects) \
	      $(LDFLAGS) \
	      -lm -lpthread
```
##包括其他makefile

-使用`include`指令在make文件之间共享公共定义
—使用`-include`（或`sinclude`）包含可选的makefile而不会出错
-将`include`指令放在变量定义之后，可能会影响包含的文件
—对于共享变量、模式规则或公共目标，使用`include````makefile
# Include common settings
include config.mk

# Include optional local configuration
-include local.mk
```
条件指令

-使用条件指令（`ifeq`,`ifneq`,`ifdef`,`ifndef`）用于平台或特定配置的规则
-在makefile级别放置条件，而不是在recipes中（在recipes中使用shell条件）
-保持条件句的简单和良好的文档```makefile
# Platform-specific settings
ifeq ($(OS),Windows_NT)
    EXE_EXT = .exe
else
    EXE_EXT =
endif

program: main.o
	$(CC) -o program$(EXE_EXT) main.o
```
##自动先决条件

-自动生成报头依赖，而不是手动维护它们
-使用编译器标志，如`-MMD`和`-MP`来生成带有依赖关系的`.d`文件
-包含生成的依赖文件与`-include $(deps)`，以避免错误，如果他们不存在```makefile
objects = main.o utils.o
deps = $(objects:.o=.d)

# Include dependency files
-include $(deps)

# Compile with automatic dependency generation
%.o: %.c
	$(CC) $(CFLAGS) -MMD -MP -c $< -o $@
```
错误处理和调试

—使用`$(error text)`或`$(warning text)`函数进行构建时诊断
-使用`make -n`（dry run）测试makefiles以查看未执行的命令
—使用`make -p`打印规则和变量库，用于调试
-在makefile的开头验证所需的变量和工具```makefile
# Check for required tools
ifeq ($(shell which gcc),)
    $(error "gcc is not installed or not in PATH")
endif

# Validate required variables
ifndef VERSION
    $(error VERSION is not defined)
endif
```
##清洁目标

-始终提供`clean`目标以删除生成的文件
-声明`clean`为假的，以避免与名为“clean”的文件冲突
—使用`-`前缀和`rm`命令，忽略文件不存在的错误
—考虑单独的`clean`（删除对象）和`distclean`（删除所有生成的文件）目标```makefile
.PHONY: clean distclean

clean:
	-rm -f $(objects)
	-rm -f $(deps)

distclean: clean
	-rm -f program config.mk
```
可移植性注意事项

-如果需要移植到其他make实现，避免GNU make特有的特性
-使用标准shell命令（优先使用POSIX shell结构）
-测试`make -B`强制重建所有目标
-记录任何平台特定的要求或使用的GNU Make扩展

性能优化

对不需要递归展开的变量使用`:=`（更快）
-避免不必要地使用`$(shell ...)`，这会创建子进程
-有效地排序先决条件（最频繁更改的文件最后）
-通过确保目标不冲突来安全地使用并行构建（`make -j`）

文档和注释

-添加标题注释，解释makefile的目的
—记录不明显的变量设置及其效果
-在注释中包括用法示例或目标
-为复杂规则或特定于平台的解决方案添加内联注释```makefile
# Makefile for building the example application
#
# Usage:
#   make          - Build the program
#   make clean    - Remove generated files
#   make install  - Install to $(PREFIX)
#
# Variables:
#   CC       - C compiler (default: gcc)
#   PREFIX   - Installation prefix (default: /usr/local)

# Compiler and flags
CC ?= gcc
CFLAGS = -Wall -Wextra -O2

# Installation directory
PREFIX ?= /usr/local
```
##特殊目标

—非文件目标使用“`.PHONY`”
—使用`.PRECIOUS`保存中间文件
-使用`.INTERMEDIATE`将文件标记为中间（自动删除）
—使用“`.SECONDARY`”防止删除中间文件
-使用`.DELETE_ON_ERROR`去除目标，如果配方失败
使用`.SILENT`来抑制所有配方的回显（谨慎使用）```makefile
# Don't delete intermediate files
.SECONDARY:

# Delete targets if recipe fails
.DELETE_ON_ERROR:

# Preserve specific files
.PRECIOUS: %.o
```
##常见模式

标准项目结构```makefile
CC = gcc
CFLAGS = -Wall -O2
objects = main.o utils.o parser.o

.PHONY: all clean install

all: program

program: $(objects)
	$(CC) -o $@ $^

%.o: %.c
	$(CC) $(CFLAGS) -c $< -o $@

clean:
	-rm -f program $(objects)

install: program
	install -d $(PREFIX)/bin
	install -m 755 program $(PREFIX)/bin
```
###管理多个程序```makefile
programs = prog1 prog2 prog3

.PHONY: all clean

all: $(programs)

prog1: prog1.o common.o
	$(CC) -o $@ $^

prog2: prog2.o common.o
	$(CC) -o $@ $^

prog3: prog3.o
	$(CC) -o $@ $^

clean:
	-rm -f $(programs) *.o
```
要避免的反模式

-不要用空格代替制表符开始配方行
—当文件列表可以使用通配符或函数生成时，避免硬编码
-不要使用`$(shell ls ...)`来获取文件列表（使用`$(wildcard ...)`代替）
-避免在配方中使用复杂的shell脚本（移动到单独的脚本文件）
-不要忘记将虚假目标声明为`.PHONY`-避免目标之间的循环依赖
除非绝对必要，否则不要使用递归make （`$(MAKE) -C subdir`）