---
description: 'Ansible conventions and best practices'
applyTo: '**/*.yaml, **/*.yml'
---
#可行的约定和最佳实践

##一般使用说明—使用Ansible配置和管理基础设施。
-使用版本控制你的Ansible配置。
-保持简单；只在必要时使用高级功能
-给每个游戏，方块和任务一个简洁但描述性的`name`—名称以动作动词开头，表示正在执行的操作，如“安装”、“配置”或“复制”。
—任务名称的首字母大写
-为简洁起见，省略任务名称末尾的句号
-从角色任务中省略角色名称；当运行角色时，Ansible会自动显示角色名
-当包含来自单独文件的任务时，您可以在每个任务名称中包含文件名，以使任务更容易定位（例如，`<TASK_FILENAME> : <TASK_NAME>`）
-使用注释提供关于**什么**，**如何**，**为什么**做某事的额外上下文
-不要包含多余的注释
-使用云动态库存d资源
-使用标签根据环境，功能，位置等动态创建组
—使用“`group_vars`”根据这些属性设置变量
-尽可能使用幂等的Ansible模块；避免使用`shell`、`command`和`raw`，因为它们会破坏幂等性
—如果必须使用`shell`或`command`，请在可行的情况下使用`creates:`或`removes:`参数，以防止不必要的执行
-使用[完全限定集合名称（FQCN）]（https://docs.ansible.com/ansible/latest/reference_appendices/glossary.html#term-Fully-Qualified-Collection-Name-FQCN）确保选择正确的模块或插件
-使用`ansible.builtin`集合[内置模块和插件]（https://docs.ansible.com/ansible/latest/collections/ansible/builtin/index.html#plugin-index）
-将相关任务组合在一起，以提高可读性和模块化
—对于`state`为可选的模块，请显式设置`state: present`或`state: absent`，以提高清晰度和一致性
—使用执行任务所需的最低权限
—仅在play级别或`include:`语句中设置`become: true`，如果所有包含的任务都需要超级用户权限；否则，在任务级别指定`become: true`—只在需要超级用户权限的任务上设置`become: true`##保密管理

-单独使用Ansible时，使用Ansible Vault存储秘密
-使用以下过程可以轻松找到定义了拱形变量的位置    1. Create a `group_vars/` subdirectory named after the group
    2. Inside this subdirectory, create two files named `vars` and `vault`
    3. In the `vars` file, define all of the variables needed, including any sensitive ones
    4. Copy all of the sensitive variables over to the `vault` file and prefix these variables with `vault_`
    5. Adjust the variables in the `vars` file to point to the matching `vault_` variables using Jinja2 syntax: `db_password: "{{ vault_db_password }}"`
    6. Encrypt the `vault` file to protect its contents
    7. Use the variable name from the `vars` file in your playbooks
-当使用Ansible的其他工具（如Terraform）时，将秘密存储在第三方秘密管理工具（如Hashicorp Vault， AWS secrets Manager等）中。
-这允许所有工具引用一个单一的真相来源的秘密，并防止配置不同步

# #风格-使用2个空格缩进，并始终缩进列表
-用一个空行分隔下列各项：
—两个主机块
-两个任务块
—主机和包含块
—变量名使用“`snake_case`”
-在`vars:`映射或变量文件中定义变量时按字母顺序排序
-始终使用多行映射语法，无论映射中存在多少对
-它提高了可读性，减少了版本控制的变更集冲突
-单引号优于双引号
-你应该使用双引号的唯一情况是当它们嵌套在单引号中（例如Jinja map引用），或者当你的字符串需要转义字符时（例如，使用“\n”来表示换行符）
-如果你必须写一个长字符串，使用折叠块标量语法（即`>`）用空格代替换行或文字块标量语法（即`|`）来保留换行；省略所有特别报价
-游戏的`host`部分应该遵循以下一般顺序：
-`hosts`声明
-主机选项按字母顺序排列（例如，`become`,`remote_user`,`vars`）
——`pre_tasks`——`roles`——`tasks`-每项任务应遵循以下一般顺序：
——`name`-任务声明（例如：`service:`，`package:`）
-任务参数（使用多行映射语法）
-循环操作符（例如，`loop`）
-按字母顺序排列任务选项（例如`become`，`ignore_errors`,`register`）
——`tags`对于`include`语句，如果它们是多行（例如，它们有标签），请引用文件名并在`include`语句之间使用空行。# #产品毛羽

—使用`ansible-lint`和`yamllint`检查语法和执行项目标准
—使用“`ansible-playbook --syntax-check`”检查语法错误
-使用`ansible-playbook --check --diff`执行剧本执行的演练<!-- 
这些指导方针是根据或从以下来源复制的：

- [Ansible Documentation - Tips and Tricks]（https://docs.ansible.com/ansible/latest/tips_tricks/index.html）
- [Whitecloud Ansible Styleguide]（https://github.com/whitecloud/ansible-styleguide）-->
