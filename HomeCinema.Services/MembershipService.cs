using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using HomeCinema.Services.Abstract;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Extensions;
using HomeCinema.Services.Utilities;

namespace HomeCinema.Services
{
   public class MembershipService : IMembershipServices
    {
        #region 'veriables'
        private readonly IEntityBaseRepository<User> _userRepository;
        private readonly IEntityBaseRepository<Role> _roleRepository;
        private readonly IEntityBaseRepository<UserRole> _userRoleRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        public MembershipService(IEntityBaseRepository<User> userRepository, 
            IEntityBaseRepository<Role> roleRepository, IEntityBaseRepository<UserRole> userRoleRepository,
            IEncryptionService encryptionService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
        }

        #region helper methods
        private void addUserToRole(User user, int roleId)
        {
            var role = _roleRepository.GetSingle(roleId);
            if (role == null)
                throw new ApplicationException("Role doesn't Exists!");

            var userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            };
            _userRoleRepository.Add(userRole);    
        }

        private bool isPasswordValid(User user, string password)
        {
            return string.Equals(_encryptionService.EncryptPassword(password, user.Salt), user.HashedPassword);
        }

        private bool isUserValid(User user, string password)
        {
            if(isPasswordValid(user, password))
            {
                return !user.IsLocked;
            }
            return false;   
        }
        #endregion

        public User CreateUser(string username, string password, string email, int[] roles)
        {
            var existingUser = _userRepository.GetSingleByUsername(username);
            if(existingUser!=null)
            {
                throw new Exception("Username is already in use.");
            }

            var passwordSalt = _encryptionService.CreateSalt();

            var user = new User()
            {
                UserName = username,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = _encryptionService.EncryptPassword(password, passwordSalt),
                DateCreated = DateTime.Now
            };
            _userRepository.Add(user);
            _unitOfWork.Commit();

            if(roles!=null || roles.Length>0)
            {
                foreach(var role in roles)
                {
                    addUserToRole(user, role);
                }
            }
            _unitOfWork.Commit();
            return user;
        }

        public User GetUser(int userId)
        {
            return _userRepository.GetSingle(userId);
        }

        public List<Role> GetUserRoles(string username)
        {
            List<Role> _results = new List<Role>();
            var existingUser = _userRepository.GetSingleByUsername(username);
            if(existingUser!=null)
            {
                foreach(var role in existingUser.UserRoles)
                {
                    _results.Add(role.Role);
                }
            }
            return _results.Distinct().ToList();
        }

        public MembershipContext ValidateUser(string username, string password)
        {
            var membershipCtx = new MembershipContext();
            var user = _userRepository.GetSingleByUsername(username);
            if(user!=null && isUserValid(user, password))
            {
                var userRoles = GetUserRoles(user.UserName);
                membershipCtx.User = user;

                var Identity = new GenericIdentity(user.UserName);
                membershipCtx.Principle = new GenericPrincipal(Identity, userRoles.Select(x => x.Name).ToArray());
            }
            return membershipCtx;
        }
    }
}
