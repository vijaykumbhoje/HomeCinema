using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Data.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using HomeCinema.Data;
using Moq;
using HomeCinema.Models;
using HomeCinema.Controllers;
using HomeCinema.Services.Abstract;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using HomeCinema.Services;

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace HomeCinema.Tests.Controller
{
    [TestClass]
    public class AccountControllerTest
    {
       
        Mock<EntityBaseRepository<UserRole>> userRoleRepo = new Mock<EntityBaseRepository<UserRole>>();
      
        Mock<EncryptionService> encryptionService = new Mock<EncryptionService>();
        Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
        Mock<IEntityBaseRepository<Error>> _errorRepo = new Mock<IEntityBaseRepository<Error>>();
        private HomeCinemaContext _dbContext;
       
        

        [TestMethod]
        [Description("Method Should Return ValidStatus for Login")]
        public void Authenticate_Login_ShouldLogin()
        {
            //Arrange
            var userRepo = SetUserRepository();
            var roleRepo = setRoleRepository();
            var memberService = new Mock<MembershipService>(userRepo, roleRepo, userRoleRepo.Object, encryptionService.Object, _unitOfWork.Object);
            LoginViewModel loginVm = new LoginViewModel
            {
                Username="Vijay",
                Password = "Test123+"
            };
            var controller = new AccountController(memberService.Object, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());

            //Act
            var response = controller.Login(controller.Request, loginVm);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.IsTrue((bool)obj["success"]);
        }
        
        [TestMethod]
        
        public void Authenticate_FalseCredentials_ShouldNotLogin()
        {
            //Arrange
            var userRepo = SetUserRepository();
            var roleRepo = setRoleRepository();
            var memberService = new Mock<MembershipService>(userRepo, roleRepo, userRoleRepo.Object, encryptionService.Object, _unitOfWork.Object);          
            LoginViewModel loginVm = new LoginViewModel
            {
                Username = "Vijay",
                Password = "Test1234"
            };
            var controller = new AccountController(memberService.Object, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());

            //Act
            var response = controller.Login(controller.Request, loginVm);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.IsFalse((bool)obj["success"]);

        }

        [TestMethod]       
        public void Register_RegisterUser_ShouldGetRegistered()
        {
            //Arrange
            var userRepo = SetUserRepository();
            var roleRepo = setRoleRepository();

            var memberService = new Mock<MembershipService>(userRepo, roleRepo, userRoleRepo.Object, encryptionService.Object, _unitOfWork.Object);
            RegistrationViewModel registrationVm = new RegistrationViewModel
            {
                Username = "ezest",
                Password = "Test123+",
                Email = "vijay@e-zest.in"                
            };
            var controller = new AccountController(memberService.Object, _errorRepo.Object, _unitOfWork.Object);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());

            //Act
            var response = controller.Register(controller.Request, registrationVm);
            var responseString = GetResponseString(response);
            JObject obj = JObject.Parse(responseString.Result);

            //Assert
            Assert.IsTrue( (bool)obj["success"]);
          
        }

        public EntityBaseRepository<User> SetUserRepository()
        {
            var userRepo = new Mock<EntityBaseRepository<User>>(MockBehavior.Default, _dbContext);
            List<User> userList = new List<User>();
            userRoleRepo = new Mock<EntityBaseRepository<UserRole>>(MockBehavior.Default, _dbContext);
            List<UserRole> uRole = new List<UserRole>();
            uRole.Add(new UserRole { Id = 1, UserId = 1, RoleId = 1 });
            uRole.Add(new UserRole { Id = 1, UserId = 2, RoleId = 1 });
            userRoleRepo.Setup(ur => ur.GetAll()).Returns(uRole.AsQueryable());

            userList.Add(new User { UserName = "chsakell", Email = "chsakells.blog@gmail.com", HashedPassword = "XwAQoiq84p1RUzhAyPfaMDKVgSwnn80NCtsE8dNv3XI=", Salt = "mNKLRbEFCH8y1xIyTXP4qA==", IsLocked = false });
            userList.Add(new User { UserName = "Vijay", Email = "vijay_kumbhoje@hotmail.com", HashedPassword = "Rgxz6zaDQ+SSe/eSJL5+csLjWwx4wSPozXku3BoLUgw=", Salt = "praPtFx8TYUNGAeypkzS2g==", IsLocked = false });
            userRepo.Setup(u => u.GetAll()).Returns(userList.AsQueryable());
            return userRepo.Object;
        }

        #region 'Private Methods'
        public EntityBaseRepository<Role> setRoleRepository()
        {
            List<Role> role = new List<Role>();
            role.Add(new Role { Id = 1, Name = "Admin" });
            var roleRepo = new Mock<EntityBaseRepository<Role>>(MockBehavior.Default, _dbContext);
            roleRepo.Setup(r => r.GetAll()).Returns(role.AsQueryable());
            return roleRepo.Object;
        }

        public async Task<string> GetResponseString(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

        public JObject returnJString(string task)
        {

            JArray jArray = JArray.Parse(task);
            JObject obj = JObject.Parse(jArray[0].ToString());
            return obj;
        }
        #endregion
    }
}
