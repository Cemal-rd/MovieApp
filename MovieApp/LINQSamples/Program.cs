using LINQSamples.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQSamples
{
    class CustomerModel
    {
        public CustomerModel()
        {
            this.Orders = new List<OrderModel>();
        }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int OrderCount { get; set; }
        public List<OrderModel> Orders { get; set; }
    }
    class OrderModel
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public List<ProductModel> Products { get; set; }
    }
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new CustomNorthwindContext())
            {

                //var sonuc = db.Database.ExecuteSqlRaw("delete from products where productId=81");
                //var sonuc = db.Database.ExecuteSqlRaw("update products set unitprice=unitprice*1.2 where categoryId=4");
                //var query = "4";

                //var products = db.Products.FromSqlRaw($"select * from products where categoryId={query}").ToList();


                var products = db.ProductModels.FromSqlRaw("select ProductId, ProductName, UnitPrice from Products").ToList();

                foreach (var item in products)
                {
                    Console.WriteLine(item.Name +" =>"+ item.Price);
                }

            }

            Console.ReadLine();
        }













        private static void Ders12(NorthwindContext db)
        {
            // Müşterilerin verdiği sipariş toplamı ?

            var customers = db.Customers
                .Where(cus => cus.CustomerId == "PERIC")
                .Select(cus => new CustomerModel
                {
                    CustomerId = cus.CustomerId,
                    CustomerName = cus.ContactName,
                    OrderCount = cus.Orders.Count,
                    Orders = cus.Orders.Select(order => new OrderModel
                    {
                        OrderId = order.OrderId,
                        Total = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice),
                        Products = order.OrderDetails.Select(od => new ProductModel
                        {
                            ProductId = od.ProductId,
                            Name = od.Product.ProductName,
                            Price = od.UnitPrice,
                            //Quantity = od.Quantity
                        }).ToList()
                    }).ToList()
                })
                .OrderBy(i => i.OrderCount)
                .ToList();

            foreach (var customer in customers)
            {
                Console.WriteLine(customer.CustomerId + "=>" + customer.CustomerName + " => " + customer.OrderCount);
                Console.WriteLine("Siparişler");
                foreach (var order in customer.Orders)
                {
                    Console.WriteLine("*****************************");
                    Console.WriteLine(order.OrderId + "=>" + order.Total);
                    foreach (var product in order.Products)
                    {
                        Console.WriteLine(product.ProductId + "=>" + product.Name + "=>" + product.Price + "=>");
                    }
                }
            }
        }
        private static void Ders11(NorthwindContext db)
        {
            //var products = db.Products.Where(p => p.CategoryId == 1).ToList();
            //var products = db.Products.Include(p=>p.Category).Where(p => p.Category.CategoryName == "Beverages").ToList();
            //var products = db.Products
            //    .Where(p => p.Category.CategoryName == "Beverages")
            //    .Select(p=>new
            //    {
            //        name = p.ProductName,
            //        id = p.CategoryId,
            //        categoryname =p.Category.CategoryName
            //    })
            //    .ToList();

            //var categories = db.Categories.Where(c => c.Products.Count() == 0).ToList();
            //var categories = db.Categories.Where(c => c.Products.Any()).ToList();

            //var products = db.Products
            //    .Select(p =>                     
            //        new {
            //            companyName = p.Supplier.CompanyName,
            //            contactName = p.Supplier.ContactName,
            //            p.ProductName
            //        }).ToList();

            // extension methods
            // query expressions

            //var products = (from p in db.Products
            //                where p.UnitPrice>10
            //               select p).ToList();

            var products = (from p in db.Products
                            join s in db.Suppliers on p.SupplierId equals s.SupplierId
                            select new
                            {
                                p.ProductName,
                                contactName = s.ContactName,
                                companyName = s.CompanyName
                            }).ToList();


            foreach (var item in products)
            {
                Console.WriteLine(item.ProductName + " " + item.companyName + " " + item.contactName);
            }
        }

        private static void Ders10(NorthwindContext db)
        {
            var p1 = new Product() { ProductId = 85 };
            var p2 = new Product() { ProductId = 84 };

            var products = new List<Product>() { p1, p2 };

            //db.Entry(p).State = EntityState.Deleted;
            db.Products.RemoveRange(products);

            db.SaveChanges();
        }

        private static void Ders9(NorthwindContext db)
        {
            var p = db.Products.Find(88);

            if (p != null)
            {
                db.Products.Remove(p);
                db.SaveChanges();
            }
        }

        private static void Ders8(NorthwindContext db)
        {
            var product = db.Products.Find(1);

            if (product != null)
            {
                product.UnitPrice = 28;

                db.Update(product);
                db.SaveChanges();
            }
        }

        private static void Ders7(NorthwindContext db)
        {
            var p = new Product() { ProductId = 1 };

            db.Products.Attach(p);

            p.UnitsInStock = 50;

            db.SaveChanges();
        }

        private static void Ders6(NorthwindContext db)
        {
            // change tracking
            var product = db.Products
                //.AsNoTracking()
                .FirstOrDefault(p => p.ProductId == 1);

            if (product != null)
            {
                product.UnitsInStock += 10;

                db.SaveChanges();

                Console.WriteLine("veri güncelledi.");
            }
        }

        private static void Ders5(NorthwindContext db)
        {
            var category = db.Categories.Where(i => i.CategoryName == "Beverages").FirstOrDefault();

            var p1 = new Product() { ProductName = "yeni ürün 11" };
            var p2 = new Product() { ProductName = "yeni ürün 12" };

            //var products = new List<Product>()
            //{
            //    p1,p2
            //};

            category.Products.Add(p1);
            category.Products.Add(p2);

            db.SaveChanges();

            Console.WriteLine("veriler eklendi");
            Console.WriteLine(p1.ProductId);
            Console.WriteLine(p2.ProductId);
        }

        private static void Ders4(NorthwindContext db)
        {
            //var result = db.Products.Count();
            //var result = db.Products.Count(i=>i.UnitPrice>10 && i.UnitPrice<30);
            //var result = db.Products.Count(i => !i.Discontinued);

            //var result = db.Products.Min(p => p.UnitPrice);
            //var result = db.Products.Where(p=>p.CategoryId==2).Max(p => p.UnitPrice);

            //var result = db.Products.Where(p=>!p.Discontinued).Average(p => p.UnitPrice);
            //var result = db.Products.Where(p=>!p.Discontinued).Sum(p => p.UnitPrice);

            //var result = db.Products.OrderBy(p=>p.UnitPrice).ToList();
            //var result = db.Products.OrderByDescending(p => p.UnitPrice).ToList();
            var result = db.Products.OrderByDescending(p => p.UnitPrice).LastOrDefault();

            Console.WriteLine(result.ProductName + ' ' + result.UnitPrice);


            //foreach (var item in result)
            //{
            //    Console.WriteLine(item.ProductName +' '+item.UnitPrice);
            //}
        }

        private static void Ders3(NorthwindContext db)
        {
            // Tüm müşteri kayıtlarını getiriniz. (Customers)
            //var customers = db.Customers.ToList();

            //foreach (var customer in customers)
            //{
            //    Console.WriteLine(customer.ContactName);
            //}
            // Tüm müşterilerin sadece CustomerId ve ContactName kolonlarını getiriniz.

            //var customers = db.Customers.Select(c=> new { c.CustomerId, c.ContactName}).ToList();

            // Almanya'da yaşayan müşterilerin adlarını getiriniz.
            //var customers = db.Customers.Select(c => new { c.ContactName, c.Country })
            //    .Where(c => c.Country == "Germany")
            //    .ToList();

            // "Diego Roel" isimli müşteri nerede yaşamaktadır?
            //var customer = db.Customers.Where(c => c.ContactName == "Diego Roel").FirstOrDefault();
            //Console.WriteLine(customer.ContactName + " " + customer.CompanyName);
            // Stokta olmayan ürünler hangileridir?

            //var products = db.Products
            //    .Select(i=>new { i.ProductName, i.UnitsInStock})
            //    .Where(i => i.UnitsInStock == 0)
            //    .ToList();

            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.ProductName +" "+ p.UnitsInStock);
            //}
            // Tüm çalışanların ad ve soyadını tek kolon şeklinde getiriniz.

            //var employess = db.Employees
            //    .Select(i => new {
            //        FullName = i.FirstName +" " +i.LastName
            //    }).ToList();

            //foreach (var emp in employess)
            //{
            //    Console.WriteLine(emp.FullName);
            //}

            // Ürünler tablosundaki ilk 5 kaydı alınız.

            //var products = db.Products.Take(5).ToList();
            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.ProductName + " " + p.ProductId);
            //}


            // Ürünler tablosundaki ikinci 5 kaydı alınız. (Take,Skip)

            var products = db.Products.Skip(5).Take(5).ToList();
            foreach (var p in products)
            {
                Console.WriteLine(p.ProductName + " " + p.ProductId);
            }
        }

        private static void Ders2(NorthwindContext db)
        {
            //var products = db.Products.Where(p=>p.UnitPrice>18).ToList();
            //var products = db.Products.Select(p=>new { p.ProductName,p.UnitPrice }).Where(p=>p.UnitPrice>18).ToList();
            //var products = db.Products.Where(p => p.UnitPrice > 18 && p.UnitPrice<30).ToList();
            //var products = db.Products.Where(p => p.CategoryId >= 1 && p.CategoryId<=5).ToList();
            //var products = db.Products.Where(p => p.CategoryId == 1 || p.CategoryId==5).ToList();
            //var products = db.Products.Where(p => p.CategoryId==1).Select(p => new { p.ProductName, p.UnitPrice }).ToList();
            //var products = db.Products.Where(i => i.ProductName == "Chai").ToList();
            var products = db.Products.Where(i => i.ProductName.Contains("Ch")).ToList();

            foreach (var p in products)
            {
                Console.WriteLine(p.ProductName + ' ' + p.UnitPrice);
            }
        }

        private static void Ders1(NorthwindContext db)
        {
            //var products = db.Products.ToList();
            //var products = db.Products.Select(p => p.ProductName).ToList();
            //var products = db.Products.Select(p => new { p.ProductName,p.UnitPrice }).ToList();
            //var products = db.Products.Select(p => 
            //    new ProductModel 
            //    { 
            //        Name = p.ProductName,
            //        Price = p.UnitPrice 
            //    }).ToList();

            //var product = db.Products.First();
            var product = db.Products.Select(p => new { p.ProductName, p.UnitPrice }).FirstOrDefault();

            Console.WriteLine(product.ProductName + ' ' + product.UnitPrice);

            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.Name + ' ' + p.Price);
            //}
        }
    }
}
