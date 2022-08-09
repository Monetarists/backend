
using XIVMarketBoard_Api.Entities;
using XIVMarketBoard_Api.Repositories.Models.Users;
using XIVMarketBoard_Api.Authorization;

using XIVMarketBoard_Api.Data;

using System.Reactive.Linq;
using static BCrypt.Net.BCrypt;

using XIVMarketBoard_Api.Helpers;
using AutoMapper;

namespace XIVMarketBoard_Api.Controller
{
    public interface IUserController
    {
        AuthenticateResponse Authenticate(AuthRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
        void Register(RegisterRequest model);
        void Update(int id, UpdateRequest model);
        void Delete(int id);
    }
    public class UserController : IUserController
    {
        private readonly XivDbContext _xivContext;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;

        public UserController(
            XivDbContext xivContext,
            IJwtUtils jwtUtils,
            IMapper mapper)
        {
            _xivContext = xivContext;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
        }

        public AuthenticateResponse Authenticate(AuthRequest model)
        {
            var user = _xivContext.Users.SingleOrDefault(x => x.UserName == model.UserName);
            // validate

            if (user == null || !Verify(model.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            // authentication successful
            var response = _mapper.Map<AuthenticateResponse>(user);
            response.Token = _jwtUtils.GenerateToken(user);
            return response;
        }

        public IEnumerable<User> GetAll()
        {
            return _xivContext.Users;
        }

        public User GetById(int id)
        {
            return getUser(id);
        }

        public void Register(RegisterRequest model)
        {
            // validate
            if (_xivContext.Users.Any(x => x.UserName == model.UserName))
                throw new AppException("Username '" + model.UserName + "' is already taken");

            // map model to new user object
            var user = _mapper.Map<User>(model);

            // hash password
            user.PasswordHash = HashPassword(model.Password);

            // save user
            _xivContext.Users.Add(user);
            _xivContext.SaveChanges();
        }

        public void Update(int id, UpdateRequest model)
        {
            var user = getUser(id);

            // validate
            if (model.UserName != user.UserName && _xivContext.Users.Any(x => x.UserName == model.UserName))
                throw new AppException("Username '" + model.UserName + "' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                user.PasswordHash = HashPassword(model.Password);

            // copy model to user and save
            _mapper.Map(model, user);
            _xivContext.Users.Update(user);
            _xivContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = getUser(id);
            _xivContext.Users.Remove(user);
            _xivContext.SaveChanges();
        }

        // helper methods

        private User getUser(int id)
        {
            var user = _xivContext.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
    }

}
