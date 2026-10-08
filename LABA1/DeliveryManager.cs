using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using laba1.Models;

namespace laba1.Services
{
    public class DeliveryManager
    {
        public List<Delivery> Deliveries { get; private set; }

        public DeliveryManager()
        {
            Deliveries = new List<Delivery>();
            LoadDeliveries();
        }

        public void AddDelivery(Delivery delivery)
        {
            if (delivery == null)
            {
                throw new ArgumentNullException(nameof(delivery));
            }
            Deliveries.Add(delivery);
            SaveDeliveries();
        }

        public void RemoveDelivery(Delivery delivery)
        {
            if (delivery == null)
            {
                throw new ArgumentNullException(nameof(delivery));
            }
            Deliveries.Remove(delivery);
            SaveDeliveries();
        }

        public void UpdateDeliveryStatus(Delivery delivery, DeliveryStatus newStatus)
        {
            if (delivery == null)
            {
                throw new ArgumentNullException(nameof(delivery));
            }
            delivery.UpdateStatus(newStatus);
            SaveDeliveries();
        }

        // Добавлено в лаб. №5: публичная обёртка над SaveDeliveries(),
        // чтобы сохранить изменения, внесённые напрямую в существующий
        // объект Delivery при редактировании (DeliveryForm.EditDeliveryButton_Click).
        public void Save()
        {
            SaveDeliveries();
        }

        private void SaveDeliveries()
        {
            File.WriteAllLines("deliveries.txt", Deliveries.Select(d =>
                $"{d.CustomerName}|{d.Address}|{d.DeliveryDate:yyyy-MM-dd}|{(int)d.Status}"));
        }

        private void LoadDeliveries()
        {
            if (!File.Exists("deliveries.txt")) return;

            var lines = File.ReadAllLines("deliveries.txt");
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 4)
                {
                    DateTime deliveryDate;
                    int statusValue;

                    if (DateTime.TryParse(parts[2], out deliveryDate) &&
                        int.TryParse(parts[3], out statusValue) &&
                        Enum.IsDefined(typeof(DeliveryStatus), statusValue))
                    {
                        Deliveries.Add(new Delivery(parts[0], parts[1], deliveryDate, (DeliveryStatus)statusValue));
                    }
                }
            }
        }
    }
}