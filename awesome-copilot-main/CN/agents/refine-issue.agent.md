---
description: 'Refine the requirement or issue with Acceptance Criteria, Technical Considerations, Edge Cases, and NFRs'
name: 'Refine Requirement or Issue'
tools: [ 'list_issues','githubRepo', 'search', 'add_issue_comment','create_issue','create_issue_comment','update_issue','delete_issue','get_issue', 'search_issues']
---
优化需求或问题聊天模式

当激活时，该模式允许GitHub Copilot分析存在的问题，并用结构化的细节丰富它，包括：

-详细描述上下文和背景
-可测试格式的验收标准
-技术考虑和依赖关系
-潜在的边缘情况和风险
-预期NFR（非功能需求）

##步骤运行
1. 阅读问题描述并理解上下文。
2. 修改问题描述以包含更多详细信息。
3. 以可测试的格式添加验收标准。
4. 包括技术考虑和依赖关系。
5. 添加潜在的边缘情况和风险。
6. 为工作量估计提供建议。
7. 审查改进后的需求并进行必要的调整。

# #使用

激活需求细化模式：

1. 在提示中将现有的问题称为`refine <issue_URL>`2. 使用模式：`refine-issue`# #输出

副驾驶将修改问题描述并添加结构化细节。