---
description: 'Clojure-specific coding patterns, inline def usage, code block templates, and namespace handling for Clojure development.'
applyTo: '**/*.{clj,cljs,cljc,bb,edn.mdx?}'
---
# Clojure开发指南

代码评估工具的使用

“使用repl”是指使用Calva Backseat Driver的**Evaluate Clojure Code**工具。它将您连接到与用户通过Calva连接到的相同的REPL。

-始终留在卡尔瓦的REPL内，而不是从终端启动第二个。
-如果没有REPL连接，请要求用户连接REPL，而不是尝试自己启动和连接。

REPL工具调用中的JSON字符串
在调用REPL工具时不要过度转义JSON参数。```json
{
  "namespace": "<current-namespace>",
  "replSessionKey": "cljs",
  "code": "(def foo \"something something\")"
}
```
##文档字符串在`defn`文档字符串紧接在函数名之后，在参数向量之前。```clojure
(defn my-function
  "This function does something."
  [arg1 arg2]
  ;; function body
  )
```
-在使用函数之前定义函数-除非真正必要，否则优先使用`declare`排序。

交互式编程（又名REPL驱动开发）

###调整数据结构元素的括号平衡
**始终在所有数据结构中垂直对齐多行元素：向量，映射，列表，集合，所有代码（因为Clojure代码是数据）。不对中会导致括号平衡器不正确地关闭括号，从而创建无效的表单```clojure
;; ❌ Wrong - misaligned vector elements
(select-keys m [:key-a
                :key-b
               :key-c])  ; Misalignment → incorrect ] placement

;; ✅ Correct - aligned vector elements
(select-keys m [:key-a
                :key-b
                :key-c])  ; Proper alignment → correct ] placement

;; ❌ Wrong - misaligned map entries
{:name "Alice"
 :age 30
:city "Oslo"}  ; Misalignment → incorrect } placement

;; ✅ Correct - aligned map entries
{:name "Alice"
 :age 30
 :city "Oslo"}  ; Proper alignment → correct } placement
```
**关键：支架平衡器依赖于一致的缩进来确定结构。

REPL依赖管理
在REPL会话期间使用`clojure.repl.deps/add-libs`进行动态依赖加载。```clojure
(require '[clojure.repl.deps :refer [add-libs]])
(add-libs '{dk.ative/docjure {:mvn/version "1.15.0"}})
```
-动态依赖加载需要Clojure 1.12或更高版本
-完美的图书馆探索和原型

检查Clojure版本```clojure
*clojure-version*
;; => {:major 1, :minor 12, :incremental 1, :qualifier nil}
```
REPL可用性原则

**不要在REPL不可用时编辑代码文件。**当REPL评估返回错误，表明REPL不可用，立即停止并通知用户。让用户在继续之前还原REPL。

####为什么这很重要
- **交互式编程需要一个工作的REPL** -没有评估就无法验证行为
- **猜测会产生bug ** -没有测试的代码更改会引入错误

结构编辑和复写优先习惯
-在修改文件之前对REPL进行修改。
-在编辑Clojure文件时，始终使用结构化编辑工具，如**插入顶级表单**，**替换顶级表单**，**创建Clojure文件**和**追加代码**，并始终先阅读它们的说明。创建新文件
—使用带有初始内容的“**创建Clojure文件**”工具
-遵循Clojure命名规则：命名空间采用kebab-case，文件路径采用snake_case（例如，`my.project.ns`→`my/project/ns.clj`）。

重新加载命名空间
编辑完文件后，在REPL中重新加载已编辑的名称空间，以便更新的定义处于活动状态。```clojure
(require 'my.namespace :reload)
```
计算前的代码缩进
一致的压痕对支架平衡器的帮助至关重要。```clojure
;; ❌
(defn my-function [x]
(+ x 2))

;; ✅
(defn my-function [x]
  (+ x 2))
```
缩进参数

保持条件和主体在不同的行上：```clojure
(when limit
  (println "Limit set to:" limit))
```
将`and`和`or`参数放在单独的行中：```clojure
(if (and condition-a
         condition-b)
  this
  that)
```
## Inline Def Pattern

首选内联def调试而不是println/console.log.内联`def`用于调试
内联`def`绑定在REPL工作期间保持中间状态可检查。
当内联绑定继续帮助探索时，保留它们。```clojure
(defn process-instructions [instructions]
  (def instructions instructions)
  (let [grouped (group-by :status instructions)]
    grouped))
```
-实时巡检。
-调试周期保持快速。
-迭代开发保持平稳。

当在聊天框中显示用户代码时，还可以使用“inline def”，以便用户可以轻松地从代码块中试验代码。用户可以使用Calva直接在代码块中计算代码。（但用户不能在那里编辑代码。）

##返回值>打印副作用

最好使用REPL并从评估中返回值，而不是将内容打印到stdout。

##读取`stdin`—当Clojure代码使用`(read-line)`时，它会通过VS Code提示用户。
避免在Babashka的nREPL中读取标准输入，因为它缺乏标准输入支持。
—如果REPL阻塞，要求用户重新启动。

数据结构首选项我们尽量保持数据结构的扁平化，主要依赖于名称空间关键字，并优化以方便解构。通常在应用程序中，我们使用命名空间关键字，最常见的是“合成”命名空间。

直接在参数列表中解构键。```clojure
(defn handle-user-request
  [{:user/keys [id name email]
    :request/keys [method path headers]
    :config/keys [timeout debug?]}]
  (when debug?
    (println "Processing" method path "for" name)))
```
这样做的诸多好处之一是使函数签名保持透明。

避免使用内置的影子
必要时重命名传入键以避免隐藏核心函数。```clojure
(defn create-item
  [{:prompt-sync.file/keys [path uri]
    file-name :prompt-sync.file/name
    file-type :prompt-sync.file/type}]
  #js {:label file-name
       :type file-type})
```
保持自由的常见符号：
——`class`——`count`——`empty?`——`filter`——`first`——`get`——`key`——`keyword`——`map`——`merge`——`name`——`reduce`——`rest`——`set`——`str`——`symbol`——`type`——`update`避免不必要的包装函数
不要包装核心函数，除非一个名称真正地阐明了组成。```clojure
(remove (set exclusions) items) ; a wrapper function would not make this clearer
```
文档的富注释表单（RCF）

富注释表单`(comment ...)`与直接的REPL求值有不同的目的。在文件编辑中使用rcf来记录已经在REPL中验证过的函数的使用模式和示例。

何时使用rcf
- **经过REPL验证后** -在文件中记录工作示例
- **使用文档** -显示如何使用函数
在代码库中保留有用的REPL发现
- **场景示例** -展示边缘案例和典型用法

RCF模式
RCF =丰富的评论表单。

当加载文件时，rcf中的代码不会被求值，这使得它们非常适合用于记录示例用法，因为人们可以轻松地随意求值其中的代码。```clojure
(defn process-user-data
  "Processes user data with validation"
  [{:user/keys [name email] :as user-data}]
  ;; implementation here
  )

(comment
  ;; Basic usage
  (process-user-data {:user/name "John" :user/email "john@example.com"})

  ;; Edge case - missing email
  (process-user-data {:user/name "Jane"})

  ;; Integration example
  (->> users
       (map process-user-data)
       (filter :valid?))

  :rcf) ; Optional marker for end of comment block
```
RCF与REPL工具的使用```clojure
;; In chat - show direct REPL evaluation:
(in-ns 'my.namespace)
(let [test-data {:user/name "example"}]
  (process-user-data test-data))

;; In files - document with RCF:
(comment
  (process-user-data {:user/name "example"})
  :rcf)
```
# #测试

从REPL运行测试
重新加载目标名称空间并从REPL执行测试以获得即时反馈。```clojure
(require '[my.project.some-test] :reload)
(clojure.test/run-tests 'my.project.some-test)
(cljs.test/run-tests 'my.project.some-test)
```
-更紧密的REPL集成。
-专注执行。
-简化调试。
-直接访问测试数据。

在调查故障时，更喜欢在测试名称空间内运行单独的测试变量。

使用REPL-First TDD工作流
在编辑文件之前对实际数据进行迭代。```clojure
(def sample-text "line 1\nline 2\nline 3\nline 4\nline 5")

(defn format-line-number [n padding marker-len]
  (let [num-str (str n)
        total-padding (- padding marker-len)]
    (str (apply str (repeat (- total-padding (count num-str)) " "))
         num-str)))

(deftest line-number-formatting
  (is (= "  5" (editor-util/format-line-number 5 3 0))
      "Single digit with padding 3, no marker space")
  (is (= " 42" (editor-util/format-line-number 42 3 0))
      "Double digit with padding 3, no marker space"))
```
# # # #的好处
-在提交更改之前验证行为
-即时反馈的增量开发
-捕获已知良好行为的测试
-以失败测试开始新工作以锁定意图

测试命名和消息传递
保持`deftest`名称描述性（area/thing风格），不使用多余的`-test`后缀。

测试断言消息样式
将期望消息直接附加到`is`，仅在对多个相关断言进行分组时才使用`testing`块。```clojure
(deftest line-marker-formatting
  (is (= "→" (editor-util/format-line-marker true))
      "Target line gets marker")
  (is (= "" (editor-util/format-line-marker false))
      "Non-target gets empty string"))

(deftest context-line-extraction
  (testing "Centered context extraction"
    (let [result (editor-util/get-context-lines "line 1\nline 2\nline 3" 2 3)]
      (is (= 3 (count (str/split-lines result)))
          "Should have 3 lines")
      (is (str/includes? result "→")
          "Should have marker"))))
```
指南:
保持关于期望的断言消息的明确性。
—使用`testing`进行分组相关检查。
-保持像`line-marker-formatting`或`context-line-extraction`这样的串大小写名称。

快乐的互动编程

记得在您的工作中使用REPL。请记住，用户不会看到您评估的内容。结果也不一样。在聊天中与用户交流你的评价和反馈。