#写数学表达式

使用Markdown在GitHub上显示数学表达式。

关于写数学表达式

为了实现数学表达式的清晰交流，GitHub支持LaTeX格式的数学。有关更多信息，请参阅Wikibooks中的[LaTeX/Mathematics]（http://en.wikibooks.org/wiki/LaTeX/Mathematics）。

GitHub的数学渲染功能使用MathJax；一个开源的、基于javascript的显示引擎。MathJax支持广泛的LaTeX宏和几个有用的可访问性扩展。有关更多信息，请参阅[MathJax文档]（http://docs.mathjax.org/en/latest/input/tex/index.html#tex-and-latex-support）和[MathJax可访问性扩展文档]（https://mathjax.github.io/MathJax-a11y/docs/#reader-guide）。

数学表达式渲染可以在GitHub Issues、GitHub discussion、pull requests、wiki和Markdown文件中找到。

##编写内联表达式有两个选项可用于将数学表达式与文本内联分隔。可以用美元符号（`$`）包围表达式，也可以用<code>$\`</code> and end it with <code>\`$</code>开始表达式。当您正在编写的表达式包含与markdown语法重叠的字符时，后一种语法非常有用。有关更多信息，请参见[基本书写和格式化语法]（https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax）。```text
This sentence uses `$` delimiters to show math inline: $\sqrt{3x-1}+(1+x)^2$
```
！[渲染Markdown的截图显示了一个内联数学表达式：根号3x - 1加上（1 + x）的平方。]（https://docs.github.com/assets/images/help/writing/inline-math-markdown-rendering.png）```text
This sentence uses $\` and \`$ delimiters to show math inline: $`\sqrt{3x-1}+(1+x)^2`$
```
！[渲染Markdown的截图显示了一个带有反勾语法的内联数学表达式：3x - 1加上（1加x）平方的平方根。]（https://docs.github.com/assets/images/help/writing/inline-backtick-math-markdown-rendering.png）

##将表达式写成块

若要将数学表达式添加为块，请开始一个新行，并用两个美元符号`$$`分隔表达式。

> [!小贴士：如果你是用……在Md文件中，您需要使用特定的格式来创建换行符，例如以反斜杠结束一行，如下面的示例所示。有关Markdown中换行符的更多信息，请参见[基本书写和格式化语法]（https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#line-breaks）。```text
**The Cauchy-Schwarz Inequality**\
$$\left( \sum_{k=1}^n a_k b_k \right)^2 \leq \left( \sum_{k=1}^n a_k^2 \right) \left( \sum_{k=1}^n b_k^2 \right)$$
```
！[截图渲染Markdown显示一个复杂的方程。黑体字在不等式公式上方写着“柯西-施瓦茨不等式”[https://docs.github.com/assets/images/help/writing/math-expression-as-a-block-rendering.png]

或者，您可以使用<code>\`\`\`math</code> code block syntax to display a math expression as a block. With this syntax, you don't need to use `$$ '分隔符。以下代码将呈现与上述相同的内容：````text
**The Cauchy-Schwarz Inequality**

```math
\left（\sum ＿k=1{^n a＿k b＿k }\right）^2 \leq\left (\sum ＿k=1{^n a＿k^2 }\right) \left （\sum ＿k=1{^n b＿k^2 }\right）```
````
##按照数学表达式书写美元符号

要将美元符号作为字符显示在与数学表达式相同的行中，需要转义非分隔符`$`，以确保该行正确呈现。

*在数学表达式中，在显式的`$`之前添加`\`符号。  ```text
  This expression uses `\$` to display a dollar sign: $`\sqrt{\$4}`$
  ```
！[渲染Markdown的截图，显示美元符号前的反斜杠如何显示该符号作为数学表达式的一部分。]

*在数学表达式外，但在同一行上，在显式的`$`周围使用span标记。  ```text
  To split <span>$</span>100 in half, we calculate $100/2$
  ```
！显示美元符号周围的span标签如何将该符号显示为内联文本而不是数学方程的一部分的渲染Markdown的截图。

##进一步阅读

* [MathJax网站]（http://mathjax.org）
*[开始在GitHub上写作和格式化]（https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github）
* [GitHub风味降价规范]（https://github.github.com/gfm/）