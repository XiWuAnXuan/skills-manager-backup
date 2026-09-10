---
name: geb-docs-system
description: GEB 系统（分形文档系统协议，Fractal Documentation System）落地执行技能。当用户要求"遵守 GEB 系统"、"分形文档"、"Fractal Documentation System"、"项目文档与代码同步"、"PROJECT_INDEX/FOLDER_INDEX/文件头注释"、或在新项目开始时需要初始化三级分形文档结构时使用。也可用于在任意项目中创建/更新项目索引文档。
agent_created: true
---

# GEB 系统（分形文档系统协议）

GEB 系统是赵纯想（@chunxiangai）基于《哥德尔、埃舍尔、巴赫》(GEB) 理念开发的**项目文档与代码严格同步的自指分形结构协议**。

## 核心原则

**代码（机器相）与文档（语义相）必须严格同构同步**：任何一处修改，都必须立即体现在另一处，否则视为"未完成"。

## 三级分形自指结构

| 层级 | 位置 | 内容 | 必备语句 |
|---|---|---|---|
| **L1 根层** | 项目根目录 `PROJECT_INDEX.md` | 项目总览、完整目录结构、Mermaid 依赖关系图 | "本文件在项目结构发生变化时必须同步更新" |
| **L2 文件夹层** | 每个文件夹内 `FOLDER_INDEX.md` | 本文件夹架构说明、文件清单 | "本文件夹变化时请更新我" |
| **L3 文件层** | 每个代码文件顶部统一文件头注释 | Input（输入/依赖）、Output（输出/功能）、Pos（在系统中的定位） | "本注释在文件修改时自动更新" |

## 执行流程

### 1. 新项目初始化（init）
在项目根目录创建：
- `PROJECT_INDEX.md`：项目总览（名称/版本/定位/技术栈/许可证）+ 完整目录树 + Mermaid 依赖图（前端/后端/桥接/外部依赖分区）+ 关键架构说明
- 核心目录（如 `src/`、`src-tauri/`、`backend/`、`frontend/`）各自创建 `FOLDER_INDEX.md`
- 参考模板：`PROJECT_INDEX.md` 结构 = 项目总览表 + 目录结构代码块 + Mermaid 图 + 架构说明 + 待办

### 2. 开发中维护（update）
- 创建/修改代码文件时，在文件顶部写入/更新 L3 文件头注释
- 新增/删除/重命名文件或目录时，同步更新对应 `FOLDER_INDEX.md`
- 项目结构变化时，同步更新 `PROJECT_INDEX.md` 的目录树与依赖图

### 3. 文件头注释模板（L3）

```js
/**
 * @file 文件名
 * @brief 一句话功能说明
 * @input 输入/依赖（依赖的模块、参数、数据）
 * @output 输出/功能（对外提供的能力、返回值、副作用）
 * @pos 在系统中的定位（所属层级、被谁调用、调用谁）
 * 本注释在文件修改时自动更新（GEB 系统 L3 文件层规范）
 */
```

Rust 版本：

```rust
//! @file 文件名
//! @brief 一句话功能说明
//! @input 输入/依赖
//! @output 输出/功能
//! @pos 在系统中的定位
//! 本注释在文件修改时自动更新（GEB 系统 L3 文件层规范）
```

### 4. FOLDER_INDEX.md 模板（L2）

```markdown
# FOLDER_INDEX.md — <文件夹名>（<一句话职责>）

> 本文件遵循 **GEB 系统 L2 文件夹层规范**。
> **本文件夹变化时请更新我。**

## 架构说明
（本文件夹的职责、设计模式、与其他模块的关系）

## 文件清单
| 路径 | 职责 |
|---|---|
| ... | ... |

---
*本文件由 GEB 系统驱动，文件夹结构变化时必须更新。*
```

## 参考资源
- 系统提示词包：https://love.chunxiang.space/prompts
- 开源自动化工具：https://github.com/Claudate/project-multilevel-index（`codex init` / `codex update-index`）
- 原理视频为付费课程内容，但结构规范 + 提示词 + 开源工具免费公开

## 注意
- 大型存量项目不必一次性给所有旧文件补文件头，可对**新建/修改**的文件逐步落实 L3
- 但 `PROJECT_INDEX.md`（L1）与核心目录 `FOLDER_INDEX.md`（L2）应在接手项目时立即创建
