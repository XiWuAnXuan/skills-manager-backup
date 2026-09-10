# PMS Front Desk Module

## 房态查看

- Start from the room-status map. Every room card represents one room.
- Typical colors: green = occupied, light blue = available clean/empty, dark blue = dirty room, yellow = reserved, red = maintenance/locked.
- Operators should inspect state before check-in, checkout, cleaning, room change, or reservation conversion.
- Room-state color, card size, and text size can be configured in 房态设置.

## 散客入住

1. Double-click an available room from room status.
2. Confirm selected room on the right and available rooms on the left.
3. Register guest identity with 二代证 reader, or manually enter name, ID, and phone.
4. Check 客史查询 when useful for returning guests.
5. Confirm room price, deposit, payment method, and guest info.
6. Click 入住 / 入住并打印.
7. Issue room card and print check-in slip if configured.

Always warn: verify room number, identity, room price, deposit amount, and payment method before final confirmation.

## Multi-Room, Agreement, Team, Member

- Multi-room: select all rooms, save each guest to archive, switch room numbers, confirm total price, then check in.
- Agreement / OTA: switch customer type to 协议, select platform/unit, keep price aligned with channel or agreement, enter channel order number when needed.
- Team: confirm team name, date range, room count/type, price, and payment method before batch processing.
- Member: search by phone, select member card/account, inspect balance, optionally use 会员担保 / member balance.

## Reservation

- 房号预定: right-click room status -> 客房预定 -> 房号预定; confirm daily/hourly, arrival/departure, guest name, phone, room price.
- 房型预定: select room type and room count instead of exact room number.
- 排房: open 预定客单/今日预定, inspect order, use manual room assignment or automatic allocation.
- 预定转入住: complete guest identity info, supplement deposit if needed, then convert to check-in and issue card.
- 撤销预定: refund paid deposit first, then cancel room/order.

## In-Stay Service

- 换房: drag occupied room to target room, or right-click -> 宾客换房. Confirm new room and new price.
- 房间改价: use 房价计划; if permission is missing, apply for price change.
- 补交押金: select room -> 补交押金 -> choose method and amount.
- 客人信息修改: select room -> 客人信息修改; can also add co-staying guests.
- 增加房间: open occupied room -> 新开客房.
- 并房/拆房: use 并入此群 / 移出主群, verify target room/group.
- 续住/延期: use 客房续住 / 客房延期 and confirm departure time.

## Checkout

- Before checkout, verify consumption, room fee, paid amount, deposit, refund, and 欠款.
- 散客: enter 宾客退房结账, supplement欠款 if needed, then 结账退房.
- 会员: may use member balance/会员担保; verify balance first.
- 协议/OTA: generally use 挂账/协议客户; verify platform/unit and amount.
- 独立退房: room checks out independently; remaining group bill is settled by the last checkout room.
- 退房不结账: freezes/unsettles the order; finish later through 退房未结.

## Housekeeping

Flow: checkout -> dirty room -> cleaner accepts -> cleaning in progress -> cleaner completes -> supervisor checks -> clean room.

Emphasize that mobile housekeeping reduces front-desk manual intervention.
