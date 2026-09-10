// ============================================================================
// MainForm_UI_Template.cs — 门锁制卡软件界面代码模板（云店掌门锁风格）
// ----------------------------------------------------------------------------
// 用法: 复制本文件到新门锁对接工程, 替换以下 TODO 业务部分即可:
//   1. LockSdk.*          → 换成新锁厂 DLL 的 P/Invoke 封装
//   2. PmsRoomTask/队列   → 按新 PMS 协议解析
//   3. 窗口标题 Text      → 换成新系统名
// 界面部分(BuildUi/ShowPicker/布局参数) 完全复用, 不要改动。
//
// 编译要求: csc /platform:x86 /codepage:65001（锁厂 DLL 为 32 位）
// 源码编码: UTF-8 with BOM（中文安全）
// ============================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DoorLock
{
    /// <summary>
    /// 门锁卡片管理主窗体（云店掌门锁风格三区块布局）
    /// 顶部: 正在开的房间大字; 中部: 卡片失效时间(掩码输入+日历); 底部: 读卡/开新卡/注销卡
    /// </summary>
    public class MainForm : Form
    {
        // SDK 调用串行化锁（防并发抢串口）
        private static readonly object SdkLock = new object();

        // ---- 控件 ----
        private Label lblRoom;              // 正在开: XXX房间 (顶部大字)
        private Label lblExpire;            // 卡片失效时间
        private MaskedTextBox mtbCheckOut;  // 失效时间(掩码输入, 大号)
        private Button btnPick;             // 日历选择按钮
        private Button btnRead;             // 读卡
        private Button btnIssue;            // 开新卡
        private Button btnCancel;           // 注销卡

        // ---- 任务队列(多房间自动连续开卡) ----
        // TODO: 替换为实际任务类型
        private readonly List<object> _queue = new List<object>();
        private int _queueIndex = -1;
        private string _lockNo;               // 锁编号(如 R1-2-1-0)
        private string _displayRoom;          // 显示房号(如 8201)

        // ---- 日志文件（界面不显示日志框, 写入统一文件） ----
        private static readonly object LogLock = new object();
        private static readonly string LogPath =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "开锁日志.log");

        public MainForm()
        {
            // TODO: 窗口标题换成新系统名
            Text = "云店掌门锁系统";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(480, 260);
            Font = new Font("Microsoft YaHei UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BuildUi();
            // 失效时间默认值: 明天 12:00
            mtbCheckOut.Text = DateTime.Now.AddDays(1).Date.AddHours(12).ToString("yyyy-MM-dd HH:mm");
            AppendLog("系统就绪, 等待开卡指令…");
        }

        // ================= 界面构建（核心模板, 勿改布局） =================
        private void BuildUi()
        {
            // ① 正在开的房间(顶部大字)
            lblRoom = new Label
            {
                Text = "等待开卡指令…",
                Location = new Point(12, 20),
                AutoSize = false,
                Size = new Size(456, 46),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 90, 160),      // 深蓝
                BackColor = Color.FromArgb(230, 241, 251),   // 浅蓝
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(lblRoom);

            // ② 失效时间: 标签 + 大号掩码输入框 + 日历按钮
            lblExpire = new Label
            {
                Text = "卡片失效时间",
                Location = new Point(12, 88),
                AutoSize = true,
                ForeColor = Color.DimGray
            };
            Controls.Add(lblExpire);

            mtbCheckOut = new MaskedTextBox
            {
                Mask = "0000-00-00 00:00",
                Location = new Point(12, 112),
                Size = new Size(410, 36),
                Font = new Font("Microsoft YaHei UI", 15F),
                TextAlign = HorizontalAlignment.Center
            };
            Controls.Add(mtbCheckOut);

            btnPick = new Button
            {
                Text = "📅",
                Location = new Point(428, 112),
                Size = new Size(40, 36),
                Font = new Font("Microsoft YaHei UI", 12F),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPick.Click += (s, e) => ShowPicker();
            Controls.Add(btnPick);

            // ③ 三个功能按钮(页面下方, 140x52 间距18)
            int btnW = 140, btnH = 52, gap = 18, startX = 12;
            btnRead = new Button
            {
                Text = "读卡",
                Location = new Point(startX, 178),
                Size = new Size(btnW, btnH),
                Font = new Font("Microsoft YaHei UI", 13F),
                BackColor = Color.FromArgb(96, 125, 139),    // 蓝灰
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRead.Click += (s, e) => RunSdk("读卡", DoRead);

            btnIssue = new Button
            {
                Text = "开新卡",
                Location = new Point(startX + btnW + gap, 178),
                Size = new Size(btnW, btnH),
                Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 120, 246),    // 亮蓝(主操作)
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnIssue.Click += (s, e) => RunSdk("开新卡", DoIssue);

            btnCancel = new Button
            {
                Text = "注销卡",
                Location = new Point(startX + (btnW + gap) * 2, 178),
                Size = new Size(btnW, btnH),
                Font = new Font("Microsoft YaHei UI", 13F),
                BackColor = Color.FromArgb(230, 126, 34),    // 橙
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => RunSdk("注销卡", DoCancel);

            Controls.AddRange(new Control[] { btnRead, btnIssue, btnCancel });
        }

        // ================= 日历弹窗（自定义, 不用 DateTimePicker 内嵌） =================
        private void ShowPicker()
        {
            DateTime t;
            if (!DateTime.TryParse(mtbCheckOut.Text, out t)) t = DateTime.Now;
            using (var dlg = new DatePickerDialog(t))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    mtbCheckOut.Text = dlg.SelectedTime.ToString("yyyy-MM-dd HH:mm");
            }
        }

        // ================= 任务加载 =================
        private void LoadTask(int idx)
        {
            if (idx < 0 || idx >= _queue.Count) return;
            _queueIndex = idx;
            // TODO: 取当前任务, 解析房号→锁编号(查映射表)→填界面
            // 例: _displayRoom = "8201"; _lockNo = "R1-2-1-0";
            string head = _queue.Count > 1 ? "(" + (_queueIndex + 1) + "/" + _queue.Count + ")" : "";
            lblRoom.Text = "正在开：" + _displayRoom + "房间" + head;
            lblRoom.BackColor = Color.FromArgb(230, 241, 251);
        }

        // ================= 业务方法（TODO: 换成新锁厂 SDK） =================
        private void DoRead()
        {
            // TODO: 调用锁厂读卡 API; 返回码 7(新卡) 是正常状态, 提示"这是一张新卡（空白卡）"
            // 成功 → ShowMsg("读卡成功", "所属房间：...\n卡型：...\n起始/失效时间")
        }

        private void DoIssue()
        {
            // TODO: 组装卡数据 "T0|R{楼}-{层}-{房}-{门}|D{yyMMddHHmm}|O{yyMMddHHmm}|L0"
            //       调用锁厂 IssueData; 成功 → ShowMsg("开卡成功", "XXX房间开卡成功")
            //       → Close() 自动关窗口(Program 层循环弹下一间); 失败 → ShowMsg 明确原因
        }

        private void DoCancel()
        {
            // TODO: 调用锁厂 CancelCard; 新卡提示"该卡本来就是新卡，无需注销"
        }

        // ================= 通用辅助（勿改） =================
        /// <summary>后台线程安全执行 SDK 操作(串行化 + 日志)</summary>
        private void RunSdk(string op, Action action)
        {
            new Thread(() =>
            {
                try
                {
                    lock (SdkLock) { action(); }
                }
                catch (Exception ex)
                {
                    ShowMsg(op + "失败", ex.Message, MessageBoxIcon.Error);
                }
            }) { IsBackground = true }.Start();
        }

        private void ShowMsg(string title, string msg, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            if (InvokeRequired) { try { BeginInvoke(new Action(() => ShowMsg(title, msg, icon))); } catch { } return; }
            MessageBox.Show(this, msg, title, MessageBoxButtons.OK, icon);
        }

        /// <summary>统一日志: 写入 {exe目录}/开锁日志.log (一行一条, 线程安全)</summary>
        private static void AppendLog(string msg)
        {
            try
            {
                lock (LogLock)
                {
                    System.IO.File.AppendAllText(LogPath,
                        "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + msg + "\r\n",
                        Encoding.UTF8);
                }
            }
            catch { }
        }
    }

    /// <summary>自定义日历弹窗: MonthCalendar 选日期 + 时/分选择 + 确定/取消</summary>
    public class DatePickerDialog : Form
    {
        private readonly MonthCalendar _cal;
        private readonly NumericUpDown _numHour, _numMin;
        public DateTime SelectedTime { get; private set; }

        public DatePickerDialog(DateTime initial)
        {
            Text = "选择卡片失效时间";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(280, 320);

            _cal = new MonthCalendar { Location = new Point(12, 12), MaxSelectionCount = 1 };
            _cal.SetDate(initial);
            Controls.Add(_cal);

            var lblT = new Label { Text = "时间:", Location = new Point(18, 208), AutoSize = true };
            Controls.Add(lblT);
            _numHour = new NumericUpDown { Location = new Point(70, 204), Size = new Size(60, 24), Minimum = 0, Maximum = 23, Value = initial.Hour };
            _numMin = new NumericUpDown { Location = new Point(138, 204), Size = new Size(60, 24), Minimum = 0, Maximum = 59, Value = initial.Minute };
            Controls.Add(_numHour);
            Controls.Add(_numMin);

            var btnOk = new Button { Text = "确定", Location = new Point(56, 250), Size = new Size(80, 32), DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "取消", Location = new Point(148, 250), Size = new Size(80, 32), DialogResult = DialogResult.Cancel };
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            AcceptButton = btnOk; CancelButton = btnCancel;

            FormClosing += (s, e) =>
            {
                if (DialogResult == DialogResult.OK)
                    SelectedTime = _cal.SelectionStart.Date
                        .AddHours((double)_numHour.Value)
                        .AddMinutes((double)_numMin.Value);
            };
        }
    }
}
