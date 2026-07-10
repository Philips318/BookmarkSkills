---
description: 'Power Apps Code Apps development standards and best practices for TypeScript, React, and Power Platform integration'
applyTo: '**/*.{ts,tsx,js,jsx}, **/vite.config.*, **/package.json, **/tsconfig.json, **/power.config.json'
---
# Power Apps Code Apps Development Instructions

使用TypeScript、React和Power Platform SDK，遵循微软官方最佳实践和预览功能，生成高质量Power Apps代码应用的说明。

##项目背景

- **Power Apps Code Apps**：代码优先的web应用开发与Power Platform集成
- **TypeScript + React**：推荐使用Vite捆绑器的前端堆栈
- **电源平台SDK**：用于连接器集成的@microsoft/power-apps（当前版本^1.0.3）
- **PAC CLI**: Power Platform CLI，用于项目管理和部署
- **端口3000**：使用Power Platform SDK进行本地开发
- **Power Apps Premium**：生产使用的最终用户许可要求

##开发标准

项目结构

-使用组织良好的文件夹结构，并明确区分关注点；  ```
  src/
  ├── components/          # Reusable UI components
  ├── hooks/              # Custom React hooks for Power Platform
  ├── generated/
  │   ├── services/       # Generated connector services (PAC CLI)
  │   └── models/         # Generated TypeScript models (PAC CLI)
  ├── utils/             # Utility functions and helpers
  ├── types/             # TypeScript type definitions
  ├── PowerProvider.tsx  # Power Platform context wrapper
  └── main.tsx          # Application entry point
  ```
—将生成的文件（`generated/services/`,`generated/models/`）与自定义代码分开
-使用一致的命名约定（文件用kebab-case，组件用PascalCase）

### TypeScript配置

—“Power Apps SDK兼容性”在“tsconfig.json”中设置“`verbatimModuleSyntax: false`”
-启用类型安全严格模式，推荐使用tsconfig.json；  ```json
  {
    "compilerOptions": {
      "target": "ES2020",
      "useDefineForClassFields": true,
      "lib": ["ES2020", "DOM", "DOM.Iterable"],
      "module": "ESNext",
      "skipLibCheck": true,
      "verbatimModuleSyntax": false,
      "moduleResolution": "bundler",
      "allowImportingTsExtensions": true,
      "resolveJsonModule": true,
      "isolatedModules": true,
      "noEmit": true,
      "jsx": "react-jsx",
      "strict": true,
      "noUnusedLocals": true,
      "noUnusedParameters": true,
      "noFallthroughCasesInSwitch": true,
      "baseUrl": ".",
      "paths": {
        "@/*": ["./src/*"]
      }
    }
  }
  ```
-使用正确的输入电源平台连接器响应
—为cleaner导入配置路径别名`"@": path.resolve(__dirname, "./src")`-为应用程序特定的数据结构定义接口
—实现错误边界和正确的错误处理类型

先进的电源平台集成

####自定义控制框架（PCF控制）
- **集成PCF控件**：在代码应用中嵌入Power Apps组件框架控件  ```typescript
  // Example: Using custom PCF control for data visualization
  import { PCFControlWrapper } from './components/PCFControlWrapper';

  const MyComponent = () => {
    return (
      <PCFControlWrapper
        controlName="CustomChartControl"
        dataset={chartData}
        configuration={chartConfig}
      />
    );
  };
  ```
- **PCF控制通信**：处理PCF和React之间的事件和数据绑定
- **自定义控件部署**：打包和部署PCF控件与代码应用程序

#### Power BI嵌入式分析
- **嵌入Power BI报告**：集成交互式仪表板和报告  ```typescript
  import { PowerBIEmbed } from 'powerbi-client-react';

  const DashboardComponent = () => {
    return (
      <PowerBIEmbed
        embedConfig={{
          type: 'report',
          id: reportId,
          embedUrl: embedUrl,
          accessToken: accessToken,
          tokenType: models.TokenType.Aad,
          settings: {
            panes: { filters: { expanded: false, visible: false } }
          }
        }}
      />
    );
  };
  ```
- **动态报表过滤**：根据Code App上下文过滤Power BI报表
—**报表导出功能**：支持PDF、Excel和图像导出

#### AI Builder集成
- **认知服务集成**：使用AI Builder模型进行表单处理、对象检测  ```typescript
  // Example: Document processing with AI Builder
  const processDocument = async (file: File) => {
    const formData = new FormData();
    formData.append('file', file);

    const result = await AIBuilderService.ProcessDocument({
      modelId: 'document-processing-model-id',
      document: formData
    });

    return result.extractedFields;
  };
  ```
- **预测模型**：集成自定义AI模型进行业务预测
- **情感分析**：使用AI Builder分析文本情感
- **目标检测**：实现图像分析和目标识别

####电源虚拟代理集成
- **聊天机器人嵌入**：在代码应用程序中集成电源虚拟代理机器人  ```typescript
  import { DirectLine } from 'botframework-directlinejs';
  import { WebChat } from 'botframework-webchat';

  const ChatbotComponent = () => {
    const directLine = new DirectLine({
      token: chatbotToken
    });

    return (
      <div style={{ height: '400px', width: '100%' }}>
        <WebChat directLine={directLine} />
      </div>
    );
  };
  ```
- **上下文传递**：与聊天机器人对话共享代码应用程序上下文
- **自定义机器人动作**：触发代码应用程序功能从机器人交互
-使用PAC CLI生成的TypeScript服务来进行连接器操作
-使用Microsoft Entra ID实现适当的认证流程
-处理连接器同意对话框和权限管理
- PowerProvider实现模式（1.0版本不需要初始化SDK）：  ```typescript
  import type { ReactNode } from "react";

  export default function PowerProvider({ children }: { children: ReactNode }) {
    return <>{children}</>;
  }
  ```
-遵循官方支持的连接器模式：
- SQL Server（包括Azure SQL）
——SharePoint
—Office 365Users/Groups- Azure数据浏览器
- OneDrive for Business
-微软团队
-数据厌恶（CRUD操作）

### React Patterns

-在所有新开发中使用带有挂钩的功能组件
-为连接器操作实现适当的加载和错误状态
-考虑使用Fluent UI React组件（在官方示例中使用）
-在适当的时候使用React Query或SWR进行数据获取和缓存
-遵循React组件组合的最佳实践
-采用移动优先的方法实施响应式设计
-安装以下官方示例中的关键依赖项：
-`@microsoft/power-apps`为电源平台SDK
-`@fluentui/react-components`为UI组件`concurrently`用于并行脚本执行（dev依赖）

###数据管理-将敏感数据存储在数据源中，而不是存储在应用程序代码中
-使用生成的模型进行类型安全的连接器操作
-实施适当的数据验证和处理
—尽可能优雅地处理离线场景
—对访问频繁的数据进行适当的缓存

####高级数据厌恶关系
- **多对多关系**：实现连接表和关系服务  ```typescript
  // Example: User-to-Role many-to-many relationship
  const userRoles = await UserRoleService.getall();
  const filteredRoles = userRoles.filter(ur => ur.userId === currentUser.id);
  ```
- **多态查找**：处理客户字段，可以引用多个实体类型  ```typescript
  // Handle polymorphic customer lookup (Account or Contact)
  const customerType = record.customerType; // 'account' or 'contact'
  const customerId = record.customerId;
  const customer = customerType === 'account'
    ? await AccountService.get(customerId)
    : await ContactService.get(customerId);
  ```
- **复杂关系查询**：使用$expand和$filter进行高效的数据检索
—**关系验证**：实现关系约束的业务规则

性能优化

-使用React。memo和使用ememo进行昂贵的计算
-为大型应用程序实现代码分割和延迟加载
-优化包的大小与树摇
-使用高效的连接器查询模式来减少API调用
-为大型数据集实现适当的分页

####离线优先架构与同步模式
Service Worker实现**：启用离线功能  ```typescript
  // Example: Service worker registration
  if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
      navigator.serviceWorker.register('/sw.js')
        .then(registration => console.log('SW registered:', registration))
        .catch(error => console.log('SW registration failed:', error));
    });
  }
  ```
—**本地数据存储**：使用IndexedDB进行离线数据持久化  ```typescript
  // Example: IndexedDB wrapper for offline storage
  class OfflineDataStore {
    async saveData(key: string, data: any) {
      const db = await this.openDB();
      const transaction = db.transaction(['data'], 'readwrite');
      transaction.objectStore('data').put({ id: key, data, timestamp: Date.now() });
    }

    async loadData(key: string) {
      const db = await this.openDB();
      const transaction = db.transaction(['data'], 'readonly');
      return transaction.objectStore('data').get(key);
    }
  }
  ```
- **同步冲突解决**：在重新联机时处理数据冲突
—**后台同步**：定时同步数据
- **渐进式Web应用程序(PWA)**：启用应用程序安装和离线功能

安全最佳实践

-不要在代码中存储机密或敏感配置
—使用Power Platform内置的认证和授权
-实施适当的输入验证和处理
-遵循web应用程序的OWASP安全指南
—遵守Power Platform数据丢失预防策略
—只支持https通信

错误处理

-在React中实现全面的错误边界
-优雅地处理特定于连接器的错误
—向用户提供有意义的错误提示
-在不暴露敏感信息的情况下适当记录错误
—对瞬态故障实现重试逻辑
—处理网络连接问题测试策略

为业务逻辑和实用程序编写单元测试
-使用React测试库测试React组件
-在测试中模拟电源平台连接器
-对关键用户流进行集成测试
-使用TypeScript来提高测试安全性
—测试错误场景和边缘情况

开发工作流程

-使用PAC CLI进行项目初始化和连接器管理
-遵循适合团队规模的git分支策略
-实施适当的代码审查流程
-使用检查和格式化工具（ESLint, Prettier）
-配置开发脚本并发使用：
——`"dev": "concurrently \"vite\" \"pac code run\""`——`"build": "tsc -b && vite build"`在CI/CD管道中实现自动化测试
-遵循发布的语义版本控制

###部署和DevOps—使用“`npm run build`”和“`pac code push`”部署
-实施适当的环境管理（开发，测试，产品）
—使用特定于环境的配置文件
—尽可能采用蓝绿色或金丝雀部署策略
-监控生产环境中的应用性能和错误
—执行适当的备份和灾难恢复程序

####多环境部署管道
—**环境相关配置**：管理dev/test/staging/prod环境  ```json
  // Example: environment-specific config files
  // config/development.json
  {
    "powerPlatform": {
      "environmentUrl": "https://dev-env.crm.dynamics.com",
      "apiVersion": "9.2"
    },
    "features": {
      "enableDebugMode": true,
      "enableAnalytics": false
    }
  }
  ```
**自动化部署管道**：使用Azure DevOps或GitHub Actions  ```yaml
  # Example Azure DevOps pipeline step
  - task: PowerPlatformToolInstaller@2
  - task: PowerPlatformSetConnectionVariables@2
    inputs:
      authenticationType: 'PowerPlatformSPN'
      applicationId: '$(AppId)'
      clientSecret: '$(ClientSecret)'
      tenantId: '$(TenantId)'
  - task: PowerPlatformPublishCustomizations@2
  ```
- **环境提升**：从dev→test→staging→prod自动升级
—**回退策略**：在部署失败时自动回退
- **配置管理**：使用Azure密钥库存储特定于环境的机密

代码质量指南

组件开发

-创建具有清晰道具接口的可重用组件
-使用复合而不是继承
-用TypeScript实现正确的道具验证
-遵循单一责任原则
-编写具有清晰命名的自文档代码

状态管理

-在简单的场景中使用React的内置状态管理
-考虑使用Redux Toolkit进行复杂的状态管理
—进行适当的状态规范化
-避免使用上下文或状态管理库进行道具钻取
-有效地使用派生状态和计算值

API集成—使用PAC命令行生成的服务保持一致性
-实现适当的request/response拦截器
—处理认证令牌管理
—实现请求重复数据删除和缓存
—使用正确的HTTP状态码处理

样式和UI

-使用一致的设计系统或组件库
-实现响应式设计与CSSGrid/Flexbox-遵循无障碍指引（WCAG 2.1）
-使用CSS-in- js或CSS模块进行组件样式化
-在适当的时候实现暗模式支持
-确保移动友好的用户界面

####高级UI/UX模式

#####设计系统实现与组件库
- **组件库结构**：构建可重用的组件系统  ```typescript
  // Example: Design system button component
  interface ButtonProps {
    variant: 'primary' | 'secondary' | 'danger';
    size: 'small' | 'medium' | 'large';
    disabled?: boolean;
    onClick: () => void;
    children: React.ReactNode;
  }

  export const Button: React.FC<ButtonProps> = ({
    variant, size, disabled, onClick, children
  }) => {
    const classes = `btn btn-${variant} btn-${size} ${disabled ? 'btn-disabled' : ''}`;
    return <button className={classes} onClick={onClick} disabled={disabled}>{children}</button>;
  };
  ```
- **设计令牌**：实现一致的间距，颜色，排版
- **组件文档**：使用Storybook作为组件文档

#####黑暗模式和主题系统
- **主题提供程序实现**：支持多个主题  ```typescript
  // Example: Theme context and provider
  const ThemeContext = createContext({
    theme: 'light',
    toggleTheme: () => {}
  });

  export const ThemeProvider: React.FC<{children: ReactNode}> = ({ children }) => {
    const [theme, setTheme] = useState<'light' | 'dark'>('light');

    const toggleTheme = () => {
      setTheme(prev => prev === 'light' ? 'dark' : 'light');
    };

    return (
      <ThemeContext.Provider value={{ theme, toggleTheme }}>
        <div className={`theme-${theme}`}>{children}</div>
      </ThemeContext.Provider>
    );
  };
  ```
- **CSS自定义属性**：使用CSS变量动态主题
- **系统偏好检测**：尊重用户的操作系统主题偏好

#####响应式设计高级模式
**容器查询**：使用基于容器的响应式设计  ```css
  /* Example: Container query for responsive components */
  .card-container {
    container-type: inline-size;
  }

  @container (min-width: 400px) {
    .card {
      display: grid;
      grid-template-columns: 1fr 1fr;
    }
  }
  ```
- **流体排版**：实现响应式字体缩放
- **自适应布局**：根据屏幕大小和上下文更改布局模式

#####动画与微互动
- **帧运动集成**：平滑的动画和过渡  ```typescript
  import { motion, AnimatePresence } from 'framer-motion';

  const AnimatedCard = () => {
    return (
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        exit={{ opacity: 0, y: -20 }}
        transition={{ duration: 0.3 }}
        whileHover={{ scale: 1.02 }}
        className="card"
      >
        Card content
      </motion.div>
    );
  };
  ```
- **加载状态**：动画骨架和进度指标
- **手势识别**：滑动，捏，触摸交互
- **性能优化**：使用CSS转换和将改变属性

#####可访问性自动化和测试
- **ARIA实现**：正确的语义标记和ARIA属性  ```typescript
  // Example: Accessible modal component
  const Modal: React.FC<{isOpen: boolean, onClose: () => void, children: ReactNode}> = ({
    isOpen, onClose, children
  }) => {
    useEffect(() => {
      if (isOpen) {
        document.body.style.overflow = 'hidden';
        const focusableElement = document.querySelector('[data-autofocus]') as HTMLElement;
        focusableElement?.focus();
      }
      return () => { document.body.style.overflow = 'unset'; };
    }, [isOpen]);

    return (
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="modal-title"
        className={isOpen ? 'modal-open' : 'modal-hidden'}
      >
        {children}
      </div>
    );
  };
  ```
- **自动化可访问性测试**：集成可访问性测试的斧头核心
- **键盘导航**：实现全键盘可访问性
- **屏幕阅读器优化**：测试与NVDA， JAWS和VoiceOver

#####国际化（i18n）和本地化
- **React-intl集成**：多语言支持  ```typescript
  import { FormattedMessage, useIntl } from 'react-intl';

  const WelcomeMessage = ({ userName }: { userName: string }) => {
    const intl = useIntl();

    return (
      <h1>
        <FormattedMessage
          id="welcome.title"
          defaultMessage="Welcome, {userName}!"
          values={{ userName }}
        />
      </h1>
    );
  };
  ```
- **语言检测**：自动进行语言检测和切换
- **RTL支持**：从右到左的语言支持阿拉伯语，希伯来语
- **日期和数字格式**：特定于地区的格式
- **翻译管理**：与翻译服务集成

当前的限制和解决方法

已知的限制

—不支持内容安全策略（CSP）
—不支持存储SAS IP限制
-没有Power Platform Git集成
支持反数据解决方案，但解决方案打包器和源代码集成受到限制
-通过SDK记录器配置支持应用程序洞察（没有内置的本地集成）

# # #工作区

-如果需要，使用其他错误跟踪解决方案
—手动部署流程
-使用外部工具进行高级分析
—规划未来迁移到支持的特性

文档标准-维护全面的README.md设置说明
-记录所有自定义组件和钩子
—包括常见问题的故障处理指南
—文档化部署流程和要求
维护版本更新的变更日志
-包括主要选择的架构决策记录

##常见问题排除

发展问题- **端口3000冲突**：使用`netstat -ano | findstr :3000`杀死现有进程，然后使用`taskkill /PID {PID} /F`—**鉴权失败**：使用`pac auth list`验证环境设置和用户权限
- **包安装失败**：使用`npm cache clean --force`清除npm缓存并重新安装
- **TypeScript编译错误**：检查veratimmodulesyntax设置和SDK兼容性
—**连接器权限错误**：确保正确的同意流程和管理员权限
- **PowerProvider问题**：确保v1.0应用程序不等待SDK初始化
- **Vite开发服务器问题**：确保主机和端口配置符合要求

###部署问题- **构建失败**：验证`npm audit`的所有依赖并检查构建配置
- **认证错误**：先用`pac auth clear`再用`pac auth create`重新认证PAC CLI
—**连接器不可用**：检查电源平台上的连接器设置和连接状态
- **性能问题**：使用`npm run build --report`优化包大小并实现缓存
- **环境不匹配**：与`pac env list`确认正确的环境选择
- **App超时错误**：检查构建输出和网络连接

###运行时问题

- **“App timed out”错误**：验证npm run build是否被执行，部署输出是否有效
—**连接器认证提示**：确保正确的同意流实现
—**数据加载失败**：检查网络请求和连接器权限
**UI渲染问题**：验证Fluent UI兼容性和响应式设计实现

最佳实践总结1. **遵循微软的官方文档和最佳实践**
2. **使用TypeScript来保证类型安全并获得更好的开发体验**
3. **实施适当的错误处理和用户反馈**
4. **优化性能和用户体验**
5. **遵循安全最佳实践和Power Platform政策**
6. **编写可维护的、可测试的、文档完备的代码**
7. **使用PAC CLI生成的服务和模型**
8. **计划未来的功能更新和迁移**
9. **实施全面的测试策略**

10. **遵循适当的DevOps和部署实践**