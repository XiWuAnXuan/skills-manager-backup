# Restaurant POS Module

## Open Table and Order

1. Each box represents a table or private room.
2. Double-click table -> 开台 -> ordering page.
3. Use top categories to filter dishes.
4. Double-click dish to add it as 未下单.

## Item and Order Operations

- Item-level operations: 赠, 删, 改价, 菜品要求.
- Whole-order request: use 整单要求, e.g. 少油少盐.
- 等叫 means kitchen can prepare materials but should not cook immediately.
- 下单 sends the order to kitchen.

## After Sending Order

- 赠菜: available if the dish was configured as giftable.
- 催菜: kitchen receives urge reminder.
- 退菜: remove/cancel a dish that guest no longer wants.

## Settlement

Supported settlement examples:

- 扫码付款 for WeChat/Alipay/UnionPay-like scan payment.
- 挂客房账 for hotel room consumption.
- 商旅 AR / 协议挂账 for enterprise or agreement settlement.
- 折扣: input 88 for 8.8 discount, 50 for 50%.
- 抹零: enter exact amount to round off.
- Cash can be used for demo.

## Reverse Settlement

Same-day settled orders can be reverse-settled before night audit. After night audit, avoid reverse settlement because reports have already been generated.

Use cases:

- Wrong settlement amount.
- Guest paid but later adds dishes.
- Guest should have received a different discount.

Workflow: 账单 -> filter by settlement time -> open order -> 反结账 -> enter ordering state -> adjust dishes/discount/refund/payment -> settle again.
