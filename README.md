# Лабораторная работа №5 — Новая функция (редактирование доставки)

Состояние: лаб. №4 (исправления) + новая функция редактирования.

## Изменения
- `LABA1/DeliveryForm.cs`: добавлена кнопка «Редактировать», обработчик
  `EditDeliveryButton_Click`, предзаполнение полей при выборе доставки
  в списке (`DeliveriesListBox_SelectedIndexChanged`).
- `LABA1/DeliveryManager.cs`: добавлен публичный метод `Save()`.
- `LABA1.Tests/EditDeliveryTests.cs` — новый файл с тестами функции
  редактирования.
- `LABA1.Tests/DeliveryManagerTests.cs` — добавлен тест `Save_Persists...`.
- `LABA1.Tests/UITests.cs` — добавлена проверка кнопки «Редактировать».

## Проверка
`Delivery.cs`/`DeliveryManager.cs` скомпилированы офлайн успешно
(.NET 8, без WinForms-зависимостей). `DeliveryForm.cs` и тестовый
проект собрать в песочнице не удалось (нет доступа к nuget.org) —
соберите в своей Visual Studio перед сдачей.
