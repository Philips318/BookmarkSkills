---
description: 'Playwright Python AI test generation instructions based on official documentation.'
applyTo: '**'
---
#剧作家Python测试生成指令

##测试编写指南

代码质量标准
- **定位器**：优先考虑面向用户，基于角色的定位器（get_by_role, get_by_label, get_by_text）的弹性和可访问性。
**断言**：通过expect API使用自动重试web优先断言(例如，expect（page).to_have_title（…））。避免期望(定位)。To_be_visible()，除非专门测试元素可见性的变化，因为更具体的断言通常更可靠。
- **超时：依赖于剧作家的内置自动等待机制。避免硬编码等待或增加默认超时。
- **清晰度**：使用描述性的测试标题（例如，def test_navigation_link_works():），清楚地说明他们的意图。添加注释只是为了解释复杂的逻辑，而不是描述简单的操作，比如“点击一个按钮”。测试结构
** import **：每个测试文件都应该以from剧作家开头。sync_api import页面，期望。
** fixture **：使用page: page fixture作为测试函数中的参数来与浏览器页面进行交互。
- **设置**：在每个测试函数的开始放置导航步骤，如page.goto（）。对于跨多个测试共享的设置操作，请使用标准的Pytest fixture。

###文件组织
- **位置**：将测试文件存储在专用的测试/目录中，或遵循现有的项目结构。
—**命名**：测试文件必须遵循test_<feature-or-page>.py命名约定，以供Pytest发现。
- **范围**：针对每个主要应用程序功能或页面一个测试文件。断言最佳实践
- **元素计数**：使用expect（定位器）。To_have_count（）断言定位器找到的元素数量。
- **文本内容**：使用expect（定位器）。To_have_text（）用于精确文本匹配和expect（定位器）。To_contain_text（）用于部分匹配。
- **导航**：使用expect（page）。to_have_url（）来验证页面URL。
- **断言风格**：为了更可靠的UI测试，`expect`优于`assert`。


# #的例子```python
import re
import pytest
from playwright.sync_api import Page, expect

@pytest.fixture(scope="function", autouse=True)
def before_each_after_each(page: Page):
    # Go to the starting url before each test.
    page.goto("https://playwright.dev/")

def test_main_navigation(page: Page):
    expect(page).to_have_url("https://playwright.dev/")

def test_has_title(page: Page):
    # Expect a title "to contain" a substring.
    expect(page).to_have_title(re.compile("Playwright"))

def test_get_started_link(page: Page):
    page.get_by_role("link", name="Get started").click()
    
    # Expects page to have a heading with the name of Installation.
    expect(page.get_by_role("heading", name="Installation")).to_be_visible()
```
测试执行策略

1. **执行**：使用pytest命令从终端运行测试。
2. **调试故障**：分析测试故障并找出根本原因