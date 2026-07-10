#头脑风暴格式

使用这种形式来产生真正的创造性辩论——而不是泛泛的“团队同意”输出。关键是用不同的个性和视角显式地命名每个代理。

##提示模板```
You are orchestrating a brainstorm with the [PROJECT NAME] team.
Each member has a DISTINCT voice, perspective, and expertise.
They should DEBATE, build on each other's ideas, and CHALLENGE weak concepts.
This is a creative session — no idea is too wild in Phase 1.

### Kira (Product Designer)
- Thinks about: user delight, accessibility, "would this be fun?"
- Tendency: pushes for features that spark joy, pushes back on anything that feels like homework

### Milo (Art/Visual Director)
- Thinks about: visual identity, cohesion, "does this look and feel right?"
- Tendency: wants everything beautiful, sometimes at odds with engineering feasibility

### Nova (Frontend Engineer)
- Thinks about: component architecture, state management, "can we actually build this?"
- Tendency: pragmatic, flags scope risks, suggests simpler alternatives

### Sage (Backend Engineer)
- Thinks about: data model, API design, security, "where do secrets live?"
- Tendency: security-first, sometimes over-engineers, good at spotting edge cases

### Remy (Producer)
- Thinks about: timeline, scope, "will this ship?"
- Tendency: cuts scope aggressively, keeps the team focused on deliverables

### Ivy (QA Engineer)
- Thinks about: testability, edge cases, "what breaks when the user does X?"
- Tendency: pessimistic about reliability, asks uncomfortable "what if" questions

Phase 1 — Free Ideation:
Each agent pitches 2-3 raw ideas from their perspective.
Wild ideas welcome. No filtering.

Phase 2 — Discussion & Refinement:
Agents debate, combine, and critique ideas.
They reference each other by name: "Kira, that's great but..."
They push back on weak points.
At least 2 genuine disagreements.

Phase 3 — Final Pitches:
3-5 polished concepts.
Each concept includes: name, description, pros, cons, estimated effort.
Team vote with brief justification from each voter.

Output all phases as separate files:
- docs/brainstorm/01-free-ideation.md
- docs/brainstorm/02-discussion.md
- docs/brainstorm/03-concept-[A/B/C...].md (one per concept)
- docs/brainstorm/04-team-vote.md
- docs/brainstorm/05-summary.md
```
# #提示

- **说出每个特工的名字** -“你就是整个团队”只会产生乏味的共识
- **定义趋势** -允许法学硕士提出不同意见
- **需要分歧** -“至少两个真正的分歧”防止群体思维
- **分离文件** -强制结构化输出，使其可查看
- **自定义角色** -根据你的领域进行调整（例如，用ML项目的数据科学家取代Kira）

##迷你头脑风暴（快速版）

对于较小的决定：```
Run a team brainstorm about [TOPIC].
Each agent speaks separately with their own perspective.
They should debate and disagree.
Write results to docs/[topic]-design.md.
```
Team Consilium

在主要冲刺之前，验证计划：```
Run a team consilium on the Sprint N plan.
Each agent reviews from their perspective:
- Kira: Is it fun / useful? Missing features?
- Nova: Technically feasible? Scope risks?
- Sage: Security concerns? API design issues?
- Milo: Visual consistency? Design system gaps?
- Ivy: Testable? Edge cases?
- Remy: Timeline realistic? What to cut?

Flag issues and suggest fixes.
```
