---
name: copilot-instructions-blueprint-generator
description: 'Technology-agnostic blueprint generator for creating comprehensive copilot-instructions.md files that guide GitHub Copilot to produce code consistent with project standards, architecture patterns, and exact technology versions by analyzing existing codebase patterns and avoiding assumptions.'
---
#副驾驶指令蓝图生成器

##配置变量
$ {PROJECT_TYPE = "自动检测|。净JavaScript Java | | |打印稿| |角Python多个| | |其他“反应}< !——初级技术——b>
$ {ARCHITECTURE_STYLE = "分层| Microservices |单片|域驱动的事件驱动| | Serverless |混合”}< !——架构方法——>
$ {CODE_QUALITY_FOCUS = "安全性能可维护性| | |可访问性|可测试性|”}< !——质量优先——b>
$ {DOCUMENTATION_LEVEL = "最小标准| |全面”}< !——文件要求——>
$ {TESTING_REQUIREMENTS = "单元集成| | E2E BDD TDD | | |”}< !——测试方法——>
${版本=“语义|弗|定制”}< !——版本控制方法——>

##生成提示生成一个全面的copilot-instructions.md文件，该文件将指导GitHub Copilot生成与我们的项目标准、体系结构和技术版本一致的代码。这些说明必须严格基于我们代码库中的实际代码模式，并避免做出任何假设。遵循以下方法：

# # # 1。核心指令结构```markdown
# GitHub Copilot Instructions

## Priority Guidelines

When generating code for this repository:

1. **Version Compatibility**: Always detect and respect the exact versions of languages, frameworks, and libraries used in this project
2. **Context Files**: Prioritize patterns and standards defined in the .github/copilot directory
3. **Codebase Patterns**: When context files don't provide specific guidance, scan the codebase for established patterns
4. **Architectural Consistency**: Maintain our ${ARCHITECTURE_STYLE} architectural style and established boundaries
5. **Code Quality**: Prioritize ${CODE_QUALITY_FOCUS == "All" ? "maintainability, performance, security, accessibility, and testability" : CODE_QUALITY_FOCUS} in all generated code

## Technology Version Detection

Before generating code, scan the codebase to identify:

1. **Language Versions**: Detect the exact versions of programming languages in use
   - Examine project files, configuration files, and package managers
   - Look for language-specific version indicators (e.g., <LangVersion> in .NET projects)
   - Never use language features beyond the detected version

2. **Framework Versions**: Identify the exact versions of all frameworks
   - Check package.json, .csproj, pom.xml, requirements.txt, etc.
   - Respect version constraints when generating code
   - Never suggest features not available in the detected framework versions

3. **Library Versions**: Note the exact versions of key libraries and dependencies
   - Generate code compatible with these specific versions
   - Never use APIs or features not available in the detected versions

## Context Files

Prioritize the following files in .github/copilot directory (if they exist):

- **architecture.md**: System architecture guidelines
- **tech-stack.md**: Technology versions and framework details
- **coding-standards.md**: Code style and formatting standards
- **folder-structure.md**: Project organization guidelines
- **exemplars.md**: Exemplary code patterns to follow

## Codebase Scanning Instructions

When context files don't provide specific guidance:

1. Identify similar files to the one being modified or created
2. Analyze patterns for:
   - Naming conventions
   - Code organization
   - Error handling
   - Logging approaches
   - Documentation style
   - Testing patterns
   
3. Follow the most consistent patterns found in the codebase
4. When conflicting patterns exist, prioritize patterns in newer files or files with higher test coverage
5. Never introduce patterns not found in the existing codebase

## Code Quality Standards

${CODE_QUALITY_FOCUS.includes("Maintainability") || CODE_QUALITY_FOCUS == "All" ? `### Maintainability
- Write self-documenting code with clear naming
- Follow the naming and organization conventions evident in the codebase
- Follow established patterns for consistency
- Keep functions focused on single responsibilities
- Limit function complexity and length to match existing patterns` : ""}

${CODE_QUALITY_FOCUS.includes("Performance") || CODE_QUALITY_FOCUS == "All" ? `### Performance
- Follow existing patterns for memory and resource management
- Match existing patterns for handling computationally expensive operations
- Follow established patterns for asynchronous operations
- Apply caching consistently with existing patterns
- Optimize according to patterns evident in the codebase` : ""}

${CODE_QUALITY_FOCUS.includes("Security") || CODE_QUALITY_FOCUS == "All" ? `### Security
- Follow existing patterns for input validation
- Apply the same sanitization techniques used in the codebase
- Use parameterized queries matching existing patterns
- Follow established authentication and authorization patterns
- Handle sensitive data according to existing patterns` : ""}

${CODE_QUALITY_FOCUS.includes("Accessibility") || CODE_QUALITY_FOCUS == "All" ? `### Accessibility
- Follow existing accessibility patterns in the codebase
- Match ARIA attribute usage with existing components
- Maintain keyboard navigation support consistent with existing code
- Follow established patterns for color and contrast
- Apply text alternative patterns consistent with the codebase` : ""}

${CODE_QUALITY_FOCUS.includes("Testability") || CODE_QUALITY_FOCUS == "All" ? `### Testability
- Follow established patterns for testable code
- Match dependency injection approaches used in the codebase
- Apply the same patterns for managing dependencies
- Follow established mocking and test double patterns
- Match the testing style used in existing tests` : ""}

## Documentation Requirements

${DOCUMENTATION_LEVEL == "Minimal" ? 
`- Match the level and style of comments found in existing code
- Document according to patterns observed in the codebase
- Follow existing patterns for documenting non-obvious behavior
- Use the same format for parameter descriptions as existing code` : ""}

${DOCUMENTATION_LEVEL == "Standard" ? 
`- Follow the exact documentation format found in the codebase
- Match the XML/JSDoc style and completeness of existing comments
- Document parameters, returns, and exceptions in the same style
- Follow existing patterns for usage examples
- Match class-level documentation style and content` : ""}

${DOCUMENTATION_LEVEL == "Comprehensive" ? 
`- Follow the most detailed documentation patterns found in the codebase
- Match the style and completeness of the best-documented code
- Document exactly as the most thoroughly documented files do
- Follow existing patterns for linking documentation
- Match the level of detail in explanations of design decisions` : ""}

## Testing Approach

${TESTING_REQUIREMENTS.includes("Unit") || TESTING_REQUIREMENTS == "All" ? 
`### Unit Testing
- Match the exact structure and style of existing unit tests
- Follow the same naming conventions for test classes and methods
- Use the same assertion patterns found in existing tests
- Apply the same mocking approach used in the codebase
- Follow existing patterns for test isolation` : ""}

${TESTING_REQUIREMENTS.includes("Integration") || TESTING_REQUIREMENTS == "All" ? 
`### Integration Testing
- Follow the same integration test patterns found in the codebase
- Match existing patterns for test data setup and teardown
- Use the same approach for testing component interactions
- Follow existing patterns for verifying system behavior` : ""}

${TESTING_REQUIREMENTS.includes("E2E") || TESTING_REQUIREMENTS == "All" ? 
`### End-to-End Testing
- Match the existing E2E test structure and patterns
- Follow established patterns for UI testing
- Apply the same approach for verifying user journeys` : ""}

${TESTING_REQUIREMENTS.includes("TDD") || TESTING_REQUIREMENTS == "All" ? 
`### Test-Driven Development
- Follow TDD patterns evident in the codebase
- Match the progression of test cases seen in existing code
- Apply the same refactoring patterns after tests pass` : ""}

${TESTING_REQUIREMENTS.includes("BDD") || TESTING_REQUIREMENTS == "All" ? 
`### Behavior-Driven Development
- Match the existing Given-When-Then structure in tests
- Follow the same patterns for behavior descriptions
- Apply the same level of business focus in test cases` : ""}

## Technology-Specific Guidelines

${PROJECT_TYPE == ".NET" || PROJECT_TYPE == "Auto-detect" || PROJECT_TYPE == "Multiple" ? `### .NET Guidelines
- Detect and strictly adhere to the specific .NET version in use
- Use only C# language features compatible with the detected version
- Follow LINQ usage patterns exactly as they appear in the codebase
- Match async/await usage patterns from existing code
- Apply the same dependency injection approach used in the codebase
- Use the same collection types and patterns found in existing code` : ""}

${PROJECT_TYPE == "Java" || PROJECT_TYPE == "Auto-detect" || PROJECT_TYPE == "Multiple" ? `### Java Guidelines
- Detect and adhere to the specific Java version in use
- Follow the exact same design patterns found in the codebase
- Match exception handling patterns from existing code
- Use the same collection types and approaches found in the codebase
- Apply the dependency injection patterns evident in existing code` : ""}

${PROJECT_TYPE == "JavaScript" || PROJECT_TYPE == "TypeScript" || PROJECT_TYPE == "Auto-detect" || PROJECT_TYPE == "Multiple" ? `### JavaScript/TypeScript Guidelines
- Detect and adhere to the specific ECMAScript/TypeScript version in use
- Follow the same module import/export patterns found in the codebase
- Match TypeScript type definitions with existing patterns
- Use the same async patterns (promises, async/await) as existing code
- Follow error handling patterns from similar files` : ""}

${PROJECT_TYPE == "React" || PROJECT_TYPE == "Auto-detect" || PROJECT_TYPE == "Multiple" ? `### React Guidelines
- Detect and adhere to the specific React version in use
- Match component structure patterns from existing components
- Follow the same hooks and lifecycle patterns found in the codebase
- Apply the same state management approach used in existing components
- Match prop typing and validation patterns from existing code` : ""}

${PROJECT_TYPE == "Angular" || PROJECT_TYPE == "Auto-detect" || PROJECT_TYPE == "Multiple" ? `### Angular Guidelines
- Detect and adhere to the specific Angular version in use
- Follow the same component and module patterns found in the codebase
- Match decorator usage exactly as seen in existing code
- Apply the same RxJS patterns found in the codebase
- Follow existing patterns for component communication` : ""}

${PROJECT_TYPE == "Python" || PROJECT_TYPE == "Auto-detect" || PROJECT_TYPE == "Multiple" ? `### Python Guidelines
- Detect and adhere to the specific Python version in use
- Follow the same import organization found in existing modules
- Match type hinting approaches if used in the codebase
- Apply the same error handling patterns found in existing code
- Follow the same module organization patterns` : ""}

## Version Control Guidelines

${VERSIONING == "Semantic" ? 
`- Follow Semantic Versioning patterns as applied in the codebase
- Match existing patterns for documenting breaking changes
- Follow the same approach for deprecation notices` : ""}

${VERSIONING == "CalVer" ? 
`- Follow Calendar Versioning patterns as applied in the codebase
- Match existing patterns for documenting changes
- Follow the same approach for highlighting significant changes` : ""}

${VERSIONING == "Custom" ? 
`- Match the exact versioning pattern observed in the codebase
- Follow the same changelog format used in existing documentation
- Apply the same tagging conventions used in the project` : ""}

## General Best Practices

- Follow naming conventions exactly as they appear in existing code
- Match code organization patterns from similar files
- Apply error handling consistent with existing patterns
- Follow the same approach to testing as seen in the codebase
- Match logging patterns from existing code
- Use the same approach to configuration as seen in the codebase

## Project-Specific Guidance

- Scan the codebase thoroughly before generating any code
- Respect existing architectural boundaries without exception
- Match the style and patterns of surrounding code
- When in doubt, prioritize consistency with existing code over external best practices
```
# # # 2。代码库分析说明

要创建copilot-instructions.md文件，首先分析代码库如下：

1. **确定准确的技术版本**：
- ${PROJECT_TYPE == “自动检测” ？通过扫描文件扩展名和配置文件来检测所有编程语言、框架和库
-从项目文件中提取精确的版本信息，package.json，。csproj等等。
—文档的版本约束和兼容性要求

2. * * * *了解体系结构:
-分析文件夹结构和模块组织
-确定清晰的层边界和组件关系
-文档组件之间的通信模式

3. **文档代码模式**：
-不同代码元素的目录命名约定
-注意文档的样式和完整性
-文档错误处理模式
-绘制测试方法和覆盖范围4. **质量标准**：
-确定实际使用的性能优化技术
-记录代码中实施的安全措施
-注意现有的无障碍功能（如适用）
-记录代码库中明显的代码质量模式

# # # 3。实现注意事项

最终的copilot-instructions.md应该：
—放置在.github/copilot目录下
-只引用代码库中存在的模式和标准
—包括明确的版本兼容性要求
-避免订明任何守则中没有明确规定的做法
-从代码库中提供具体的示例
-全面而简洁，足以让副驾驶有效使用

重要：只包含基于在代码库中实际观察到的模式的指导。明确指示Copilot优先考虑与现有代码的一致性，而不是外部最佳实践或更新的语言功能。
”

##预期输出一个全面的copilot-instructions.md文件，它将指导GitHub Copilot生成与现有技术版本完全兼容并遵循已建立的模式和体系结构的代码。