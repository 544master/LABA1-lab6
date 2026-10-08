using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using laba1.Models;

namespace laba1.Tests
{
    [TestClass]
    public class EditDeliveryTests
    {
        [TestMethod]
        public void EditDelivery_ShouldUpdateCustomerName()
        {
            var delivery = new Delivery("Иванов И.И.", "г. Москва, ул. Ленина, 10",
                DateTime.Now.AddDays(2));

            delivery.CustomerName = "Иванова А.И.";

            Assert.AreEqual("Иванова А.И.", delivery.CustomerName);
        }

        [TestMethod]
        public void EditDelivery_ShouldUpdateAddress()
        {
            var delivery = new Delivery("Петров П.П.", "г. Казань, ул. Баумана, 1",
                DateTime.Now.AddDays(1));

            delivery.Address = "г. Казань, ул. Баумана, 15";

            Assert.AreEqual("г. Казань, ул. Баумана, 15", delivery.Address);
        }

        [TestMethod]
        public void EditDelivery_ShouldUpdateDeliveryDate()
        {
            var delivery = new Delivery("Сидоров С.С.", "г. Самара, ул. Мира, 2",
                DateTime.Now.AddDays(1));
            var newDate = DateTime.Now.AddDays(5);

            delivery.DeliveryDate = newDate;

            Assert.AreEqual(newDate.Date, delivery.DeliveryDate.Date);
        }

        [TestMethod]
        public void EditDelivery_StatusUnaffectedByFieldEdit()
        {
            var delivery = new Delivery("Кузнецова А.В.", "г. Омск, ул. Гагарина, 3",
                DateTime.Now.AddDays(1));
            delivery.UpdateStatus(DeliveryStatus.В_пути);

            delivery.Address = "г. Омск, ул. Гагарина, 30";

            Assert.AreEqual(DeliveryStatus.В_пути, delivery.Status);
        }
    }
}
