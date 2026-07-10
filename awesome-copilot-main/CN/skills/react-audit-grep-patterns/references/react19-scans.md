# React 19审计-完成扫描命令

按此顺序运行。每个部分都映射到react19-auditor中的一个阶段。

---

第1阶段-移除api（有问题-必须修复）```bash
# 1. ReactDOM.render - REMOVED
grep -rn "ReactDOM\.render\s*(" \
  src/ --include="*.js" --include="*.jsx" 2>/dev/null

# 2. ReactDOM.hydrate - REMOVED
grep -rn "ReactDOM\.hydrate\s*(" \
  src/ --include="*.js" --include="*.jsx" 2>/dev/null

# 3. unmountComponentAtNode - REMOVED
grep -rn "unmountComponentAtNode" \
  src/ --include="*.js" --include="*.jsx" 2>/dev/null

# 4. findDOMNode - REMOVED
grep -rn "findDOMNode\|ReactDOM\.findDOMNode" \
  src/ --include="*.js" --include="*.jsx" 2>/dev/null

# 5. createFactory - REMOVED
grep -rn "createFactory\|React\.createFactory" \
  src/ --include="*.js" --include="*.jsx" 2>/dev/null

# 6. react-dom/test-utils imports - most exports REMOVED
grep -rn "from 'react-dom/test-utils'\|from \"react-dom/test-utils\"\|require.*react-dom/test-utils" \
  src/ --include="*.js" --include="*.jsx" 2>/dev/null

# 7. Legacy Context API - REMOVED
grep -rn "contextTypes\|childContextTypes\|getChildContext" \
  src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." 2>/dev/null

# 8. String refs - REMOVED
grep -rn "this\.refs\." \
  src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." 2>/dev/null
```
---

第2阶段-不推荐的api（应该迁移）```bash
# 9. forwardRef - deprecated (ref now direct prop)
grep -rn "forwardRef\|React\.forwardRef" \
  src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." 2>/dev/null

# 10. defaultProps on function components - REMOVED for function components
grep -rn "\.defaultProps\s*=" \
  src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." 2>/dev/null

# 11. useRef() without initial value
grep -rn "useRef()\|useRef( )" \
  src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." 2>/dev/null

# 12. propTypes (runtime validation silently dropped)
grep -rn "\.propTypes\s*=" \
  src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l

# 13. react-test-renderer - deprecated
grep -rn "react-test-renderer\|TestRenderer" \
  src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." 2>/dev/null

# 14. Unnecessary React default imports (new JSX transform)
grep -rn "^import React from 'react'" \
  src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." 2>/dev/null
```
---

阶段3 -测试文件扫描```bash
# act() from wrong location
grep -rn "from 'react-dom/test-utils'" \
  src/ --include="*.test.js" --include="*.test.jsx" \
       --include="*.spec.js" --include="*.spec.jsx" 2>/dev/null

# Simulate usage - REMOVED
grep -rn "Simulate\." \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# react-test-renderer in tests
grep -rn "from 'react-test-renderer'" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# Spy call count assertions (may need StrictMode delta updates)
grep -rn "toHaveBeenCalledTimes" \
  src/ --include="*.test.*" --include="*.spec.*" | head -20 2>/dev/null

# console.error call count assertions (React 19 error reporting change)
grep -rn "console\.error.*toHaveBeenCalledTimes\|toHaveBeenCalledTimes.*console\.error" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null
```
---

阶段4 - StrictMode行为改变```bash
# StrictMode usage
grep -rn "StrictMode\|React\.StrictMode" \
  src/ --include="*.js" --include="*.jsx" 2>/dev/null

# Spy assertions that may be affected by StrictMode double-invoke change
grep -rn "toHaveBeenCalledTimes\|\.mock\.calls\.length" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null
```
---

完整的摘要脚本```bash
#!/bin/bash
echo "=============================="
echo "React 19 Migration Audit Summary"
echo "=============================="
echo ""
echo "REMOVED APIs (Critical):"
echo "  ReactDOM.render:             $(grep -rn "ReactDOM\.render\s*(" src/ --include="*.js" --include="*.jsx" | wc -l | tr -d ' ') hits"
echo "  ReactDOM.hydrate:            $(grep -rn "ReactDOM\.hydrate\s*(" src/ --include="*.js" --include="*.jsx" | wc -l | tr -d ' ') hits"
echo "  unmountComponentAtNode:      $(grep -rn "unmountComponentAtNode" src/ --include="*.js" --include="*.jsx" | wc -l | tr -d ' ') hits"
echo "  findDOMNode:                 $(grep -rn "findDOMNode" src/ --include="*.js" --include="*.jsx" | wc -l | tr -d ' ') hits"
echo "  react-dom/test-utils:        $(grep -rn "from 'react-dom/test-utils'" src/ --include="*.js" --include="*.jsx" | wc -l | tr -d ' ') hits"
echo "  Legacy context:              $(grep -rn "contextTypes\|childContextTypes\|getChildContext" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l | tr -d ' ') hits"
echo "  String refs:                 $(grep -rn "this\.refs\." src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l | tr -d ' ') hits"
echo ""
echo "DEPRECATED APIs:"
echo "  forwardRef:                  $(grep -rn "forwardRef" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l | tr -d ' ') hits"
echo "  defaultProps (fn comps):     $(grep -rn "\.defaultProps\s*=" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l | tr -d ' ') hits"
echo "  useRef() no arg:             $(grep -rn "useRef()" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l | tr -d ' ') hits"
echo ""
echo "TEST FILE ISSUES:"
echo "  react-dom/test-utils:        $(grep -rn "from 'react-dom/test-utils'" src/ --include="*.test.*" --include="*.spec.*" | wc -l | tr -d ' ') hits"
echo "  Simulate usage:              $(grep -rn "Simulate\." src/ --include="*.test.*" | wc -l | tr -d ' ') hits"
```
