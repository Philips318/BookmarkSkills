数据工厂集成

Microsoft Fabric包括用于ETL/ELT编排的数据工厂：

- 180+连接器**用于数据源
- **复制活动**用于数据移动
- **数据流Gen2**转换
- **笔记本活动**用于Spark处理
—**调度**和触发器

管道活动

|活动|描述||----------|-------------|
|复制数据|在数据源和Lakehouse |之间移动数据
|笔记本|执行Spark笔记本|
| Dataflow |运行Dataflow Gen2转换|
|存储过程|执行SQL过程|
| ForEach |遍历|项
| If条件|条件分支|
|获取元数据|检索file/folder元数据|
|湖屋维护|优化和真空三角洲表|

编排模式```
Pipeline: Daily_ETL_Pipeline
├── Get Metadata (check for new files)
├── ForEach (process each file)
│   ├── Copy Data (bronze layer)
│   └── Notebook (silver transformation)
├── Notebook (gold aggregation)
└── Lakehouse Maintenance (optimize tables)
```

---