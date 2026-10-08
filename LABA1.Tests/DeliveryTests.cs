using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using laba1.Models;

namespace laba1.Tests
{
    [TestClass]
    public class DeliveryTests
    {
        [TestMethod]
        public void Constructor_SetsPropertiesCorrectly()
        {
            var delivery = new Delivery("Иванов И.И.", "г. Москва, ул. Ленина, 10",
                new DateTime(2026, 7, 10));

            Assert.AreEqual("Иванов И.И.", delivery.CustomerName);
            Assert.AreEqual("г. Москва, ул. Ленина, 10", delivery.Address);
            Assert.AreEqual(new DateTime(2026, 7, 10), delivery.DeliveryDate);
        }

        [TestMethod]
        public void Constructor_SetsDefaultStatusToNew()
        {
            var delivery = new Delivery("Петров П.П.", "г. Санкт-Петербург, Невский пр., 5",
                DateTime.Now.AddDays(2));

            Assert.AreEqual(DeliveryStatus.Новый, delivery.Status);
        }

        [TestMethod]
        public void Constructor_WithStatus_SetsGivenStatus()
        {
            // Второй конструктор используется DeliveryManager при загрузке из файла
            var delivery = new Delivery("Смирнов С.С.", "г. Казань, ул. Баумана, 1",
                DateTime.Now.AddDays(1), DeliveryStatus.В_пути);

            Assert.AreEqual(DeliveryStatus.В_пути, delivery.Status);
        }

        [TestMethod]
        public void UpdateStatus_ChangesStatusToInTransit()
        {
            var delivery = new Delivery("Сидоров С.С.", "г. Казань, ул. Баумана, 1",
                DateTime.Now.AddDays(1));

            delivery.UpdateStatus(DeliveryStatus.В_пути);

            Assert.AreEqual(DeliveryStatus.В_пути, delivery.Status);
        }

        [TestMethod]
        public void UpdateStatus_ChangesStatusToDelivered()
        {
            var delivery = new Delivery("Кузнецова А.В.", "г. Новосибирск, ул. Мира, 20",
                DateTime.Now);

            delivery.UpdateStatus(DeliveryStatus.Доставлен);

            Assert.AreEqual(DeliveryStatus.Доставлен, delivery.Status);
        }
    }
}
