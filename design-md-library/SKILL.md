---
name: design-md-library
description: Local library of 70+ DESIGN.md design-system specs extracted from real top-tier products (Apple, Claude, Linear, Vercel, Stripe, Tesla, Nike, Notion, Figma...). Use when the user wants a website "in the style of" a named brand or product, or asks to replicate/reference a real product's design language, palette, typography, or tokens. Read the brand's DESIGN.md and build strictly to that spec.
---

# DESIGN.md 品牌风格库

本地库根目录：`C:\Users\35325\.claude\design-library\awesome-design-md\design-md\<brand>\DESIGN.md`
（每个品牌目录还有一个 `README.md`，是该站风格的简要介绍；来源：VoltAgent/awesome-design-md）

## 用法

1. 用户点名某个品牌/产品风格时（例如"做成 Claude 官网那种感觉"、"参考 Linear 的风格"），先用 Read 工具读取对应品牌目录下的 `DESIGN.md`。
2. 把其中的设计 token（颜色、字体、间距、圆角、组件规则、动效）当作项目的**硬性设计规范**来执行，不要只"参考大意"。
3. 推荐与 `design-taste-frontend` 技能叠加使用：DESIGN.md 提供视觉语言，taste 技能提供反 AI 味硬规则与 Pre-Flight 检查清单。
4. 用户没点名品牌但描述了气质时，从下表挑**最接近的一个**读取。一个项目只用一份 DESIGN.md，禁止混用多个品牌。
5. 若品牌不在库中，不要臆造，告知用户并改用 design-taste-frontend 的常规流程。

## 可用品牌目录名（按字母序）

airbnb, airtable, apple, binance, bmw, bmw-m, bugatti, cal, claude, clay,
clickhouse, cohere, coinbase, composio, cursor, dell-1996, elevenlabs, expo,
ferrari, figma, framer, hashicorp, hp, ibm, intercom, kraken, lamborghini,
linear.app, lovable, mastercard, meta, minimax, mintlify, miro, mistral.ai,
mongodb, nike, nintendo-2001, notion, nvidia, ollama, opencode.ai, pinterest,
playstation, posthog, raycast, renault, replicate, resend, revolut, runwayml,
sanity, sentry, shopify, slack, spacex, spotify, starbucks, stripe, supabase,
superhuman, tesla, theverge, together.ai, uber, vercel, vodafone, voltagent,
warp, webflow, wired, wise, x.ai, zapier
