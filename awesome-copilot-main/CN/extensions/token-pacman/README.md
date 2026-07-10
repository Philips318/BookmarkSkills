# Token吃豆人GitHub Copilot画布，将实时会话ai信用使用情况可视化为吃豆人棋盘。吃豆人在消耗积分时吃小球，幽灵追逐他，水果里程碑出现，当配置的会话积分限制超过时游戏结束。

# #文件

-`extension.mjs`-画布声明，环回服务器，实时usage/quota同步和代理操作。
-`assets/preview.png`-画廊预览图像所需的令人敬畏的副驾驶画布目录。
-`assets/token-pacman.jpg`-源代码截图包括为画廊。
-`copilot-extension.json`-副驾驶扩展name/version元数据用于gist安装。
-`canvas.json`-很棒的副驾驶画廊元数据。
-`package.json`-生成的网站目录使用的扩展元数据。

# #安装

要求Copilot安装提交的扩展URL：```text
Install this extension: https://github.com/github/awesome-copilot/tree/main/extensions/token-pacman
```
共享的要点版本也可在以下网址获得：```text
https://gist.github.com/jamesmontemagno/75d701d25f49c94ba332529fb8ec1346
```
代理操作

-`sync_usage`-从活动会话累积的ai信用使用和计划授权中刷新画布。
-`set_limit { limit }`-设置触发游戏结束和重新同步颗粒板的ai信用限制。
-`reset_run`-清除可见的水果条纹，并开始一个新的追逐不改变现场会话信用总额。