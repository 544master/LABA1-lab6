using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using laba1.Models;
using laba1.Services;

namespace laba1.Forms
{
    public class DeliveryForm : Form
    {
        private DeliveryManager deliveryManager;
        private TextBox customerNameTextBox;
        private TextBox addressTextBox;
        private DateTimePicker deliveryDatePicker;
        private ComboBox statusComboBox;
        private Button addDeliveryButton;
        private Button removeDeliveryButton;
        private Button updateStatusButton;
        private Button editDeliveryButton;
        private ListBox deliveriesListBox;

        // Соответствие текста в statusComboBox и значений перечисления DeliveryStatus.
        // Введено при исправлении ошибки в лабораторной работе №4: элементы
        // ComboBox ("Новый", "В пути", "Доставлен") не совпадают дословно с именами
        // членов перечисления (в частности "В_пути" содержит подчёркивание),
        // поэтому прямой Enum.Parse(...) был бы ненадёжен.
        private static readonly Dictionary<string, DeliveryStatus> StatusByDisplayText = new()
        {
            { "Новый", DeliveryStatus.Новый },
            { "В пути", DeliveryStatus.В_пути },
            { "Доставлен", DeliveryStatus.Доставлен }
        };

        public DeliveryForm()
        {
            this.Text = "Управление доставкой";
            this.Width = 600;
            this.Height = 500;

            // Инициализация элементов управления
            customerNameTextBox = new TextBox
            {
                Location = new Point(10, 10),
                Width = 150,
                PlaceholderText = "Имя клиента",
                Name = "customerNameTextBox" // добавлено в лаб. №6 для UI-автоматизации (FlaUI)
            };

            addressTextBox = new TextBox
            {
                Location = new Point(170, 10),
                Width = 200,
                PlaceholderText = "Адрес",
                Name = "addressTextBox"
            };

            deliveryDatePicker = new DateTimePicker
            {
                Location = new Point(380, 10),
                Name = "deliveryDatePicker"
            };

            statusComboBox = new ComboBox
            {
                Location = new Point(10, 40),
                Width = 100,
                Items = { "Новый", "В пути", "Доставлен" },
                Name = "statusComboBox",
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            addDeliveryButton = new Button
            {
                Location = new Point(10, 70),
                Text = "Добавить",
                Width = 100,
                Name = "addDeliveryButton"
            };
            addDeliveryButton.Click += AddDeliveryButton_Click;

            removeDeliveryButton = new Button
            {
                Location = new Point(120, 70),
                Text = "Удалить",
                Width = 100,
                Name = "removeDeliveryButton"
            };
            removeDeliveryButton.Click += RemoveDeliveryButton_Click;

            updateStatusButton = new Button
            {
                Location = new Point(220, 70),
                Text = "Обновить статус",
                Width = 120,
                Name = "updateStatusButton"
            };
            updateStatusButton.Click += UpdateStatusButton_Click;

            // Кнопка "Редактировать" — новая функция, добавленная в лабораторной работе №5
            editDeliveryButton = new Button
            {
                Location = new Point(350, 70),
                Text = "Редактировать",
                Width = 110,
                Name = "editDeliveryButton"
            };
            editDeliveryButton.Click += EditDeliveryButton_Click;

            deliveriesListBox = new ListBox
            {
                Location = new Point(10, 100),
                Width = 560,
                Height = 250,
                Name = "deliveriesListBox"
            };
            // Предзаполнение полей при выборе доставки в списке (лаб. №5)
            deliveriesListBox.SelectedIndexChanged += DeliveriesListBox_SelectedIndexChanged;

            // Добавление на форму
            this.Controls.Add(customerNameTextBox);
            this.Controls.Add(addressTextBox);
            this.Controls.Add(deliveryDatePicker);
            this.Controls.Add(statusComboBox);
            this.Controls.Add(addDeliveryButton);
            this.Controls.Add(removeDeliveryButton);
            this.Controls.Add(updateStatusButton);
            this.Controls.Add(editDeliveryButton);
            this.Controls.Add(deliveriesListBox);

            // Инициализация менеджера
            deliveryManager = new DeliveryManager();
            UpdateDeliveriesList();
        }

        private void UpdateDeliveriesList()
        {
            deliveriesListBox.Items.Clear();
            foreach (var delivery in deliveryManager.Deliveries)
            {
                deliveriesListBox.Items.Add($"{delivery.CustomerName} - {delivery.Address} ({delivery.Status})");
            }
        }

        private void AddDeliveryButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(customerNameTextBox.Text) ||
                string.IsNullOrEmpty(addressTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            // Проверка даты доставки — исправление дефекта TC-004 (лаб. №4).
            // DateTimePicker изначально не был ограничен снизу и принимал прошедшие даты.
            if (deliveryDatePicker.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Дата доставки не может быть в прошлом!");
                return;
            }

            DateTime deliveryDate = deliveryDatePicker.Value;
            Delivery newDelivery = new Delivery(customerNameTextBox.Text, addressTextBox.Text, deliveryDate);

            try
            {
                deliveryManager.AddDelivery(newDelivery);
                customerNameTextBox.Clear();
                addressTextBox.Clear();
                UpdateDeliveriesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveDeliveryButton_Click(object sender, EventArgs e)
        {
            // ИСПРАВЛЕНО в лаб. №4: раньше адрес восстанавливался разбором строки
            // "Клиент - Адрес (Статус)" и никогда не совпадал с реальным Address,
            // потому что суффикс "(Статус)" не отделялся. Теперь используется
            // SelectedIndex — порядок элементов списка всегда совпадает с порядком
            // deliveryManager.Deliveries, так как оба заполняются в UpdateDeliveriesList().
            int index = deliveriesListBox.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("Выберите доставку для удаления!");
                return;
            }

            var deliveryToRemove = deliveryManager.Deliveries[index];

            try
            {
                deliveryManager.RemoveDelivery(deliveryToRemove);
                UpdateDeliveriesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateStatusButton_Click(object sender, EventArgs e)
        {
            // ИСПРАВЛЕНО в лаб. №4: тот же переход на SelectedIndex, плюс явное
            // сопоставление текста ComboBox со значением enum через StatusByDisplayText,
            // так как "В пути" (текст) и "В_пути" (имя значения enum) не совпадают.
            int index = deliveriesListBox.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("Выберите доставку для обновления статуса!");
                return;
            }

            if (statusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите новый статус!");
                return;
            }

            var deliveryToUpdate = deliveryManager.Deliveries[index];
            var newStatus = StatusByDisplayText[statusComboBox.SelectedItem.ToString()];

            try
            {
                deliveryManager.UpdateDeliveryStatus(deliveryToUpdate, newStatus);
                UpdateDeliveriesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Новая функция (лаб. №5): редактирование данных доставки
        private void DeliveriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = deliveriesListBox.SelectedIndex;
            if (index == -1) return;

            var delivery = deliveryManager.Deliveries[index];
            customerNameTextBox.Text = delivery.CustomerName;
            addressTextBox.Text = delivery.Address;
            deliveryDatePicker.Value = delivery.DeliveryDate;
        }

        private void EditDeliveryButton_Click(object sender, EventArgs e)
        {
            int index = deliveriesListBox.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("Выберите доставку для редактирования!");
                return;
            }

            if (string.IsNullOrEmpty(customerNameTextBox.Text) ||
                string.IsNullOrEmpty(addressTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (deliveryDatePicker.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Дата доставки не может быть в прошлом!");
                return;
            }

            var delivery = deliveryManager.Deliveries[index];
            delivery.CustomerName = customerNameTextBox.Text;
            delivery.Address = addressTextBox.Text;
            delivery.DeliveryDate = deliveryDatePicker.Value;

            deliveryManager.Save();
            UpdateDeliveriesList();
            // Убран MessageBox.Show("Доставка отредактирована!", ...): это была
            // чисто информационная блокирующая диалоговая форма без функциональной
            // необходимости — она мешала автоматизированному UI-тестированию
            // (лаб. №6), так как виснет, ожидая ручного нажатия "ОК".
        }
    }
}
