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
    // UI-тесты приложения "Управление доставкой" (FlaUI).
    //
    // ВАЖНО: тесты рассчитаны на выполнение В ПОРЯДКЕ ОБЪЯВЛЕНИЯ (Test1 -> Test2 ->
    // Test3 -> Test4), запускайте через "Run All", а не по одному вразнобой.
    // Данные (deliveries.txt) НЕ стираются между Test1/2/3/4 — каждый следующий
    // тест работает с реальными доставками, созданными в Test1, точно так же,
    // как при обычной ручной работе с приложением. Очистка происходит только
    // перед самим Test1 (см. TestInitialize) — это гарантирует чистый старт
    // при каждом повторном запуске всего набора тестов.
    [TestClass]
    [DoNotParallelize]
    public class DeliveryFormUITests
    {
        private Application _app = null!;
        private UIA3Automation _automation = null!;
        private Window _mainWindow = null!;

        // Обновите путь на фактический путь к собранному .exe в вашей системе
        private const string ExePath = @"..\..\..\..\LABA1\bin\Debug\net10.0-windows\LABA1.exe";
        private const int Pause = 1500;

        private static string DataFilePath =>
            Path.Combine(Path.GetDirectoryName(Path.GetFullPath(ExePath))!, "deliveries.txt");

        // MSTest автоматически подставляет сюда контекст текущего теста —
        // используем TestContext.TestName, чтобы понять, что сейчас Test1,
        // и только тогда стереть старые данные (перед запуском приложения,
        // а не после — иначе приложение уже успеет прочитать старый файл).
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

        // Выбирает элемент списка по индексу (0 = первый). Кликает по ПЕРВОМУ
        // элементу списка (он всегда есть и всегда на виду — клик по нему надёжен),
        // это даёт списку реальный фокус ввода и выбирает индекс 0, а дальше
        // нужное количество раз "нажимает" (полностью, press+release) стрелку
        // вниз — это встроенная клавиатурная навигация WinForms ListBox, она
        // не зависит от того, куда именно попадёт клик мышью по конкретному
        // элементу (что было источником нестабильности раньше).
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

        // Тест 1: создание четырёх заявок
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

        // Тест 2: редактирование заявки, созданной в Test1 —
        // "Дима, г. Москва" (индекс 2) меняется на "Саша, г. Владивосток"
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

        // Тест 3: обновление статуса заявки "Вася, г. Воронеж" (индекс 1) на "Доставлен"
        [TestMethod]
        public void Test3_UpdateVasyaStatusToDelivered()
        {
            SelectItemAtIndex(1); // Вася - г. Воронеж

            var statusComboBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("statusComboBox"));
            statusComboBox.Click();
            Thread.Sleep(Pause);
            Keyboard.Type("Д"); // единственный пункт на "Д" в списке — "Доставлен"
            Thread.Sleep(Pause);

            // ВАЖНО: клик по ComboBox открывает выпадающий список. Если сразу
            // после этого кликнуть по кнопке "Обновить статус", Windows часто
            // просто закрывает открытый список этим кликом, не передавая его
            // кнопке — из-за этого статус не обновлялся. Tab переводит фокус
            // дальше и закрывает список, окончательно фиксируя выбор "Доставлен".
            Keyboard.Type(VirtualKeyShort.TAB);
            Thread.Sleep(Pause);

            var updateButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("updateStatusButton")).AsButton();
            updateButton.Click();
            Thread.Sleep(Pause);

            var updated = GetList().Items.FirstOrDefault(i => i.Text.Contains("Вася"));
            Assert.IsNotNull(updated, "Заявка \"Вася\" пропала из списка");
            Assert.IsTrue(updated!.Text.Contains("Доставлен"), "Статус заявки \"Вася\" не был обновлён на \"Доставлен\"");
        }

        // Тест 4: удаление заявки "Никита, г. Екатеринбург" (индекс 3)
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
