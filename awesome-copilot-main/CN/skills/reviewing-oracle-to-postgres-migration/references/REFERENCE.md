#参考索引

|文件|简要描述|| --- | --- |
| [empty-strings-handling.md](empty-strings-handling.md) | Oracle将"视为NULL；PostgreSQL保留空字符串的不同模式，以便在代码、测试和迁移中对齐行为。|
| [no-data-found-exceptions.md](no-data-found-exceptions.md) | Oracle SELECT INTO抛出“no data found”；PostgreSQL没有添加显式的NOT FOUND处理来镜像Oracle的行为。|
| [oracle-parentheses-from-clause.md](oracle-parentheses-from-clause.md) |允许`FROM(TABLE_NAME)`语法；PostgreSQL要求`FROM TABLE_NAME`-删除表名周围不必要的括号。|
| [oracle-to-postgres-sorting.md](oracle-to-postgres-sorting.md) |如何在PostgreSQL中使用COLLATE “C”和DISTINCT包装模式来保持oracle式的排序。|
| [oracle-to-postgres-to-char-numeric.md](oracle-to-postgres-to-char-numeric.md)；PostgreSQL要求格式字符串-使用CAST（数字作为文本）代替。|
| [oracle-to-postgres-type-coercion.md](oracle-to-postgres-type-coercion.md) | PostgreSQL严格类型检查与Oracle隐式强制转换-通过引用或转换字面量修复比较错误。|
| [postgres-concurrent-transactions.md](postgres-concurrent-transactions.md) | PostgreSQL只允许一个活动的com每个连接指定结果或使用单独的连接以避免并发操作错误。|
| [postgres-refcursor-handling.md](postgres-refcursor-handling.md) | refcursor处理差异；PostgreSQL需要通过游标名来读取结果。|
| [oracle-to-postgres-timestamp-timezone.md](oracle-to-postgres-timestamp-timezone.md) | CURRENT_TIMESTAMP / NOW（）返回PostgreSQL的utc标准化时间戳；Npgsql显示DateTime。类型=未指定的强制UTC连接打开和应用程序代码。|