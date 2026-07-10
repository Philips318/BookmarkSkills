#测试文件扫描-两个审计器

专门扫描测试文件问题。在R18和R19审核期间运行。

---

##设置文件```bash
# Find test setup files
find src/ -name "setupTests*" -o -name "jest.setup*" 2>/dev/null
find . -name "jest.config.js" -o -name "jest.config.ts" 2>/dev/null | grep -v "node_modules"

# Check setup file for legacy patterns
grep -n "ReactDOM\|react-dom/test-utils\|Enzyme\|configure\|Adapter" \
  src/setupTests.js 2>/dev/null
```
---

##导入扫描```bash
# All react-dom/test-utils imports in tests
grep -rn "from 'react-dom/test-utils'\|require.*react-dom/test-utils" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# Enzyme imports
grep -rn "from 'enzyme'\|require.*enzyme" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# react-test-renderer
grep -rn "from 'react-test-renderer'" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# Old act location
grep -rn "act.*from 'react-dom'" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null
```
---

##渲染模式扫描```bash
# ReactDOM.render in tests (should use RTL render)
grep -rn "ReactDOM\.render\s*(" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# Enzyme shallow/mount
grep -rn "shallow(\|mount(" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# Custom render helpers
find src/ -name "test-utils.js" -o -name "renderWithProviders*" \
  -o -name "customRender*" -o -name "render-helpers*" 2>/dev/null
```
---

断言扫描```bash
# Call count assertions (StrictMode sensitive)
grep -rn "toHaveBeenCalledTimes" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# console.error assertions (React error logging changed in R19)
grep -rn "console\.error" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# Intermediate state assertions (batching sensitive)
grep -rn "fireEvent\|userEvent" \
  src/ --include="*.test.*" --include="*.spec.*" -A 1 \
  | grep "expect\|getBy\|queryBy" | head -20 2>/dev/null
```
---

##异步扫描```bash
# act() usage
grep -rn "\bact(" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null

# waitFor usage (good - check these are properly async)
grep -rn "waitFor\|findBy" \
  src/ --include="*.test.*" --include="*.spec.*" | wc -l

# setTimeout in tests (may be batching-sensitive)
grep -rn "setTimeout\|setInterval" \
  src/ --include="*.test.*" --include="*.spec.*" 2>/dev/null
```
