#权限和令牌

每个工作流运行都会得到一个自动的`GITHUB_TOKEN`。它的范围是爆炸半径，如果一个台阶是
妥协了，所以把范围缩小到最小。

##默认设置太宽泛

如果工作流没有`permissions:`块，它继承repository/organization默认值。在
对于大多数作用域，默认为**read/write的旧的或允许的repos **。单个注入命令
或者恶意依赖，然后运行推送代码、发布版本或批准pr的能力。

##最低特权配方

在顶层设置限制性默认值，然后仅在需要时提升每个作业。```yaml
# Deny by default
permissions: {}

jobs:
  build:
    permissions:
      contents: read          # checkout only
    runs-on: ubuntu-latest
    steps: [...]

  comment:
    permissions:
      contents: read
      pull-requests: write    # this job posts a comment; nothing else
    runs-on: ubuntu-latest
    steps: [...]
```
常用范围：`contents`、`pull-requests`、`issues`、`actions`、`packages`、`id-token`、`deployments`,`checks`,`statuses`。分别是`read`、`write`或`none`。

要标记的发现

*任何地方都没有`permissions:`块→MEDIUM（继承可能广泛的默认值）。
*`permissions: write-all`→HIGH。
*一个`write`作用域的作业的步骤永远不会使用→HIGH（删除它）。
*顶级的`write`应该存在于一个工作中→MEDIUM（向下移动）。

## OIDC代替长期存在的云秘密

将静态云密钥（`AWS_ACCESS_KEY_ID`等）存储为回购秘密意味着泄漏是永久性的
直到手动旋转。首选OpenID连接：工作流向云请求一个短期令牌
提供程序信任，范围为repo/branch，在几分钟内到期。```yaml
permissions:
  id-token: write     # required to request the OIDC token
  contents: read
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: aws-actions/configure-aws-credentials@<sha>
        with:
          role-to-assume: arn:aws:iam::123456789012:role/my-ci-role
          aws-region: us-east-1
      # no AWS_ACCESS_KEY_ID / AWS_SECRET_ACCESS_KEY secrets needed
```
同样的模式也存在于Azure （`azure/login`）、GCP （`google-github-actions/auth`）、HashiCorp
金库等。在云计算方面，将信任策略范围限定到特定的回购，理想情况下是a
特定的branch/environment，因此分支或其他repo不能承担该角色。

##秘密卫生

*只在需要秘密的工作中引用秘密。
*永远不要`echo`一个秘密或启用shell跟踪（`set -x`）在处理一个步骤。
*不要把秘密传递给你没有确认和审查过的第三方行动。
*记住fork`pull_request`运行没有秘密-不要试图通过切换到`pull_request_target`（参见`triggers-and-privilege.md`）。