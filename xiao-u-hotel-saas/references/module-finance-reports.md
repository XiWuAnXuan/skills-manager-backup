# Finance and Reports Module

## Key Principle

Separate 未退房 and 已退房 states. Deposits, refunds, room revenue, and settlement reports can look different before and after checkout.

## Checkout Money Checks

- Guest paid amount must be greater than or equal to bill consumption before normal checkout.
- If paid amount is insufficient, the system blocks checkout and shows欠款.
- Supplement deposit/payment first, then checkout.
- For agreement/OTA guests, verify whether the guest already paid platform-side; if yes, use agreement posting rather than on-site settlement.

## Report Paths

- 营业日报: 报表 -> 今日报表 -> 营业日报.
- 结算统计日报: 报表 -> 今日报表 -> 结算统计日报.

## 营业日报

Explain daily operating amount and month-to-date amount. It may include room deposit/payment and non-room consumption depending on state and configuration.

## 结算统计日报

Explain grouped room revenue, non-room revenue, payment-method settlement amounts, receipts, refunds, and balances.

## Writing Finance Explanations

Use concrete scenarios:

- 未退房: room fee + deposit paid, still in house.
- 已退房: deposit refunded or surplus returned after settlement.

Always label whether a number is 房费, 押金, 退款, 已付, 欠款, or 消费合计.
