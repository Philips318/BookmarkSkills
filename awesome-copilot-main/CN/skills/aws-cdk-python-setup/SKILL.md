---
name: aws-cdk-python-setup
description: Setup and initialization guide for developing AWS CDK (Cloud Development Kit) applications in Python. This skill enables users to configure environment prerequisites, create new CDK projects, manage dependencies, and deploy to AWS.
---
# AWS CDK Python设置说明

此技能为使用**Python**使用**AWS CDK（云开发工具包）**项目提供了设置指导。

---

# #先决条件

启动前，请确保已安装以下工具：

- **Node.js**≥14.15.0 - AWS CDK命令行要求
- **Python**≥3.7 -用于编写CDK代码
- **AWS CLI** -管理凭证和资源
- **Git** -版本控制和项目管理

---

##安装步骤

# # # 1。安装AWS CDK命令行```bash
npm install -g aws-cdk
cdk --version
```
# # # 2。配置AWS凭证```bash
# Install AWS CLI (if not installed)
brew install awscli

# Configure credentials
aws configure
```
根据提示输入您的AWS访问密钥、秘密访问密钥、默认区域和输出格式。

# # # 3。创建一个新的CDK项目```bash
mkdir my-cdk-project
cd my-cdk-project
cdk init app --language python
```
你的项目将包括：
-`app.py`-主应用程序入口点
-`my_cdk_project/`- CDK栈定义
-`requirements.txt`- Python依赖项
—`cdk.json`—配置文件

# # # 4。设置Python虚拟环境```bash
# macOS/Linux
source .venv/bin/activate

# Windows
.venv\Scripts\activate
```
# # # 5。安装Python依赖项```bash
pip install -r requirements.txt
```
主要依赖关系:
-`aws-cdk-lib`-核心CDK结构
-`constructs`-基本构造库

---

##开发流程

合成CloudFormation模板```bash
cdk synth
```
生成包含CloudFormation模板的`cdk.out/`。

###将堆栈部署到AWS```bash
cdk deploy
```
审查并确认部署到已配置的AWS帐户。

### Bootstrap（仅限首次部署）```bash
cdk bootstrap
```
为资产存储准备环境资源，如S3桶。

---

最佳实践

—工作前一定要激活虚拟环境。
—部署前执行`cdk diff`命令预览变更。
-使用开发帐户进行测试。
-遵循python命名和目录约定。
-保持`requirements.txt`固定一致的构建。

---

##故障排除提示

如果出现问题，请检查：

—AWS凭证配置正确。
—默认区域设置正确。
—“Node.js”和“Python”版本满足最低要求。
—执行`cdk doctor`命令诊断环境问题。