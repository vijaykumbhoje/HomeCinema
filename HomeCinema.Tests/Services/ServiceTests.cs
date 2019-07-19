using HomeCinema.Data;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Services.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Tests.Helper;
using HomeCinema.Services;

namespace HomeCinema.Tests.Controller
{
    [TestFixture]
    public class ServiceTests
    {
        private IMembershipServices _membershipServices;
        private IUnitOfWork _unitOfWork;
        private IQueryable<User> _iQUsers;
        private List<User> _users;
        private EntityBaseRepository<User> _userRepository;

        private HomeCinemaContext _dbEntities;
        private EncryptionService _encryptionService;
        string correctUserName = "vijay";
        string correctPassword = "Test123+";
        string hashedPassword = "Rgxz6zaDQ+SSe/eSJL5+csLjWwx4wSPozXku3BoLUgw=";
        string wrongUserName = "Vijay1";
        string wrongPassword = "Test1234";

        [SetUp]
        public void SetUp()
        {
            _users = SetUpUser();
            _dbEntities = new Mock<HomeCinemaContext>().Object;
           var userRepository = new Mock<IEntityBaseRepository<User>>();
            var roleRepository = new Mock<IEntityBaseRepository<Role>>();
            var userRoleRepository = new Mock<IEntityBaseRepository<UserRole>>();
            var encryptionService = new Mock<IEncryptionService>();
            var unitofWork = new Mock<IUnitOfWork>();

            unitofWork.SetupGet(s => s.userRepository).Returns(_userRepository);
            _unitOfWork = unitofWork.Object;
            _membershipServices = new MembershipService(userRepository.Object, roleRepository.Object, userRoleRepository.Object, encryptionService.Object, _unitOfWork);
        }

        [Test]
        public void LoginServiceTest()
        {
            var returnId = _membershipServices.ValidateUser(correctUserName, correctPassword);

            var firstOrDefault = _users.Where(u => u.UserName == correctUserName).FirstOrDefault(u => u.HashedPassword == hashedPassword);
            if (firstOrDefault != null)
                Assert.That(returnId, Is.EqualTo(firstOrDefault.Id));
        }

        [Test]
        public void WrongCredentialsLoginServiceTest()
        {
            var returnId = _membershipServices.ValidateUser(wrongUserName, wrongPassword);

            var firstOrDefault = _users.Where(u => u.UserName == correctUserName).FirstOrDefault(u => u.HashedPassword == wrongPassword);
            if (firstOrDefault != null)
                Assert.That(returnId, firstOrDefault!=null ? Is.EqualTo(firstOrDefault.Id): Is.EqualTo(0));
        }

        


        #region 'Private Methods'

        private EntityBaseRepository<User> SetUpUserRepository()
        {
            var mockRepo = new Mock<EntityBaseRepository<User>>(MockBehavior.Default, _dbEntities);
            mockRepo.Setup(u => u.GetAll()).Returns(_iQUsers);
            if(_iQUsers!=null)
            {
                _users = _iQUsers.ToList();                    
            }
            mockRepo.Setup(u => u.GetSingle(It.IsAny<int>()))
                .Returns(new Func<int, User>(id => _users.Find(p => p.Id.Equals(id))));




            return mockRepo.Object;
        }
        private static List<User> SetUpUser()
        {
            var userId = new int();
            var users = DataInitializer.GetAllUsers();
            foreach (User u in users)
                u.Id = ++userId;
            return users;
        }

        #endregion
    }
}
