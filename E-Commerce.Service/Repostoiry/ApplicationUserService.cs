using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Repostoiry
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailsService _emailsService;
        private readonly AppDBContext _applicationDBContext;
        public ApplicationUserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IEmailsService _emailsService
            , AppDBContext _applicationDBContext)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            this._emailsService = _emailsService;
            this._applicationDBContext = _applicationDBContext;

        }
        public async Task<string> AddUserAsync(User user, string password)
        {
            var trans = await _applicationDBContext.Database.BeginTransactionAsync();
            try
            {
                //if Email is Exist
                var existUser = await _userManager.FindByEmailAsync(user.Email);
                //email is Exist
                if (existUser != null) return "EmailIsExist";

                //if username is Exist
                var userByUserName = await _userManager.FindByNameAsync(user.UserName);
                //username is Exist
                if (userByUserName != null) return "UserNameIsExist";
                //Create
                var createResult = await _userManager.CreateAsync(user, password);
                //Failed
                if (!createResult.Succeeded)
                    return string.Join(",", createResult.Errors.Select(x => x.Description).ToList());

                await _userManager.AddToRoleAsync(user, "User");

                //Send Confirm Email
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var resquestAccessor = _httpContextAccessor.HttpContext.Request;
                //var returnUrl = resquestAccessor.Scheme + "://" + resquestAccessor.Host + _urlHelper.Action("ConfirmEmail", "Authentication", new { userId = user.Id, code = code });
                //var message = $"To Confirm Email Click Link: <a href='{returnUrl}'>Link Of Confirmation</a>";
                ////$"/Api/V1/Authentication/ConfirmEmail?userId={user.Id}&code={code}";
                ////message or body
                //await _emailsService.SendEmail(user.Email, message, "ConFirm Email");

                await trans.CommitAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return "Failed";
            }

        }

    }
    }

