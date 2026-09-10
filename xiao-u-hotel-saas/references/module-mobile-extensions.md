# Mobile, Invoice, Interface, and Permission Module

## Guest Mini-Program

Guest can enter through room QR code, front desk QR code, official account link, or similar entrance. Possible services:

- Request cleaning.
- Invoice request.
- Borrow items.
- Butler/service requests.
- Book room and extend stay.
- Mall shopping.
- Member registration, recharge, points, member price.
- Room ordering and rental services.

## Owner Assistant / Management App

For boss, manager, finance, service staff, or engineering staff:

- View revenue, sold rooms, average room price, occupancy, source analysis.
- View room-status map synchronized with PC.
- Change room status from mobile.
- View occupied-room details, consumption, logs, deposits.
- Add consumption, supplement deposit, mark cleaning complete, and handle checkout when permitted.

## Mobile Cashier and Restaurant

Mobile cashier supports small shop, cart, or service scenarios. Staff can scan guest payment code from phone without relying on PC.

Restaurant mobile operations can support ordering and settlement, including room-account posting and agreement posting.

## Permissions

Permissions should be modular. Business data should be limited to boss/manager/finance. Front desk and service staff should only receive required modules.

## Invoices

When invoice platform is connected, system can show invoice status, room number, guest name, invoice time, order amount, invoiced amount, and remaining invoiceable amount.

Avoid duplicate invoices: if amount is already fully invoiced, the system should not allow repeated invoicing.

Team rooms can issue invoice by total amount or partial amount, depending on actual need.

## External Interfaces

Examples: PSB/public security, invoice platform, Wi-Fi real-name authentication, parking/gate systems, door lock/card issuing, printer.

Whether integration is possible depends on the other system's API/data availability and local vendor requirements.
