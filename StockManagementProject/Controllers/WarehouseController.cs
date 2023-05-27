﻿using StockManagementProject.DataAccessLayer;
using StockManagementProject.Interfaces;
using StockManagementProject.Models;
using StockManagementProject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockManagementProject.Controllers
{
    internal class WarehouseController : IController<Warehouse>
    {
        WarehouseRepository repository = new WarehouseRepository();
        WarehouseProductStockRepository productStockRepository = new WarehouseProductStockRepository();
        ProductRepository productRepository = new ProductRepository();
        DataContext db = new DataContext();

        public void Add()
        {
            Console.Clear();
            Console.WriteLine(repository.Add(SetValue()) ? "Ekleme Başarılı" : "Ekleme Başarısız");
            Thread.Sleep(2000);

        }

        public void Delete()
        {
            GetAll();
            Console.Write("Silinecek Depo Id Giriniz: ");
            int id = Convert.ToInt32(Console.ReadLine());
            

            if (repository.Delete(id))
            {
                Console.Clear();
                Console.WriteLine("Silme İşlemi Başarılı");
                Console.WriteLine();
                GetAll();
                Console.WriteLine("Ana Menü İçin Bir Tuşa Basın");
            }
            else
            {
                Console.WriteLine("Silme İşlemi Başarısız");
            }
            Console.ReadKey();
        }

        public Warehouse Get()
        {
            
            GetAll();
            Console.WriteLine();
            Console.Write("Depo Id Giriniz: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Console.Clear();
            Warehouse warehouse = repository.GetById(id);
            User depoYonetici = db.User.FirstOrDefault(x => x.Id == warehouse.ManagerId);


            

            if (warehouse != null)
            {
                Console.WriteLine("Depo Detayları");
                Console.WriteLine("-------------");
                Console.WriteLine("Id              : " + warehouse.Id);
                Console.WriteLine("Depo İsmi       : " + warehouse.Name);
                Console.WriteLine("Deponun Semti   : " + warehouse.District);
                Console.WriteLine("Depo Yönetici Id: " + warehouse.ManagerId);
                Console.WriteLine("Depo Yöneticisi : " + depoYonetici.Name);
                Console.WriteLine("Depo Durumu     : " + (warehouse.IsStatus ? "Aktif" : "Pasif"));
                Console.WriteLine();
                Console.WriteLine();
                
                WarehouseProducts(warehouse.Id);
               
            }
            else
            {
                Console.WriteLine("Depo Bulunamadı");
            }
            return warehouse;
        }


        public void GetAll()
        {
            Console.WriteLine("Depoların Listesi");
            Console.WriteLine();
            if (repository.GetAll().Count > 0)
            {
              
                foreach (var depo in repository.GetAll())
                {

                    Warehouse warehouse = repository.GetById(depo.Id);
                    User depoYonetici = db.User.FirstOrDefault(x => x.Id == warehouse.ManagerId);

                    Console.WriteLine("Depo Id:           "+depo.Id);
                    Console.WriteLine("Depo İsmi:         "+depo.Name);
                    Console.WriteLine("Depo Semti:        "+depo.District);
                    Console.WriteLine("Depo Yönetici Id:  "+depo.ManagerId);
                    Console.WriteLine("Depo Yönetici Adı: "+depoYonetici.Name);
                    Console.WriteLine("Depo Durum:        "+(depo.IsStatus ? "Aktif":"Pasif"));
                    Console.WriteLine();


                }
            }
            else
            {
                Console.WriteLine("Depo Listesi Boş");
            }
           
        }

        public void Menu()
        {
            bool status = true;
            while (status)
            {
                Console.Clear();

                Console.WriteLine("Depo İşlemi Seçiniz");
                Console.WriteLine("-------------------");
                Console.WriteLine("Depo Ekle       (1)");
                Console.WriteLine("Depo Detayları  (2)");
                Console.WriteLine("Depo Sil        (3)");
                Console.WriteLine("Depo Güncelle   (4)");
                Console.WriteLine("Depo Listesi    (5)");
                Console.WriteLine("Üst Menü        (0)");
                Console.WriteLine();
                Console.Write("İşlem Seçiniz: ");
                int select = Convert.ToInt32(Console.ReadLine());
                Console.Clear();

                switch (select)
                {
                    case 1: Add(); break;
                    case 2: Get(); Console.ReadKey(); break;
                    case 3: Delete(); break;
                    case 4: Update(); break;
                    case 5: GetAll(); Console.ReadKey(); break;
                    case 0: status = !status; break;
                    default: Console.WriteLine("Tanımsız İşlem Tekrar Deneyiniz"); break;
                }
            }

        }

        public Warehouse SetValue()
        {
            Warehouse warehouse = new Warehouse();

            Console.Write("Depo İsmi:  ");
            warehouse.Name = Console.ReadLine();

            Console.Write("Semt İsmi:  ");
            warehouse.District = Console.ReadLine();


            bool status = true;
            while (status)
            {
                UserController userList = new UserController();
                userList.GetAll();
                Console.WriteLine("--------------------");
                Console.Write("Depo Yönetici Id: ");

                string managerid = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(managerid) && int.TryParse(managerid, out int managerIdInt))
                {
                    User managerisIsNull = db.User.FirstOrDefault(x => x.Id == managerIdInt);

                    if (managerisIsNull != null)
                    {
                        warehouse.ManagerId = managerIdInt;
                        status = false;
                    }
                    else
                    {
                        Console.WriteLine("Geçerli bir Yönetici Id Giriniz");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Geçerli bir Yönetici Id Giriniz");
                    Console.WriteLine();
                }
            }



            Console.Write("Depo Durumu Aktif(A) Pasif(P):  ");
            warehouse.IsStatus = Console.ReadLine().Substring(0, 1).ToLower() == "a" ? true : false;

            return warehouse;
        }

        public void Update()
        {
            DataContext db = new DataContext();
            GetAll();
            Console.Write("Düzenlemek istediğiniz deponun Id sini girin: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Warehouse warehouse = db.Warehouse.FirstOrDefault(x => x.Id == id);

            if (warehouse!=null)
            {
                Warehouse setWarehouse = SetValue();
                if (setWarehouse!=null)
                {
                    setWarehouse.Id = warehouse.Id;
                    warehouse = setWarehouse;
                    repository.Update(warehouse);
                    Console.WriteLine("Güncelleme başarılı");
                }
                else
                {
                    Console.WriteLine("Güncelleme başarısız.");
                }

            }
        }

        public void WarehouseProducts(int warehouseId)
        {
            Console.WriteLine("Depodaki ürünler: ");
            Console.WriteLine();
            Console.WriteLine("Ürün Id \t Ürün İsmi \t Ürün Miktarı");

            var sorgu = from w in repository.GetAll()
                        join wp in productStockRepository.GetAll() on w.Id equals wp.WarehouseId
                        join p in productRepository.GetAll() on wp.ProductId equals p.Id
                        where w.Id == warehouseId
                        select new { p.Name, wp.ProductId, wp.ProductQuantity };

            
            foreach (var item in sorgu)
            {
                
                Console.WriteLine($"{item.ProductId} \t\t {item.Name} \t\t {item.ProductQuantity}");
            }
        }
    }
}
