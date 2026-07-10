#回购行动中心GitHub Copilot画布，用于浏览存储库GitHub Actions工作流、查看最近的运行、在模态中打开丰富的workflow/run细节，以及触发手动`workflow_dispatch`运行。

# #文件

-`extension.mjs`-画布服务器，GitHub Actions数据加载，模态细节UI，和工作流运行动作。
-`assets/preview.png`-画廊预览图像的扩展目录。`copilot-extension.json`-副驾驶扩展name/version元数据。
-`package.json`-用于编目和打包的扩展元数据。
-`.github/plugin/plugin.json`-插件元数据使用的扩展市场和网站。

# #安装

要求Copilot安装提交的扩展URL：```text
Install this extension: https://github.com/github/awesome-copilot/tree/main/extensions/repo-actions-hub
```
您也可以将文件夹复制到以下位置之一：

—`~/.copilot/extensions/repo-actions-hub/`—用户范围
-`.github/extensions/repo-actions-hub/`-项目范围

在应用程序中重新加载扩展，然后打开`repo-actions-hub`画布。

代理操作

-`get_state`-返回活动存储库的当前工作流和最近运行状态。
-`refresh`-重新加载工作流和最近从GitHub Actions运行。
-`get_workflow_details { workflowId }`-检查工作流，它的调度支持，输入，YAML和最近的运行。
-`run_workflow { workflowId, ref?, inputs? }`-触发一个工作流的`workflow_dispatch`运行。