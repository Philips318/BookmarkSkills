---
description: 'Scala 2.12/2.13 programming language coding conventions and best practices following Databricks style guide for functional programming, type safety, and production code quality.'
applyTo: '**/*.scala, **/build.sbt, **/build.sc'
---
# Scala最佳实践

基于[Databricks Scala风格指南]（https://github.com/databricks/scala-style-guide）

##核心原则

###编写简单代码
代码编写一次，但读取和修改多次。通过编写简单的代码来优化长期的可读性和可维护性。

默认不变性
-总是选择`val`而不是`var`使用来自`scala.collection.immutable`的不可变集合
Case类的构造函数参数不应该是可变的
—使用复制构造器创建修改后的实例```scala
// Good - Immutable case class
case class Person(name: String, age: Int)

// Bad - Mutable case class
case class Person(name: String, var age: Int)

// To change values, use copy constructor
val p1 = Person("Peter", 15)
val p2 = p1.copy(age = 16)

// Good - Immutable collections
val users = List(User("Alice", 30), User("Bob", 25))
val updatedUsers = users.map(u => u.copy(age = u.age + 1))
```
纯函数
-函数应该是确定的和无副作用的
-将纯逻辑与效果分开
-对有效果的方法使用显式类型```scala
// Good - Pure function
def calculateTotal(items: List[Item]): BigDecimal =
  items.map(_.price).sum

// Bad - Impure function with side effects
def calculateTotal(items: List[Item]): BigDecimal = {
  println(s"Calculating total for ${items.size} items")  // Side effect
  val total = items.map(_.price).sum
  saveToDatabase(total)  // Side effect
  total
}
```
命名约定

类和对象```scala
// Classes, traits, objects - PascalCase
class ClusterManager
trait Expression
object Configuration

// Packages - all lowercase ASCII
package com.databricks.resourcemanager

// Methods/functions - camelCase
def getUserById(id: Long): Option[User]
def processData(input: String): Result

// Constants - uppercase in companion object
object Configuration {
  val DEFAULT_PORT = 10000
  val MAX_RETRIES = 3
  val TIMEOUT_MS = 5000L
}
```
变量和参数```scala
// Variables - camelCase, self-evident names
val serverPort = 1000
val clientPort = 2000
val maxRetryAttempts = 3

// One-character names OK in small, localized scope
for (i <- 0 until 10) {
  // ...
}

// Do NOT use "l" (Larry) - looks like "1", "|", "I"
```
# # #枚举```scala
// Enumeration object - PascalCase
// Values - UPPER_CASE with underscores
private object ParseState extends Enumeration {
  type ParseState = Value

  val PREFIX,
      TRIM_BEFORE_SIGN,
      SIGN,
      VALUE,
      UNIT_BEGIN,
      UNIT_END = Value
}
```
语法样式

行长度和间距```scala
// Limit lines to 100 characters
// One space before and after operators
def add(int1: Int, int2: Int): Int = int1 + int2

// One space after commas
val list = List("a", "b", "c")

// One space after colons
def getConf(key: String, defaultValue: String): String = {
  // code
}

// Use 2-space indentation
if (true) {
  println("Wow!")
}

// 4-space indentation for long parameter lists
def newAPIHadoopFile[K, V, F <: NewInputFormat[K, V]](
    path: String,
    fClass: Class[F],
    kClass: Class[K],
    vClass: Class[V],
    conf: Configuration = hadoopConfiguration): RDD[(K, V)] = {
  // method body
}

// Class with long parameters
class Foo(
    val param1: String,  // 4 space indent
    val param2: String,
    val param3: Array[Byte])
  extends FooInterface  // 2 space indent
  with Logging {

  def firstMethod(): Unit = { ... }  // blank line above
}
```
### 30法则

—一个方法应该包含少于30行代码
—一个类应该包含少于30个方法

###大括号```scala
// Always use curly braces for multi-line blocks
if (true) {
  println("Wow!")
}

// Exception: one-line ternary (side-effect free)
val result = if (condition) value1 else value2

// Always use braces for try-catch
try {
  foo()
} catch {
  case e: Exception => handle(e)
}
```
###长文字```scala
// Use uppercase L for long literals
val longValue = 5432L  // Do this
val badValue = 5432l   // Don't do this - hard to see
```
# # #括号```scala
// Methods with side-effects - use parentheses
class Job {
  def killJob(): Unit = { ... }  // Correct - changes state
  def getStatus: JobStatus = { ... }  // Correct - no side-effect
}

// Callsite should match declaration
new Job().killJob()  // Correct
new Job().getStatus  // Correct
```
# # #进口```scala
// Avoid wildcard imports unless importing 6+ entities
import scala.collection.mutable.{Map, HashMap, ArrayBuffer}

// OK to use wildcard for implicits or 6+ items
import scala.collection.JavaConverters._
import java.util.{Map, HashMap, List, ArrayList, Set, HashSet}

// Always use absolute paths
import scala.util.Random  // Good
// import util.Random     // Don't use relative

// Import order (with blank lines):
import java.io.File
import javax.servlet.http.HttpServlet

import scala.collection.mutable.HashMap
import scala.util.Random

import org.apache.spark.SparkContext
import org.apache.spark.rdd.RDD

import com.databricks.MyClass
```
模式匹配```scala
// Put match on same line if method is entirely pattern match
def test(msg: Message): Unit = msg match {
  case TextMessage(text) => handleText(text)
  case ImageMessage(url) => handleImage(url)
}

// Single case closures - same line
list.zipWithIndex.map { case (elem, i) =>
  // process
}

// Multiple cases - indent and wrap
list.map {
  case a: Foo => processFoo(a)
  case b: Bar => processBar(b)
  case _ => handleDefault()
}

// Match on type only - don't expand all args
case class Pokemon(name: String, weight: Int, hp: Int, attack: Int, defense: Int)

// Bad - brittle when fields change
targets.foreach {
  case Pokemon(_, _, hp, _, defense) =>
    // error prone
}

// Good - match on type
targets.foreach {
  case p: Pokemon =>
    val loss = math.min(0, myAttack - p.defense)
    p.copy(hp = p.hp - loss)
}
```
匿名函数```scala
// Avoid excessive parentheses
// Correct
list.map { item =>
  transform(item)
}

// Correct
list.map(item => transform(item))

// Wrong - unnecessary braces
list.map(item => {
  transform(item)
})

// Wrong - excessive nesting
list.map({ item => ... })
```
###中缀方法```scala
// Avoid infix for non-symbolic methods
list.map(func)  // Correct
list map func   // Wrong

// OK for operators
arrayBuffer += elem
```
##语言特性

避免在类上使用apply（）```scala
// Avoid apply on classes - hard to trace
class TreeNode {
  def apply(name: String): TreeNode = { ... }  // Don't do this
}

// OK on companion objects as factory
object TreeNode {
  def apply(name: String): TreeNode = new TreeNode(name)  // OK
}
```
### override Modifier```scala
// Always use override - even for abstract methods
trait Parent {
  def hello(data: Map[String, String]): Unit
}

class Child extends Parent {
  // Without override, this might not actually override!
  override def hello(data: Map[String, String]): Unit = {
    println(data)
  }
}
```
避免在构造函数中解构```scala
// Don't use destructuring binds in constructors
class MyClass {
  // Bad - creates non-transient Tuple2
  @transient private val (a, b) = someFuncThatReturnsTuple2()

  // Good
  @transient private val tuple = someFuncThatReturnsTuple2()
  @transient private val a = tuple._1
  @transient private val b = tuple._2
}
```
避免直呼其名```scala
// Avoid call-by-name parameters
// Bad - caller can't tell if executed once or many times
def print(value: => Int): Unit = {
  println(value)
  println(value + 1)
}

// Good - explicit function type
def print(value: () => Int): Unit = {
  println(value())
  println(value() + 1)
}
```
避免使用多个参数列表```scala
// Avoid multiple parameter lists (except for implicits)
// Bad
case class Person(name: String, age: Int)(secret: String)

// Good
case class Person(name: String, age: Int, secret: String)

// Exception: separate list for implicits (but avoid implicits!)
def foo(x: Int)(implicit ec: ExecutionContext): Future[Int]
```
符号方法```scala
// Only use for arithmetic operators
class Vector {
  def +(other: Vector): Vector = { ... }  // OK
  def -(other: Vector): Vector = { ... }  // OK
}

// Don't use for other methods
// Bad
channel ! msg
stream1 >>= stream2

// Good
channel.send(msg)
stream1.join(stream2)
```
类型推断```scala
// Always type public methods
def getUserById(id: Long): Option[User] = { ... }

// Always type implicit methods
implicit def stringToInt(s: String): Int = s.toInt

// Type variables when not obvious (3 second rule)
val user: User = complexComputation()

// OK to omit when obvious
val count = 5
val name = "Alice"
```
###返回语句```scala
// Avoid return in closures - uses exceptions under the hood
def receive(rpc: WebSocketRPC): Option[Response] = {
  tableFut.onComplete { table =>
    if (table.isFailure) {
      return None  // Don't do this - wrong thread!
    }
  }
}

// Use return as guard to simplify control flow
def doSomething(obj: Any): Any = {
  if (obj eq null) {
    return null
  }
  // do something
}

// Use return to break loops early
while (true) {
  if (cond) {
    return
  }
}
```
递归和尾递归```scala
// Avoid recursion unless naturally recursive (trees, graphs)
// Use @tailrec for tail-recursive methods
@scala.annotation.tailrec
def max0(data: Array[Int], pos: Int, max: Int): Int = {
  if (pos == data.length) {
    max
  } else {
    max0(data, pos + 1, if (data(pos) > max) data(pos) else max)
  }
}

// Prefer explicit loops for clarity
def max(data: Array[Int]): Int = {
  var max = Int.MinValue
  for (v <- data) {
    if (v > max) {
      max = v
    }
  }
  max
}
```
值得一提的是# # #```scala
// Avoid implicits unless:
// 1. Building a DSL
// 2. Implicit type parameters (ClassTag, TypeTag)
// 3. Private type conversions within your class

// If you must use them, don't overload
object ImplicitHolder {
  // Bad - can't selectively import
  def toRdd(seq: Seq[Int]): RDD[Int] = { ... }
  def toRdd(seq: Seq[Long]): RDD[Long] = { ... }
}

// Good - distinct names
object ImplicitHolder {
  def intSeqToRdd(seq: Seq[Int]): RDD[Int] = { ... }
  def longSeqToRdd(seq: Seq[Long]): RDD[Long] = { ... }
}
```
##类型安全

代数数据类型```scala
// Sum types - sealed traits with case classes
sealed trait PaymentMethod
case class CreditCard(number: String, cvv: String) extends PaymentMethod
case class PayPal(email: String) extends PaymentMethod
case class BankTransfer(account: String, routing: String) extends PaymentMethod

def processPayment(payment: PaymentMethod): Either[Error, Receipt] = payment match {
  case CreditCard(number, cvv) => chargeCreditCard(number, cvv)
  case PayPal(email) => chargePayPal(email)
  case BankTransfer(account, routing) => chargeBankAccount(account, routing)
}

// Product types - case classes
case class User(id: Long, name: String, email: String, age: Int)
case class Order(id: Long, userId: Long, items: List[Item], total: BigDecimal)
```
###选项大于null```scala
// Use Option instead of null
def findUserById(id: Long): Option[User] = {
  database.query(id)
}

// Use Option() to guard against nulls
def myMethod1(input: String): Option[String] = Option(transform(input))

// Don't use Some() - it won't protect against null
def myMethod2(input: String): Option[String] = Some(transform(input)) // Bad

// Pattern matching on Option
def processUser(id: Long): String = findUserById(id) match {
  case Some(user) => s"Found: ${user.name}"
  case None => "User not found"
}

// Don't call get() unless absolutely sure
val user = findUserById(123).get  // Dangerous!

// Use getOrElse, map, flatMap, fold instead
val name = findUserById(123).map(_.name).getOrElse("Unknown")
```
###错误处理```scala
sealed trait ValidationError
case class InvalidEmail(email: String) extends ValidationError
case class InvalidAge(age: Int) extends ValidationError
case class MissingField(field: String) extends ValidationError

def validateUser(data: Map[String, String]): Either[ValidationError, User] = {
  for {
    name <- data.get("name").toRight(MissingField("name"))
    email <- data.get("email").toRight(MissingField("email"))
    validEmail <- validateEmail(email)
    ageStr <- data.get("age").toRight(MissingField("age"))
    age <- ageStr.toIntOption.toRight(InvalidAge(-1))
  } yield User(name, validEmail, age)
}
```
Try vs Exceptions```scala
// Don't return Try from APIs
// Bad
def getUser(id: Long): Try[User]

// Good - explicit throws
@throws(classOf[DatabaseConnectionException])
def getUser(id: Long): Option[User]

// Use NonFatal for catching exceptions
import scala.util.control.NonFatal

try {
  dangerousOperation()
} catch {
  case NonFatal(e) =>
    logger.error("Operation failed", e)
  case e: InterruptedException =>
    // handle interruption
}
```
# #集合

###首选不可变集合```scala
import scala.collection.immutable._

// Good
val numbers = List(1, 2, 3, 4, 5)
val doubled = numbers.map(_ * 2)
val evens = numbers.filter(_ % 2 == 0)

val userMap = Map(
  1L -> "Alice",
  2L -> "Bob"
)
val updated = userMap + (3L -> "Charlie")

// Use Stream (Scala 2.12) or LazyList (Scala 2.13) for lazy sequences
val fibonacci: LazyList[BigInt] =
  BigInt(0) #:: BigInt(1) #:: fibonacci.zip(fibonacci.tail).map { case (a, b) => a + b }

val first10 = fibonacci.take(10).toList
```
一元链```scala
// Avoid chaining more than 3 operations
// Break after flatMap
// Don't chain with if-else blocks

// Bad - too complex
database.get(name).flatMap { elem =>
  elem.data.get("address").flatMap(Option.apply)
}

// Good - more readable
def getAddress(name: String): Option[String] = {
  if (!database.contains(name)) {
    return None
  }

  database(name).data.get("address") match {
    case Some(null) => None
    case Some(addr) => Option(addr)
    case None => None
  }
}

// Don't chain with if-else
// Bad
if (condition) {
  Seq(1, 2, 3)
} else {
  Seq(1, 2, 3)
}.map(_ + 1)

// Good
val seq = if (condition) Seq(1, 2, 3) else Seq(4, 5, 6)
seq.map(_ + 1)
```
# #性能

使用while循环```scala
// For performance-critical code, use while instead of for/map
val arr = Array.fill(1000)(Random.nextInt())

// Slow
val newArr = arr.zipWithIndex.map { case (elem, i) =>
  if (i % 2 == 0) 0 else elem
}

// Fast
val newArr = new Array[Int](arr.length)
var i = 0
while (i < arr.length) {
  newArr(i) = if (i % 2 == 0) 0 else arr(i)
  i += 1
}
```
###选项vs null```scala
// For performance-critical code, prefer null over Option
class Foo {
  @javax.annotation.Nullable
  private[this] var nullableField: Bar = _
}
```
###使用private[this]```scala
// private[this] generates fields, not accessor methods
class MyClass {
  private val field1 = ...        // Might use accessor
  private[this] val field2 = ...  // Direct field access

  def perfSensitiveMethod(): Unit = {
    var i = 0
    while (i < 1000000) {
      field2  // Guaranteed field access
      i += 1
    }
  }
}
```
Java集合```scala
// For performance, prefer Java collections
import java.util.{ArrayList, HashMap}

val list = new ArrayList[String]()
val map = new HashMap[String, Int]()
```
# #并发

###首选ConcurrentHashMap```scala
// Use java.util.concurrent.ConcurrentHashMap
private[this] val map = new java.util.concurrent.ConcurrentHashMap[String, String]

// Or synchronized map for low contention
private[this] val map = java.util.Collections.synchronizedMap(
  new java.util.HashMap[String, String]
)
```
显式同步```scala
class Manager {
  private[this] var count = 0
  private[this] val map = new java.util.HashMap[String, String]

  def update(key: String, value: String): Unit = synchronized {
    map.put(key, value)
    count += 1
  }

  def getCount: Int = synchronized { count }
}
```
###原子变量```scala
import java.util.concurrent.atomic._

// Prefer Atomic over @volatile
val initialized = new AtomicBoolean(false)

// Clearly express only-once execution
if (!initialized.getAndSet(true)) {
  initialize()
}
```
# #测试

###拦截特定异常```scala
import org.scalatest._

// Bad - too broad
intercept[Exception] {
  thingThatThrows()
}

// Good - specific type
intercept[IllegalArgumentException] {
  thingThatThrows()
}
```
## SBT配置```scala
// build.sbt
ThisBuild / version := "0.1.0-SNAPSHOT"
ThisBuild / scalaVersion := "2.13.12"
ThisBuild / organization := "com.example"

lazy val root = (project in file("."))
  .settings(
    name := "my-application",

    libraryDependencies ++= Seq(
      "org.typelevel" %% "cats-core" % "2.10.0",
      "org.typelevel" %% "cats-effect" % "3.5.2",

      // Testing
      "org.scalatest" %% "scalatest" % "3.2.17" % Test,
      "org.scalatestplus" %% "scalacheck-1-17" % "3.2.17.0" % Test
    ),

    scalacOptions ++= Seq(
      "-encoding", "UTF-8",
      "-feature",
      "-unchecked",
      "-deprecation",
      "-Xfatal-warnings"
    )
  )
```
# #杂项

###使用nanoTime```scala
// Use nanoTime for durations, not currentTimeMillis
val start = System.nanoTime()
doWork()
val elapsed = System.nanoTime() - start

import java.util.concurrent.TimeUnit
val elapsedMs = TimeUnit.NANOSECONDS.toMillis(elapsed)
```
URI优于URL```scala
// Use URI instead of URL (URL.equals does DNS lookup!)
val uri = new java.net.URI("http://example.com")
// Not: val url = new java.net.URL("http://example.com")
```
# #总结

1. **编写简单代码** -优化可读性和可维护性
2. **使用不可变数据** - val，不可变集合，case类
3. **避免语言特性** -限制隐式，避免符号方法
4. **类型公共api ** -方法和字段的显式类型
5. **更喜欢显式而不是隐式** -清晰比简洁更好
6. **使用标准库** -不要重新发明轮子
7. **遵循命名约定** - PascalCase， camelCase, UPPER_CASE
8. **保持方法小** - 30法则
9. **显式处理错误** -选项，要么，异常与@抛出
10. **优化前的配置文件** -衡量，不要猜测

完整的细节，请参阅[Databricks Scala风格指南]（https://github.com/databricks/scala-style-guide）。