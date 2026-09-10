---
name: agnes-imagegen
description: Generate images using the Agnes Image 2.1 Flash API (text-to-image and image-to-image). Use when the user asks to create, edit, or transform images via the agnes-image-2.1-flash model, including text-to-image, image-to-image, style transfer, poster design, social media assets, or any image generation task backed by Agnes AI.
---

# Agnes Image Generation

## Quick Reference

**Script path:** ``scripts/imagegen.py``
**API endpoint:** `https://apihub.agnes-ai.com/v1/images/generations`
**Model name:** `agnes-image-2.1-flash`
**Pricing:** $0.003 per image

## Prerequisites

- Python 3.7+ (uses only stdlib: urllib, base64, json, argparse)
- Set ``API_KEY`` in ``scripts/imagegen.py`` (line 10) before first use

## Workflows

### 1. Text-to-Image (URL output)

Generate an image from a text prompt and download the result:

```
python scripts/imagegen.py text2img -p "prompt" -s WIDTHxHEIGHT -o output.png
```

Examples:

```
python scripts/imagegen.py text2img -p "A luminous floating city above a misty canyon at sunrise, cinematic realism" -s 1024x768 -o city.png --format url
```

### 2. Text-to-Image (Base64 output)

Generate an image and receive Base64 data directly from the API:

```
python scripts/imagegen.py text2img -p "prompt" -o output.png --format base64
```

### 3. Image-to-Image (URL output)

Edit or transform an existing image by URL or Data URI:

```
python scripts/imagegen.py img2img -p "convert to cyberpunk style" -i "https://example.com/input.png" -o result.png
```

### 4. Image-to-Image (Base64 output)

```
python scripts/imagegen.py img2img -p "prompt" -i "image_url_or_data_uri" -o result.png --format base64
```

## Prompt Tips

For best results, include:
- **Subject** - main object or character
- **Scene/Environment** - background, setting
- **Visual Style** - e.g. cinematic realism, watercolor, pixel art
- **Lighting** - e.g. golden hour, neon, soft studio light
- **Camera Angle** - e.g. wide-angle, close-up, isometric
- **Composition** - e.g. balanced, rule of thirds, symmetrical
- **Detail Level** - e.g. ultra-detailed, high-information-density

For image-to-image, follow: [Modification] + [New Style/Scene] + [Add/Remove Elements] + [Preserve Elements]

## Common Issues

- response_format must go inside ``extra_body``, not at top level. The script handles this automatically.
- Do NOT pass ``tags: ["img2img"]``. The script does not include it.
- Input image URL must be public HTTPS. Use Data URI Base64 if not accessible.
- Timeout set to 360s by default.
