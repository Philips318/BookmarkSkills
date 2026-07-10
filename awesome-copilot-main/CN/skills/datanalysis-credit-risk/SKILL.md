---
name: datanalysis-credit-risk
description: Credit risk data cleaning and variable screening pipeline for pre-loan modeling. Use when working with raw credit data that needs quality assessment,  missing value analysis, or variable selection before modeling. it covers data loading and formatting, abnormal period filtering, missing rate calculation, high-missing variable removal,low-IV variable filtering, high-PSI variable removal, Null Importance denoising, high-correlation variable removal, and cleaning report generation. Applicable scenarios arecredit risk data cleaning, variable screening, pre-loan modeling preprocessing.
---
#数据清理和变量筛选

##快速入门```bash
# Run the complete data cleaning pipeline
python ".github/skills/datanalysis-credit-risk/scripts/example.py"
```
完成进程描述

数据清洗管道包括以下11个步骤，每个步骤独立执行，不删除原始数据：1. **获取数据** -加载和格式化原始数据
2. **组织样本分析** -统计每个组织的样本数量和不良样本率
3. **分离OOS数据** -将OOS样本从建模样本中分离出来
4. **过滤异常月份** -去除不良样本计数或总样本计数不足的月份
5. **计算缺失率** -计算每个功能的整体和组织级缺失率
6. **删除高缺失率功能** -删除整体缺失率超过阈值的功能
7. **删除低IV功能** -删除功能与整体IV过低或IV过低在太多的组织
8. **删除高PSI功能** -删除不稳定PSI功能
9. **Null Importance去噪** -使用标签置换法去除噪声特征
10. **删除高相关特征** -删除基于原始的高相关特征l获得
11. **导出报表** -生成包含所有步骤的详细信息和统计信息的Excel报表##核心功能

|功能|用途|模块||------|------|----------|
|`get_dataset()`|加载并格式化数据|引用。func |
|`org_analysis()`|组织样例分析|参考资料。func |
|`missing_check()`|计算缺失率|引用。func |
|`drop_abnormal_ym()`|过滤异常月份|引用。分析|
|`drop_highmiss_features()`| Drop高缺失率特性|引用。分析|
|`drop_lowiv_features()`|降低IV功能|参考。分析|
|`drop_highpsi_features()`| Drop高PSI特性|参考。分析|
|`drop_highnoise_features()`|零重要性去噪|引用。分析|
|`drop_highcorr_features()`|删除高相关性特征|参考。分析|
|`iv_distribution_by_org()`| IV分布统计信息|引用。分析|
|`psi_distribution_by_org()`| PSI分布统计信息|引用。分析|
|`value_ratio_distribution_by_org()`|值比分布统计数据|引用。分析|
|`export_cleaning_report()`|导出清理报告|引用。分析|

##参数说明###数据加载参数
-`DATA_PATH`：数据文件路径（最好是parquet格式）
—`DATE_COL`：日期列名
—`Y_COL`：标签列名
—`ORG_COL`：组织列名
—`KEY_COLS`：主键列名称列表

OOS组织配置
-`OOS_ORGS`：样本外组织列表

月份过滤参数异常
-`min_ym_bad_sample`：每月最小坏样数（默认10）
-`min_ym_sample`：每月最小样本总数（默认500）

缺少速率参数
-`missing_ratio`：总体缺失率阈值（默认0.6）

### IV参数
-`overall_iv_threshold`：整体IV阈值（默认0.1）
-`org_iv_threshold`：单一组织IV阈值（默认0.1）
-`max_org_threshold`：最大可容忍的低IV组织数（默认2）### PSI参数
-`psi_threshold`: PSI阈值（默认0.1）
-`max_months_ratio`：最大不稳定月比（默认为1/3）
-`max_orgs`：最大不稳定组织数（默认6）

###空重要性参数
-`n_estimators`：树的数量（默认100）
-`max_depth`：最大树深度（默认5）
—`gain_threshold`：增益差阈值（默认为50）

高相关性参数
—`max_corr`：相关阈值（默认0.9）
-`top_n_keep`：保留前N个特性的原始增益排名（默认20）

##输出报告

生成的Excel报表包含以下表格：1. ** ** ** -所有步骤的汇总信息，包括操作结果和条件
2. ** -每个组织的样品计数和坏样率
3. ** - OOS样本和建模样本计数
4. **Step4-删除异常月份
5. ** -每个特性的整体和组织级缺失率
6. **Step5-不同值比范围内的特征分布
7. **Step6-被移除的高缺失率特征
8. **Step7-IV：每个组织和整体的每个特征的IV值
9. **Step7-IV ** -不满足IV条件和低IV组织的特征
10. **Step7-IV -不同IV范围的特征分布
11. **Step8-PSI -每个组织每个功能每月的PSI值
12. **Step8-PSI ** -不满足PSI条件和组织不稳定的特性13. **Step8-PSI -不同PSI范围内的特征分布
14. **步骤9-零重要性被去除的噪声特征
15. **步骤10-去除高相关性特征# #特性

—**交互式输入**：可在每一步执行前输入参数，支持默认值
- **独立执行**：每一步独立执行，不删除原始数据，便于对比分析
—**完整报表**：生成完整的Excel报表，包含详细信息、统计数据和分布情况
- **多进程支持**:IV和PSI计算支持多进程加速
- **组织级分析**：支持组织级统计和modeling/OOS区分