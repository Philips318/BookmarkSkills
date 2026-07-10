---
name: unit-test-vue-pinia
category: testing
description: 'Write and review unit tests for Vue 3 + TypeScript + Vitest + Pinia codebases. Use when creating or updating tests for components, composables, and stores; mocking Pinia with createTestingPinia; applying Vue Test Utils patterns; and enforcing black-box assertions over implementation details.'
---
# unit-test-vue-pinia

使用此技能可以创建或审查Vue组件、可组合组件和Pinia存储的单元测试。让测试保持小规模、确定性和行为优先。

# #工作流程

1. 首先确定行为边界：组件UI行为、可组合行为或存储行为。
2. 选择能够证明该行为的最窄的测试样式。
3. 使用仍然覆盖该场景的最不强大的选项设置Pinia。
4. 通过诸如道具、表单更新、按钮单击、发出的子事件和存储api等公共输入驱动测试。
5. 在考虑任何实例级断言之前，断言可观察输出和副作用。
6. 返回或检查带有明确的行为导向名称的测试，并注意任何剩余的覆盖率缺口。

核心规则-每个测试测试一个行为。
-首先断言可观察的input/output行为（渲染文本，发出事件，回调调用，存储状态更改）。
-避免实现耦合断言。
-仅在没有合理的DOM、prop、emit或存储级断言的特殊情况下访问`wrapper.vm`。
-更喜欢在`beforeEach()`中显式设置，并在每次测试中重置模拟。
-使用`references/pinia-patterns.md`中签入的参考材料作为标准Pinia测试设置的本地真实来源。

## Pinia测试方法

首先使用`references/pinia-patterns.md`，当签入的示例没有涵盖这种情况时，再回到Pinia的测试食谱。

组件测试的默认模式

在挂载时使用`createTestingPinia`作为全局插件。
首选`createSpy: vi.fn`作为默认值，以获得一致性和更简单的动作监视断言。```ts
const wrapper = mount(ComponentUnderTest, {
	global: {
		plugins: [
			createTestingPinia({
				createSpy: vi.fn,
			}),
		],
	},
});
```
默认情况下，操作被存根和监视。
当测试只需要验证是否调用（或未调用）某个操作时，使用`stubActions: true`（默认值）。

接受最小的Pinia设置

以下也是有效的，不应该被标记为不正确：

-`createTestingPinia({})`测试时不断言piia动作间谍行为。
-`createTestingPinia({ initialState: ... })`或`createTestingPinia({ stubActions: ... })`没有`createSpy`，当测试只需要状态播种或动作存根行为，不检查生成的间谍。
-当需要mocking/seeding相关存储时，`setActivePinia(createTestingPinia(...))`在store/composable-focused测试中（不安装组件）。

当动作监视断言是测试意图的一部分时，使用`createSpy: vi.fn`。

###只在需要时执行实际操作

只有当测试必须验证动作的真实行为和副作用时，才使用`stubActions: false`。对于简单的“was called”断言，默认情况下不要打开它。```ts
const wrapper = mount(ComponentUnderTest, {
	global: {
		plugins: [
			createTestingPinia({
				createSpy: vi.fn,
				stubActions: false,
			}),
		],
	},
});
```
###种子存储状态`initialState````ts
const wrapper = mount(ComponentUnderTest, {
	global: {
		plugins: [
			createTestingPinia({
				createSpy: vi.fn,
				initialState: {
					counter: { n: 20 },
					user: { name: "Leia Organa" },
				},
			}),
		],
	},
});
```
通过`createTestingPinia`添加piia插件```ts
const wrapper = mount(ComponentUnderTest, {
	global: {
		plugins: [
			createTestingPinia({
				createSpy: vi.fn,
				plugins: [myPiniaPlugin],
			}),
		],
	},
});
```
边缘情况下的Getter覆盖模式```ts
const pinia = createTestingPinia({ createSpy: vi.fn });
const store = useCounterStore(pinia);

store.double = 999;
// @ts-expect-error test-only reset of overridden getter
store.double = undefined;
```
纯存储单元测试

当目标是在不呈现组件的情况下验证存储状态转换和操作行为时，建议使用`createPinia()`进行纯存储测试。只有在需要存根依赖存储、种子测试副本或操作监视时才使用`createTestingPinia()`。```ts
beforeEach(() => {
	setActivePinia(createPinia());
});

it("increments", () => {
	const counter = useCounterStore();
	counter.increment();
	expect(counter.n).toBe(1);
});
```
##我们的测试工具方法

遵循Vue测试工具指南：<https://test-utils.vuejs.org/guide/>-对于集中的单元测试，默认为浅挂载。
-仅当集成行为是主题时挂载完整组件树。
-通过道具、类用户交互和发出的事件驱动行为。
-首选`findComponent(...).vm.$emit(...)`子存根事件，而不是触及父内部。
—仅在异步更新时使用`nextTick`。
-使用`wrapper.emitted(...)`断言发出的事件和有效负载。
-仅在没有DOM断言、发出的事件断言、道具断言或存储级断言可以表达行为时访问`wrapper.vm`。将其视为异常，并保持断言的范围很窄。

关键测试片段

发出和断言有效载荷：```ts
await wrapper.find("button").trigger("click");
expect(wrapper.emitted("submit")?.[0]?.[0]).toBe("Mango Mission");
```
更新输入并断言输出：```ts
await wrapper.find("input").setValue("Agent Violet");
await wrapper.find("form").trigger("submit");
expect(wrapper.emitted("save")?.[0]?.[0]).toBe("Agent Violet");
```
测试编写工作流

1. 确定要测试的行为边界。
2. 构建最少的fixture数据（只有该行为所需的字段）。
3. 配置Pinia和所需的测试副本。
4. 通过公众输入触发行为。
5. 断言公共输出和副作用。
6. 重构测试名来描述行为，而不是实现。

约束和安全—不测试private/internal的实现细节。
—不要过度使用快照的动态UI行为。
-如果只有一个行为重要，不要断言大型对象中的每个字段。
-保持虚假数据的确定性；避免随机值。
-不要声称一个Pinia设置是错误的，当它是上面接受的最小设置之一。
-除非测试行为需要额外的表面面积，否则不要针对更深的安装或实际动作重写工作测试。
在审查期间显式地标记缺失的测试覆盖率、脆弱的选择器和实现耦合断言。

##输出合同

-对于`create`或`update`，返回完成的测试代码加上描述所选Pinia策略的简短说明。
-对于`review`，先返回具体结果，然后再返回缺失覆盖或脆性风险。
-当最安全的选择模棱两可时，说明驱动所选测试设置的假设。

# #引用- `references/pinia-patterns.md`
- Pinia测试食谱：<https://pinia.vuejs.org/cookbook/testing.html>- Vue测试工具指南：<https://test-utils.vuejs.org/guide/>