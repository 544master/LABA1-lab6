using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using laba1.Models;
using laba1.Services;

namespace laba1.Tests
{
    // Тесты, фиксирующие граничные и некорректные случаи на уровне модели.
    // Используются в лабораторной работе №4 при анализе ошибок.
    [TestClass]
    public class BoundaryConditionTests
    {
        [TestMethod]
        public void AddDelivery_WithEmptyCustomerName_IsCurrentlyAcceptedWithoutValidation()
        {
            // Класс Delivery не проверяет пустые строки — фиксируем текущее поведение
            var delivery = new Delivery("", "г. Москва, ул. Ленина, 10", DateTime.Now.AddDays(1));

            Assert.AreEqual(string.Empty, delivery.CustomerName);
        }

        [TestMethod]
        public void RemoveDelivery_ByReference_WorksCorrectlyAtManagerLevel()
        {
            // На уровне DeliveryManager удаление по ссылке работает корректно —
            // реальная проблема (лаб. №3/№4) была именно в разборе строки
            // в DeliveryForm.RemoveDeliveryButton_Click, а не в самом менеджере.
            var manager = new DeliveryManager();
            var delivery = new Delivery("Тест", "Тестовый адрес", DateTime.Now.AddDays(1));
            manager.AddDelivery(delivery);

            manager.RemoveDelivery(delivery);

            Assert.IsFalse(manager.Deliveries.Contains(delivery));
        }
    }
}
