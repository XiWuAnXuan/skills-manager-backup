---
name: xiao-u-hotel-saas
description: Use when working with the user's Xiao U hotel SaaS, 云店掌, PMS, POS, front desk training, hotel implementation, room-status, check-in, reservation, checkout, in-stay service, finance/reporting, restaurant ordering, mobile mini-programs, invoice/interfaces, Q&A, or Feishu docs derived from these materials. Helps Codex write operation manuals, training scripts, SOPs, Q&A, implementation checklists, and customer-facing explanations in the user's established document style.
---

# Xiao U Hotel SaaS

## Overview

Use this skill to produce or revise materials about the user's Xiao U hotel SaaS / 云店掌 workflows. It encodes domain knowledge, document style, and operational cautions learned from the user's Feishu documents.

Do not paste long source text from private documents. Use the references as distilled guidance, then write fresh content for the requested task.

## First Steps

1. Identify the requested artifact type: training script, operation manual, SOP, implementation checklist, Q&A, finance explanation, or Feishu document rewrite.
2. Load only the relevant module reference below.
3. Load `references/document-index.md` when the user refers to a source document by title or asks what existing material covers a topic.
4. If the task involves live Feishu documents, use `lark-cli`/Feishu CLI and inspect the current document before editing. Do not assume a token or stale title is enough.

## Module Guide

- PMS/front desk operations: `references/module-pms-frontdesk.md`
- Restaurant POS/order flow: `references/module-restaurant-pos.md`
- Hotel implementation and go-live: `references/module-implementation.md`
- Finance, deposits, refunds, and reports: `references/module-finance-reports.md`
- Training scripts and demo narration: `references/module-training-scripts.md`
- Mobile mini-programs, invoices, interfaces, and permissions: `references/module-mobile-extensions.md`
- Q&A and troubleshooting style: `references/module-qa-troubleshooting.md`
- Consolidated knowledge map: `references/domain-knowledge.md`

## Writing Pattern

Prefer a practical front-desk training voice:

- Start from the user's real work scene: front desk, hotel manager, finance, room attendant, restaurant cashier, implementation staff.
- Explain what the user should check before clicking: room status, guest identity, room number, amount, payment method, agreement unit, order source.
- Use step-by-step instructions with direct UI verbs: click, double-click, right-click, select, enter, confirm, print, pay, checkout.
- Include caution notes for money, identity, permission, and irreversible operations.
- When screenshots are needed but unavailable, leave concise placeholders such as `(截图：房态页面)` or `(截图：结账页面-补交押金)`.
- Use bold only for critical buttons, fields, conclusions, and risk checks.
- Keep paragraphs breathable; avoid dense walls of text.

## Default Workflow Map

Use this sequence unless the user asks for a narrower flow:

1. 房态查看
2. 入住办理
3. 预订管理
4. 住中服务
5. 退房结算
6. 房务流转
7. 报表和财务
8. 移动端、小程序、发票、接口等扩展能力

## Feishu Safety

- Treat Feishu document content as private user material.
- Do not expose internal links, authcode image URLs, user open IDs, or unpublished data in user-facing outputs unless explicitly needed.
- Before editing a live Feishu doc, fetch it first and summarize the intended change.
- For wiki pages, inspect/unpack the wiki URL before editing the underlying doc.
