using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Chats;
using E_Commerce.Core.Features.Chats.Commands.Models;
using E_Commerce.Core.Features.Chats.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/conversations")]
    [Authorize]
    [Produces("application/json")]
    public class ChatsController(
        ISender mediator,
        IUnitOfWork uow,
        ICurrentUserService cu) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ConversationDto>>), 200)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetConversationsQuery(page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpGet("{id:guid}/messages")]
        [ProducesResponseType(typeof(ApiResponse<ConversationPageDto>), 200)]
        public async Task<IActionResult> GetMessages(
            Guid id,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 30,
            CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetConversationMessagesQuery(id, page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<ConversationDto>), 201)]
        public async Task<IActionResult> Start([FromBody] StartConversationRequest req, CancellationToken ct)
        {
            var r = await mediator.Send(new StartConversationCommand(
                req.CompanyId, req.InitialMessage, req.Subject,
                req.RelatedProductId, req.RelatedOrderId, req.RelatedRfqId), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/messages")]
        [ProducesResponseType(typeof(ApiResponse<MessageDto>), 201)]
        public async Task<IActionResult> SendMessage(
            Guid id, [FromBody] SendMessageRequest req, CancellationToken ct)
        {
            var r = await mediator.Send(new SendMessageCommand(
                id, req.Content, req.Type, req.Attachments,
                req.ReplyToMessageId, req.CardDataJson), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/read")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> MarkRead(
            Guid id, [FromBody] MarkReadRequest req, CancellationToken ct)
        {
            var r = await mediator.Send(new MarkMessagesReadCommand(id, req.MessageIds), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/archive")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var conv = await uow.Conversations.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Conversation), id);

            bool isBuyer = conv.BuyerId == cu.UserId.Value;
            bool isCompanyMember = cu.OwnedCompanyId == conv.CompanyId;
            if (!isBuyer && !isCompanyMember)
                throw new ForbiddenException("You are not a participant in this conversation.");

            if (isBuyer) conv.ArchiveForBuyer();
            else conv.ArchiveForCompany();

            uow.Conversations.Update(conv);
            await uow.SaveChangesAsync(ct);
            return Ok(ApiResponse<object>.Ok("Conversation archived."));
        }

        [HttpPost("{id:guid}/unarchive")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Unarchive(Guid id, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var conv = await uow.Conversations.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Conversation), id);

            bool isBuyer = conv.BuyerId == cu.UserId.Value;
            bool isCompanyMember = cu.OwnedCompanyId == conv.CompanyId;
            if (!isBuyer && !isCompanyMember)
                throw new ForbiddenException("You are not a participant in this conversation.");

            if (isBuyer) conv.UnarchiveForBuyer();
            else conv.UnarchiveForCompany();

            uow.Conversations.Update(conv);
            await uow.SaveChangesAsync(ct);
            return Ok(ApiResponse<object>.Ok("Conversation unarchived."));
        }
    }
}
