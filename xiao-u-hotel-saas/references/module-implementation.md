# Implementation and Go-Live Module

## Preparation

Collect hotel go-live data before configuration: room types, room prices, payment methods, users, permissions, member rules, basic data, and required interfaces.

## Backend Setup Checklist

- Import room types and prices: 酒店 -> 基础数据 -> 房型房价.
- Add service/product item such as 增免房费.
- Configure price-change permissions.
- Configure room price strategy.
- Add member levels in group backend.
- Add unknown/default values to the data dictionary where required.
- Configure payment methods: 担保押金付, 扫码付款, 商旅企业挂账, 银行卡刷卡付.
- Disable unsuitable guarantee/pre-auth methods when the hotel should not use them.
- Configure PMS public settings.
- Register merchant users by phone and bind via customer scan code.
- Grant user permissions after account creation.

## Handover and Training

Recommend training order:

1. Room status overview.
2. Check-in and deposit/payment.
3. In-stay services.
4. Checkout and settlement.
5. Housekeeping handoff.
6. Reports and mobile management.
7. Agreement/OTA and invoice/interface topics.

Use practice prompts after each section. Ask trainees to perform a check-in, add consumption, supplement deposit, checkout, and change dirty room to clean room through the correct flow.
