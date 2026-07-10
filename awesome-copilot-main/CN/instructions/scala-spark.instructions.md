---
description: 'Best practices for building Apache Spark applications in Scala, covering DataFrames, Datasets, SparkSQL, performance tuning, testing, and production deployment patterns.'
applyTo: '**/*.scala, **/build.sbt, **/build.sc'
---
# Scala + Apache Spark最佳实践

在Scala中编写高效、可维护和生产就绪的Apache Spark应用程序的指南。

# #依赖性

# # # SBT```scala
val sparkVersion = "3.5.1"

libraryDependencies ++= Seq(
  "org.apache.spark" %% "spark-core"   % sparkVersion % "provided",
  "org.apache.spark" %% "spark-sql"    % sparkVersion % "provided",
  "org.apache.spark" %% "spark-mllib"  % sparkVersion % "provided",
  "org.apache.spark" %% "spark-streaming" % sparkVersion % "provided"
)
```
# # # Maven```xml
<properties>
    <spark.version>3.5.1</spark.version>
    <scala.binary.version>2.13</scala.binary.version>
</properties>

<dependencies>
    <dependency>
        <groupId>org.apache.spark</groupId>
        <artifactId>spark-core_${scala.binary.version}</artifactId>
        <version>${spark.version}</version>
        <scope>provided</scope>
    </dependency>
    <dependency>
        <groupId>org.apache.spark</groupId>
        <artifactId>spark-sql_${scala.binary.version}</artifactId>
        <version>${spark.version}</version>
        <scope>provided</scope>
    </dependency>
    <dependency>
        <groupId>org.apache.spark</groupId>
        <artifactId>spark-mllib_${scala.binary.version}</artifactId>
        <version>${spark.version}</version>
        <scope>provided</scope>
    </dependency>
    <dependency>
        <groupId>org.apache.spark</groupId>
        <artifactId>spark-streaming_${scala.binary.version}</artifactId>
        <version>${spark.version}</version>
        <scope>provided</scope>
    </dependency>
</dependencies>
```
将Spark依赖项标记为`"provided"`，因为集群在运行时提供它们。只在胖JAR中绑定特定于应用程序的库。

## SparkSession设置

始终使用`SparkSession`作为单一入口点：```scala
import org.apache.spark.sql.SparkSession

val spark: SparkSession = SparkSession.builder()
  .appName("MyApplication")
  .config("spark.sql.shuffle.partitions", "200")
  .config("spark.serializer", "org.apache.spark.serializer.KryoSerializer")
  .getOrCreate()

import spark.implicits._
```
—不要在同一个JVM中创建多个`SparkSession`实例。
-避免在应用程序代码中硬编码`master`；通过`--master`在提交时设置它。

##数据框架vs数据集vs rdd

对于大多数工作负载，首选**DataFrame API** (untyped`Dataset[Row]`)。当编译时类型安全证明序列化开销合理时，使用**Datasets** (typed)。避免原始的rdd，除非你需要低级控制。```scala
import org.apache.spark.sql.{DataFrame, Dataset}

// Preferred — DataFrame API
val df: DataFrame = spark.read.parquet("data/events")
val result = df
  .filter($"status" === "active")
  .groupBy($"region")
  .agg(count("*").as("total"))

// Typed Dataset — use when schema safety matters
case class Event(id: Long, status: String, region: String)
val ds: Dataset[Event] = df.as[Event]
val active = ds.filter(_.status == "active")
```
##模式管理

在读取半结构化数据时总是显式定义模式，而不是依赖模式推断：```scala
import org.apache.spark.sql.types._

val schema = StructType(Seq(
  StructField("id", LongType, nullable = false),
  StructField("name", StringType, nullable = true),
  StructField("timestamp", TimestampType, nullable = false),
  StructField("amount", DecimalType(18, 2), nullable = true),
  StructField("tags", ArrayType(StringType), nullable = true)
))

val df = spark.read
  .schema(schema)
  .json("data/events/*.json")
```
—模式推断（`inferSchema=true`）读取整个数据源，对于大文件来说代价很高。
-对于Parquet和Delta，模式是嵌入的-不需要显式定义。

##列表达式

在转换中首选`col()`或`$""`而不是字符串列名，以便及早发现错误：```scala
import org.apache.spark.sql.functions._

// Good — type-checked column references
df.select(col("name"), $"amount" * 1.1 as "adjusted_amount")

// Avoid — string-only references delay errors to runtime
df.select("name", "amount")
```
# #连接

###广播连接

广播连接的小部分，当它适合执行器内存时（通常小于100 MB）：```scala
import org.apache.spark.sql.functions.broadcast

val enriched = largeDF.join(
  broadcast(smallLookupDF),
  Seq("key"),
  "left"
)
```
避免笛卡尔积

除非有意，否则不要使用交叉连接。启用保障：```scala
spark.conf.set("spark.sql.crossJoin.enabled", "false")
```
倾斜处理

对于歪斜键的连接，盐键来分配负载：```scala
import org.apache.spark.sql.functions._

val saltBuckets = 10
val saltedLeft = leftDF.withColumn("salt", (rand() * saltBuckets).cast("int"))
val saltedRight = rightDF
  .crossJoin((0 until saltBuckets).toDF("salt"))

val result = saltedLeft
  .join(saltedRight, Seq("join_key", "salt"))
  .drop("salt")
```
权衡的是，右侧增长了10倍，所以这只适用于右侧相当小或倾斜严重到足以证明这一点。对于Spark 3。x+， aqqe的内置倾斜连接处理（`spark.sql.adaptive.skewJoin.enabled = true`）可以自动完成此操作，而无需手动添加。

分区和bucket

###写分区

按高基数过滤器列（例如，日期）划分输出：```scala
df.write
  .partitionBy("year", "month")
  .mode("overwrite")
  .parquet("output/events")
```
避免对高基数列（例如，user ID）进行分区，这会创建数百万个小文件。

### Shuffle partition

根据数据量调优`spark.sql.shuffle.partitions`：```scala
// Default is 200; adjust based on data size
// Rule of thumb: target 128 MB per partition
spark.conf.set("spark.sql.shuffle.partitions", "400")
```
重分区vs合并```scala
// Repartition — full shuffle, use to increase or evenly distribute partitions
df.repartition(100, $"key")

// Coalesce — no shuffle, use only to reduce partition count
df.coalesce(10)
```
不要在大型数据集上使用`coalesce(1)`——它会强制所有数据通过一个任务。

缓存和持久化

仅当DataFrame被多次重用时缓存：```scala
import org.apache.spark.storage.StorageLevel

val cached = expensiveDF.persist(StorageLevel.MEMORY_AND_DISK)
cached.count() // materialize the cache

// Use cached DF multiple times
val summary = cached.groupBy("region").count()
val filtered = cached.filter($"amount" > 1000)

// Always unpersist when done
cached.unpersist()
```
-首选`MEMORY_AND_DISK`而不是`MEMORY_ONLY`，以避免在驱逐时重新计算。
-永远不要缓存只使用一次的数据帧。

## udf -谨慎使用

选择内置的Spark SQL函数而不是udf。udf禁用Catalyst优化并需要序列化：```scala
import org.apache.spark.sql.functions._

// Good — use built-in functions
df.withColumn("upper_name", upper($"name"))
  .withColumn("name_length", length($"name"))

// Avoid — UDF for something built-in functions handle
val upperUdf = udf((s: String) => s.toUpperCase)
df.withColumn("upper_name", upperUdf($"name"))
```
当UDF不可避免时，为了SparkSQL兼容性，建议使用`spark.udf.register`，并显式处理空值：```scala
val parseStatus = udf((raw: String) => {
  Option(raw).map(_.trim.toLowerCase) match {
    case Some("active") | Some("enabled")  => "ACTIVE"
    case Some("inactive") | Some("disabled") => "INACTIVE"
    case _                                   => "UNKNOWN"
  }
})
```
##窗口函数

使用窗口函数进行排名、运行总数和lag/lead计算：```scala
import org.apache.spark.sql.expressions.Window

val windowSpec = Window
  .partitionBy("department")
  .orderBy($"salary".desc)

val ranked = df
  .withColumn("rank", rank().over(windowSpec))
  .withColumn("dense_rank", dense_rank().over(windowSpec))
  .withColumn("row_number", row_number().over(windowSpec))
  .withColumn("running_total", sum($"salary").over(
    Window.partitionBy("department").orderBy("hire_date")
      .rowsBetween(Window.unboundedPreceding, Window.currentRow)
  ))
```
##错误处理

错误的记录处理```scala
val df = spark.read
  .option("mode", "PERMISSIVE")            // default: keeps corrupt rows
  .option("columnNameOfCorruptRecord", "_corrupt_record")
  .schema(schema)
  .json("data/events")

val clean = df.filter($"_corrupt_record".isNull).drop("_corrupt_record")
val bad   = df.filter($"_corrupt_record".isNotNull)
bad.write.json("data/quarantine")
```
基于累加器的错误计数```scala
val parseErrors = spark.sparkContext.longAccumulator("parseErrors")

val parsed = df.map { row =>
  try {
    parseRow(row)
  } catch {
    case _: Exception =>
      parseErrors.add(1)
      null
  }
}.filter(_ != null)

println(s"Parse errors: ${parseErrors.value}")
```
**警告：**累加器只保证准确的内部动作（`count`,`collect`,`write`）。如果任务由于失败而被重新尝试，累加器可以超时计数。对于精确的错误跟踪，请选择上面的隔离模式；蓄电池仅用于运行监测。

##流（结构化流）```scala
val stream = spark.readStream
  .format("kafka")
  .option("kafka.bootstrap.servers", "broker:9092")
  .option("subscribe", "events")
  .option("startingOffsets", "latest")
  .load()

val parsed = stream
  .selectExpr("CAST(value AS STRING) as json")
  .select(from_json($"json", schema).as("data"))
  .select("data.*")

val query = parsed.writeStream
  .format("delta")
  .option("checkpointLocation", "/checkpoints/events")
  .outputMode("append")
  .trigger(Trigger.ProcessingTime("30 seconds"))
  .start("output/events")

query.awaitTermination()
```
—始终设置容错检查点位置。
-使用`Trigger.ProcessingTime`或`Trigger.AvailableNow`-避免在生产中使用`Trigger.Once`（使用`AvailableNow`代替）。

##三角湖一体化```scala
import io.delta.tables.DeltaTable

// Upsert / merge
val target = DeltaTable.forPath(spark, "data/customers")

target.as("t")
  .merge(updatesDF.as("s"), "t.id = s.id")
  .whenMatched.updateAll()
  .whenNotMatched.insertAll()
  .execute()

// Time travel
val yesterday = spark.read
  .format("delta")
  .option("timestampAsOf", "2025-01-15")
  .load("data/customers")

// Optimize and vacuum
target.optimize().executeCompaction()
target.vacuum(168) // retain 7 days
```
性能调优检查表

1. **最小化洗牌** -使用`broadcast`连接，预分区数据，避免不必要的`groupBy`。
2. **避免在大数据框架上使用`collect()`-它会将所有数据拉到驱动程序中。
3. **首选`explain(true)`**在运行昂贵的作业之前检查物理计划。
4. **启用自适应查询执行(AQE)**：   ```scala
   spark.conf.set("spark.sql.adaptive.enabled", "true")
   spark.conf.set("spark.sql.adaptive.coalescePartitions.enabled", "true")
   spark.conf.set("spark.sql.adaptive.skewJoin.enabled", "true")
   ```
5. **使用柱状格式** （Parquet, Delta， ORC）在CSV/JSON分析工作负载。
6. **谓词下推** -过滤在查询计划的早期；在连接之前放置过滤器。
7. **列修剪** -`select`只需要列而不是`select("*")`。
8. **避免在`groupBy`**之前使用`distinct()`-聚合已经重复数据删除。

# #测试

单元测试转换

如果可能的话，测试没有SparkSession的纯转换函数：```scala
import org.scalatest.funsuite.AnyFunSuite

class TransformationsTest extends AnyFunSuite {
  test("parseStatus maps known values correctly") {
    assert(parseStatus("active") == "ACTIVE")
    assert(parseStatus("DISABLED") == "INACTIVE")
    assert(parseStatus(null) == "UNKNOWN")
  }
}
```
与SparkSession集成测试

对数据帧级别的测试使用共享的`SparkSession`：```scala
import org.apache.spark.sql.SparkSession
import org.scalatest.BeforeAndAfterAll
import org.scalatest.funsuite.AnyFunSuite

trait SparkTestBase extends AnyFunSuite with BeforeAndAfterAll {
  lazy val spark: SparkSession = SparkSession.builder()
    .master("local[2]")
    .appName("test")
    .config("spark.sql.shuffle.partitions", "2")
    .getOrCreate()

  override def afterAll(): Unit = {
    spark.stop()
    super.afterAll()
  }
}

class EventPipelineTest extends SparkTestBase {
  import spark.implicits._

  test("pipeline filters inactive events") {
    val input = Seq(
      Event(1L, "active", "US"),
      Event(2L, "inactive", "EU")
    ).toDS()

    val result = filterActive(input)
    assert(result.count() == 1)
    assert(result.collect().head.status == "active")
  }
}
```
##应用打包

### Fat JAR with sbt-assembly```scala
// project/plugins.sbt
addSbtPlugin("com.eed3si9n" % "sbt-assembly" % "2.1.5")

// build.sbt
assembly / assemblyMergeStrategy := {
  case PathList("META-INF", _*) => MergeStrategy.discard
  case _                        => MergeStrategy.first
}
```
### Spark提交```bash
spark-submit \
  --class com.example.MainApp \
  --master yarn \
  --deploy-mode cluster \
  --num-executors 10 \
  --executor-memory 8g \
  --executor-cores 4 \
  --conf spark.sql.adaptive.enabled=true \
  --conf spark.serializer=org.apache.spark.serializer.KryoSerializer \
  target/scala-2.13/my-app-assembly-1.0.jar \
  --input s3://bucket/input \
  --output s3://bucket/output
```
常见反模式

|反模式|为什么不好|修复||---|---|---|
|`collect()`上大数据|驱动|上OOM使用`take(n)`，`show()`，或写存储|
|`count()`inside循环|每次触发完整的DAG计算|缓存并计数一次|
|禁用催化剂优化器|使用`org.apache.spark.sql.functions._`|
|可变引用引起混淆|链式转换或使用`val`|
|CSV/JSON的模式推断|读取整个源代码，脆弱|显式定义`StructType`|
|`coalesce(1)`对大数据|单任务瓶颈|使用`repartition`合理计数|
|在rdd上嵌套`map`|二次复杂度|使用`join`或`broadcast`|
忽略数据倾斜|散列任务，OOM |盐键或使用AQE倾斜处理|

##动态分配启用动态分配，使Spark可以根据工作负载需求上下伸缩执行器。这对于共享集群至关重要，其中固定执行器在空闲阶段计算浪费资源：```scala
spark.conf.set("spark.dynamicAllocation.enabled", "true")
spark.conf.set("spark.dynamicAllocation.minExecutors", "2")
spark.conf.set("spark.dynamicAllocation.maxExecutors", "50")
spark.conf.set("spark.dynamicAllocation.initialExecutors", "5")
spark.conf.set("spark.dynamicAllocation.executorIdleTimeout", "60s")
spark.conf.set("spark.dynamicAllocation.schedulerBacklogTimeout", "1s")
```
或者通过`spark-submit`：```bash
spark-submit \
  --conf spark.dynamicAllocation.enabled=true \
  --conf spark.dynamicAllocation.minExecutors=2 \
  --conf spark.dynamicAllocation.maxExecutors=50 \
  --conf spark.shuffle.service.enabled=true \
  ...
```
关键的设置:

|设置|目的||---|---|
|`minExecutors`|层—始终至少保持这么多执行器运行|
|`maxExecutors`|防止集群|被独占的上限
|`initialExecutors`| |自动缩放启动前的开始计数
|`executorIdleTimeout`|在此持续时间（默认为60s）后删除空闲执行器|
|`schedulerBacklogTimeout`|当任务挂起这么长时间时请求新的执行器|

- **需要`spark.shuffle.service.enabled=true`**在YARN/Mesos-一个外部shuffle服务保留shuffle文件后，执行程序被删除。如果没有它，被删除的执行程序将丢失其shuffle数据，从而强制进行代价高昂的重新计算。
在**Kubernetes**上，使用`spark.dynamicAllocation.shuffleTracking.enabled=true`代替（不需要外部shuffle服务）。
**不要将**`--num-executors`和动态分配组合在一起，它们会发生冲突。启用动态分配时删除`--num-executors`。