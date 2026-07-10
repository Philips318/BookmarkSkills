---
description: 'Shell scripting best practices and conventions for bash, sh, zsh, and other shells'
applyTo: '**/*.sh'
---
# Shell脚本指南

为bash、sh、zsh和其他shell编写干净、安全且可维护的shell脚本的说明。

##一般原则生成干净、简单、简洁的代码
-确保脚本易于阅读和理解
-在有助于理解脚本工作原理的地方添加注释
—生成简洁、简单的echo输出，提供执行状态
—避免不必要的回波输出和过多的日志记录
-在可用时使用shellcheck进行静态分析
-除非另有说明，否则假定脚本用于自动化和测试，而不是用于生产系统
-首选安全展开：双引号变量引用（`"$var"`），为了清晰起见使用`${var}`，避免使用`eval`-在可移植性要求允许的情况下使用现代Bash特性（`[[ ]]`,`local`，数组）；只有在需要时才回退到POSIX结构
—为结构化数据选择可靠的解析器，而不是专门的文本处理

##错误处理和安全-始终使`set -euo pipefail`在错误，捕获未设置的变量和地面管道故障时快速失败
—执行前需要确认所有参数
—提供清晰的错误信息和上下文
—使用`trap`清理临时资源或处理脚本终止时的意外退出
用`readonly`（或`declare -r`）声明不可变值，以防止意外重赋值
-使用`mktemp`安全地创建临时文件或目录，并确保它们在清理处理程序中被删除

##脚本结构

—除非另有指定，否则以清晰的shebang开头：`#!/bin/bash`-包括头注释解释脚本的目的
-为顶部的所有变量定义默认值
-对可重用的代码块使用函数
-创建可重用的函数，而不是重复类似的代码块
—保持主执行流程清晰易读

##使用JSON和YAML-首选专用解析器（`jq`用于JSON，`yq`用于yaml -或`jq`用于通过`yq`转换的JSON），而不是使用`grep`、`awk`或shell字符串分割进行临时文本处理
-当`jq`/`yq`不可用或不合适时，选择您环境中可用的下一个最可靠的解析器，并明确说明如何安全地使用它
-验证所需字段的存在，并明确地处理missing/invalid数据路径（例如，通过检查`jq`退出状态或使用`// empty`）
-引用jq/yq过滤器来防止shell扩展，当你需要普通字符串时更喜欢`--raw-output`—将解析器错误视为致命错误：在使用结果之前，与`set -euo pipefail`或test command success结合使用
-文档解析器依赖于脚本的顶部，如果`jq`/`yq`（或替代工具）是必需的，但没有安装，则快速失败并提供有用的消息```bash
#!/bin/bash

# ============================================================================
# Script Description Here
# ============================================================================

set -euo pipefail

cleanup() {
    # Remove temporary resources or perform other teardown steps as needed
    if [[ -n "${TEMP_DIR:-}" && -d "$TEMP_DIR" ]]; then
        rm -rf "$TEMP_DIR"
    fi
}

trap cleanup EXIT

# Default values
RESOURCE_GROUP=""
REQUIRED_PARAM=""
OPTIONAL_PARAM="default-value"
readonly SCRIPT_NAME="$(basename "$0")"

TEMP_DIR=""

# Functions
usage() {
    echo "Usage: $SCRIPT_NAME [OPTIONS]"
    echo "Options:"
    echo "  -g, --resource-group   Resource group (required)"
    echo "  -h, --help            Show this help"
    exit 0
}

validate_requirements() {
    if [[ -z "$RESOURCE_GROUP" ]]; then
        echo "Error: Resource group is required"
        exit 1
    fi
}

main() {
    validate_requirements

    TEMP_DIR="$(mktemp -d)"
    if [[ ! -d "$TEMP_DIR" ]]; then
        echo "Error: failed to create temporary directory" >&2
        exit 1
    fi
    
    echo "============================================================================"
    echo "Script Execution Started"
    echo "============================================================================"
    
    # Main logic here
    
    echo "============================================================================"
    echo "Script Execution Completed"
    echo "============================================================================"
}

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -g|--resource-group)
            RESOURCE_GROUP="$2"
            shift 2
            ;;
        -h|--help)
            usage
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Execute main function
main "$@"

```
