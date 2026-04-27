//using E_Commerce.Core.BaseResponse;
//using E_Commerce.Core.Features.Authentication.Commands.Models;
//using E_Commerce.Data.Identity;
//using E_Commerce.Data.Result;
//using E_Commerce.Service.Interfase;
//using MediatR;
//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace E_Commerce.Core.Features.Authentication.Commands.Handlers
//{
//    public class AuthenticationCommandHandler : ResponseHandler,
//        IRequestHandler<SignInCommand, Response<JwtAuthResult>>,
//        IRequestHandler<RefreshTokenCommand, Response<JwtAuthResult>>
//    {
//        private readonly UserManager<User> _userManager;
//        private readonly SignInManager<User> _signInManager;
//        private readonly IAuthenticationService _authenticationService;
//        public AuthenticationCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager , IAuthenticationService authenticationService)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _authenticationService = authenticationService;
//        }
//        public async Task<Response<JwtAuthResult>> Handle(SignInCommand request, CancellationToken cancellationToken)
//        {//Check if user is exist or not
//            var user = await _userManager.FindByNameAsync(request.UserName);
//            //Return The UserName Not Found
//            if (user == null) return BadRequest<JwtAuthResult>("UserNameIsNotExist");
//            //try To Sign in 
//            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
//            //if Failed Return Passord is wrong
//            if (!signInResult.Succeeded) return BadRequest<JwtAuthResult>("PasswordNotCorrect");
//            //confirm email
//            if (!user.EmailConfirmed)
//                return BadRequest<JwtAuthResult>("EmailNotConfirmed");
//            //Generate Token
//            var result = await _authenticationService.GetJWTToken(user);
//            //return Token 
//            return Success(result);
//        }

//        public Task<Response<JwtAuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
