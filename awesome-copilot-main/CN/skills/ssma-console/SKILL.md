---
name: ssma-console
description: "Use when: SSMA console operations — create project, generate assessment report, convert schema, migrate data, Oracle to SQL Server migration, schema conversion, data migration"
---
# SSMA Console - Oracle到SQL Server迁移

生成XML配置并直接调用`SSMAforOracleConsole.exe`—不需要外部脚本或包装器。

**操作**（为了“完全迁移”而运行）：
1. **创建-项目-连接源和目标，映射模式
2. **生成-报告** -评估报告
3. ** migration -schema** -转换和部署模式
4. ** migration -data** -端到端转换、部署、迁移数据

##收集输入

询问缺少的参数。默认值在括号中。

**Oracle**：主机（`localhost`）、端口（`1521`）、实例*（必选，服务名）*、用户、密码、架构
**SQL Server**：服务器，数据库，用户，密码，加密（`true`），信任服务器证书（`true`），目标架构（`dbo`）
**项目**：名称（`ssma-migration`），文件夹（`.`），类型（`sql-server-2022`-也是`2016`/`2017`/`2019`/`2025`/`sql-azure`）， SSMA路径（`C:\Program Files\Microsoft SQL Server Migration Assistant for Oracle\bin\SSMAforOracleConsole.exe`）

##生成XML文件在写入前解析所有`{PLACEHOLDER}`标记。生成3个文件：### `ssma-variables.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<variables>
  <variable name="$WorkingFolder$" value="{PROJECT_FOLDER}" />
  <variable name="$ProjectType$" value="{PROJECT_TYPE}" />
  <variable name="$ProjectName$" value="{PROJECT_NAME}" />
  <variable-group name="OracleConnection">
    <variable name="$OracleHostName$" value="{ORACLE_HOST}" />
    <variable name="$OracleInstance$" value="{ORACLE_INSTANCE}" />
    <variable name="$OraclePort$" value="{ORACLE_PORT}" />
    <variable name="$OracleUserName$" value="{ORACLE_USER}" />
    <variable name="$OraclePassword$" value="{ORACLE_PASSWORD}" />
    <variable name="$OracleSchemaName$" value="{ORACLE_SCHEMA}" />
  </variable-group>
  <variable-group name="SQLServerConnection">
    <variable name="$SQLServerName$" value="{SQL_SERVER}" />
    <variable name="$SQLServerDb$" value="{SQL_DATABASE}" />
    <variable name="$SQLServerUsrID$" value="{SQL_USER}" />
    <variable name="$SQLServerPwd$" value="{SQL_PASSWORD}" />
  </variable-group>
  <variable-group name="ReportSettings">
    <variable name="$SummaryReportFile$" value="Reports\Assessment\AssessmentReport.xml" />
    <variable name="$ConversionReportFile$" value="Reports\Conversion\ConversionReport.xml" />
    <variable name="$ConversionReportFolder$" value="Reports\Conversion" />
    <variable name="$DataMigrationReportFile$" value="Reports\Migration\DataMigrationReport.xml" />
    <variable name="$SynchronizationReportFolder$" value="Reports\Synchronization" />
  </variable-group>
</variables>
```

### `ssma-servers.xml`
**CRITICAL**：使用`tns-name-mode`-`standard-mode`将实例视为SID， ORA-12505失败。```xml
<?xml version="1.0" encoding="utf-8"?>
<servers>
  <oracle name="source_oracle">
    <tns-name-mode>
      <connection-provider value="OracleClient" />
      <service-name value="(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = $OracleHostName$)(PORT = $OraclePort$)))(CONNECT_DATA =(SERVICE_NAME = $OracleInstance$)))" />
      <user-id value="$OracleUserName$" />
      <password value="$OraclePassword$" />
    </tns-name-mode>
  </oracle>
  <sql-server name="target_sqlserver">
    <sql-server-authentication>
      <server value="$SQLServerName$" />
      <database value="$SQLServerDb$" />
      <user-id value="$SQLServerUsrID$" />
      <password value="$SQLServerPwd$" />
      <encrypt value="{ENCRYPT}" />
      <trust-server-certificate value="{TRUST_CERT}" />
    </sql-server-authentication>
  </sql-server>
</servers>
```
###操作脚本XML

每个操作生成一个脚本。所有脚本都共享这个通用的`<config>`块（为migrate-schema/migrate-data添加`<object-overwrite action="overwrite" />`，为迁移数据添加`<data-migration-connection source-use-last-used="true" target-server="target_sqlserver" />`，为schema/dataops使用`every-5%`progress）：```xml
<config>
  <output-providers>
    <output-window suppress-messages="false" destination="stdout" />
    <upgrade-project action="yes" />
    <user-input-popup mode="continue" />
    <progress-reporting enable="true" report-messages="true" report-progress="every-10%" />
    <log-verbosity level="info" />
  </output-providers>
</config>
```
所有脚本都以`<script-commands>`开头：```xml
<create-new-project project-folder="$WorkingFolder$" project-name="$ProjectName$"
                    overwrite-if-exists="true" project-type="$ProjectType$" />
<connect-source-database server="source_oracle">
  <object-to-collect object-name="$OracleSchemaName$" />
</connect-source-database>
```
**CRITICAL**：总是包含`<object-to-collect>`-没有它，`map-schema`会失败并显示“Source namespace was not found”。

**每个操作命令**（在序言之后，在`<save-project />`之前）：

|操作|文件|前导|后的命令|-----------|------|------------------------|
|创建项目|`ssma-create-project.xml`|`connect-target-database`→`map-schema source-schema="$OracleSchemaName$" sql-server-schema="$SQLServerDb$.{TARGET_SCHEMA}"`|
|生成-报告|`ssma-assessment.xml`|`generate-assessment-report object-name="$OracleSchemaName$" object-type="Schemas" write-summary-report-to="$SummaryReportFile$" verbose="true" report-errors="true"`|
| migration -schema |`ssma-schema.xml`|`connect-target-database`→`map-schema`→`convert-schema`（to`$ConversionReportFile$`）→`synchronize-target object-name="$SQLServerDb$.{TARGET_SCHEMA}"`|
| migrate-data |`ssma-data.xml`|与migrate-schema +`refresh-from-database`→`migrate-data object-name="$OracleSchemaName$.Tables" object-type="category"`（to`$DataMigrationReportFile$`）→`close-project`|相同

# #执行

向用户显示解析后的XML和命令。运行前请确认。```powershell
New-Item -ItemType Directory -Force -Path "Reports\Assessment","Reports\Conversion","Reports\Migration","Reports\Synchronization","Logs" | Out-Null
& "{SSMA_CONSOLE_PATH}" -s "{SCRIPT_XML}" -c "ssma-servers.xml" -v "ssma-variables.xml" -l "Logs\{OPERATION}.log"
```
##报告结果

检查退出代码（`0`= success），读取日志和报告（`Reports\Assessment\`、`Reports\Conversion\`、`Reports\Migration\`），总结发现。

# #约束

-没有外部脚本-没有`.ps1`，`.bat`,`.sh`—执行前请确认连接详情
-解析所有占位符-在最终XML中没有`{...}`—执行前创建输出目录

##已知陷阱

|修复||---------|-----|
使用`tns-name-mode`，而不是`standard-mode`|
|`Source namespace was not found`|将`<object-to-collect>`添加到`connect-source-database`|`not found in metabase`on`force-load`|使用`object-to-collect`代替-`force-load`是不可靠的|
|`SQL Server Agent is not running`|仅警告- BCP客户端迁移仍然有效|