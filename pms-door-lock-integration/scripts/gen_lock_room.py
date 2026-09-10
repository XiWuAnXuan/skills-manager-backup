#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
gen_lock_room.py — 批量生成 lock_room.txt 门锁映射表

PMS 门锁对接中，lock_room.txt 每行格式: R{楼}-{层}-{房}-{门}={房门号}
本脚本按 8xxx 房号规则生成:
    锁编号 = R1-{房号第2位}-{房号后两位去前导零}-0

用法:
    python gen_lock_room.py <房号列表文件> [输出文件]
    房号列表文件: 每行一个房号, 如 8201/8202/... 或已含 "=" 的行(原样保留)

示例:
    python gen_lock_room.py rooms.txt                 # 输出 lock_room.txt
    python gen_lock_room.py rooms.txt out.txt         # 指定输出
    或直接:
    python gen_lock_room.py --inline 8201 8202 8203   # 命令行直接给房号

规则可配置(通过环境变量):
    LOCK_BUILDING=1   # 楼号, 默认 1
    LOCK_DOOR=0       # 门号, 默认 0
    LOCK_PREFIX=R     # 前缀, 默认 R
"""

import os
import shutil
import sys
import datetime


def gen_lock_no(room: str, building: str = "1", door: str = "0", prefix: str = "R"):
    """按 8xxx 房号规则生成锁编号; 无法识别返回 None"""
    s = room.strip()
    if len(s) == 4 and s[0] == "8" and s[1:].isdigit():
        floor = s[1]                # 房号第2位 = 楼层
        room_no = str(int(s[2:]))   # 房号后两位去前导零 = 房间号
        return f"{prefix}{building}-{floor}-{room_no}-{door}"
    return None


def process(rooms_path: str, out_path: str) -> int:
    """读取房号列表, 生成映射表; 返回新生成条数"""
    lines = open(rooms_path, "r", encoding="utf-8", errors="ignore").read().splitlines()
    building = os.environ.get("LOCK_BUILDING", "1")
    door = os.environ.get("LOCK_DOOR", "0")
    prefix = os.environ.get("LOCK_PREFIX", "R")

    out, bad, generated = [], [], 0
    for i, raw in enumerate(lines):
        line = raw.strip()
        if not line:
            out.append(raw)
            continue
        if "=" in line:            # 已含映射的行原样保留
            out.append(line)
            continue
        lock = gen_lock_no(line, building, door, prefix)
        if lock:
            out.append(f"{lock}={line}")
            generated += 1
        else:
            bad.append((i + 1, line))
            out.append(raw)

    # 写回 (CRLF, 与 Windows 记事本兼容)
    content = "\r\n".join(out) + "\r\n"
    with open(out_path, "w", encoding="utf-8", newline="") as f:
        f.write(content)
    return generated, bad


def main():
    # 用法1: --inline 直接给房号
    if len(sys.argv) >= 3 and sys.argv[1] == "--inline":
        rooms = sys.argv[2:]
        out_path = "lock_room.txt"
        tmp = "__rooms_tmp.txt"
        with open(tmp, "w", encoding="utf-8") as f:
            f.write("\n".join(rooms))
        gen, bad = process(tmp, out_path)
        os.remove(tmp)
    else:
        if len(sys.argv) < 2:
            print(__doc__)
            sys.exit(1)
        rooms_path = sys.argv[1]
        out_path = sys.argv[2] if len(sys.argv) > 2 else "lock_room.txt"

        # 备份已有输出
        if os.path.exists(out_path):
            bak = f"{out_path}.bak-{datetime.date.today().strftime('%Y%m%d')}"
            shutil.copy2(out_path, bak)
            print(f"[备份] {out_path} -> {bak}")

        gen, bad = process(rooms_path, out_path)

    print(f"[完成] 已生成 {gen} 条映射 -> {out_path}")
    if bad:
        print(f"[警告] {len(bad)} 行无法识别(原样保留): {bad[:10]}")
    else:
        print("[OK] 全部行识别成功")


if __name__ == "__main__":
    main()
