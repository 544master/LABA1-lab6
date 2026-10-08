using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using laba1.Models;
using laba1.Services;

namespace laba1.Tests
{
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
            var manager = new DeliveryManager();
            var delivery = new Delivery("Тест", "Тестовый адрес", DateTime.Now.AddDays(1));
            manager.AddDelivery(delivery);

            manager.RemoveDelivery(delivery);

            Assert.IsFalse(manager.Deliveries.Contains(delivery));
        }
    }
}
