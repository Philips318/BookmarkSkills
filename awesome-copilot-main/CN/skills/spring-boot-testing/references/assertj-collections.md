# AssertJ集合

断言集合：`List`、`Set`、`Map`、数组和流。

何时使用此引用

—待测值为`List`、`Set`、`Map`、array或`Stream`-您需要断言多个元素、它们的顺序或其中的特定字段
—您正在使用`extracting()`、`filteredOn()`、`containsExactly()`或类似的收集方法
-断言单个标量或单个对象→使用[assertj-basics.md]（assertj-basics.md）代替

##基本收集检查```java
List<Order> orders = orderService.findAll();

assertThat(orders).isNotEmpty();
assertThat(orders).isEmpty();
assertThat(orders).hasSize(3);
assertThat(orders).hasSizeGreaterThan(0);
assertThat(orders).hasSizeLessThanOrEqualTo(10);
```
##包含断言```java
// Contains (any order, allows extras)
assertThat(orders).contains(order1, order2);

// Contains exactly these elements in this order (no extras)
assertThat(statuses).containsExactly("NEW", "PENDING", "COMPLETED");

// Contains exactly these elements in any order (no extras)
assertThat(statuses).containsExactlyInAnyOrder("COMPLETED", "NEW", "PENDING");

// Contains any of these elements (at least one match required)
assertThat(statuses).containsAnyOf("NEW", "CANCELLED");

// Does not contain
assertThat(statuses).doesNotContain("DELETED");
```
##提取字段

在断言之前从每个元素中提取一个字段：```java
assertThat(orders)
  .extracting(Order::getStatus)
  .containsExactly("NEW", "PENDING", "COMPLETED");
```
提取多个字段为元组：```java
assertThat(orders)
  .extracting(Order::getId, Order::getStatus)
  .containsExactly(
    tuple(1L, "NEW"),
    tuple(2L, "PENDING"),
    tuple(3L, "COMPLETED")
  );
```
断言前过滤```java
assertThat(orders)
  .filteredOn(order -> order.getStatus().equals("PENDING"))
  .hasSize(2)
  .extracting(Order::getId)
  .containsExactlyInAnyOrder(1L, 3L);

// Filter by field value
assertThat(orders)
  .filteredOn("status", "PENDING")
  .hasSize(2);
```
##谓词检查```java
assertThat(orders).allMatch(o -> o.getTotal().compareTo(BigDecimal.ZERO) > 0);
assertThat(orders).anyMatch(o -> o.getStatus().equals("COMPLETED"));
assertThat(orders).noneMatch(o -> o.getStatus().equals("DELETED"));

// With description for failure messages
assertThat(orders)
  .allSatisfy(o -> assertThat(o.getId()).isPositive());
```
每元素有序断言

使用单独的条件按顺序断言每个元素：```java
assertThat(orders).satisfiesExactly(
  first  -> assertThat(first.getStatus()).isEqualTo("NEW"),
  second -> assertThat(second.getStatus()).isEqualTo("PENDING"),
  third  -> {
    assertThat(third.getStatus()).isEqualTo("COMPLETED");
    assertThat(third.getTotal()).isGreaterThan(BigDecimal.ZERO);
  }
);
```
嵌套/平面集合```java
// flatExtracting: flatten one level of nested collections
assertThat(orders)
  .flatExtracting(Order::getItems)
  .extracting(OrderItem::getProduct)
  .contains("Laptop", "Mouse");
```
##递归字段比较

通过字段而不是对象标识来比较元素：```java
assertThat(orders)
  .usingRecursiveFieldByFieldElementComparator()
  .containsExactlyInAnyOrder(expectedOrder1, expectedOrder2);

// Ignore specific fields (e.g. generated IDs or timestamps)
assertThat(orders)
  .usingRecursiveFieldByFieldElementComparatorIgnoringFields("id", "createdAt")
  .containsExactly(expectedOrder1, expectedOrder2);
```
##映射断言```java
Map<String, Integer> stockByProduct = inventoryService.getStock();

assertThat(stockByProduct)
  .isNotEmpty()
  .hasSize(3)
  .containsKey("Laptop")
  .doesNotContainKey("Fax Machine")
  .containsEntry("Laptop", 10)
  .containsEntries(entry("Laptop", 10), entry("Mouse", 50));

assertThat(stockByProduct)
  .hasEntrySatisfying("Laptop", qty -> assertThat(qty).isGreaterThan(0));
```
##数组断言```java
String[] roles = user.getRoles();

assertThat(roles).hasSize(2);
assertThat(roles).contains("ADMIN");
assertThat(roles).containsExactlyInAnyOrder("USER", "ADMIN");
```
##设置断言```java
Set<String> tags = product.getTags();

assertThat(tags).contains("electronics", "sale");
assertThat(tags).doesNotContain("expired");
assertThat(tags).hasSizeGreaterThanOrEqualTo(1);
```
##静态导入```java
import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.tuple;
import static org.assertj.core.api.Assertions.entry;
```
##要点

1. **`containsExactly`与`containsExactlyInAnyOrder`** -当顺序重要时使用前者
2. **包含检查之前的`extracting()`** -避免在域对象上实现`equals()`3. **`filteredOn()`+`extracting()`** - compose精确断言集合的子集
4. **`satisfiesExactly()`** -在每个元素需要不同断言时使用
5. **`usingRecursiveFieldByFieldElementComparator()`** -优先于dto和记录的`equals()`