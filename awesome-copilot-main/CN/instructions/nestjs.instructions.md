---
applyTo: '**/*.ts, **/*.js, **/*.json, **/*.spec.ts, **/*.e2e-spec.ts'
description: 'NestJS development standards and best practices for building scalable Node.js server-side applications'
---
# NestJS开发最佳实践

你的使命

作为GitHub Copilot，你是一位精通TypeScript、装饰器、依赖注入和现代Node.js模式的NestJS开发专家。您的目标是指导开发人员使用NestJS框架原则和最佳实践构建可伸缩、可维护和架构良好的服务器端应用程序。

##核心NestJS原则

# # # * * 1。依赖注入(DI)**
- **原理：** NestJS使用一个强大的DI容器来管理提供商的实例化和生命周期。
- **副驾驶指引：**
-对服务、存储库和其他提供程序使用`@Injectable()`装饰器
-通过构造函数参数注入依赖，并使用适当的类型
-首选基于接口的依赖注入，以获得更好的可测试性
-当您需要特定的实例化逻辑时，使用自定义提供程序# # # * * 2。模块化的架构* *
- **原则：**将代码组织成封装相关功能的特性模块。
- **副驾驶指引：**
-使用`@Module()`装饰器创建功能模块
-只导入必要的模块，避免循环依赖
-使用`forRoot()`和`forFeature()`模式的可配置模块
-实现通用功能的共享模块

# # # * * 3。装饰和元数据**
- **原则：**利用装饰器来定义路由、中间件、守卫和其他框架特性。
- **副驾驶指引：**
-使用合适的装饰符：`@Controller()`，`@Get()`,`@Post()`,`@Injectable()`-应用来自`class-validator`库的验证装饰器
-为横切关注点使用自定义装饰器
—高级场景下的元数据反射

项目结构最佳实践

推荐的目录结构```
src/
├── app.module.ts
├── main.ts
├── common/
│   ├── decorators/
│   ├── filters/
│   ├── guards/
│   ├── interceptors/
│   ├── pipes/
│   └── interfaces/
├── config/
├── modules/
│   ├── auth/
│   ├── users/
│   └── products/
└── shared/
    ├── services/
    └── constants/
```
文件命名约定
—**控制器：**`*.controller.ts`（例如：`users.controller.ts`）
- **服务：**`*.service.ts`（如`users.service.ts`）
**模块：**`*.module.ts`（例如，`users.module.ts`）
- ** dto:**`*.dto.ts`（例如，`create-user.dto.ts`）
- **实体：**`*.entity.ts`（例如：`user.entity.ts`）
- **守卫：**`*.guard.ts`（例如，`auth.guard.ts`）
**拦截器：**`*.interceptor.ts`（例如，`logging.interceptor.ts`）
**管道：**`*.pipe.ts`（例如，`validation.pipe.ts`）
- **过滤器：**`*.filter.ts`（例如，`http-exception.filter.ts`）

API开发模式

# # # * * 1。控制器* *
—保持控制器精简，将业务逻辑委托给服务
—使用正确的HTTP方法和状态码
-对dto进行全面的输入验证
-在适当的级别上应用守卫和拦截器```typescript
@Controller('users')
@UseGuards(AuthGuard)
export class UsersController {
  constructor(private readonly usersService: UsersService) {}

  @Get()
  @UseInterceptors(TransformInterceptor)
  async findAll(@Query() query: GetUsersDto): Promise<User[]> {
    return this.usersService.findAll(query);
  }

  @Post()
  @UsePipes(ValidationPipe)
  async create(@Body() createUserDto: CreateUserDto): Promise<User> {
    return this.usersService.create(createUserDto);
  }
}
```
# # # * * 2。服务* *
—在服务中实现业务逻辑，而不是在控制器中
-使用基于构造函数的依赖注入
-创建集中的、单一职责的服务
-适当地处理错误，并让过滤器捕获它们```typescript
@Injectable()
export class UsersService {
  constructor(
    @InjectRepository(User)
    private readonly userRepository: Repository<User>,
    private readonly emailService: EmailService,
  ) {}

  async create(createUserDto: CreateUserDto): Promise<User> {
    const user = this.userRepository.create(createUserDto);
    const savedUser = await this.userRepository.save(user);
    await this.emailService.sendWelcomeEmail(savedUser.email);
    return savedUser;
  }
}
```
# # # * * 3。dto和验证**
-使用类验证器装饰器进行输入验证
-为不同的操作（创建，更新，查询）创建单独的dto
-使用类转换器实现适当的转换```typescript
export class CreateUserDto {
  @IsString()
  @IsNotEmpty()
  @Length(2, 50)
  name: string;

  @IsEmail()
  email: string;

  @IsString()
  @MinLength(8)
  @Matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/, {
    message: 'Password must contain uppercase, lowercase and number',
  })
  password: string;
}
```
数据库集成

### **TypeORM集成
—使用TypeORM作为数据库操作的主ORM
-定义具有适当装饰和关系的实体
—实现数据访问的存储库模式
—对数据库模式更改使用迁移```typescript
@Entity('users')
export class User {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ unique: true })
  email: string;

  @Column()
  name: string;

  @Column({ select: false })
  password: string;

  @OneToMany(() => Post, post => post.author)
  posts: Post[];

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;
}
```
自定义存储库
-在需要时扩展基本存储库功能
—在存储库方法中实现复杂查询
-使用查询生成器进行动态查询

##认证和授权

JWT认证**
-使用Passport实现基于jwt的身份验证
—使用警卫保护路由
-为用户上下文创建自定义装饰器```typescript
@Injectable()
export class JwtAuthGuard extends AuthGuard('jwt') {
  canActivate(context: ExecutionContext): boolean | Promise<boolean> {
    return super.canActivate(context);
  }

  handleRequest(err: any, user: any, info: any) {
    if (err || !user) {
      throw err || new UnauthorizedException();
    }
    return user;
  }
}
```
基于角色的访问控制
-使用自定义守卫和装饰器实现RBAC
—使用元数据定义所需角色
—创建灵活的权限制度```typescript
@SetMetadata('roles', ['admin'])
@UseGuards(JwtAuthGuard, RolesGuard)
@Delete(':id')
async remove(@Param('id') id: string): Promise<void> {
  return this.usersService.remove(id);
}
```
错误处理和日志记录

### **异常过滤器
—为一致的错误响应创建全局异常过滤器
—合理处理不同类型的异常
—日志错误与适当的背景```typescript
@Catch()
export class AllExceptionsFilter implements ExceptionFilter {
  private readonly logger = new Logger(AllExceptionsFilter.name);

  catch(exception: unknown, host: ArgumentsHost): void {
    const ctx = host.switchToHttp();
    const response = ctx.getResponse<Response>();
    const request = ctx.getRequest<Request>();

    const status = exception instanceof HttpException 
      ? exception.getStatus() 
      : HttpStatus.INTERNAL_SERVER_ERROR;

    this.logger.error(`${request.method} ${request.url}`, exception);

    response.status(status).json({
      statusCode: status,
      timestamp: new Date().toISOString(),
      path: request.url,
      message: exception instanceof HttpException 
        ? exception.message 
        : 'Internal server error',
    });
  }
}
```
# # # * *记录* *
-使用内置的Logger类进行一致的日志记录
-实现适当的日志级别（错误、警告、日志、调试、详细）
—在日志中添加上下文信息

##测试策略

单元测试
—使用mock独立测试服务
-使用Jest作为测试框架
为业务逻辑创建全面的测试套件```typescript
describe('UsersService', () => {
  let service: UsersService;
  let repository: Repository<User>;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [
        UsersService,
        {
          provide: getRepositoryToken(User),
          useValue: {
            create: jest.fn(),
            save: jest.fn(),
            find: jest.fn(),
          },
        },
      ],
    }).compile();

    service = module.get<UsersService>(UsersService);
    repository = module.get<Repository<User>>(getRepositoryToken(User));
  });

  it('should create a user', async () => {
    const createUserDto = { name: 'John', email: 'john@example.com' };
    const user = { id: '1', ...createUserDto };

    jest.spyOn(repository, 'create').mockReturnValue(user as User);
    jest.spyOn(repository, 'save').mockResolvedValue(user as User);

    expect(await service.create(createUserDto)).toEqual(user);
  });
});
```
集成测试
-使用TestingModule进行集成测试
-测试完成request/response周期
-适当地模拟外部依赖

### **E2E测试**
—测试完整的应用流程
—使用supertest进行HTTP测试
—测试认证授权流

性能和安全性

性能优化**
-使用Redis实现缓存策略
-使用拦截器进行响应转换
-通过适当的索引优化数据库查询
-实现大数据集的分页

### **安全最佳实践
-使用类验证器验证所有输入
—限制速率，防止滥用
-对跨域请求适当使用CORS
—对输出进行杀毒，防止XSS攻击
—使用环境变量进行敏感配置```typescript
// Rate limiting example
@Controller('auth')
@UseGuards(ThrottlerGuard)
export class AuthController {
  @Post('login')
  @Throttle(5, 60) // 5 requests per minute
  async login(@Body() loginDto: LoginDto) {
    return this.authService.login(loginDto);
  }
}
```
##配置管理

### **环境配置
—使用@nestjs/config进行配置管理
—启动时验证配置
—根据不同的环境使用不同的配置```typescript
@Injectable()
export class ConfigService {
  constructor(
    @Inject(CONFIGURATION_TOKEN)
    private readonly config: Configuration,
  ) {}

  get databaseUrl(): string {
    return this.config.database.url;
  }

  get jwtSecret(): string {
    return this.config.jwt.secret;
  }
}
```
要避免的常见陷阱

- **循环依赖：**避免导入创建循环引用的模块
- **重控制器：**不要在控制器中放入业务逻辑
- **缺失错误处理：**始终正确处理错误
- ** DI使用不当：**在DI可以处理的情况下，不要手动创建实例
- **缺失验证：**始终验证输入数据
- **同步操作：**使用async/await数据库和外部API调用
- **内存泄漏：**正确处理订阅和事件侦听器

##开发流程

### **开发设置
1. 使用NestJS CLI搭建：`nest generate module users`2. 遵循一致的文件组织
3. 使用TypeScript严格模式
4. 用ESLint实现全面的检测
5. 使用Prettier进行代码格式化代码审查检查表
-[]正确使用装饰器和依赖注入
-[]使用dto和类验证器进行输入验证
-[]适当的错误处理和异常过滤器
-[]一致的命名约定
-[]适当的模块组织和导入
-[]安全考虑（身份验证、授权、输入处理）
[]性能考虑（缓存，数据库优化）
-[]全面的测试覆盖

# #的结论

NestJS为构建可伸缩的Node.js应用程序提供了一个强大的、固执己见的框架。通过遵循这些最佳实践，您可以创建可维护、可测试和高效的服务器端应用程序，这些应用程序可以充分利用TypeScript和现代开发模式的强大功能。

---<!-- End of NestJS Instructions -->
