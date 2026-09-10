import argparse
import base64
import json
import os
import sys
import urllib.error
import urllib.request

# ============================================================
# 占位 API Key，使用前请替换为你的真实密钥
# ============================================================
API_KEY = "sk-fVfaaGPm4GgeMj2x37ZgilqZPwbTGuMtMvZah3FK7e6FjJql"

BASE_URL = "https://apihub.agnes-ai.com"
API_ENDPOINT = f"{BASE_URL}/v1/images/generations"
MODEL = "agnes-image-2.1-flash"


def _build_request(payload):
    """构建 HTTP POST 请求对象"""
    data = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(
        API_ENDPOINT,
        data=data,
        headers={
            "Authorization": f"Bearer {API_KEY}",
            "Content-Type": "application/json",
        },
        method="POST",
    )
    return req


def _parse_args():
    p = argparse.ArgumentParser(description="使用 Agnes Image 2.1 Flash 生成图片")
    sub = p.add_subparsers(dest="mode", required=True)

    # --- text2img ---
    t2p = sub.add_parser("text2img", help="文生图")
    t2p.add_argument("-p", "--prompt", required=True, help="提示词")
    t2p.add_argument(
        "-s", "--size", default="1024x768", help="图片尺寸 (默认 1024x768)"
    )
    t2p.add_argument("-o", "--output", required=True, help="输出图片文件路径")
    t2p.add_argument(
        "--format",
        choices=["url", "base64"],
        default="url",
        help="获取图片的方式: url (先获取链接再下载) 或 base64 (直接返回数据)",
    )

    # --- img2img ---
    i2p = sub.add_parser("img2img", help="图生图")
    i2p.add_argument("-p", "--prompt", required=True, help="编辑提示词")
    i2p.add_argument(
        "-i", "--image", required=True, help="输入图片 (公网 URL 或 data URI)"
    )
    i2p.add_argument(
        "-s", "--size", default="1024x768", help="图片尺寸 (默认 1024x768)"
    )
    i2p.add_argument("-o", "--output", required=True, help="输出图片文件路径")
    i2p.add_argument(
        "--format",
        choices=["url", "base64"],
        default="url",
        help="输出格式",
    )

    return p.parse_args()


def _handle_response(resp, fmt, output_path):
    """解析 API 响应并保存图片"""
    body = json.loads(resp.read().decode("utf-8"))
    if "error" in body:
        print(
            f"[错误] {json.dumps(body['error'], ensure_ascii=False)}", file=sys.stderr
        )
        sys.exit(1)

    data = body.get("data", [])
    if not data:
        print("[错误] 响应中没有 data 字段", file=sys.stderr)
        sys.exit(1)

    item = data[0]

    if fmt == "url":
        image_url = item.get("url")
        if not image_url:
            print("[错误] URL 模式下响应缺少 url 字段", file=sys.stderr)
            sys.exit(1)
        print(f"[信息] 正在从 URL 下载: {image_url}")
        img_data = urllib.request.urlopen(image_url).read()
    else:
        b64 = item.get("b64_json")
        if not b64:
            print("[错误] Base64 模式下响应缺少 b64_json 字段", file=sys.stderr)
            sys.exit(1)
        img_data = base64.b64decode(b64)

    with open(output_path, "wb") as f:
        f.write(img_data)
    print(f"[完成] 图片已保存到: {output_path}")


def main():
    args = _parse_args()

    if args.mode == "text2img":
        payload = {
            "model": MODEL,
            "prompt": args.prompt,
            "size": args.size,
        }
        if args.format == "url":
            payload["extra_body"] = {"response_format": "url"}
        else:
            payload["return_base64"] = True

    elif args.mode == "img2img":
        payload = {
            "model": MODEL,
            "prompt": args.prompt,
            "size": args.size,
            "image": [args.image],
        }
        if args.format == "url":
            payload["extra_body"] = {"response_format": "url"}
        else:
            payload["extra_body"] = {"response_format": "b64_json"}

    req = _build_request(payload)
    try:
        with urllib.request.urlopen(req, timeout=360) as resp:
            _handle_response(resp, args.format, args.output)
    except urllib.error.HTTPError as e:
        err_body = e.read().decode("utf-8", errors="replace")
        print(f"[HTTP {e.code}] {err_body}", file=sys.stderr)
        sys.exit(1)
    except Exception as e:
        print(f"[错误] {e}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
