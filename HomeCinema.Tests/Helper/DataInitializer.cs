using HomeCinema.Entities;
using HomeCinema.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Tests.Helper
{
   public class DataInitializer
    {
        EncryptionService encryptionService = new EncryptionService();
        public static List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    Id=1,
                    FirstName= "Mohammad",
                    LastName="Yundt",
                    IdentityCard= "979c8829-4c22-41f6-ad84-ff2854636b3f",
                    DateofBirth= Convert.ToDateTime("1985-10-30 00:00:00.000"),
                    UniqueKey = Guid.Parse("DB9D976B-3286-4891-B1C0-0BEDED0165DC"),
                    Mobile = "1234567890",
                    Email = "izaiah@collinscarroll.uk",
                    RegistrationDate = Convert.ToDateTime("2019-07-10 14:34:35.810"),
                },
                 new Customer
                {
                    Id=2,
                    FirstName= "Dina",
                    LastName="Reichel",
                    IdentityCard= "588e06f5-9457-4feb-a6b0-966c2896728a",
                    DateofBirth= Convert.ToDateTime("1985-11-30 00:00:00.000"),
                    UniqueKey = Guid.Parse("9DED27C7-F9A7-4411-9438-0A5EF70E8FCA"),
                    Mobile = "1234567890",
                    Email = "braeden@kira.com",
                    RegistrationDate = Convert.ToDateTime("2019-07-11 14:34:35.817"),
                },
                  new Customer
                {
                    Id=3,
                    FirstName= "Raphaelle",
                    LastName="Stroman",
                    IdentityCard= "b524de9b-e40b-4d8f-ac8b-3fbb250f1fc6",
                    DateofBirth= Convert.ToDateTime("1985-12-30 00:00:00.000"),
                    UniqueKey = Guid.Parse("E533F23F-A424-40B6-8C05-DEC43484A6CD"),
                    Mobile = "1234567890",
                    Email = "tyrell.kris@ornvolkman.us",
                    RegistrationDate = Convert.ToDateTime("2019-07-12 14:34:35.820")
                }
            };
            return customers;
        }

        public static List<User> GetAllUsers()
        {
            var users = new List<User>
            {
                 new User
                {
                    UserName = "chsakell",
                    Email ="chsakells.blog@gmail.com",
                    Salt = "mNKLRbEFCH8y1xIyTXP4qA==",
                    HashedPassword = "XwAQoiq84p1RUzhAyPfaMDKVgSwnn80NCtsE8dNv3XI=",
                    IsLocked= false,
                    DateCreated = Convert.ToDateTime("2019-07-10 14:34:36.140")
                },
                new User
                {
                    UserName = "Vijay",
                    Email ="vijay_kumbhoje@hotmail.com",
                    Salt = "praPtFx8TYUNGAeypkzS2g==",
                    HashedPassword = "Rgxz6zaDQ+SSe/eSJL5+csLjWwx4wSPozXku3BoLUgw=",
                    IsLocked= false,
                    DateCreated = Convert.ToDateTime("2019-07-10 14:35:42.073")
                }
               
            };
            return users;
        }
    }
}
