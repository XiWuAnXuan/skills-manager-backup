# PMS 门锁调用协议完整参考

> 本文档基于对"云店掌 SaaS PMS（Electron）"与"汉庭/华住 MF-NK 1扇区门锁"对接项目的实战逆向与实测，可作为同类 PMS 门锁对接的协议蓝本。不同 PMS 字段名可能有差异，但调用模式（spawn exe + URL编码JSON + 记录文件回报）高度通用。

## 1. 调用模式总览

```
PMS 前端(Electron 渲染进程)
  └─ $overallNews.$emit("Door_lock_call", lockData)     # 单间
  └─ $overallNews.$emit("Door_lock_call_initiate", ...) # 批量入住(ruzhu)
        ↓
PMS 主进程 background.js
  └─ spawn("{上级}/Yolosoft_9dpmsLock/pms_interface.exe",
           [encodeURIComponent(JSON.stringify(lockData))])
        ↓
pms_interface.exe（对接程序）
  ├─ 解析 JSON → 按 action 调用锁厂 DLL（IssueData/CancelCard/ReadData）
  ├─ 写记录文件 {exe目录}/{yyyyMMdd}/{fileName}.txt（GBK 编码 JSON）
  └─ stdout 输出结果 JSON
        ↓
PMS 轮询读取记录文件 → 识别开卡/退房/读卡结果
```

**关键约定**：
- 部署路径：`{PMS安装目录上级}/Yolosoft_9dpmsLock/pms_interface.exe`（PMS 代码中硬编码的相对路径，必须一致）。
- 门锁模块文件由 PMS 后台从服务器下载：`pms_interface.exe` + `pms_interface.txt`（校验文件）。
- 结果回收靠**记录文件**（GBK JSON），stdout 仅为辅助。

## 2. 指令 JSON 结构（PMS → exe）

```json
{
  "lock": {
    "lockmodel": "锁型号(后台配置, 如 XinShiJuMenJin2024)",
    "lockno": "锁号"
  },
  "room": {
    "lockno": "锁号",
    "roomno": "1-3-11-0 或 8201",   // 房号(锁编号格式或房门号, 两种都可能!)
    "endtime": "2026-08-07 12:00:00"
  },
  "order": {
    "order_id": "2000000017",
    "lock_type": "锁类型",
    "room_id": "房间ID",
    "room_no": "房号",
    "start_time": "2026-08-06 14:00:00",
    "end_time": "2026-08-07 12:00:00",
    "action": "开卡|复制卡|增加卡|HotelCheckOut|ReadCard"
  },
  "fileName": "UID"   // 结果文件名的唯一标识
}
```

**兼容性提示**：实际下发格式多变——可能是单对象、多房间数组 `[...]`、或 `{data:[...]}` 包装；数组元素可能是嵌套 `room/order` 也可能是扁平字段（`room_no`/`out_time`/`order_id`/`lockAction`）。解析器需兼容三种形态。

## 3. action 与动作映射

| action / remark | 执行 | 说明 |
|---|---|---|
| `开卡` / `复制卡` / `增加卡` / 空 | `IssueData` 制卡 | 房号取 `room.roomno` 或 `order.room_no`，时间取 `order.start_time/end_time` |
| `HotelCheckOut` | `CancelCard` 退房注销 | 需卡在读卡器上 |
| `ReadCard` | `ReadData` 读卡 | 输出卡数据 |
| 无房间号但有 action | 打开"请进行XX操作"界面 | 读卡/注销类指令，无制卡任务 |

## 4. 卡数据格式（汉庭 MF-NK 1扇区）

```
T0|R1-2-1-0|D2608061550|O2608071400|L0|F
│  │         │          │          │└─ F=挂失位(正常)
│  │         │          │          └─ L0=挂失
│  │         │          └─ O=结束时间(yyMMddHHmm)
│  │         └─ D=起始时间(yyMMddHHmm)
│  └─ R=锁编号(楼-层-房-门)
└─ T=卡型(0=客人卡)
```

- **R 字段的"1-2-1-0"是锁编号**（楼-层-房-门），不是房门号 8201——这就是需要 lock_room.txt 映射的原因。
- 实测读卡返回的完整数据带 `|F` 后缀（挂失位），写入时可不带，读取解析时兼容。
- 时间格式统一 `yyMMddHHmm`（两位年）。

## 5. 记录文件格式（exe → PMS 回报）

- 路径：`{exe目录}/{yyyyMMdd}/{fileName}.txt`（fileName 来自指令，缺省用 order_id 或自动生成 UID）。
- 编码：**GBK**（必须，PMS 按 GBK 读）。
- 内容：JSON，字段至少含：

```json
{
  "order_id": "2000000017",
  "action": "开卡",
  "code": "00",
  "message": "成功",
  "card_no": "8BFEB23B",
  "card_data": "T0|R1-2-1-0|D2608061550|O2608071400|L0"
}
```

- **这是 PMS 确认"这间房开卡成功"的凭证，绝对不要删除或合并这些文件**；如需人工日志，另建统一日志文件（如 `开锁日志.log`，一行一条追加）。

## 6. lock_room.txt 门号映射

```
R1-2-1-0=8201    # 锁编号 = 房门号
R1-2-2-0=8202
```

- 位置：exe 同目录；**手动维护**；每次制卡前重新读取。
- 用途：PMS 下发房门号(8201) → 查表转锁编号(R1-2-1-0) 才能正确制卡/开门。
- 生成规则（8xxx 房号实测反推）：`锁编号 = R1-{楼层}-{房间号}-0`，楼层=房号第 2 位数字，房间号=房号后两位去前导零，楼号固定 1、门号固定 0。

## 7. 锁厂 DLL 接口（汉庭 NewICdll.dll 实测）

| 函数 | 签名 | 说明 |
|---|---|---|
| `OpenPort` | `int OpenPort()` | 打开串口（读 set.ini 的 com） |
| `IssueData` | `int IssueData(char* CData, char* CNum, int nBeepFlag)` | **制卡（写卡）** |
| `ReadData` | `int ReadData(char* CardData, char* CNum, int nBeepFlag)` | 读卡 |
| `CancelCard` | `int CancelCard(char* CNum, int nBeepFlag)` | 注销卡 |
| `GetCardNo` | `int GetCardNo(char* CardNo, int nBeepFlag)` | 获取卡号 |
| `BeepNow` | `int BeepNow(int nBeepFlag)` | 蜂鸣测试 |
| `ClosePort` | `int ClosePort()` | 关闭串口 |
| `RecoverLicense` | `int RecoverLicense()` | 恢复授权 |

- 调用约定：**StdCall**；32 位 DLL → 编译目标必须 x86。
- 配置 `set.ini`：`com=串口号`、`Pwd=密钥`、`section=扇区`。
- SDK 返回码语义见 `troubleshooting.md`。

## 8. 逆向提取要点（快速复现）

1. 安装包识别：`file xxx.exe` 或头部 16 字节（NSIS 特征 `NullsoftInst`）。
2. NSIS 内嵌 7z：`7z l xxx.exe` 可见 `$PLUGINSDIR\app-32.7z`；**提取失败时**用 Python 在安装包二进制中搜索 7z 魔数 `b'7z\xbc\xaf\x27\x1c'`，取最后一个偏移截取（7z 子文件是未压缩存储，可直接截取）。
3. asar 解析（新版格式）：
   ```python
   import struct, json
   data = open('app.asar','rb').read()
   json_size = struct.unpack('<I', data[12:16])[0]
   header = json.loads(data[16:16+json_size])
   data_start = 16 + json_size
   if data_start % 4: data_start += 4 - (data_start % 4)
   # header['files'] 树形结构, offset/size 是字符串需 int()
   ```
4. 关键词搜索：`pms_interface`、`Door_lock_call`、`门锁`、`发卡`、`lockmodel`、`spawn`。
5. 交叉验证：在**原 exe 二进制**中搜索协议键名（`lockno/roomno/endtime/order_id/action/fileName`，UTF-16LE 编码），命中即确认协议闭环。
