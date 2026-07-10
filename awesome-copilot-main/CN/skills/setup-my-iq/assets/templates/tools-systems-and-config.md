#工具、系统和配置

##主要工具

- ** ide:** <！——例如，VS Code——>
- ** ai:** <！——例如，GitHub Copilot——>
- **项目管理：** <！——例如，Azure DevOps, Jira——b>
- **通讯：** <！——例如，Teams、Slack——b>
- **备注：** <！-例如，黑曜石，OneNote - b>
- **源代码控制：** <！——例如，Git——b>

# # < !——Notes/Vault系统——>

|设置|值||---------|-------|
|Vault/notes路径| <！——full path——> |
|动作项| <！——操作项文件的路径——> |

会议记录

|设置|值||---------|-------|
|会议文件夹| <！——full path——> |
|格式| <！-例如，黑曜石兼容markdown -> |
|文件组织| <！——例如，{folder}/YYYY-MM/YYYY-MM-DD-Title.md——> |

会议标签

标签基于标题模式匹配应用于会议记录。会议类型标签标识会议的格式。团队标签标识会议属于哪个project/team。

|标签|类型|模式||-----|------|----------|
| < !——标签名称——> |会议类型| <！——逗号分隔模式——> |
| < !——标签名称——> |团队| <！——逗号分隔模式——> |

---

# # < !——Team/Workstream名称1——>

Azure DevOps<!-- If this workstream does not use ADO, delete this section or leave it empty. -->
|设置|值||---------|-------|
|组织| <！——org name——> |
|项目| <！——项目名——> |
|团队| <！——球队名称——> |
|区域路径| <！——area path——> |

记分卡排除

|设置|值||---------|-------|
|排除标题模式| <！——从报告中排除的模式——> |

# # #报告

|设置|值||---------|-------|
| Sprint更新输出| <！——path——> |
|每周更新输出| <！——path——> |
|每周更新前缀| <！——例如，weekly-update——> |
|路线图文件| <！——path，或者留空以跳过——> |
|上下文文件模式|`*.md`|

战略支柱<!-- These are used as grouping headers in weekly updates and sprint reports.
     Define them even if the workstream does not use ADO. The ADO Epics column
     is optional; leave it empty for non-ADO workstreams. -->
|支柱| ADO史诗|定义||--------|-----------|------------|
| < !——柱名——> | <！——史诗名称，或空——> | <！——简单的定义——b> |

---

# # < !——Team/Workstream名称2——>

Azure DevOps<!-- If this workstream does not use ADO, delete this section or leave it empty. -->
|设置|值||---------|-------|
|组织| <！——org name——> |
|项目| <！——todo——b> |
|团队| <！——todo——b> |
|区域路径| <！——todo——b> |

# # #报告

|设置|值||---------|-------|
|每周更新输出| <！——path——> |
|每周更新前缀| <！——例如，weekly-update——> |

战略支柱

|支柱| ADO史诗|定义||--------|-----------|------------|
| < !——柱名——> | | <！——简单的定义——b> |

---<!-- Add workstream sections as needed. Common settings go at the top, workstream-specific config goes under each heading.
     Every workstream needs at least: Reporting and Strategic Pillars.
     Azure DevOps is optional. -->
