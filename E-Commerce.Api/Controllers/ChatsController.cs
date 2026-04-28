using E_Commerce.Core.Features.Chats;
using E_Commerce.Core.Features.Chats.Commands.Models;
using E_Commerce.Core.Features.Chats.Oueries.Models;
using E_Commerce.Core.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/conversations")]
    [Authorize]
    [Produces("application/json")]
    public class ChatsController(ISender mediator) : ControllerBase
    {

        [HttpGet("Get-All-Conversations")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ConversationDto>>), 200)]
        public async Task<IActionResult> GetAll(
         [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
         CancellationToken ct = default)
        {
            var r = await mediator.Send(new GetConversationsQuery(page, pageSize), ct);
            return StatusCode(r.StatusCode, r);
        }

        /// <summary>Get paginated messages for a specific conversation.</summary>
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

     
        [HttpPost("Start-Conversation")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ApiResponse<ConversationDto>), 201)]
        public async Task<IActionResult> Start([FromBody] StartConvRequest req, CancellationToken ct)
        {
            var r = await mediator.Send(new StartConversationCommand(req.CompanyId, req.InitialMessage), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/messages")]
        [ProducesResponseType(typeof(ApiResponse<MessageDto>), 201)]
        public async Task<IActionResult> SendMessage(
            Guid id, [FromBody] SendMessageDto dto, CancellationToken ct)
        {
            var r = await mediator.Send(
                new SendMessageCommand(id, dto.Content, dto.AttachmentUrl, dto.Type), ct);
            return StatusCode(r.StatusCode, r);
        }

        [HttpPost("{id:guid}/read")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
        {
            var r = await mediator.Send(new MarkMessagesReadCommand(id), ct);
            return StatusCode(r.StatusCode, r);
        }
    }
    public record StartConvRequest(Guid CompanyId, string InitialMessage);
}

