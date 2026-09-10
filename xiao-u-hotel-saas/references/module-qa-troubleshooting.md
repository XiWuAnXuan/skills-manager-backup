# Q&A and Troubleshooting Module

## Answer Style

Use short, operational answers:

1. State what the issue means.
2. Give the quickest check path.
3. Give the exact operation steps.
4. Add risk/caution if money, invoice, identity, or reports are involved.
5. If uncertain, say which live Feishu doc/table should be inspected.

## Common Q&A Patterns

### 房价错了怎么办？

- Same-day or before report generation: modify original room price when permission allows.
- After report generation or checkout: use adjustment/negative offset logic instead of editing the closed room directly.
- Always check operation logs.

### 押金不够退不了房怎么办？

- Open checkout page.
- Check欠款.
- Supplement payment/deposit.
- Confirm paid amount equals or exceeds bill amount.
- Complete checkout.

### OTA/协议客人是否现场收款？

- If platform-side already paid, use agreement posting/挂账.
- Verify agreement unit/platform and amount.
- Finance later settles with the platform/company.

### 发票金额为什么不能再开？

- Check order amount, already invoiced amount, and remaining invoiceable amount.
- Fully invoiced orders should not be invoiced again.

### 退房后还能改房价吗？

- Do not edit the closed room as if it were still in-house.
- Use accounting adjustment/write-off logic depending on timing and report state.

## Live Source Reminder

The observed `知识问答` page contains a sub-page list rather than direct answers. If a user asks for exact Q&A content, inspect child pages or the `系统问题解答` Bitable before answering with source-specific certainty.
