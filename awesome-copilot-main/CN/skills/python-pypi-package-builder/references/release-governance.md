发布治理——分支、保护、OIDC和访问控制

##目录
1. (分支策略)(# 1-branch-strategy)
2. [分支保护规则]（#2-branch-protection-rules）
3. [基于标签的发布模型]（#3- Tag-Based Release - Model）
4. [基于角色的访问控制]（#4-基于角色的访问控制）
5. [使用OIDC（可信发布）的安全发布]（#5-secure-publishing-with-oidc-trusted-publishing）
6. [在CI中验证标签作者]（#6-validate-tag-author-in-ci）
7. [防止无效发布标签]（#7- Prevent - invalidate - Release - Tags）
8. （#8-full-publishyml-with-governance-gates）

---

# # 1。分支策略

使用清晰的分支层次结构将开发工作与可发布代码分开。```
main          ← stable; only receives PRs from develop or hotfix/*
develop       ← integration branch; all feature PRs merge here first
feature/*     ← new capabilities (e.g., feature/add-redis-backend)
fix/*         ← bug fixes (e.g., fix/memory-leak-on-close)
hotfix/*      ← urgent production fixes; PR directly to main + cherry-pick to develop
release/*     ← (optional) release preparation (e.g., release/v2.0.0)
```
# # #规则

|规则|为什么是||---|---|
|不直接推到`main`|防止稳定支路|意外断裂
|所有通过PR进行的更改|在合并|之前强制审查+ CI
至少需要一个批准|第二对眼睛对所有的变化|
永远不要合并损坏的代码
|只有标签触发发布|，没有分支推送|的特别发布

---

# # 2。分支保护规则

在**GitHub→设置→分支→为`main`和`develop`添加规则**。

###为`main````yaml
# Equivalent GitHub branch protection config (for documentation)
branch: main
rules:
  - require_pull_request_reviews:
      required_approving_review_count: 1
      dismiss_stale_reviews: true
  - require_status_checks_to_pass:
      contexts:
        - "Lint, Format & Type Check"
        - "Test (Python 3.11)"   # at minimum; add all matrix versions
      strict: true               # branch must be up-to-date before merge
  - restrict_pushes:
      allowed_actors: []         # nobody — only PR merges
  - require_linear_history: true # prevents merge commits on main
```
###为`develop````yaml
branch: develop
rules:
  - require_pull_request_reviews:
      required_approving_review_count: 1
  - require_status_checks_to_pass:
      contexts: ["CI"]
      strict: false   # less strict for the integration branch
```
###通过GitHub CLI```bash
# Protect main (requires gh CLI and admin rights)
gh api repos/{owner}/{repo}/branches/main/protection \
  --method PUT \
  --input - <<'EOF'
{
  "required_status_checks": {
    "strict": true,
    "contexts": ["Lint, Format & Type Check", "Test (Python 3.11)"]
  },
  "enforce_admins": false,
  "required_pull_request_reviews": {
    "required_approving_review_count": 1,
    "dismiss_stale_reviews": true
  },
  "restrictions": null
}
EOF
```
---

# # 3。基于标签的发布模型

**只有`main`上的注释标签才会触发释放。**分支推送和PR合并永远不会发布。

标签命名约定```
vMAJOR.MINOR.PATCH           # Stable:         v1.2.3
vMAJOR.MINOR.PATCHaN         # Alpha:           v2.0.0a1
vMAJOR.MINOR.PATCHbN         # Beta:            v2.0.0b1
vMAJOR.MINOR.PATCHrcN        # Release Candidate: v2.0.0rc1
```
发布工作流```bash
# 1. Merge develop → main via PR (reviewed, CI green)

# 2. Update CHANGELOG.md on main
#    Move [Unreleased] entries to [vX.Y.Z] - YYYY-MM-DD

# 3. Commit the changelog
git checkout main
git pull origin main
git add CHANGELOG.md
git commit -m "chore: release v1.2.3"

# 4. Create and push an annotated tag
git tag -a v1.2.3 -m "Release v1.2.3"
git push origin v1.2.3          # ← ONLY the tag; not --tags (avoids pushing all tags)

# 5. Confirm: GitHub Actions publish.yml triggers automatically
#    Monitor: Actions tab → publish workflow
#    Verify:  https://pypi.org/project/your-package/
```
为什么要标注标签？

带注释的标记（`git tag -a`）携带标记器标识、日期和消息——轻量级标记则如此
不是。`setuptools_scm`可以同时使用这两种标记，但是对于发布治理来说，带注释的标记更安全，因为
它们记录了“谁”创建了这个标签。

---

# # 4。基于角色的访问控制

|角色|他们能做什么||---|---|
| **维护者** |创建发布标签，批准pr，管理分支保护|
| **贡献者** |开放PRs到`develop`；不能推送到`main`或创建发布标签|
| **CI (GitHub Actions)** |通过OIDC发布到PyPI；无法推送代码或创建标签|

通过GitHub团队实现```bash
# Create a Maintainers team and restrict tag creation to that team
gh api repos/{owner}/{repo}/tags/protection \
  --method POST \
  --field pattern="v*"
# Then set allowed actors to the Maintainers team only
```
---

# # 5。使用OIDC（可信发布）进行安全发布

**永远不要将PyPI API令牌存储为GitHub秘密。**使用可信发布（OIDC）代替。
PyPI项目授权一个特定的GitHub存储库+工作流+环境-不长寿命
交换秘密。

一次性PyPI设置

1. 到https://pypi.org/manage/project/your-package/settings/publishing/2. 单击**添加新的发布者**
3. 填写:
**所有者：** your-github-username
- **存储库：** your-repo-name
- **工作流名称：**`publish.yml`- **环境名称：**`release`（必须与工作流中的`environment:`键匹配）
4. 保存。不需要令牌。

GitHub环境设置

1. 转到**GitHub→设置→环境→新环境**→命名为`release`2. 添加一个保护规则：**需要审查者**（可选，但建议额外的安全）
3. 添加部署分支规则：**只支持匹配`v*`**的标签

最小`publish.yml`使用OIDC```yaml
# .github/workflows/publish.yml
name: Publish to PyPI

on:
  push:
    tags:
      - "v[0-9]+.[0-9]+.[0-9]+*"   # Matches v1.0.0, v2.0.0a1, v1.2.3rc1

jobs:
  publish:
    name: Build and publish
    runs-on: ubuntu-latest
    environment: release       # Must match the PyPI Trusted Publisher environment name
    permissions:
      id-token: write          # Required for OIDC — grants a short-lived token to PyPI
      contents: read

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0       # REQUIRED for setuptools_scm

      - uses: actions/setup-python@v5
        with:
          python-version: "3.11"

      - name: Install build
        run: pip install build

      - name: Build distributions
        run: python -m build

      - name: Validate distributions
        run: pip install twine ; twine check dist/*

      - name: Publish to PyPI
        uses: pypa/gh-action-pypi-publish@release/v1
        # No `password:` or `user:` needed — OIDC handles authentication
```
---

# # 6。在CI中验证标记作者

通过检查允许列表中的`GITHUB_ACTOR`来限制谁可以触发释放。
将此添加为发布作业的第一步，以快速失败。```yaml
- name: Validate tag author
  run: |
    ALLOWED_USERS=("your-github-username" "co-maintainer-username")
    if [[ ! " ${ALLOWED_USERS[*]} " =~ " ${GITHUB_ACTOR} " ]]; then
      echo "::error::Release blocked: ${GITHUB_ACTOR} is not an authorised releaser."
      exit 1
    fi
    echo "Release authorised for ${GITHUB_ACTOR}."
```
# # #笔记

-`GITHUB_ACTOR`是推标签的人的GitHub用户名。
-为了可维护性，将允许列表存储在一个单独的文件中（例如，`.github/MAINTAINERS`）。
-对于团队：用GitHub API调用替换用户名检查来验证团队成员资格。

---

# # 7。防止无效释放标签

拒绝由不遵循版本控制约定的标记触发的工作流运行。
这样可以防止意外发布来自`test`、`backup-old`或`v1`等标记。```yaml
- name: Validate release tag format
  run: |
    # Accepts: v1.0.0  v1.0.0a1  v1.0.0b2  v1.0.0rc1  v1.0.0.post1
    if [[ ! "${GITHUB_REF}" =~ ^refs/tags/v[0-9]+\.[0-9]+\.[0-9]+(a|b|rc|\.post)[0-9]*$ ]] && \
       [[ ! "${GITHUB_REF}" =~ ^refs/tags/v[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
      echo "::error::Tag '${GITHUB_REF}' does not match the required format v<MAJOR>.<MINOR>.<PATCH>[pre]."
      exit 1
    fi
    echo "Tag format valid: ${GITHUB_REF}"
```
### Regex解释

|模式|匹配||---|---|
|`v[0-9]+\.[0-9]+\.[0-9]+`|`v1.0.0`,`v12.3.4`|
|`(a\|b\|rc)[0-9]*`|`v1.0.0a1`,`v2.0.0rc2`|
|`\.post[0-9]*`|`v1.0.0.post1`|

---

# # 8。完整的`publish.yml`与治理门

完整的工作流程，包括标签验证、作者检查、TestPyPI门和产品发布。```yaml
# .github/workflows/publish.yml
name: Publish to PyPI

on:
  push:
    tags:
      - "v[0-9]+.[0-9]+.[0-9]+*"

jobs:
  publish:
    name: Build, validate, and publish
    runs-on: ubuntu-latest
    environment: release
    permissions:
      id-token: write
      contents: read

    steps:
      - name: Validate release tag format
        run: |
          if [[ ! "${GITHUB_REF}" =~ ^refs/tags/v[0-9]+\.[0-9]+\.[0-9]+(a[0-9]*|b[0-9]*|rc[0-9]*|\.post[0-9]*)?$ ]]; then
            echo "::error::Invalid tag format: ${GITHUB_REF}"
            exit 1
          fi

      - name: Validate tag author
        run: |
          ALLOWED_USERS=("your-github-username")
          if [[ ! " ${ALLOWED_USERS[*]} " =~ " ${GITHUB_ACTOR} " ]]; then
            echo "::error::${GITHUB_ACTOR} is not authorised to release."
            exit 1
          fi

      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - uses: actions/setup-python@v5
        with:
          python-version: "3.11"

      - name: Install build tooling
        run: pip install build twine

      - name: Build
        run: python -m build

      - name: Validate distributions
        run: twine check dist/*

      - name: Publish to TestPyPI
        uses: pypa/gh-action-pypi-publish@release/v1
        with:
          repository-url: https://test.pypi.org/legacy/
        continue-on-error: true   # Non-fatal; remove if you always want this to pass

      - name: Publish to PyPI
        uses: pypa/gh-action-pypi-publish@release/v1
```
安全检查表

- [] PyPI可信发布配置（没有API令牌存储在GitHub）
- [] GitHub`release`环境有分支保护：标签只匹配`v*`-[]标签格式验证步骤是作业的第一步
-[]定期维护和审查允许用户列表
[]日志中没有秘密打印（检查所有`echo`和`run`步骤）
—[]`permissions:`的取值范围仅为`id-token: write`，不为`write-all`