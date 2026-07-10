# Dataverse SDK for Python - Pandas集成指南

# #概述
将Dataverse SDK与pandas DataFrames集成到数据科学和分析工作流的指南。SDK的JSON响应格式无缝映射到pandas dataframe，使数据科学家能够使用熟悉的数据操作工具处理dataaverse数据。

---

# # 1。PandasODataClient简介

什么是PandasODataClient？`PandasODataClient`是标准`DataverseClient`的一个薄包装，它以pandas DataFrame格式返回数据，而不是原始JSON字典。这使得它非常适合：
-数据科学家处理表格数据
-分析和报告工作流程
-数据挖掘和清理
-与机器学习管道集成

###安装要求```bash
# Install core dependencies
pip install PowerPlatform-Dataverse-Client
pip install azure-identity

# Install pandas for data manipulation
pip install pandas
```
何时使用PandasODataClient
✅**需要时使用：**
-数据挖掘和分析
-处理表格数据
-与statistical/ML库集成
-高效的数据操作

❌**使用DataverseClient代替当你需要：**
-仅支持实时CRUD操作
-文件上传操作
—元数据操作
-单记录操作

---

# # 2。基本数据框架工作流

将查询结果转换为DataFrame```python
from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient
import pandas as pd

# Setup authentication
base_url = "https://<myorg>.crm.dynamics.com"
credential = InteractiveBrowserCredential()
client = DataverseClient(base_url=base_url, credential=credential)

# Query data
pages = client.get(
    "account",
    select=["accountid", "name", "creditlimit", "telephone1"],
    filter="statecode eq 0",
    orderby=["name"]
)

# Collect all pages into one DataFrame
all_records = []
for page in pages:
    all_records.extend(page)

# Convert to DataFrame
df = pd.DataFrame(all_records)

# Display first few rows
print(df.head())
print(f"Total records: {len(df)}")
```
###查询参数映射到DataFrame```python
# All query parameters return as columns in DataFrame
df = pd.DataFrame(
    client.get(
        "account",
        select=["accountid", "name", "creditlimit", "telephone1", "createdon"],
        filter="creditlimit > 50000",
        orderby=["creditlimit desc"]
    )
)

# Result is a DataFrame with columns:
# accountid | name | creditlimit | telephone1 | createdon
```
---

# # 3。使用熊猫进行数据探索

基本探索```python
import pandas as pd
from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient

client = DataverseClient("https://<myorg>.crm.dynamics.com", InteractiveBrowserCredential())

# Load account data
records = []
for page in client.get("account", select=["accountid", "name", "creditlimit", "industrycode"]):
    records.extend(page)

df = pd.DataFrame(records)

# Explore the data
print(df.shape)           # (1000, 4)
print(df.dtypes)          # Data types
print(df.describe())      # Statistical summary
print(df.info())          # Column info and null counts
print(df.head(10))        # First 10 rows
```
过滤和选择```python
# Filter rows by condition
high_value = df[df['creditlimit'] > 100000]

# Select specific columns
names_limits = df[['name', 'creditlimit']]

# Multiple conditions
filtered = df[(df['creditlimit'] > 50000) & (df['industrycode'] == 1)]

# Value counts
print(df['industrycode'].value_counts())
```
排序和分组```python
# Sort by column
sorted_df = df.sort_values('creditlimit', ascending=False)

# Group by and aggregate
by_industry = df.groupby('industrycode').agg({
    'creditlimit': ['mean', 'sum', 'count'],
    'name': 'count'
})

# Group statistics
print(df.groupby('industrycode')['creditlimit'].describe())
```
数据清理```python
# Handle missing values
df_clean = df.dropna()                    # Remove rows with NaN
df_filled = df.fillna(0)                  # Fill NaN with 0
df_ffill = df.fillna(method='ffill')      # Forward fill

# Check for duplicates
duplicates = df[df.duplicated(['name'])]
df_unique = df.drop_duplicates()

# Data type conversion
df['creditlimit'] = pd.to_numeric(df['creditlimit'])
df['createdon'] = pd.to_datetime(df['createdon'])
```
---

# # 4。数据分析模式

聚合和汇总```python
# Create summary report
summary = df.groupby('industrycode').agg({
    'accountid': 'count',
    'creditlimit': ['mean', 'min', 'max', 'sum'],
    'name': lambda x: ', '.join(x.head(3))  # Sample names
}).round(2)

print(summary)
```
时间序列分析```python
# Convert to datetime
df['createdon'] = pd.to_datetime(df['createdon'])

# Resample to monthly
monthly = df.set_index('createdon').resample('M').size()

# Extract date components
df['year'] = df['createdon'].dt.year
df['month'] = df['createdon'].dt.month
df['day_of_week'] = df['createdon'].dt.day_name()
```
连接和合并操作```python
# Load two related tables
accounts = pd.DataFrame(client.get("account", select=["accountid", "name"]))
contacts = pd.DataFrame(client.get("contact", select=["contactid", "parentcustomerid", "fullname"]))

# Merge on relationship
merged = accounts.merge(
    contacts,
    left_on='accountid',
    right_on='parentcustomerid',
    how='left'
)

print(merged.head())
```
统计分析```python
# Correlation matrix
correlation = df[['creditlimit', 'industrycode']].corr()

# Distribution analysis
print(df['creditlimit'].describe())
print(df['creditlimit'].skew())
print(df['creditlimit'].kurtosis())

# Percentiles
print(df['creditlimit'].quantile([0.25, 0.5, 0.75]))
```
---

# # 5。数据透视表和报表

创建数据透视表```python
# Pivot table by industry and status
pivot = pd.pivot_table(
    df,
    values='creditlimit',
    index='industrycode',
    columns='statecode',
    aggfunc=['sum', 'mean', 'count']
)

print(pivot)
```
生成报告```python
# Sales report by industry
industry_report = df.groupby('industrycode').agg({
    'accountid': 'count',
    'creditlimit': 'sum',
    'name': 'first'
}).rename(columns={
    'accountid': 'Account Count',
    'creditlimit': 'Total Credit Limit',
    'name': 'Sample Account'
})

# Export to CSV
industry_report.to_csv('industry_report.csv')

# Export to Excel
industry_report.to_excel('industry_report.xlsx')
```
---

# # 6。数据可视化

Matplotlib集成```python
import matplotlib.pyplot as plt

# Create visualizations
fig, axes = plt.subplots(2, 2, figsize=(12, 10))

# Histogram
df['creditlimit'].hist(bins=30, ax=axes[0, 0])
axes[0, 0].set_title('Credit Limit Distribution')

# Bar chart
df['industrycode'].value_counts().plot(kind='bar', ax=axes[0, 1])
axes[0, 1].set_title('Accounts by Industry')

# Box plot
df.boxplot(column='creditlimit', by='industrycode', ax=axes[1, 0])
axes[1, 0].set_title('Credit Limit by Industry')

# Scatter plot
df.plot.scatter(x='creditlimit', y='industrycode', ax=axes[1, 1])
axes[1, 1].set_title('Credit Limit vs Industry')

plt.tight_layout()
plt.show()
```
###海运一体化```python
import seaborn as sns

# Correlation heatmap
plt.figure(figsize=(8, 6))
sns.heatmap(df[['creditlimit', 'industrycode']].corr(), annot=True)
plt.title('Correlation Matrix')
plt.show()

# Distribution plot
sns.distplot(df['creditlimit'], kde=True)
plt.title('Credit Limit Distribution')
plt.show()
```
---

# # 7。机器学习集成

为ML准备数据```python
from sklearn.preprocessing import StandardScaler
from sklearn.model_selection import train_test_split

# Load and prepare data
records = []
for page in client.get("account", select=["accountid", "creditlimit", "industrycode", "statecode"]):
    records.extend(page)

df = pd.DataFrame(records)

# Feature engineering
df['log_creditlimit'] = np.log1p(df['creditlimit'])
df['industry_cat'] = pd.Categorical(df['industrycode']).codes

# Split features and target
X = df[['industrycode', 'log_creditlimit']]
y = df['statecode']

# Train-test split
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2)

print(f"Training set: {len(X_train)}, Test set: {len(X_test)}")
```
构建分类模型```python
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import classification_report

# Train model
model = RandomForestClassifier(n_estimators=100)
model.fit(X_train, y_train)

# Evaluate
y_pred = model.predict(X_test)
print(classification_report(y_test, y_pred))

# Feature importance
importances = pd.Series(
    model.feature_importances_,
    index=X.columns
).sort_values(ascending=False)

print(importances)
```
---

# # 8。高级数据框操作

自定义函数```python
# Apply function to columns
df['name_length'] = df['name'].apply(len)

# Apply function to rows
df['category'] = df.apply(
    lambda row: 'High' if row['creditlimit'] > 100000 else 'Low',
    axis=1
)

# Conditional operations
df['adjusted_limit'] = df['creditlimit'].where(
    df['statecode'] == 0,
    df['creditlimit'] * 0.5
)
```
字符串操作```python
# String methods
df['name_upper'] = df['name'].str.upper()
df['name_starts'] = df['name'].str.startswith('A')
df['name_contains'] = df['name'].str.contains('Inc')
df['name_split'] = df['name'].str.split(',').str[0]

# Replace and substitute
df['industry'] = df['industrycode'].map({
    1: 'Retail',
    2: 'Manufacturing',
    3: 'Technology'
})
```
重塑数据```python
# Transpose
transposed = df.set_index('name').T

# Stack/Unstack
stacked = df.set_index(['name', 'industrycode'])['creditlimit'].unstack()

# Melt long format
melted = pd.melt(df, id_vars=['name'], var_name='metric', value_name='value')
```
---

# # 9。性能优化

高效的数据加载```python
# Load large datasets in chunks
all_records = []
chunk_size = 1000

for page in client.get(
    "account",
    select=["accountid", "name", "creditlimit"],
    top=10000,        # Limit total records
    page_size=chunk_size
):
    all_records.extend(page)
    if len(all_records) % 5000 == 0:
        print(f"Loaded {len(all_records)} records")

df = pd.DataFrame(all_records)
print(f"Total: {len(df)} records")
```
内存优化```python
# Reduce memory usage
# Use categorical for repeated values
df['industrycode'] = df['industrycode'].astype('category')

# Use appropriate numeric types
df['creditlimit'] = pd.to_numeric(df['creditlimit'], downcast='float')

# Delete columns no longer needed
df = df.drop(columns=['unused_col1', 'unused_col2'])

# Check memory usage
print(df.memory_usage(deep=True).sum() / 1024**2, "MB")
```
查询优化```python
# Apply filters on server, not client
# ✅ GOOD: Filter on server
accounts = client.get(
    "account",
    filter="creditlimit > 50000",  # Server-side filter
    select=["accountid", "name", "creditlimit"]
)

# ❌ BAD: Load all, filter locally
all_accounts = client.get("account")  # Loads everything
filtered = [a for a in all_accounts if a['creditlimit'] > 50000]  # Client-side
```
---

# # 10。完整的例子：销售分析```python
import pandas as pd
import numpy as np
from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient

# Setup
client = DataverseClient(
    "https://<myorg>.crm.dynamics.com",
    InteractiveBrowserCredential()
)

# Load data
print("Loading account data...")
records = []
for page in client.get(
    "account",
    select=["accountid", "name", "creditlimit", "industrycode", "statecode", "createdon"],
    orderby=["createdon"]
):
    records.extend(page)

df = pd.DataFrame(records)
df['createdon'] = pd.to_datetime(df['createdon'])

# Data cleaning
df = df.dropna()

# Feature engineering
df['year'] = df['createdon'].dt.year
df['month'] = df['createdon'].dt.month
df['year_month'] = df['createdon'].dt.to_period('M')

# Analysis
print("\n=== ACCOUNT OVERVIEW ===")
print(f"Total accounts: {len(df)}")
print(f"Total credit limit: ${df['creditlimit'].sum():,.2f}")
print(f"Average credit limit: ${df['creditlimit'].mean():,.2f}")

print("\n=== BY INDUSTRY ===")
industry_summary = df.groupby('industrycode').agg({
    'accountid': 'count',
    'creditlimit': ['sum', 'mean']
}).round(2)
print(industry_summary)

print("\n=== BY STATUS ===")
status_summary = df.groupby('statecode').agg({
    'accountid': 'count',
    'creditlimit': 'sum'
})
print(status_summary)

# Export report
print("\n=== EXPORTING REPORT ===")
industry_summary.to_csv('industry_analysis.csv')
print("Report saved to industry_analysis.csv")
```
---

# # 11。已知的限制

-`PandasODataClient`目前需要从查询结果手动创建DataFrame
-非常大的数据框（数百万行）可能会遇到内存限制
-熊猫操作是客户端；服务器端聚合对于大型数据集更有效
—文件操作需要标准的`DataverseClient`，而不是熊猫包装

---

# # 12。相关资源

-[熊猫文档]（https://pandas.pydata.org/docs/）
-[官方示例：quickstart_pandas.py]（https://github.com/microsoft/PowerPlatform-DataverseClient-Python/blob/main/examples/quickstart_pandas.py）
- [SDK for Python README]（https://github.com/microsoft/PowerPlatform-DataverseClient-Python/blob/main/README.md）
- [Microsoft Learn: Working with data]（https://learn.microsoft.com/en-us/power-apps/developer/data-platform/sdk-python/work-data）