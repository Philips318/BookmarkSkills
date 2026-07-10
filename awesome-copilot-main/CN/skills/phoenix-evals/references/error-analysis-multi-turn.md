错误分析：多回合对话

调试复杂的多回合对话跟踪。

##方法

1. **端到端优先** -对话是否达到目标？
2. **找到第一个故障** -追溯根本原因
3. **简化** -在多轮调试前尝试单轮调试
4. **N-1测试** -隔离特定回合与能力问题

##查找第一个上游故障```
Turn 1: User asks about flights ✓
Turn 2: Assistant asks for dates ✓
Turn 3: User provides dates ✓
Turn 4: Assistant searches WRONG dates ← FIRST FAILURE
Turn 5: Shows wrong flights (consequence)
Turn 6: User frustrated (consequence)
```
专注于第4回合，而不是第6回合。

##先简化

调试多圈前，先测试单圈：```python
# If single-turn also fails → problem is retrieval/knowledge
# If single-turn passes → problem is conversation context
response = chat("What's the return policy for electronics?")
```
## N-1测试

假设匝数为1到N-1，测试匝数为N：```python
context = conversation[:n-1]
response = chat_with_context(context, user_message_n)
# Compare to actual turn N
```
这样可以隔离错误是来自上下文还是来自底层功能。

# #检查表

1. 谈话达到目的了吗？(E2E)
2. 哪一个弯道先出了问题？
3. 你能单圈繁殖吗？
4. 错误来自上下文还是能力？(n - 1测试)