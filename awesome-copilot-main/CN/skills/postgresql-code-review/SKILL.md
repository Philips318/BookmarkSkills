---
name: postgresql-code-review
description: 'PostgreSQL-specific code review assistant focusing on PostgreSQL best practices, anti-patterns, and unique quality standards. Covers JSONB operations, array usage, custom types, schema design, function optimization, and PostgreSQL-exclusive security features like Row Level Security (RLS).'
---
# PostgreSQL代码审查助手

专家PostgreSQL代码审查${selection}（或整个项目，如果没有选择）。专注于PostgreSQL特有的最佳实践、反模式和质量标准。

🎯postgresql特定的审查区域

JSONB最佳实践```sql
-- ❌ BAD: Inefficient JSONB usage
SELECT * FROM orders WHERE data->>'status' = 'shipped';  -- No index support

-- ✅ GOOD: Indexable JSONB queries
CREATE INDEX idx_orders_status ON orders USING gin((data->'status'));
SELECT * FROM orders WHERE data @> '{"status": "shipped"}';

-- ❌ BAD: Deep nesting without consideration
UPDATE orders SET data = data || '{"shipping":{"tracking":{"number":"123"}}}';

-- ✅ GOOD: Structured JSONB with validation
ALTER TABLE orders ADD CONSTRAINT valid_status 
CHECK (data->>'status' IN ('pending', 'shipped', 'delivered'));
```
### Array Operations Review```sql
-- ❌ BAD: Inefficient array operations
SELECT * FROM products WHERE 'electronics' = ANY(categories);  -- No index

-- ✅ GOOD: GIN indexed array queries
CREATE INDEX idx_products_categories ON products USING gin(categories);
SELECT * FROM products WHERE categories @> ARRAY['electronics'];

-- ❌ BAD: Array concatenation in loops
-- This would be inefficient in a function/procedure

-- ✅ GOOD: Bulk array operations
UPDATE products SET categories = categories || ARRAY['new_category']
WHERE id IN (SELECT id FROM products WHERE condition);
```
PostgreSQL架构设计回顾```sql
-- ❌ BAD: Not using PostgreSQL features
CREATE TABLE users (
    id INTEGER,
    email VARCHAR(255),
    created_at TIMESTAMP
);

-- ✅ GOOD: PostgreSQL-optimized schema
CREATE TABLE users (
    id BIGSERIAL PRIMARY KEY,
    email CITEXT UNIQUE NOT NULL,  -- Case-insensitive email
    created_at TIMESTAMPTZ DEFAULT NOW(),
    metadata JSONB DEFAULT '{}',
    CONSTRAINT valid_email CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$')
);

-- Add JSONB GIN index for metadata queries
CREATE INDEX idx_users_metadata ON users USING gin(metadata);
```
自定义类型和域```sql
-- ❌ BAD: Using generic types for specific data
CREATE TABLE transactions (
    amount DECIMAL(10,2),
    currency VARCHAR(3),
    status VARCHAR(20)
);

-- ✅ GOOD: PostgreSQL custom types
CREATE TYPE currency_code AS ENUM ('USD', 'EUR', 'GBP', 'JPY');
CREATE TYPE transaction_status AS ENUM ('pending', 'completed', 'failed', 'cancelled');
CREATE DOMAIN positive_amount AS DECIMAL(10,2) CHECK (VALUE > 0);

CREATE TABLE transactions (
    amount positive_amount NOT NULL,
    currency currency_code NOT NULL,
    status transaction_status DEFAULT 'pending'
);
```
🔍postgresql特有的反模式

性能反模式
- **避免使用特定于postgresql的索引**：不使用GIN/GiST为适当的数据类型
- **滥用JSONB**：把JSONB当作一个简单的字符串字段
- **忽略数组操作符**：使用低效的数组操作
- **分区键选择差**：没有有效地利用PostgreSQL分区

模式设计问题
- **不使用ENUM类型**：对有限的值集使用VARCHAR
- **忽略约束**：缺少数据验证的CHECK约束
- **数据类型错误**：使用VARCHAR而不是TEXT或CITEXT
- **缺少JSONB结构**：未经验证的非结构化JSONB

###功能和触发问题```sql
-- ❌ BAD: Inefficient trigger function
CREATE OR REPLACE FUNCTION update_modified_time()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();  -- Should use TIMESTAMPTZ
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- ✅ GOOD: Optimized trigger function
CREATE OR REPLACE FUNCTION update_modified_time()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Set trigger to fire only when needed
CREATE TRIGGER update_modified_time_trigger
    BEFORE UPDATE ON table_name
    FOR EACH ROW
    WHEN (OLD.* IS DISTINCT FROM NEW.*)
    EXECUTE FUNCTION update_modified_time();
```
📊PostgreSQL扩展使用回顾

扩展最佳实践```sql
-- ✅ Check if extension exists before creating
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- ✅ Use extensions appropriately
-- UUID generation
SELECT uuid_generate_v4();

-- Password hashing
SELECT crypt('password', gen_salt('bf'));

-- Fuzzy text matching
SELECT word_similarity('postgres', 'postgre');
```
##🛡️PostgreSQL安全审查

行级安全（RLS）```sql
-- ✅ GOOD: Implementing RLS
ALTER TABLE sensitive_data ENABLE ROW LEVEL SECURITY;

CREATE POLICY user_data_policy ON sensitive_data
    FOR ALL TO application_role
    USING (user_id = current_setting('app.current_user_id')::INTEGER);
```
###权限管理```sql
-- ❌ BAD: Overly broad permissions
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO app_user;

-- ✅ GOOD: Granular permissions
GRANT SELECT, INSERT, UPDATE ON specific_table TO app_user;
GRANT USAGE ON SEQUENCE specific_table_id_seq TO app_user;
```
🎯PostgreSQL代码质量检查表

模式设计
-[]使用合适的PostgreSQL数据类型（CITEXT， JSONB, arrays）
-[]为约束值使用ENUM类型
—[]实现适当的CHECK约束
-[]使用TIMESTAMPTZ代替TIMESTAMP
-[]定义可重用约束的自定义域

性能考虑
-[]合适的索引类型（GIN用于JSONB/arrays， GiST用于range）
- [] JSONB查询使用包含操作符（@>,?）
-[]使用postgresql特定操作符的数组操作
-[]正确使用窗口函数和cte
- [] postgresql专用函数的高效使用PostgreSQL特性的使用
-[]在适当的地方使用扩展
-[]在适当的时候在PL/pgSQL中实现存储过程
-[]利用PostgreSQL的高级SQL特性
-[]使用postgresql特有的优化技术
-[]实现正确的函数错误处理

安全性和合规性
—[]根据需要实现行级别安全
—[]合理的角色和权限管理
-[]使用PostgreSQL自带的加密功能
-[]使用PostgreSQL特性实现审计跟踪

📝postgresql特定的审查指南1. **数据类型优化**：确保适当使用postgresql特定类型
2. **索引策略**：检查索引类型并确保使用了特定于postgresql的索引
3. **JSONB结构**：验证JSONB模式设计和查询模式
4. **功能质量**：审查PL/pgSQL功能的效率和最佳实践
5. **扩展使用**：验证PostgreSQL扩展的正确使用
6. **性能特性**：检查PostgreSQL高级特性的利用率
7. **安全实现**：回顾postgresql特定的安全特性

关注PostgreSQL的独特功能，并确保代码利用了PostgreSQL的特殊之处，而不是将其视为一个通用的SQL数据库。