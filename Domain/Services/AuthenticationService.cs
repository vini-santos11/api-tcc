using CrossCutting.Configurations;
using Domain.Commands.User;
using Domain.Enumerables;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Querys;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Domain.Services
{
    public class AuthenticationService
    {
        private IUserRoleRepository UserRoleRepository { get; }
        private TokenConfiguration TokenConfiguration { get; }
        private SignInManager<AppUser> SignInManager { get; }
        private UserManager<AppUser> UserManager { get; }
        private IUserRepository UserRepository { get; }
        private ContactService ContactService { get; }
        public AuthenticationService(
            IOptions<TokenConfiguration> tokenConfiguration,
            IUserRoleRepository userRoleRepository,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IUserRepository userRepository,
            ContactService contactService)
        {
            TokenConfiguration = tokenConfiguration.Value;
            UserRoleRepository = userRoleRepository;
            UserRepository = userRepository;
            ContactService = contactService;
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public string GenerateRefreshToken(UserTokenQuery userInfo)
        {
            var randomNumber = new byte[32];
            using var generatorNumber = RandomNumberGenerator.Create();
            generatorNumber.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber);

            var user = UserRepository.Find(Convert.ToUInt32(userInfo.UserId));
            user.RefreshToken = refreshToken;
            UserRepository.Update(user);

            return refreshToken;
        }

        public UserTokenQuery GenerateToken(AppUser user, ERoles role)
        {
            var createDate = DateTime.Now;
            var expirationDate = createDate.AddSeconds(TokenConfiguration.Seconds);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, Convert.ToString(user.Id)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Iat, createDate.ToString("yyyy-MM-dd HH:mm:ss")),
                new Claim(JwtRegisteredClaimNames.Exp, expirationDate.ToString("yyyy-MM-dd HH:mm:ss")),
                new Claim(ClaimTypes.GivenName, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.Surname ?? string.Empty),
                new Claim(ClaimTypes.UserData, Convert.ToString(user.Id)),
                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)),
                new Claim(ClaimTypes.Role, role?.Name ?? string.Empty)
            };

            var identity = new ClaimsIdentity(new GenericIdentity(Convert.ToString(user.Id), "Auth"), claims);
            var handler = new JwtSecurityTokenHandler();

            var securityAccessToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = TokenConfiguration.Issuer,
                Audience = TokenConfiguration.Audience,
                SigningCredentials = TokenConfiguration.SigningCredentials,
                Subject = identity,
                NotBefore = createDate,
                Expires = expirationDate
            });

            var userInfo = new UserTokenQuery
            {
                UserId = Convert.ToString(user.Id),
                AccessToken = handler.WriteToken(securityAccessToken),
            };

            userInfo.RefreshToken = String.IsNullOrEmpty(userInfo.RefreshToken) ? GenerateRefreshToken(userInfo) : userInfo.RefreshToken;

            return userInfo;
        }

        private async Task<UserTokenQuery> ValidateLogin(string login, string roleName, string password = null)
        {
            var tokenSource = new CancellationTokenSource();
            try
            {
                var user = await UserRepository.FindByNameAsync(login, tokenSource.Token) ?? throw new Exception();

                if (UserManager.IsLockedOutAsync(user).Result)
                    throw new ValidateException(Messages.BlockedUser);

                var defaultDate = DateTime.Now;
                if (user.LockedEndDate.GetValueOrDefault(defaultDate) < defaultDate)
                    throw new ValidateException(Messages.BlockedUser);

                if (user.ChangePassword)
                    throw new ValidateException(Messages.PasswordChangeRequired);

                var roles = await UserRepository.GetRolesAsync(user, tokenSource.Token);
                if ((roles == null) && (roles.Count == 1) && (string.IsNullOrEmpty(roleName)))
                    roleName = roles[0];

                if ((roles == null) || (!roles.Contains(roleName?.ToUpper())))
                    throw new ValidateException(Messages.InvalidProfile);
                if (!string.IsNullOrEmpty(password))
                {
                    var result = await SignInManager.PasswordSignInAsync(login, password, false, true);
                    if (!result.Succeeded)
                        throw new ValidateException(Messages.InvalidUserOrPassword);
                }
                else
                    await SignInManager.SignInAsync(user, true, null);

                user.LastAccessDate = DateTime.Now;
                UserRepository.Update(user);

                return GenerateToken(user, ERoles.FindByName<ERoles>(roleName));
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        private static ValidateException MessageRequired(string fieldName)
        {
            return new ValidateException(string.Format(Messages.FieldRequired, fieldName));
        }

        public async Task<UserTokenQuery> RegisterUser(RegisterCommand command)
        {
            if (string.IsNullOrEmpty(command.Name))
                throw MessageRequired("Nome");

            if (string.IsNullOrEmpty(command.SecondName))
                throw MessageRequired("Sobrenome");

            if (string.IsNullOrEmpty(command.DocumentNumber))
                throw MessageRequired("Documento");

            if (string.IsNullOrEmpty(command.Password))
                throw MessageRequired("Senha");

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var tokenSource = new CancellationTokenSource();
            try
            {
                var user = await UserRepository.FindByNameAsync(command.DocumentNumber, tokenSource.Token);
                if (user != null)
                    throw new ValidateException(Messages.AlreadyRegisteredUser);

                var contact = ContactService.FindByDocumentNumber(command.DocumentNumber);
                if (contact == null)
                    contact = ContactService.CreateContactFromRegister(command);
                else
                    contact = ContactService.UpdateContactFromRegister(contact.Id, command);

                user = new AppUser
                {
                    Id = contact.Id,
                    LastAccessDate = DateTime.Now
                };

                var result = await UserManager.CreateAsync(user, command.Password);

                if (!result.Succeeded)
                    throw new ConflictedException(Messages.ErrorRegisteringUser, result.Errors);

                var userRole = new AppUserRole
                {
                    UserId = user.Id,
                    RoleId = ERoles.User.Id
                };

                if (!UserRoleRepository.Exists(userRole))
                    UserRoleRepository.Add(userRole);

                scope.Complete();
                return await Task.FromResult(GenerateToken(user, ERoles.User));
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        public async Task<UserTokenQuery> AuthenticateUser(AccessCommand command)
        {
            return await ValidateLogin(command.Login, command.Role, command.Password);
        }

        public async Task<UserTokenQuery> AuthenticateRefreshToken(RefreshCommand command)
        {
            return await ValidateLogin(command.RefreshToken, command.Role);
        }
    }
}
