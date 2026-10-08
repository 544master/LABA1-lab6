using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows.Forms;
using laba1.Forms;

namespace laba1.Tests
{
    [TestClass]
    public class UITests
    {
        private DeliveryForm _form = null!;

        [TestInitialize]
        public void SetUp()
        {
            _form = new DeliveryForm();
        }

        private T? FindControl<T>(string name) where T : Control =>
            _form.Controls.OfType<T>().FirstOrDefault(c => c.Name == name);

        [TestMethod]
        public void AddDeliveryButton_IsPresentAndEnabled()
        {
            var button = FindControl<Button>("addDeliveryButton");
            Assert.IsNotNull(button);
            Assert.IsTrue(button!.Enabled);
        }

        [TestMethod]
        public void RemoveDeliveryButton_IsPresent()
        {
            var button = FindControl<Button>("removeDeliveryButton");
            Assert.IsNotNull(button);
        }

        [TestMethod]
        public void EditDeliveryButton_IsPresent()
        {
            var button = FindControl<Button>("editDeliveryButton");
            Assert.IsNotNull(button);
        }

        [TestMethod]
        public void UpdateStatusButton_IsPresent()
        {
            var button = FindControl<Button>("updateStatusButton");
            Assert.IsNotNull(button);
        }

        [TestMethod]
        public void DeliveriesListBox_IsPresentAndEnabled()
        {
            var listBox = FindControl<ListBox>("deliveriesListBox");
            Assert.IsNotNull(listBox);
            Assert.IsTrue(listBox!.Enabled);
        }
    }
}
