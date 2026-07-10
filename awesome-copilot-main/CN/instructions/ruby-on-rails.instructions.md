---
description: 'Ruby on Rails coding conventions and guidelines'
applyTo: '**/*.rb'
---
# Ruby on Rails

##一般指引-遵循RuboCop风格指南并使用`rubocop`，`standardrb`或`rufo`等工具进行一致的格式化。
-variables/methods使用snake_case，classes/modules.使用CamelCase
-保持方法的简洁和重点；使用早期返回、保护子句和私有方法来降低复杂性。
-选择有意义的名字，而不是简短或通用的名字。
-只在必要的时候评论-避免解释显而易见的事情。
-对类、方法和模块应用单一职责原则。
-更喜欢组合而不是继承；将可重用逻辑提取到模块或服务中。
—保持控制器精简业务逻辑到模型、服务或command/query对象。
-应用“胖模型，瘦控制器”的模式，并使用干净的抽象。
-将业务逻辑提取到服务对象中，以实现可重用性和可测试性。
—使用局部组件或视图组件，减少重复，简化视图。
—使用`unless`用于阴性条件，但为了清晰起见，避免使用`else`。
-避免深度嵌套条件-倾向于保护子句和方法提取。
-使用安全导航（`&.`），而不是多次`nil`检查。
—优先选择`.present?`，`.blank?`和`.any?`，而不是手动检查nil/empty。
—路由和控制器动作遵循RESTful约定。
-使用Rails生成器来一致地支撑资源。
—使用强参数将属性安全加入白名单。
-首选枚举和类型属性，以获得更好的模型清晰度和验证。
保持迁移与数据库无关；尽可能避免原始SQL。
—总是为外键和经常查询的列添加索引。
-在DB级别定义`null: false`和`unique: true`，而不仅仅是在模型中。
-使用`find_each`迭代大型数据集，以减少内存使用。
-在模型中限定查询范围或使用查询对象以提高清晰度和重用性。
——使用`before_action`回调要谨慎——避免在其中使用业务逻辑。
—使用`Rails.cache`存储计算量大或访问频率高的数据。
-用`Rails.root.join(...)`构造文件路径，而不是硬编码。
—在关联中使用`class_name`和`foreign_key`用于显式关系。
-使用`Rails.application.credentials`或ENV变量保留代码库中的秘密和配置。
为模型、服务和助手编写独立的单元测试。
-使用request/system测试覆盖端到端逻辑。
—使用后台作业（ActiveJob）进行非阻塞操作，如发送电子邮件或调用api。
-使用`FactoryBot`（RSpec）或fixture （Minitest）干净地设置测试数据。
-避免使用`puts`-调试`byebug`、`pry`或日志工具。
-用YARD或RDoc记录复杂的代码路径和方法。## App目录结构

—在`app/services`目录下定义服务对象，封装业务逻辑。
-使用位于`app/forms`中的表单对象来管理验证和提交逻辑。
—在`app/serializers`目录中实现JSON序列化器，格式化API响应。
—在`app/policies`中定义授权策略，控制用户对资源的访问。
通过在`app/graphql`内部组织模式、查询和突变来构建GraphQL API。
在`app/validators`中创建自定义验证器来强制执行专门的验证逻辑。
-隔离和封装复杂的ActiveRecord查询在`app/queries`更好的重用和可测试性。
在`app/types`目录中定义自定义数据类型和强制逻辑，以扩展或覆盖ActiveModel类型行为。

# #命令—使用“`rails generate`”创建新的模型、控制器和迁移。
—使用“`rails db:migrate`”应用数据库迁移。
—使用`rails db:seed`用初始数据填充数据库。
—使用`rails db:rollback`恢复上次迁移。
-使用`rails console`与REPL环境中的Rails应用程序交互。
—使用“`rails server`”启动开发服务器。
—使用`rails test`运行测试套件。
—使用`rails routes`列出应用中所有已定义的路由。
—使用`rails assets:precompile`编译用于生产的资产。


API开发最佳实践-使用Rails的`resources`结构路由，以遵循RESTful约定。
-使用命名空间路由（例如，`/api/v1/`）进行版本控制和向前兼容性。
-使用`ActiveModel::Serializer`或`fast_jsonapi`序列化响应以获得一致的输出。
-为每个响应返回适当的HTTP状态码（例如，200 OK, 201 Created, 422 Unprocessable Entity）。
—使用`before_action`过滤器加载和授权资源，而不是业务逻辑。
-为返回大型数据集的端点利用分页（例如，`kaminari`或`pagy`）。
-使用中间件或宝石（如`rack-attack`）限制速率和节流敏感端点。
—以结构化JSON格式返回错误，包括错误码、消息和详细信息。
—使用强参数对输入参数进行消毒和白名单处理。
-使用自定义序列化器或演示器将内部逻辑与响应格式解耦。
-当急切加载相关时，通过使用`includes`避免N+1查询数据。
-为发送电子邮件或与外部api同步等非阻塞任务实现后台作业。
—记录request/response元数据，用于调试、可观察和审计。
-文档端点使用OpenAPI (Swagger),`rswag`，或`apipie-rails`。
-使用CORS头（`rack-cors`）允许跨域访问您的API时需要。
-确保敏感数据不会在API响应或错误消息中暴露。前端开发最佳实践-使用`app/javascript`作为管理JavaScript包，模块和前端逻辑在Rails 6+与Webpacker或esbuild的主目录。
-按组件或域构建JavaScript，而不是按文件类型，以保持模块化。
-利用Hotwire （Turbo + Stimulus）在rails原生应用中进行实时更新和最小化JavaScript。
-使用刺激控制器将行为绑定到HTML并以声明的方式管理UI逻辑。
-在`app/assets/stylesheets`下使用SCSS模块，Tailwind或BEM约定组织样式。
通过将重复标记提取到部分或组件中来保持视图逻辑的整洁。
-在所有视图中使用语义HTML标签并遵循可访问性（a11y）最佳实践。
-避免内联JavaScript和样式；相反，应该将逻辑移动到分离`.js`或`.scss`文件，以提高清晰度和可重用性。
-优化资源（图像，字体，图标）使用资源管道或捆绑器为每个Ing和压缩。
-使用`data-*`属性来桥接rails生成的HTML和Stimulus的前端交互。
-使用系统测试（Capybara）或集成测试工具（如Cypress或剧作家）测试前端功能。
-使用特定于环境的资产加载，以防止生产中不必要的脚本或样式。
遵循设计系统或组件库来保持UI的一致性和可扩展性。
-使用延迟加载、Turbo帧和延迟JS优化首次绘制时间（TTFP）和资产加载。测试指南-使用`test/models`（Minitest）或`spec/models`（RSpec）为模型编写单元测试以验证业务逻辑。
-使用fixture （Minitest）或工厂使用`FactoryBot`（RSpec）来干净一致地管理测试数据。
-组织`test/controllers`或`spec/requests`下的控制器规格，以测试RESTful API行为。
—首选RSpec中的`before`块或Minitest中的`setup`块初始化通用测试数据。
—避免在测试中碰到外部api—使用`WebMock`、`VCR`或`stub_request`来隔离测试环境。
-使用Minitest中的`system tests`或`feature specs`与RSpec中的Capybara来模拟完整的用户流。
-将缓慢且昂贵的测试（例如，外部服务、文件上传）隔离到单独的测试类型或标记中。
-运行测试覆盖工具，如`SimpleCov`，以确保足够的代码覆盖。
-避免测试中出现`sleep`；使用`perform_enqueued_jobs`（Minitest）或`ActiveJob::TestHelper`与RSpec。
—使用数据库清理工具（`rails test:prepare`、`DatabaseCleaner`、xqz8）1xqz)在测试之间保持干净状态。
-通过使用`ActiveJob::TestHelper`或`have_enqueued_job`匹配器排队和执行作业来测试后台作业。
-使用CI工具（例如，GitHub Actions, CircleCI）确保测试在不同环境中一致运行。
-使用自定义匹配器（RSpec）或自定义断言（Minitest）来实现可重用和表达性测试逻辑。
-按类型标记测试（例如，`:model`、`:request`、`:feature`），以便更快地进行有针对性的测试运行。
避免脆弱的测试——除非明确需要，否则不要依赖特定的时间戳、随机数据或顺序。
-编写跨多个层（模型、视图、控制器）的端到端流集成测试。
保持测试快速、可靠，并且像生产代码一样DRY。