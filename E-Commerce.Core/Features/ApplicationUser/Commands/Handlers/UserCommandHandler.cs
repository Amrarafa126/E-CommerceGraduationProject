//using AutoMapper;
//using E_Commerce.Core.BaseResponse;
//using E_Commerce.Core.Features.ApplicationUser.Commands.Models;
//using E_Commerce.Data.Identity;
//using E_Commerce.Service.Interfase;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;


//namespace E_Commerce.Core.Features.ApplicationUser.Commands.Handlers
//{
//    public class UserCommandHandler : 
//        IRequestHandler<AddUserCommand, ApiResponse<string>>,
//        IRequestHandler<EditUserCommand, Response<string>>,
//        IRequestHandler<DeleteUserCommand, Response<string>>
//        ,IRequestHandler<ChangeUserPasswordCommand, Response<string>>
//    {
//        private readonly IMapper _mapper;
//        private readonly UserManager<User> _userManager;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly IApplicationUserService _applicationUserService;
//        public UserCommandHandler(IMapper mapper, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IApplicationUserService applicationUserService)
//        {
//            _mapper = mapper;
//            _userManager = userManager;
//            _httpContextAccessor = httpContextAccessor;
//            _applicationUserService = applicationUserService;
//        }

//        public async Task<Response<string>> Handle(AddUserCommand request, CancellationToken cancellationToken)
//        {
//            var identityUser = _mapper.Map<User>(request);
//            //Create
//            var createResult = await _applicationUserService.AddUserAsync(identityUser, request.Password);
//            switch (createResult)
//            {
//                case "EmailIsExist": return BadRequest<string>("EmailIsExist");
//                case "UserNameIsExist": return BadRequest<string>("UserNameIsExist");
//                case "ErrorInCreateUser": return BadRequest<string>("FaildToAddUser");
//                case "Failed": return BadRequest<string>("TryToRegisterAgain");
//                case "Success": return Success<string>("");
//                default: return BadRequest<string>(createResult);
//            }
//        }

//        public async Task<Response<string>> Handle(EditUserCommand request, CancellationToken cancellationToken)
//        {
//            var oldUser = await _userManager.FindByIdAsync(request.Id.ToString());
//            //if Not Exist notfound
//            if (oldUser == null) return NotFound<string>();
//            //mapping
//            var newUser = _mapper.Map(request, oldUser);
//            //if username is Exist
//            var userByUserName = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == newUser.UserName && x.Id != newUser.Id);
//            //username is Exist
//            if (userByUserName != null) return BadRequest<string>("UserNameIsExist");
//            //update
//            var result = await _userManager.UpdateAsync(newUser);
//            //result is not success
//            if (!result.Succeeded) return BadRequest<string>("UpdateFailed");
//            //message
//            return Success<string>("Updated");
//        }

//        public async Task<Response<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
//        {
//            //check if user is exist
//            var user = await _userManager.FindByIdAsync(request.Id.ToString());
//            //if Not Exist notfound
//            if (user == null) return NotFound<string>();
//            //Delete the User
//            var result = await _userManager.DeleteAsync(user);
//            //in case of Failure
//            if (!result.Succeeded) return BadRequest<string>("DeletedFailed");
//            return Success("Deleted");
//        }

//        public async Task<Response<string>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
//        {
//            //get user
//            //check if user is exist
//            var user = await _userManager.FindByIdAsync(request.Id.ToString());
//            //if Not Exist notfound
//            if (user == null) return NotFound<string>();

//            //Change User Password
//            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
//            //var user1=await _userManager.HasPasswordAsync(user);
//            //await _userManager.RemovePasswordAsync(user);
//            //await _userManager.AddPasswordAsync(user, request.NewPassword);

//            //result
//            if (!result.Succeeded) return BadRequest<string>(result.Errors.FirstOrDefault().Description);
//            return Success("ChangedPassword");
//        }
//    }
//}