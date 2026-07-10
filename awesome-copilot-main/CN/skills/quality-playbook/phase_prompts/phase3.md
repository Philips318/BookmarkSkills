{skill_fallback_guide}

你是一名质量工程师，继续一阶段一阶段地运行质量剧本。阶段1-2已经完成。

阅读这些文件来获取上下文：
1.quality/PROGRESS.md-运行元数据，阶段状态，工件清单
2.quality/EXPLORATION.md—第一阶段的发现（特别是“第二阶段的候选bug”部分）
3.quality/REQUIREMENTS.md-衍生的需求和用例
4.quality/CONTRACTS.md-行为契约
5.SKILL.md-阅读第3阶段部分（“第3阶段：代码审查和回归测试”）。同时读取references/review_protocols.md.解析SKILL.md和参考/目录通过上面的文档回退列表；不要假设任何单一的安装布局。执行阶段3：代码审查+回归测试。
对于每个确认的bug，运行每个quality/RUN_CODE_REVIEW.md.的3次代码审查：
-添加到quality/BUGS.md与### BUG-NNN标题格式
写一个回归测试（xfail-marked）
生成quality/patches/BUG-NNN-regression-test.patch（每个确认的bug都必须生成）
-生成quality/patches/BUG-NNN-fix.patch（强烈建议）
编写代码评审报告到quality/code_reviews/
-更新PROGRESS.mdBUG跟踪器

强制的网格步骤（杠杆2，v1.5.2） -模式标记的req

对于quality/REQUIREMENTS.md中每个有`Pattern:`字段的REQ (`whitelist`,`parity`，或`compensation`)，你必须在为该REQ编写任何BUG条目之前生成一个补偿网格。

* *步骤1。枚举授权项集。**机械提取源- uapi头，规格部分，记录常数。不要发明。例如：对于VIRTIO_F_RING_RESET-family， grep`include/uapi/linux/virtio_config.h`for`VIRTIO_F_*`并列出REQ覆盖的位。* *步骤2。列举这些地点。**来自REQ的每个站点UCs （UC-N）。UC-N。b,……)。如果REQ有一个单一的伞形UC，但是是模式标记的，则网格在项目上是一维的。

* *步骤3。生成网格。**写`quality/compensation_grid.json`，每个REQ一个条目：```json
{
  "schema_version": "1.5.2",
  "reqs": {
    "REQ-010": {
      "pattern": "whitelist",
      "items": ["RING_RESET", "ADMIN_VQ", "NOTIF_CONFIG_DATA", "SR_IOV"],
      "sites": ["PCI", "MMIO", "vDPA"],
      "cells": [
        {"cell_id": "REQ-010/cell-RING_RESET-PCI", "item": "RING_RESET", "site": "PCI", "present": true,  "evidence": "drivers/virtio/virtio_pci_modern.c:XXX-YYY"},
        {"cell_id": "REQ-010/cell-RING_RESET-MMIO", "item": "RING_RESET", "site": "MMIO", "present": false, "evidence": "drivers/virtio/virtio_mmio.c: no match for RING_RESET"}
      ]
    }
  }
}
```
单元格id是机械的：`REQ-<N>/cell-<item>-<site>`。没有空格，大写的item/site标识符是自然的。

* *步骤4。应用BUG-default规则。**对于每个单元格：
-该条目在权威源AND中定义
-该项在任何共享过滤器AND中都不存在
-该项目不在网站的补偿路径中

→单元格默认为BUG。使用单元格的文件发出一个`### BUG-NNN`条目：行引用、规范基础和预期与实际的行为。包括一条`- Covers: [REQ-N/cell-<item>-<site>]`线（参见schemas.md§8的现场合同）。

* *第5步。降级到QUESTION需要一个结构化的JSON记录。**每个降级单元格追加一条记录到`quality/compensation_grid_downgrades.json`：```json
{
  "schema_version": "1.5.2",
  "downgrades": [
    {
      "cell_id": "REQ-010/cell-RING_RESET-MMIO",
      "authority_ref": "include/uapi/linux/virtio_config.h:116",
      "site_citation": "drivers/virtio/virtio_mmio.c:109-131",
      "reason_class": "intentionally-partial",
      "falsifiable_claim": "MMIO does not support RING_RESET because the MMIO transport predates the feature bit and kernel docs at Documentation/virtio/virtio_mmio.rst:42-55 state the transport is frozen at its v1.0 feature set; falsifiable by showing MMIO re-sets bit 40 under any kernel release."
    }
  ]
}
```
-`reason_class`enum:`out-of-scope | deprecated | platform-gated | handled-upstream | intentionally-partial`。
—`authority_ref`、`site_citation`、`falsifiable_claim`为必填项，非空。
-`falsifiable_claim`必须陈述一个可观察到的使断言错误的条件。
-缺少任何必需的字段，或在enum之外的`reason_class`，或零长度`falsifiable_claim`→单元格在第5阶段门时间恢复到BUG。没有重新提示的循环。

* *步骤6。自检。**在完成此REQ的BUGS.md之前，请验证网格中的每个单元格是否出现在：
-一些BUG的`- Covers: [...]`列表，或
-一个降级记录在`quality/compensation_grid_downgrades.json`。

任何缺少这两个单元的单元都将无法通过第5阶段基数门。建议在第3阶段进行这种自检；阻塞门在阶段5运行。

工作示例- RING_RESET网格（virtio）

REQ-010模式：白名单。项目：{RING_RESET， ADMIN_VQ, NOTIF_CONFIG_DATA, SR_IOV}。站点：{PCI， MMIO, vDPA}。网格：4 × 3 = 12个单元格。代码检查显示PCI实现了所有这四个；MMIO没有实现这四种功能（冻结在v1.0功能集）；vDPA实现了NOTIF_CONFIG_DATA，但不实现其他三个。

网格（在场=T，缺席=F）：

| | PCI | MMIO | vDPA ||-----------------------|-----|------|------|
| ring_reset | t | f | f |
| admin_vq | t | f | f |
| notif_config_data | t | f | t |
| sr_iov | t | f | f |

BUG-default应用于每个F单元格（总共8个）。可能的整合:

MMIO忽略VIRTIO_F_RING_RESET
-主要要求：REQ-010
-封面：[REQ-010/cell-RING_RESET-MMIO]

vDPA忽略了VIRTIO_F_RING_RESET
-主要要求：REQ-010
-封面：[REQ-010/cell-RING_RESET-vDPA]

BUG-003: vDPA缺少ADMIN_VQ连接
-主要要求：REQ-010
-封面：[REQ-010/cell-ADMIN_VQ-vDPA]

### BUG-004: MMIO忽略NOTIF_CONFIG_DATA协商（公共过滤器间隙）
-主要要求：REQ-010
-封面：[REQ-010/cell-NOTIF_CONFIG_DATA-MMIO]MMIO + vDPA都错过了SR_IOV的传播
-主要要求：REQ-010
-封面：[REQ-010/cell-SR_IOV-MMIO，REQ-010/cell-SR_IOV-vDPA]
-合并原理：两个传输中的共享修复路径经过相同的特征位过滤器；共享帮助器上的单个补丁关闭两个单元。

如果审稿人认为MMIO ADMIN_VQ是故意超出范围的，因为ADMIN_VQ是一个仅限pci的规范功能，降级记录将是：```json
{
  "cell_id": "REQ-010/cell-ADMIN_VQ-MMIO",
  "authority_ref": "include/uapi/linux/virtio_pci.h:NN",
  "site_citation": "drivers/virtio/virtio_mmio.c: no admin virtqueue implementation",
  "reason_class": "out-of-scope",
  "falsifiable_claim": "ADMIN_VQ is MMIO-scoped — falsifiable by citing any virtio-spec normative text requiring ADMIN_VQ on non-PCI transports."
}
```
联合检查：8个bug覆盖单元+ 1个降级单元= 9。网格有12个单元格；目前的细胞不需要覆盖。总共：8个F细胞通过BUGs覆盖+ 1个通过降级=所有9个缺失细胞。网格→清洁。

迭代模式附录（强制增量写入，阶段8）

在迭代模式（间隙/未过滤/奇偶校验/对抗性）下运行时，在识别时立即将候选BUG存根写入磁盘，而不是在审查结束时。路径:`quality/code_reviews/<iteration>-candidates.md`。每个候选人一个`### CANDIDATE-NNN`标题，至少有一个文件行引用。审核人员仅在完全分类后才将候选程序升级为BUGS.md中已确认的bug。

确认清单（杠杆2，v1.5.2）

在编写阶段3完成检查点到PROGRESS.md之前，明确确认阶段3摘要中的每个项目：1. 对于每个模式标记的REQ，我在`quality/compensation_grid.json`中生成了一个补偿网格。
2. 对于每个网格，我机械地应用BUG-default规则。
3. 为模式标记的REQ发出的每个BUG都有一个带有有效单元格id的`- Covers: [...]`字段。
4. 每个覆盖列表有≥2个条目的BUG都有一个非空的`- Consolidation rationale: ...`字段。
5. 对于每个降级的单元格，我在`quality/compensation_grid_downgrades.json`中编写了一个完整的结构化记录，其中包含所有五个必需字段和一个有效的`reason_class`。
6. 对于每个模式标记的REQ，覆盖列表+降级单元格的并集等于网格的单元格集。

标记阶段3（代码审查+回归测试）在PROGRESS.md中完成（使用复选框格式`- [x] Phase 3 - Code Review`-不要切换到表格）。

重要：不要进入第4阶段（规格审核）。下一阶段将使用一个新的上下文窗口运行规范审计。