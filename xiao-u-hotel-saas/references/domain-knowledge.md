# Xiao U Hotel SaaS Domain Knowledge

## Core Product Concepts

- 小 U 酒店 / 云店掌 is described as a hotel SaaS management system covering PMS front desk work, POS/restaurant ordering, housekeeping, finance reports, guest mini-programs, owner/manager mobile tools, permissions, invoices, and external interfaces.
- Main user roles: 前台, 酒店管理人员/老板, 财务, 房务人员, 服务人员/工程人员, 实施人员, 客人.
- Common customer types: 散客, 团队, 会员, 协议客户, OTA/平台客人, 中介.

## PMS Front Desk Flow

### 房态查看

- The room-status page is the first daily entry point. Each room card represents one room.
- Typical colors: green = occupied, light blue = available clean/empty, dark blue = dirty room, yellow = reserved, red = maintenance/locked.
- Operators should inspect the current room state before check-in, checkout, cleaning, or room changes.
- Room-state color, card size, and text size can be adjusted in 房态设置.

### 入住办理

- Start from an available room on the room-status page, usually by double-clicking the room.
- The check-in page has available rooms on the left and selected rooms on the right.
- Register identity through 二代证 reader when available; otherwise manually enter name, ID, and phone.
- For multi-room check-in, select rooms, enter each guest, save to archive, switch rooms, then confirm total room price/deposit.
- Always verify guest identity, room number, room price, deposit amount, and payment method before clicking 入住/入住并打印.
- After check-in, the system may open card issuing and print check-in documents.

### Customer-Type Variants

- 协议/OTA: switch customer type to 协议/平台, select the agreement unit/platform, keep room price aligned with platform or contract bottom price, enter channel order number when needed.
- 团队: confirm team name, dates, room count/type, price, and payment method; then process similarly to multi-room check-in.
- 会员: search by phone, select member card/account, check member balance, optionally use 会员担保/会员余额.

## Reservation Flow

- Reservation types: 房号预定 and 房型预定.
- Before reserving, confirm whether it is daily rental or hourly, arrival/departure time, guest name, phone, room type/count, price, and customer type.
- For room assignment, use 预定客单/今日预定, inspect order details, choose manual room assignment or automatic allocation.
- Reservation-to-check-in: open the reservation, complete guest identity info, supplement deposit if needed, then convert to check-in and issue card.
- Cancellation: open the reservation order and cancel the room. If deposit was paid, refund deposit first.

## In-Stay Service

- Change room by dragging room card or right-clicking the occupied room and choosing 宾客换房. Confirm new room and price.
- Change price through 房价计划; permission may be required, otherwise apply for price change.
- Supplement deposit from the room's right-side action area; choose payment method and amount.
- Modify guest info through 客人信息修改; can also add co-staying guests.
- Add room from an existing occupied room through 新开客房.
- Merge rooms through 并入此群; verify target room/group.
- Extend stay through 客房续住/客房延期; confirm departure time.

## Checkout and Finance

- Before checkout, verify room fee, item consumption, deposit, paid amount,欠款, refund, and settlement method.
- Add room consumption from 宾客消费 or 商品消费及赔偿. Double-click product to add quantity; double-click existing item to cancel, modify, or transfer.
- 散客退房: enter checkout page, check欠款, supplement deposit/payment when needed, then 结账退房.
- 会员退房: similar to normal checkout, but may use member balance/会员担保.
- 协议/OTA退房: use agreement posting/挂账, verify agreement unit and amount. OTA/platform guests may not need on-site payment if already paid to platform.
- 独立退房: room checks out independently; remaining group bill is paid by the last checkout room.
- 退房不结账 freezes/unsettles the order; later find it under 退房未结 to complete settlement.
- Finance distinction: before checkout, deposit and payments appear differently from after checkout/refund. Explain "未退房" and "已退房" separately when documenting reports.

## Housekeeping Flow

- After checkout, room becomes dirty.
- Housekeeping mini-program can receive cleaning reminders.
- Flow: dirty room -> cleaner accepts -> cleaning in progress -> cleaner completes -> supervisor checks -> clean room.
- Emphasize that this reduces front desk manual intervention.

## Reports

- 营业日报 path: 报表 -> 今日报表 -> 营业日报.
- 营业日报 includes daily operating amount and month-to-date amount. Sources include room deposit/payment and non-room consumption.
- 结算统计日报 path: 报表 -> 今日报表 -> 结算统计日报.
- Settlement report groups room revenue, non-room revenue, payment-method settlement amounts, receipts, refunds, and balances.

## Restaurant/POS Flow

- Table/room boxes represent tables or private rooms.
- Double-click table -> open table -> ordering page.
- Filter dishes by top categories; double-click dishes to add to pending order.
- Pending dishes can be gifted, deleted, repriced, or given item-specific notes.
- Whole-order notes apply to all dishes, e.g. 少油少盐.
- 等叫 tells kitchen to prepare but not cook immediately.
- 下单 sends order to kitchen. After ordering, support gifted dish, urge dish, and return dish.
- Settlement supports scan payment, room-account posting, 商旅 AR posting, discounts, rounding, cash demonstration.
- Same-day orders can be reverse-settled before night audit; after night audit it is not recommended.

## Implementation Checklist

- Collect hotel go-live preparation data.
- Import room types and room prices in 酒店 -> 基础数据 -> 房型房价.
- Add product/service item such as 增免房费.
- Configure price-change permissions.
- Configure room price strategy.
- Add member levels in group backend.
- Add default/unknown values to data dictionary where needed.
- Configure payment methods: 担保押金付, 扫码付款, 商旅企业挂账, 银行卡刷卡付; disable unsuitable guarantee/pre-auth options if required.
- Configure PMS public settings.
- Register merchant users by phone and bind customer scan code.
- Grant appropriate permissions after user creation.

## Training and Explanation Voice

- Start with the main flow: 看房态 -> 办入住 -> 住中服务 -> 退房结算 -> 房务打扫 -> 数据管理/运营扩展.
- Use simple spoken explanations: "很简单", "跟着这个思路", "前台先确认", but keep final documents professional.
- Insert practice prompts after training sections, e.g. "大家实操练习一下".
- For demos, use concrete amounts and rooms, e.g. room fee 488 + 100 deposit; paid amount must cover bill before checkout.
