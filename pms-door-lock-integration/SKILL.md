---
name: pms-door-lock-integration
description: |
  门锁系统对接技能 This skill should be used when integrating a hotel PMS (Property Management System, e.g. 云店掌/绿云/西软) with a third-party door lock system (门锁软件, e.g. 汉庭/华住 MF-NK 1扇区门锁、创新佳、同创、Temic/EM 感应式门锁、雅洁、MFCard). It covers: (1) how to reverse-engineer the PMS→lock-interface calling protocol (spawn exe + URL-encoded JSON), (2) how to write a pms_interface.exe replacement that calls the lock vendor DLL (NewICdll.dll / LCRFRW_SDK.dll etc.), (3) both lock SDK calling styles — string-style card data (T0|R楼-层-房-门|D..|O..|L0) and parameter-style (门锁编号+时间+流水号), (4) the lock_room.txt room↔lock-number mapping table, (5) room-number sync from PMS click to lock software prefill, and (6) a troubleshooting knowledge base (compile pitfalls, 0-based COM index, SDK return codes, single-instance conflicts). Trigger keywords: PMS门锁对接, 门锁接口, pms_interface.exe, 制卡程序, 发卡对接, lock_room.txt, 房号同步到门锁, 汉庭门锁对接, Temic门锁, LCRFRW_SDK.
agent_created: true
---

# PMS 门锁系统对接 Skill

## Overview

指导如何将酒店 PMS 系统与第三方门锁软件对接：从**逆向摸清 PMS 调用协议**，到**编写对接程序**（替代原 pms_interface.exe），再到**现场部署验证**。适用于"PMS 点击房间 → 打开制卡界面 → 人工确认 → 发卡机写卡"这一标准业务链路。

对接的本质是一条链：`PMS(Electron 前端) → spawn pms_interface.exe <URL编码JSON> → exe 调锁厂 DLL → 发卡器写卡 → 写记录文件回报 PMS`。

## Workflow Decision Tree

开始任何对接任务前，先按以下顺序判断当前处于哪个环节：

```
1. 已知 PMS 调用协议？  ──否──▶ 逆向分析（见「一、摸清 PMS 调用协议」）
        │是
2. 有锁厂 SDK？        ──否──▶ 向锁厂要 DLL + 接口文档；无文档则 Dump 导出函数
        │是
3. 需要写对接程序？    ──是──▶ 按「三、编写对接程序」实现
        │否
4. 有房号↔锁编号映射？ ──否──▶ 建 lock_room.txt（见「四、门号映射」）
        │是
5. 现场部署 + 实测（见「五、房号同步机制」+「六、部署验证」）
```

## 一、摸清 PMS 调用协议（逆向分析）

多数 PMS 是 Electron 桌面应用，安装包可解包直接读源码。流程：

1. **识别安装包类型**：NSIS（`file` 命令 / 头部特征 `NullsoftInst`）、Inno Setup、Squirrel 等。
2. **解包**：NSIS 包内嵌 `$PLUGINSDIR\app-32.7z`，用 7-Zip 提取；**若 7z 对 NSIS 虚拟路径提取失败**，直接在安装包二进制中按 7z 魔数 `7z¼¯'` 定位并截取（见 `scripts/` 中方法，或 references/protocol.md）。
3. **解压 asar**：Electron 核心代码在 `resources\app.asar`。新版 asar 头格式：`[0:4]magic [4:8]headerSize [8:12]unpaddedSize [12:16]jsonSize [16:]JSON`，**offset/size 字段是字符串类型需 int() 转换**。
4. **定位门锁调用**：搜索关键词 `pms_interface`、`Door_lock_call`、`lockmodel`、`encodeURIComponent`。
   `spawn` + `encodeURIComponent(JSON.stringify(...))` 出现处即调用点。
   ⚠ **别只翻主进程**：云店掌实测把调用点放在**渲染进程** `js/app.<hash>.js` 里
   （nodeIntegration 开启，直接 `require("child_process")`），
   主进程 `background.js` 里搜 `pms_interface` / `Door_lock_call` 是 0 命中，容易误判为"这个 PMS 没有门锁功能"。
   逆向时按**文件逐个体检关键词命中数**，而不是只看 background.js。
   > 另有 `ffi-napi` / `@serialport` 出现在 asar 里属于正常依赖，不代表走 ffi 直调 DLL。

典型结论（云店掌 PMS 实测）：PMS 用 `child_process.spawn` 启动 `{PMS安装目录上级}/Yolosoft_9dpmsLock/pms_interface.exe`，参数为 **URL 编码后的 JSON**；exe 操作完把结果写成 **GBK 编码 JSON** 到 `{exe目录}/{yyyyMMdd}/{fileName}.txt`，PMS 轮询该文件认结果。
`fileName` 由 PMS 用 4 位 base36 随机串生成并注入指令，**必须原样作为回执文件名返回**。

完整协议细节与指令字段表见 `references/protocol.md`。

## 二、确认门锁 SDK

- 锁厂一般提供：接口文档（docx）、示例程序、DLL（如汉庭 `NewICdll.dll`）、配置文件（`set.ini`：串口 `com`、密钥 `Pwd`、扇区 `section`）。
- **32 位 DLL 必须用 x86 目标编译**（C# 加 `/platform:x86`；Rust 需 i686 交叉目标）。
- 接口文档优先读"接口函数说明"章节，字段格式（如 `T0|R楼-层-房-门|DyyMMddHHmm|OyyMMddHHmm|L0`）是写卡数据的依据。
- 无文档时可用 DumpBin/Python `pefile` 导出函数名辅助判断（导出名通常直白：`OpenPort/IssueData/ReadData/CancelCard`）。

## 三、编写对接程序（替代 pms_interface.exe）

推荐 **C# WinForms**（锁厂示例多为 C#、编译简单、部署即拷即用）。工程结构：

| 文件            | 职责                                                                      |
| --------------- | ------------------------------------------------------------------------- |
| `LockSdk.cs`    | 锁厂 DLL P/Invoke 封装（StdCall、x86）                                    |
| `IniConfig.cs`  | set.ini 读写（串口/扇区/密钥）                                            |
| `PmsCommand.cs` | PMS 指令解析（URL解码→JSON→任务列表）+ 写记录文件                         |
| `LockRoom.cs`   | lock_room.txt 映射表（房号↔锁编号双向查询）                               |
| `MainForm.cs`   | 简洁界面：顶部"正在开：XXX房间"大字 + 失效时间 + 读卡/开新卡/注销卡三按钮 |
| `Program.cs`    | 入口：无参数提示退出（禁止手动模式）；有参数解析任务，逐窗口处理          |

**关键实现要点**：

- P/Invoke 用 `CallingConvention.StdCall`，字符缓冲用 `StringBuilder`。
- **先分清锁厂属于哪一类**，两者的 `LockSdk.cs` 写法完全不同：
  - **字符串式**（汉庭 `NewICdll.dll` 等）：一张卡的全部内容拼成一个字符串
    `T0|R{楼}-{层}-{房}-{门}|D{起始yyMMddHHmm}|O{结束yyMMddHHmm}|L0`（实测原版格式带 `|F` 后缀，读取时兼容），
    调 `IssueData` 一次性写入。
  - **参数式**（Temic/EM `LCRFRW_SDK.dll` 等）：没有卡数据串，改用多个入参表达
    `门锁编号 / 起始时间 YYMMDDHH / 流水号 / 时间单位 / 时长 / 退房钟点 / 反锁`，
    详见 `references/protocol.md` 第 7.1 节。
- **串口号常是 0 基索引**：`mif_selecom(itemIndex, baud)` 配 `COM1..COM16` 的下拉列表，
  意味着「COM11 要传 10」。这类偏移是"官方 Demo 能通、自己写的程序不通"的常见原因，
  建议配置里让人填人读 COM 号，代码内部统一换算，并在注释里写死这条规则。
- **发卡参数必须在 UI 线程预取**（后台线程访问控件会抛跨线程异常）。
- **SDK 返回码 7 = 新卡（空白卡）是正常状态**，不要当失败处理（读卡/注销都要单独分支友好提示）。
- 打开串口后**等待 400ms 再操作**，否则发卡机未就绪会误报"无卡"。
- SDK 调用要包 `try/finally` 恢复按钮状态，否则 DLL 异常会把界面卡死。
- 记录文件必须写 `{exe目录}/{yyyyMMdd}/{fileName}.txt`（GBK JSON），
  必含 `order_id`；开卡类带 `remark`（开卡/复制卡/增加卡），读卡/退房类带 `action`
  （`ReadCard` / `HotelCheckOut`）——这是 PMS 认结果的凭证，**不能删除或合并**。

完整代码模式与示例见 `references/protocol.md` 和 `references/troubleshooting.md`。

## 四、门号映射（lock_room.txt）

PMS 下发的是**房门号**（如 `8201`），而制卡/开锁需要**锁编号**（如 `R1-2-1-0`），靠 `lock_room.txt` 映射：

```
R1-2-1-0=8201     ← 锁编号(楼-层-房-门) = 房门号
R1-2-2-0=8202
```

- 文件放 **exe 同目录**，**手动维护**，程序每次制卡前重新读取（改完即生效）。
- **生成规则**（从实测基准反推）：房号 `8{楼层}{房间号}`（如 8201 → 楼1、层2、房1、门0）。楼层=第 2 位数字，房间号=后两位去前导零，楼号固定 1、门号固定 0。
- **自适应逻辑**：PMS 下发锁编号格式（R 开头）直接用；下发房门号则查表转换；查不到则按规则补全并日志提示。
- 批量生成工具：`scripts/gen_lock_room.py`。

## 五、房号同步机制（PMS 点击 → 门锁软件）

### 触发时机

PMS 客户端点击某房间开卡 → 前端 emit `Door_lock_call`（批量入住为 `Door_lock_call_initiate`，多房间进 `Unlocking.list` 队列）→ Electron 主进程 spawn 对接 exe 并传参。

### 同步流程（对接程序侧）

1. 解析参数：URL 解码 → JSON（兼容**单对象 / 多房间数组 / `{data:[...]}` 包装**三种形态）。
2. 逐间任务：取 `room.roomno` 或 `order.room_no` → 判断格式 → 查 lock_room.txt 得锁编号。
3. 界面预填：顶部显示"正在开：8201房间"、失效时间自动填入（可改）、房号/楼层**不是人工填写**。
4. 操作员点【开新卡】→ `IssueData` 制卡 → 成功弹窗"XXX房间开卡成功" → 自动关闭该窗口 → 自动弹出下一间（多房间），全部完成自动退出。
5. 每间写记录文件 + stdout 输出，供 PMS 认结果。

### 异常处理

- **参数解析失败**：日志记录原始参数（URL编码 + 解码后），不崩溃；界面提示"未识别到有效指令"。
- **查表失败**：回退为按规则补全锁编号，日志提示映射缺失。
- **重复开卡冲突（多进程抢串口）**：采用**单实例 Mutex + 命名管道（方案B）**——新进程检测到已有实例时把参数通过管道转发给已运行实例后退出；已运行实例**自动更新房号/失效时间**（`Show+Activate+BringToFront`）。进程内用 `SdkLock` 串行化 SDK 调用，杜绝并发抢串口。
- **串口被占**：明确报错"打开串口失败…确认发卡机未被其他程序占用"。
- **Windows 前台锁定**：后台进程 `Activate/BringToFront` 可能只闪任务栏图标；如需强制前台需 `AttachThreadInput+SetForegroundWindow+模拟Alt`（注意：**用户可能不接受强制抢前台**，先确认需求再实现）。

## 六、部署与验证

1. **编译**：双击 `build.bat`（纯 ASCII 脚本，输出 x86 exe）；部署目录放 4 件套：`pms_interface.exe` + `NewICdll.dll` + `set.ini` + `lock_room.txt`（全部同目录）。
2. **替换原版**：备份原 exe，将新 exe 放入 `{PMS安装目录上级}/Yolosoft_9dpmsLock/`（路径必须与 PMS 配置一致）。
3. **模拟测试**（先于 PMS 实测）：cmd 中运行 `pms_interface.exe "<URL编码JSON>"`，验证界面弹出、房号预填、制卡成功、记录文件生成。
4. **实测验收**：PMS 点房间 → 界面自动带出房号 → 制卡 → 用锁厂读卡工具核对卡数据 → 门锁上刷卡（最终验证）。

## Resources

- `references/protocol.md` — PMS 门锁调用协议完整参考：指令 JSON 字段表、action 映射、记录文件格式、卡数据格式、返回码表。
- `references/troubleshooting.md` — 对接经验知识库：编译坑（bat 编码/if 括号块）、运行时坑、SDK 返回码语义、常见问题速查表。
- `scripts/gen_lock_room.py` — 从房号列表批量生成 lock_room.txt 映射表（含备份与规则校验）。

> 详细协议、代码片段与速查表分别见上述 reference 文件，SKILL.md 只保留流程骨架。
