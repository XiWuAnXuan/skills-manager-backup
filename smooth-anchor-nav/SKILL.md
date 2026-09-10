---
name: smooth-anchor-nav
description: 实现带偏移补偿的平滑锚点导航（目录/TOC 点击平滑滚动到章节，标题不被固定页眉遮挡）。Use when 用户要求锚点平滑滚动、目录跳转动画、smooth scroll、TOC 导航，或抱怨锚点跳转"生硬/无动画/被 header 挡住"。
---

# 平滑锚点导航（带偏移补偿）

目标效果：点击目录/锚点链接 → 页面以缓动动画平滑滚动 → 目标标题停在固定页眉下方合适位置 → 地址栏 hash 同步更新。零服务器开销，纯客户端。

## 方案决策（先做这一步）

**默认选 JS（rAF + 缓动）方案**，不要先试 CSS 方案。原因：

- CSS `scroll-behavior: smooth` 受操作系统「减弱动态效果」设置支配——Windows 关闭「动画效果」或 macOS 开启「减弱动态效果」时**完全静默失效**，表现为生硬瞬跳。用户桌面环境很常见这样设置，排查成本高（本 skill 的诞生就是因为 CSS 方案在用户机器上无效被打回）。
- 原生 smooth 的时长和曲线不可控（Chrome 快而僵），做不出"高级感"的缓动。
- JS 方案逐帧驱动，只要脚本执行就一定有动画，时长/曲线完全可控。

CSS `scroll-behavior` 仅作为兜底追加（管 hash 直达和非 JS 路径），可包 `@media (prefers-reduced-motion: no-preference)`，但**不要**把它当主方案交付。

## 核心实现（框架无关）

```ts
const SCROLL_OFFSET = 80 // 落点距视口顶部的距离，必须 > 固定页眉高度
const SCROLL_DURATION_MS = 550

let activeScrollFrame = 0 // 模块级：新动画取消旧动画，连点不抖

function animateScrollTo(targetY: number) {
  cancelAnimationFrame(activeScrollFrame)
  const startY = window.scrollY
  const delta = targetY - startY
  if (Math.abs(delta) < 1) return

  const startTime = performance.now()
  // easeInOutCubic：起步与停靠都柔和
  const ease = (t: number) => (t < 0.5 ? 4 * t ** 3 : 1 - (-2 * t + 2) ** 3 / 2)

  const step = (now: number) => {
    const progress = Math.min(1, (now - startTime) / SCROLL_DURATION_MS)
    window.scrollTo(0, startY + delta * ease(progress))
    if (progress < 1) activeScrollFrame = requestAnimationFrame(step)
  }
  activeScrollFrame = requestAnimationFrame(step)
}
```

点击接线（React 示例；原生 JS 用事件委托同理）：

```tsx
const handleLinkClick = (id: string) => (event: React.MouseEvent<HTMLAnchorElement>) => {
  const heading = document.getElementById(id)
  if (!heading) return // 找不到目标 → 回退原生锚点跳转

  event.preventDefault()
  const targetY = heading.getBoundingClientRect().top + window.scrollY - SCROLL_OFFSET
  animateScrollTo(Math.max(0, targetY))
  window.history.pushState(null, '', `#${id}`) // hash 可分享，且不触发二次跳转
}
// <a href={`#${id}`} onClick={handleLinkClick(id)}>
```

保留 `href="#id"`：右键复制链接、SEO、无 JS 时的回退都靠它。

## 偏移补偿的两处一致性（易漏）

偏移量在两个地方出现，**数值必须一致**，否则"点目录到达的位置"和"打开带 hash 的链接到达的位置"不一样：

1. JS 落点：`targetY = 标题位置 - SCROLL_OFFSET`
2. CSS 兜底（hash 直达/浏览器前进后退）：标题元素上的 `scroll-margin-top`

例如页眉收缩后高 56px → 取 offset 80px（5rem），则 CSS 写 `h2, h3 { scroll-margin-top: 5rem }`，JS 写 `SCROLL_OFFSET = 80`。

若目录带滚动高亮（IntersectionObserver 触发线），保证 `SCROLL_OFFSET < 触发线位置`，这样点击到达后该章节必然被判定为激活态。

## 已知坑（都是实战踩过的）

- **系统动画设置**：见方案决策。用户说"没动画/很生硬"时第一嫌疑是 CSS 方案 + 系统关闭动画，换 JS 方案。
- **prettier-plugin-tailwindcss 会吃 className 模板字符串里的前导空格**：`` `base${cond ? '' : ' extra'}` `` 提交时被钩子格式化成 `'extra'`（空格丢失 → class 拼成无效的 `baseextra`），且每次手动修复都会被 pre-commit 再次删掉，症状是 lint-staged 反复报 "Prevented an empty git commit"。**规避：class 拼接一律用数组 `['base', cond ? '' : 'extra'].filter(Boolean).join(' ')`**。
- **滚动容器不是 window 的页面**：如果实际滚动发生在内层容器（`overflow-y: auto` 的 div），把 `window.scrollY / window.scrollTo` 换成该容器的 `scrollTop`，`getBoundingClientRect` 相对差值同理换算。先用 DevTools 确认谁在滚。
- **`pushState` 而非直接改 `location.hash`**：后者会触发原生跳转，和动画打架。

## 验收清单

- [ ] 点击目录任意章节：有明显的加速-减速平滑动画（约 0.5s）
- [ ] 到位后标题完整可见，不被固定页眉遮挡
- [ ] 地址栏 hash 已更新；复制该 URL 新开页面，落点与点击到达的一致
- [ ] 快速连点多个章节：动画切换干净，无抖动回弹
- [ ] 若有滚动高亮：到达后对应目录项处于激活态
