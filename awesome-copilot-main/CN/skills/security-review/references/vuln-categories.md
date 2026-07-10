#漏洞类别-深度引用

此文件包含每个漏洞类别的详细检测指南。
在扫描工作流程的第4步加载此文件。

---

# # 1。注塑缺陷

SQL注入
注意事项：**
- SQL查询中的字符串连接或插值
-原始`.query()`，`.execute()`，`.raw()`调用与变量
—ORM`whereRaw()`,`selectRaw()`,`orderByRaw()`，用户输入
-二阶SQLi：数据安全地存储，然后不安全地使用
-使用未经处理的输入调用存储过程

**检测信号（所有语言）：**```
"SELECT ... " + variable
`SELECT ... ${variable}`
f"SELECT ... {variable}"
"SELECT ... %s" % variable   # Only safe with proper driver parameterization
cursor.execute("... " + input)
db.raw(`... ${req.params.id}`)
```
**安全模式（参数化）：**```js
db.query('SELECT * FROM users WHERE id = ?', [userId])
User.findOne({ where: { id: userId } })  // ORM safe
```
* *升级检查:* *
—查询结果是否在其他查询中使用？(二阶)
—table/column名称是否由用户控制？（不能参数化-必须allowlist）

---

跨站点脚本（XSS）
注意事项：**
—包含用户数据的`innerHTML`、`outerHTML`、`document.write()`- React中的`dangerouslySetInnerHTML`-模板引擎渲染未转义：`{{{ var }}}`（车把），`!= var`（Pug）
- jQuery`.html()`，`.append()`与用户数据
—包含用户数据的`eval()`、`setTimeout(string)`、`setInterval(string)`—基于DOM:`location.hash`、`document.referrer`、`window.name`写入DOM
—存储XSS：用户输入保存到DB，以后不转义渲染

**框架检测：**
- **React**：默认安全，除了`dangerouslySetInnerHTML`- **Angular**：默认安全，除了`bypassSecurityTrustHtml`- **Vue**：默认安全，除了`v-html`- **香草JS**：每个DOM写入都是可疑的

---

命令注入
**查找内容（Node.js）：**```js
exec(userInput)
execSync(`ping ${host}`)
spawn('sh', ['-c', userInput])
child_process.exec('ls ' + dir)
```
**查找内容（Python）：**```python
os.system(user_input)
subprocess.call(user_input, shell=True)
eval(user_input)
```
**查找内容（PHP）：**```php
exec($input)
system($_GET['cmd'])
passthru($input)
`$input`  # backtick operator
```
**使用数组形式的spawn/subprocess不带shell=True；对命令使用允许列表。

---

服务器端请求伪造（SSRF）
注意事项：**
—URL由用户控制的HTTP请求
- Webhooks， URL预览，图像获取功能
-获取外部url的PDF生成器
-重定向到用户提供的url

高风险目标:* * * *
—AWS元数据服务：`169.254.169.254`—内部服务：`localhost`、`127.0.0.1`、`10.x.x.x`、`192.168.x.x`—云元数据端点

* *检测:* *```js
fetch(req.body.url)
axios.get(userSuppliedUrl)
http.get(params.webhook)
```
---

# # 2。认证与访问控制

破损对象级别授权（BOLA / IDOR）
注意事项：**
—资源id直接取自URL/params，不进行所有权检查
-`findById(req.params.id)`，不验证`userId === currentUser.id`-数字顺序id（容易猜到）

**脆弱模式示例：**```js
// VULNERABLE: no ownership check
app.get('/api/documents/:id', async (req, res) => {
  const doc = await Document.findById(req.params.id);
  res.json(doc);
});

// SAFE: verify ownership
app.get('/api/documents/:id', async (req, res) => {
  const doc = await Document.findOne({ _id: req.params.id, owner: req.user.id });
  if (!doc) return res.status(403).json({ error: 'Forbidden' });
  res.json(doc);
});
```
---

JWT漏洞
注意事项：**
-接受`alg: "none"`-弱密码或硬编码密码：`secret`，`password`,`1234`-无过期（`exp`索赔）验证
-算法混乱（RS256→HS256降级）
- JWT存储在`localStorage`（XSS风险；首选httpOnly cookie）

* *检测:* *```js
jwt.verify(token, secret, { algorithms: ['HS256'] })  // Check algorithms array
jwt.decode(token)  // WARNING: decode does NOT verify signature
```
---

###缺少认证/授权
注意事项：**
-管理或敏感端点缺少认证中间件
—`app.use(authMiddleware)`之后与之前定义的路由
-功能标志或调试端点暴露在生产环境中
- GraphQL解析器在字段级别缺少认证检查

---

# # # CSRF
注意事项：**
—不带CSRF令牌的状态更改操作（POST/PUT/DELETE）
- api仅依赖于cookie进行验证，没有SameSite属性
-会话cookie中缺少`SameSite=Strict`或`SameSite=Lax`---

# # 3。机密和敏感数据暴露

###代码内秘密
寻找以下模式：```
API_KEY = "sk-..."
password = "hunter2"
SECRET = "abc123"
private_key = "-----BEGIN RSA PRIVATE KEY-----"
aws_secret_access_key = "wJalrXUtn..."
```
熵启发式：字符串> 20个字符与高字符变化在赋值上下文中
可能是秘密，即使变量名没有这样说。

###在日志/错误消息```js
console.log('User password:', password)
logger.info({ user, token })   // token shouldn't be logged
res.status(500).json({ error: err.stack })  // stack traces expose internals
```
API响应中的敏感数据
—返回完整用户对象，包括`password_hash`，`ssn`,`credit_card`—在错误响应中包含内部id或系统路径

---

# # 4。密码学

弱算法
|算法|问题|替换为||-----------|-------|--------------|
| MD5 |安全漏洞| SHA-256或bcrypt（密码）|
| SHA-1 |碰撞攻击| SHA-256 |
| DES / 3DES |弱密钥大小| AES-256-GCM |
| RC4 |坏| AES-GCM |
| ECB模式| No IV，模式可见| GCM或CBC随机IV |

弱随机性```js
// VULNERABLE
Math.random()                    // not cryptographically secure
Date.now()                       // predictable
Math.random().toString(36)       // weak token generation

// SAFE
crypto.randomBytes(32)           // Node.js
secrets.token_urlsafe(32)        // Python
```
###密码哈希```python
# VULNERABLE
hashlib.md5(password.encode()).hexdigest()
hashlib.sha256(password.encode()).hexdigest()

# SAFE
bcrypt.hashpw(password, bcrypt.gensalt(rounds=12))
argon2.hash(password)
```
---

# # 5。不安全的依赖关系

###标记：
—已安装版本范围内已知cve的包
-包弃> 2年没有安全更新
-包具有极其广泛的权限为其规定的目的
-导入已知的坏包的传递依赖
-固定版本明显落后于当前（可能未打补丁的vulns）

高危包监视列表：参见`references/vulnerable-packages.md`---

# # 6。业务逻辑

竞态条件（TOCTOU）```js
// VULNERABLE: check then act without atomic lock
const balance = await getBalance(userId);
if (balance >= amount) {
  await deductBalance(userId, amount);  // race condition between check and deduct
}

// SAFE: use atomic DB transaction or optimistic locking
await db.transaction(async (trx) => {
  const user = await User.query(trx).forUpdate().findById(userId);
  if (user.balance < amount) throw new Error('Insufficient funds');
  await user.$query(trx).patch({ balance: user.balance - amount });
});
```
缺少速率限制
标记端点：
-接受身份验证凭证（登录，2FA）
—发送邮件或短信
-执行昂贵的操作
-暴露用户枚举（密码重置，注册）

---

# # 7。路径遍历```python
# VULNERABLE
filename = request.args.get('file')
with open(f'/var/uploads/{filename}') as f:  # ../../../../etc/passwd

# SAFE
filename = os.path.basename(request.args.get('file'))
safe_path = os.path.join('/var/uploads', filename)
if not safe_path.startswith('/var/uploads/'):
    abort(400)
```
