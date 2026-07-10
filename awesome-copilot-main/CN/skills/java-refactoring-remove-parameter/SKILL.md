---
name: java-refactoring-remove-parameter
description: 'Refactoring using Remove Parameter in Java Language'
---
#用Remove参数重构Java方法

# #的作用

您是重构Java方法方面的专家。

下面是代表**Remove Parameter**的**2个示例**（在重构之前和之后都有标题代码）。

##重构前的代码```java
public Backend selectBackendForGroupCommit(long tableId, ConnectContext context, boolean isCloud)
        throws LoadException, DdlException {
    if (!Env.getCurrentEnv().isMaster()) {
        try {
            long backendId = new MasterOpExecutor(context)
                    .getGroupCommitLoadBeId(tableId, context.getCloudCluster(), isCloud);
            return Env.getCurrentSystemInfo().getBackend(backendId);
        } catch (Exception e) {
            throw new LoadException(e.getMessage());
        }
    } else {
        return Env.getCurrentSystemInfo()
                .getBackend(selectBackendForGroupCommitInternal(tableId, context.getCloudCluster(), isCloud));
    }
}
```
##重构后的代码：```java
public Backend selectBackendForGroupCommit(long tableId, ConnectContext context)
        throws LoadException, DdlException {
    if (!Env.getCurrentEnv().isMaster()) {
        try {
            long backendId = new MasterOpExecutor(context)
                    .getGroupCommitLoadBeId(tableId, context.getCloudCluster());
            return Env.getCurrentSystemInfo().getBackend(backendId);
        } catch (Exception e) {
            throw new LoadException(e.getMessage());
        }
    } else {
        return Env.getCurrentSystemInfo()
                .getBackend(selectBackendForGroupCommitInternal(tableId, context.getCloudCluster()));
    }
}
```
##重构前代码2：```java
NodeImpl( long id, long firstRel, long firstProp )
{
     this( id, false );
}
```
重构后的代码2：```java
NodeImpl( long id)
{
     this( id, false );
}
```
# #任务

应用**删除参数**以提高可读性、可测试性、可维护性、可重用性、模块化、内聚性、低耦合性和一致性。

始终返回一个完整且可编译的方法（Java 17）。

内部执行中间步骤：
-首先，分析每个方法并识别未使用或冗余的参数（即，可以从类字段，常量或其他方法调用中获得的值）。
-对于每个合格的方法，从其定义和所有内部调用中删除不必要的参数。
-确保该方法在移除参数后继续正常工作。
-只输出一个‘ ’`java`‘ ’块内的重构代码。
-不要从原始方法中删除任何功能。
-在每个修改的方法上面加上一行注释，说明删除了哪个参数以及删除的原因。

需要重构的代码：现在，评估所有带有未使用参数的方法，并使用**Remove Parameter**进行重构