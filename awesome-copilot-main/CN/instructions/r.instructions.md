---
description: 'R language and document formats (R, Rmd, Quarto): coding standards and Copilot guidance for idiomatic, safe, and consistent code generation.'
applyTo: '**/*.R, **/*.r, **/*.Rmd, **/*.rmd, **/*.qmd'
---
# R编程语言说明

# #目的

帮助GitHub Copilot跨项目生成习惯的、安全的、可维护的R代码。

##核心约定- **匹配项目风格。**如果文件显示了一个首选项（tidyverse vs. base R,`%>%`vs.`|>`），遵循它。
- **更喜欢清晰，矢量化的代码。**保持功能小，避免隐藏的副作用。
- **限定examples/snippets**中的非基函数，如`dplyr::mutate()`，`stringr::str_detect()`。在项目代码中，使用`library()`是可以接受的，如果这是回购规范的话。
- **命名：**`lower_snake_case`为objects/files；在名字中避免点。
- **副作用：**永远不要调用`setwd()`；首选项目相对路径（例如，`here::here()`）。
- **可重复性：**设置种子局部周围随机操作使用`withr::with_seed()`- **验证：**验证和约束用户输入；尽可能使用输入检查和允许列表。
- **安全：**避免`eval(parse())`、未经验证的shell调用和未参数化的SQL。

###管道操作符- **本机管道`|>`(R≥4.1.0):**优先于R≥4.1（无额外依赖）。
**Magrittr管道`%>%`:**继续使用在项目已经承诺的Magrittr或当你需要的功能，如`.`，`%T>%`，或`%$%`。
- **保持一致：**不要将`|>`和`%>%`混合在同一个脚本中，除非有明确的技术原因。

性能考虑**大型数据集：**考虑`data.table`；对您的工作负载进行基准测试。
- **dplyr兼容性：**使用`dtplyr`编写dplyr语法，转换为数据。表操作自动提高性能。
**性能分析：**使用`profvis::profvis()`来识别代码中的性能瓶颈。优化前的概要文件。
- **缓存：**使用`memoise::memoise()`缓存昂贵的函数结果。对于重复的API调用或复杂的计算特别有用。
- **向量化：**倾向于向量化操作而不是循环。使用`purrr::map_*()`族或`apply()`族满足剩余的迭代需求。

##工具和质量- **格式：**`styler`（tidyverse风格），空格缩进，~100字符行。
- **Linting:**`lintr`通过`.lintr`配置。
- **预提交：**考虑`precommit`钩子自动到lint/format。
- **Docs:** roxygen2用于导出函数（`@param`,`@return`,`@examples`）。
**测试：**更喜欢小的、纯的、可组合的、易于单元测试的函数。
- **依赖：**管理`renv`；添加包后的快照。
- **路径：**首选`fs`和`here`的可移植性。

##数据争吵&I/O- **数据帧：**更喜欢tidyverse重文件中的标题；否则底`data.frame()`就可以了。
- **迭代：**在tidyverse代码中使用`purrr`。在基本样式代码中，首选类型稳定的矢量化模式，如`vapply()`（用于原子输出）或`Map()`（用于元素操作），而不是显式的`for`循环，当它们提高清晰度或性能时。
- **字符串和日期：**使用`stringr`/`lubridate`如果已经存在；否则，使用明确的基础帮助器（例如，`nchar()`,`substr()`，`as.Date()`与显式格式）。
**I/O:**更喜欢显式的，键入的阅读器（例如，`readr::read_csv()`）；明确解析假设。

# #策划

-对于出版质量的图，首选`ggplot2`。保持图层可读，并标记轴和单位。

##错误处理-在结构化环境中，使用`rlang::abort()`/`rlang::warn()`；在纯基数代码中，使用`stop()`/`warning()`。
—对于可恢复操作：
—当需要相同类型的类型回退值时，使用`purrr::possibly()`（更简单）。
—当您需要捕获结果和错误以供以后的检查或记录时，使用`purrr::safely()`。
-在base R中使用`tryCatch()`进行细粒度控制或与非整齐代码的兼容性。
-对于正常流，首选一致的返回结构类型输出，只有在需要错误详细信息时才使用结构化列表。

安全最佳实践- **命令执行：**优先选择`processx::run()`或`sys::exec_wait()`，而不是`system()`；验证并清理所有参数。
—**数据库查询：**使用参数化的`DBI`查询，防止SQL注入。
**文件路径：**规范和清理用户提供的路径（例如，`fs::path_sanitize()`），并根据allowlists进行验证。
- **凭据：**永远不要硬编码秘密。使用env vars (`Sys.getenv()`)，在VCS外配置，或者`keyring`。

# #闪亮的

模块化UI和服务器逻辑的重要应用程序。使用`eventReactive()`/`observeEvent()`进行显式依赖。
-验证输入与`req()`和清晰，用户友好的消息。
-数据库使用连接池（`pool`）；避免长寿命的全局对象。
-隔离昂贵的计算，小状态首选`reactiveVal()`/`reactiveValues()`。

## R降音符/四开调-保持块集中；首选显式块选项（`echo`、`message`、`warning`）。
-避免全局状态；更喜欢当地的帮手。对于确定性块，使用`withr::with_seed()`。

##副驾驶员专用指导

-如果当前文件使用tidyverse， **建议使用tidyverse优先模式**（例如，`dplyr::across()`而不是替换动词）。如果base- r样式存在，则使用base习语。
-在建议中限定非基础呼叫（例如，`dplyr::mutate()`）。
-当习惯使用时，建议对循环进行矢量化或整洁的解决方案。
-比起长管道，更喜欢小的辅助函数。
-当多种方法是等价的，优先考虑可读性和类型稳定性，并解释权衡。

---

##最小示例```r
# Base R variant
scores <- data.frame(id = 1:5, x = c(1, 3, 2, 5, 4))
safe_log <- function(x) tryCatch(log(x), error = function(e) NA_real_)
scores$z <- vapply(scores$x, safe_log, numeric(1))

# Tidyverse variant (if this file uses tidyverse)
result <- tibble::tibble(id = 1:5, x = c(1, 3, 2, 5, 4)) |>
dplyr::mutate(z = purrr::map_dbl(x, purrr::possibly(log, otherwise = NA_real_))) |>
dplyr::filter(z > 0)

# Example reusable helper with roxygen2 doc
#' Compute the z-score of a numeric vector
#' @param x A numeric vector
#' @return Numeric vector of z-scores
#' @examples z_score(c(1, 2, 3))
z_score <- function(x) (x - mean(x, na.rm = TRUE)) / stats::sd(x, na.rm = TRUE)
```
