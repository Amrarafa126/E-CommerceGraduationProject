using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Companies.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Data.ValueObjects;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Companies.Commands.Handlers
{
    public class CompanyHandlersCommands(IUnitOfWork uow, IPasswordService pwd, ITokenService tok, IMapper mapper) : 
        IRequestHandler<RegisterSellerCommand,ApiResponse<string>>
    {
     
      

       
       public async Task<ApiResponse<string>>Handle(RegisterSellerCommand req, CancellationToken ct)
        {

            if (await uow.Users.ExistsAsync(u => u.Email == req.Email.ToLower(), ct))
                throw new ConflictException($"Email '{req.Email}' is already registered.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                // Step 1 – User
                var user = User.CreateSupplier(req.Email, pwd.HashPassword(req.Password),
                    req.FirstName, req.LastName, req.PhoneNumber);
                await uow.Users.AddAsync(user, ct);
                await uow.SaveChangesAsync(ct);

                // Step 2 – Company
                var address = new Address(req.Street, req.City, req.State, req.Country, req.PostalCode);
                var contact = new ContactInfo(req.ContactEmail, req.ContactPhone);
                var company = Company.Create(user.Id, req.CompanyName, req.CompanyDescription,
                    address, contact, req.YearEstablished, req.EmployeesCount);
                await uow.Companies.AddAsync(company, ct);
                await uow.SaveChangesAsync(ct);

                // Step 3 – Wallet
                var wallet = Wallet.Create(company.Id);
                await uow.Wallets.AddAsync(wallet, ct);

                // Step 4 – Link user → company
                user.AssignCompany(company.Id);
                var refresh = tok.GenerateRefreshToken();
                user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(30));
                uow.Users.Update(user);

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var userDto = mapper.Map<UserDto>(user);

                var accessToken = tok.GenerateAccessToken(user);
                var expiry = DateTime.UtcNow.AddMinutes(60);

               var response = new AuthResponseDto(accessToken, refresh, expiry, userDto);

            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
            return ApiResponse<string>.Created("Seller registration successful. Awaiting company approval.");

        }

      
    } 
    }

