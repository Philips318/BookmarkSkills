---
description: 'Guidelines for GitHub Copilot to write comments to achieve self-explanatory code with less comments. Examples are in JavaScript but it should work on any language that has comments.'
applyTo: '**'
---
#自解释代码注释说明

##核心原理
**编写能说明问题的代码。只在必要的时候解释为什么，而不是什么
大多数时候我们不需要注释。

注释指南

###❌避免这些注释类型

* * * *明显的评论```javascript
// Bad: States the obvious
let counter = 0;  // Initialize counter to zero
counter++;  // Increment counter by one
```
* * * *多余的评论```javascript
// Bad: Comment repeats the code
function getUserName() {
    return user.name;  // Return the user's name
}
```
* * * *过时的评论```javascript
// Bad: Comment doesn't match the code
// Calculate tax at 5% rate
const tax = price * 0.08;  // Actually 8%
```
###✅编写这些注释类型

**复杂的业务逻辑```javascript
// Good: Explains WHY this specific calculation
// Apply progressive tax brackets: 10% up to 10k, 20% above
const tax = calculateProgressiveTax(income, [0.10, 0.20], [10000]);
```
* * * *不明显的算法```javascript
// Good: Explains the algorithm choice
// Using Floyd-Warshall for all-pairs shortest paths
// because we need distances between all nodes
for (let k = 0; k < vertices; k++) {
    for (let i = 0; i < vertices; i++) {
        for (let j = 0; j < vertices; j++) {
            // ... implementation
        }
    }
}
```
* * * *正则表达式模式```javascript
// Good: Explains what the regex matches
// Match email format: username@domain.extension
const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
```
**API约束或问题**```javascript
// Good: Explains external constraint
// GitHub API rate limit: 5000 requests/hour for authenticated users
await rateLimiter.wait();
const response = await fetch(githubApiUrl);
```
##决策框架

在写评论之前，问：
1. **代码是否自解释？**→无需评论
2. **一个更好的variable/function名字会消除这种需要吗？**→重构
3. **这解释了为什么，而不是什么吗？好评论
4. **这对未来的维护者有帮助吗？好评论

注释的特殊情况

公共api```javascript
/**
 * Calculate compound interest using the standard formula.
 * 
 * @param {number} principal - Initial amount invested
 * @param {number} rate - Annual interest rate (as decimal, e.g., 0.05 for 5%)
 * @param {number} time - Time period in years
 * @param {number} compoundFrequency - How many times per year interest compounds (default: 1)
 * @returns {number} Final amount after compound interest
 */
function calculateCompoundInterest(principal, rate, time, compoundFrequency = 1) {
    // ... implementation
}
```
配置和常量```javascript
// Good: Explains the source or reasoning
const MAX_RETRIES = 3;  // Based on network reliability studies
const API_TIMEOUT = 5000;  // AWS Lambda timeout is 15s, leaving buffer
```
# # #注释```javascript
// TODO: Replace with proper user authentication after security review
// FIXME: Memory leak in production - investigate connection pooling
// HACK: Workaround for bug in library v2.1.0 - remove after upgrade
// NOTE: This implementation assumes UTC timezone for all calculations
// WARNING: This function modifies the original array instead of creating a copy
// PERF: Consider caching this result if called frequently in hot path
// SECURITY: Validate input to prevent SQL injection before using in query
// BUG: Edge case failure when array is empty - needs investigation
// REFACTOR: Extract this logic into separate utility function for reusability
// DEPRECATED: Use newApiFunction() instead - this will be removed in v3.0
```
要避免的反模式

死代码注释```javascript
// Bad: Don't comment out code
// const oldFunction = () => { ... };
const newFunction = () => { ... };
```
### Changelog Comments```javascript
// Bad: Don't maintain history in comments
// Modified by John on 2023-01-15
// Fixed bug reported by Sarah on 2023-02-03
function processData() {
    // ... implementation
}
```
分隔符注释```javascript
// Bad: Don't use decorative comments
//=====================================
// UTILITY FUNCTIONS
//=====================================
```
质量检查表

在提交之前，请确保您的评论：
-[]解释为什么，而不是什么
-[]语法正确，清晰
-[]随着代码的发展将保持准确
-[]为代码理解增加真正的价值
-[]被适当地放置（在它们所描述的代码之上）
-[]使用正确的拼写和专业语言

# #总结

记住：**最好的注释是你不需要写的注释，因为代码是自文档化的