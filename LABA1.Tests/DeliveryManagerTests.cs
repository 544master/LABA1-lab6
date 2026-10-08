using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using laba1.Models;
using laba1.Services;

namespace laba1.Tests
{
    [TestClass]
    public class DeliveryManagerTests
    {
        private DeliveryManager _manager = null!;
        private const string DataFile = "deliveries.txt";

        [TestInitialize]
        public void SetUp()
        {
            if (File.Exists(DataFile)) File.Delete(DataFile);
            _manager = new DeliveryManager();
        }

        [TestCleanup]
        public void TearDown()
        {
            if (File.Exists(DataFile)) File.Delete(DataFile);
        }

        [TestMethod]
        public void AddDelivery_AddsItemToList()
        {
            var delivery = new Delivery("Иванов И.И.", "г. Москва, ул. Ленина, 10",
                DateTime.Now.AddDays(3));

            _manager.AddDelivery(delivery);

            Assert.AreEqual(1, _manager.Deliveries.Count);
            Assert.AreEqual("Иванов И.И.", _manager.Deliveries.First().CustomerName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddDelivery_WhenNull_ThrowsArgumentNullException()
        {
            _manager.AddDelivery(null!);
        }

        [TestMethod]
        public void RemoveDelivery_RemovesItemFromList()
        {
            var delivery = new Delivery("Петров П.П.", "г. Санкт-Петербург, Невский пр., 5",
                DateTime.Now.AddDays(1));
            _manager.AddDelivery(delivery);

            _manager.RemoveDelivery(delivery);

            Assert.AreEqual(0, _manager.Deliveries.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveDelivery_WhenNull_ThrowsArgumentNullException()
        {
            _manager.RemoveDelivery(null!);
        }

        [TestMethod]
        public void UpdateDeliveryStatus_UpdatesStatusOfExistingDelivery()
        {
            var delivery = new Delivery("Сидоров С.С.", "г. Казань, ул. Баумана, 1",
                DateTime.Now.AddDays(1));
            _manager.AddDelivery(delivery);

            _manager.UpdateDeliveryStatus(delivery, DeliveryStatus.Доставлен);

            Assert.AreEqual(DeliveryStatus.Доставлен, delivery.Status);
        }

        [TestMethod]
        public void SaveAndLoadDeliveries_PersistsDataBetweenInstances()
        {
            var delivery = new Delivery("Кузнецова А.В.", "г. Новосибирск, ул. Мира, 20",
                new DateTime(2026, 8, 1));
            _manager.AddDelivery(delivery);

            var newManager = new DeliveryManager();

            Assert.AreEqual(1, newManager.Deliveries.Count);
            Assert.AreEqual("Кузнецова А.В.", newManager.Deliveries.First().CustomerName);
        }

        [TestMethod]
        public void Save_PersistsDirectlyEditedDelivery()
        {
            // Проверка метода Save(), добавленного в лаб. №5 для функции редактирования
            var delivery = new Delivery("Волкова Е.С.", "г. Тверь, ул. Советская, 4",
                DateTime.Now.AddDays(3));
            _manager.AddDelivery(delivery);

            delivery.Address = "г. Тверь, ул. Советская, 40";
            _manager.Save();

            var reloaded = new DeliveryManager();
            var found = reloaded.Deliveries.Find(d => d.CustomerName == "Волкова Е.С.");
            Assert.IsNotNull(found);
            Assert.AreEqual("г. Тверь, ул. Советская, 40", found!.Address);
        }
    }
}
