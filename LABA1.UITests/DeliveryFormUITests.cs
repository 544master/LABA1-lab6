using System;
using System.IO;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace laba1.UITests
{
   
    [TestClass]
    [DoNotParallelize]
    public class DeliveryFormUITests
    {
        private Application _app = null!;
        private UIA3Automation _automation = null!;
        private Window _mainWindow = null!;

        private const string ExePath = @"..\..\..\..\LABA1\bin\Debug\net10.0-windows\LABA1.exe";
        private const int Pause = 1500;

        private static string DataFilePath =>
            Path.Combine(Path.GetDirectoryName(Path.GetFullPath(ExePath))!, "deliveries.txt");
        public TestContext TestContext { get; set; } = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            if (TestContext.TestName == nameof(Test1_CreateFourDeliveries))
            {
                if (File.Exists(DataFilePath))
                {
                    File.Delete(DataFilePath);
                }
            }

            _app = Application.Launch(ExePath);
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation);
            Thread.Sleep(Pause);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _automation?.Dispose();
            _app?.Close();
        }

        private ListBox GetList() =>
            _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("deliveriesListBox")).AsListBox();

        private void AddDelivery(string name, string address)
        {
            var nameBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("customerNameTextBox")).AsTextBox();
            var addressBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addressTextBox")).AsTextBox();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addDeliveryButton")).AsButton();

            nameBox.Text = name;
            addressBox.Text = address;
            addButton.Click();
            Thread.Sleep(Pause);
        }

        private void SelectItemAtIndex(int targetIndex)
        {
            var list = GetList();
            var firstItem = list.Items.First();
            firstItem.Click();
            Thread.Sleep(Pause);

            for (int i = 0; i < targetIndex; i++)
            {
                Keyboard.Type(VirtualKeyShort.DOWN);
                Thread.Sleep(400);
            }
            Thread.Sleep(Pause);
        }

        [TestMethod]
        public void Test1_CreateFourDeliveries()
        {
            AddDelivery("Ваня", "г. Санкт-Петербург");
            AddDelivery("Вася", "г. Воронеж");
            AddDelivery("Дима", "г. Москва");
            AddDelivery("Никита", "г. Екатеринбург");

            var items = GetList().Items;
            Assert.AreEqual(4, items.Length, "Должно быть создано ровно 4 доставки");
            Assert.IsTrue(items.Any(i => i.Text.Contains("Ваня") && i.Text.Contains("Новый")));
            Assert.IsTrue(items.Any(i => i.Text.Contains("Вася") && i.Text.Contains("Новый")));
            Assert.IsTrue(items.Any(i => i.Text.Contains("Дима") && i.Text.Contains("Новый")));
            Assert.IsTrue(items.Any(i => i.Text.Contains("Никита") && i.Text.Contains("Новый")));
        }

        [TestMethod]
        public void Test2_EditDimaToSasha()
        {
            SelectItemAtIndex(2); // Дима - г. Москва

            var nameBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("customerNameTextBox")).AsTextBox();
            var addressBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addressTextBox")).AsTextBox();
            nameBox.Text = "Саша";
            addressBox.Text = "г. Владивосток";

            var editButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("editDeliveryButton")).AsButton();
            editButton.Click();
            Thread.Sleep(Pause);

            var items = GetList().Items;
            Assert.IsFalse(items.Any(i => i.Text.Contains("Дима")), "Старое имя \"Дима\" всё ещё встречается в списке");
            Assert.IsTrue(items.Any(i => i.Text.Contains("Саша") && i.Text.Contains("Владивосток")),
                "Заявка не была отредактирована на \"Саша, г. Владивосток\"");
        }

        [TestMethod]
        public void Test3_UpdateVasyaStatusToDelivered()
        {
            SelectItemAtIndex(1); // Вася - г. Воронеж

            var statusComboBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("statusComboBox"));
            statusComboBox.Click();
            Thread.Sleep(Pause);
            Keyboard.Type("Д"); // единственный пункт на "Д" в списке — "Доставлен"
            Thread.Sleep(Pause);

            Keyboard.Type(VirtualKeyShort.TAB);
            Thread.Sleep(Pause);

            var updateButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("updateStatusButton")).AsButton();
            updateButton.Click();
            Thread.Sleep(Pause);

            var updated = GetList().Items.FirstOrDefault(i => i.Text.Contains("Вася"));
            Assert.IsNotNull(updated, "Заявка \"Вася\" пропала из списка");
            Assert.IsTrue(updated!.Text.Contains("Доставлен"), "Статус заявки \"Вася\" не был обновлён на \"Доставлен\"");
        }

        [TestMethod]
        public void Test4_DeleteNikita()
        {
            SelectItemAtIndex(3); // Никита - г. Екатеринбург

            var removeButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("removeDeliveryButton")).AsButton();
            removeButton.Click();
            Thread.Sleep(Pause);

            var items = GetList().Items;
            Assert.IsFalse(items.Any(i => i.Text.Contains("Никита")), "Заявка \"Никита\" не была удалена");
            Assert.AreEqual(3, items.Length, "После удаления должно остаться 3 заявки");
        }
    }
}
