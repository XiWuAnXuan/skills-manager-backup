---
name: geb
description: GEB 分形文档系统（Fractal Documentation System）。维护 PROJECT_INDEX.md、FOLDER_INDEX.md 与代码文件头 Input/Output/Pos 的同构同步。Use when user says 初始化索引、更新索引、检查索引、GEB、分形文档、geb init/update/check, or when making structural code changes that require index sync.
---

# GEB 分形文档系统（操作技能）

受《哥德尔、埃舍尔、巴赫》启发：**代码（机器相）与文档（语义相）严格同构**。
全局规则见 `~/.claude/CLAUDE.md`。本技能给出可执行步骤与模板。

## 何时启用

- 用户：初始化索引 / 更新索引 / 检查索引 / GEB / 分形文档
- 结构变更后：新增/删除/移动代码文件，或修改 import/export/public API/路由

## 三级结构速查

```
PROJECT_INDEX.md                 # L1 根索引 + Mermaid
folder/FOLDER_INDEX.md           # L2 文件夹索引
folder/file.ts                   # L3 文件头 Input/Output/Pos
```

## 过滤规则

跳过：`PROJECT_INDEX.md`、`FOLDER_INDEX.md`；非代码扩展名；
`node_modules` `.git` `dist` `build` `.next` `out` `target` `vendor` `__pycache__` `.cache` `coverage` `.turbo` `.venv` `venv` `.velite`；
文件 > 500KB。

允许扩展名：`.js .jsx .ts .tsx .mjs .cjs .py .java .kt .rs .go .cpp .c .h .php .rb .swift .cs`

## 结构 vs 实现

- **结构变更** → 更新 L3 + L2 + L1
- **实现变更** → 不更新索引

结构关键字示例：`import` `require` `from` `export` `class` `interface` `function` `def` `fn` `async`（以及新增/删除文件、改路径）

---

## 命令 A：初始化索引（init）

1. 扫描项目代码树（应用过滤）
2. 为每个代码文件写入/补全文件头（L3），已有合法三字段则修正而非整段抹掉用户其它注释
3. 为每个含代码的文件夹生成 `FOLDER_INDEX.md`（L2）
4. 生成根 `PROJECT_INDEX.md`（L1 + Mermaid）
5. 简短汇报：文件数、文件夹索引数、是否已有遗留文档

### L3 模板（TS/JS）

```typescript
/**
 * Input: <deps>
 * Output: <exports>
 * Pos: <layer-role>
 *
 * 本注释在文件修改时自动更新
 */
```

### L3 模板（Python）

```python
"""
Input: <deps>
Output: <exports>
Pos: <layer-role>

本注释在文件修改时自动更新
"""
```

### L2 模板

```markdown
# FOLDER_INDEX — <path>/

## 架构说明
- <2–5 行说明本文件夹职责与调用关系>

## 文件清单
| 文件 | 职责 |
|------|------|
| `foo.ts` | ... |

## 自指
本文件夹变化时请更新本文件，并同步更新 `PROJECT_INDEX.md`。
```

### L1 模板

```markdown
# PROJECT_INDEX

## 项目概览
- **名称**:
- **定位**:
- **技术栈**:
- **入口**:

## 目录结构
\`\`\`
...
\`\`\`

## 模块依赖（Mermaid）
\`\`\`mermaid
graph TD
  A --> B
\`\`\`

## 关键约定
- 代码与本索引同构同步（GEB）

## 自指
当项目结构变化时，请更新本文件。
```

---

## 命令 B：更新索引（update）

对变更集：

1. 过滤非代码/排除目录
2. 判定结构 vs 实现
3. 结构变更则：重写相关文件头 → 刷新所属 `FOLDER_INDEX.md` → 刷新 `PROJECT_INDEX.md` 与图
4. 删除文件时从 L2/L1 移除；新建目录时补 L2
5. 静默或一行：`✅ GEB 索引已更新`

---

## 命令 C：检查索引（check）

报告：
- 缺文件头的代码文件
- 缺 `FOLDER_INDEX.md` 的代码目录
- 缺 `PROJECT_INDEX.md`
- L2 清单与真实文件不一致
- 可选：明显循环依赖

默认只报告；用户要求时再修复。

---

## Pos 推断提示

| 路径线索 | Pos 层级 |
|----------|----------|
| `app/` `pages/` `routes/` | 路由/页面层 |
| `components/` | UI 组件层 |
| `lib/` `utils/` `helpers/` | 工具层 |
| `services/` | 业务服务层 |
| `models/` `schemas/` | 数据/模型层 |
| `hooks/` | UI 钩子层 |
| `api/` `controllers/` | API 层 |
| 配置根文件 `*.config.*` | 配置层 |

---

## 完成前检查

- 结构变更是否已同步 L1/L2/L3
- 未完成索引同步则不得声称任务完成
- 不覆盖无关 README/设计文档

## 参考

- 全局规则：`~/.claude/CLAUDE.md`（姊妹：`~/.grok/AGENTS.md`）
- 开源实现：https://github.com/Claudate/project-multilevel-index
- 提示词来源介绍：https://love.chunxiang.space/prompts
