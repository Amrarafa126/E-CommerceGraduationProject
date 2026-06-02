using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Service.Payment;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/webhooks")]
    [Produces("application/json")]
    public class WebhooksController(
        ISender mediator,
        IPaymobClient paymobClient,
        IOptions<PaymobOptions> paymobOptions) : ControllerBase
    {
        /// <summary>
        /// Paymob callback/webhook endpoint.
        /// Paymob sends transaction results here after payment completion.
        /// </summary>
        [HttpPost("paymob")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> PaymobWebhook([FromBody] JsonElement payload, CancellationToken ct)
        {
            try
            {
                // Parse Paymob payload
                var obj = payload.GetProperty("obj");
                var order = obj.GetProperty("order");

                var hmac = payload.TryGetProperty("hmac", out var hmacProp)
                    ? hmacProp.GetString() ?? string.Empty
                    : string.Empty;

                // Verify HMAC signature
                var payloadDict = ExtractPaymobPayloadForHmac(obj);
                var isValid = paymobClient.VerifyWebhookHmac(
                    paymobOptions.Value.HmacSecret,
                    payloadDict,
                    hmac);

                if (!isValid)
                {
                    return BadRequest(ApiResponse<object>.Fail("Invalid webhook signature.", 400));
                }

                var paymobOrderId = order.GetProperty("id").GetInt64();
                var success = obj.GetProperty("success").GetBoolean();
                var transactionId = obj.TryGetProperty("id", out var txnProp)
                    ? txnProp.GetInt64().ToString()
                    : null;
                var amountCents = obj.GetProperty("amount_cents").GetDecimal();
                var currency = obj.GetProperty("currency").GetString() ?? "EGP";

                var command = new ProcessPaymobWebhookCommand(
                    hmac, paymobOrderId, success, transactionId, amountCents, currency);

                var result = await mediator.Send(command, ct);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Invalid webhook payload: {ex.Message}",
                    StatusCode = 400
                });
            }
        }

        private static Dictionary<string, string> ExtractPaymobPayloadForHmac(JsonElement obj)
        {
            var dict = new Dictionary<string, string>();
            void Add(string key, string? value) { if (value != null) dict[key] = value; }

            Add("amount_cents", TryGetString(obj, "amount_cents"));
            Add("created_at", TryGetString(obj, "created_at"));
            Add("currency", TryGetString(obj, "currency"));
            Add("delivery_fees", TryGetString(obj, "delivery_fees"));
            Add("id", TryGetString(obj, "id"));
            Add("integration_id", TryGetString(obj, "integration_id"));
            Add("is_3d", TryGetString(obj, "is_3d"));
            Add("is_auth", TryGetString(obj, "is_auth"));
            Add("is_capture", TryGetString(obj, "is_capture"));
            Add("is_refunded", TryGetString(obj, "is_refunded"));
            Add("is_standalone", TryGetString(obj, "is_standalone"));
            Add("is_voided", TryGetString(obj, "is_voided"));

            if (obj.TryGetProperty("order", out var orderProp) && orderProp.ValueKind == JsonValueKind.Object)
                Add("order.id", TryGetString(orderProp, "id"));

            Add("owner", TryGetString(obj, "owner"));
            Add("pending", TryGetString(obj, "pending"));

            if (obj.TryGetProperty("source_data", out var sourceProp) && sourceProp.ValueKind == JsonValueKind.Object)
            {
                Add("source_data.pan", TryGetString(sourceProp, "pan"));
                Add("source_data.sub_type", TryGetString(sourceProp, "sub_type"));
                Add("source_data.type", TryGetString(sourceProp, "type"));
            }

            Add("success", TryGetString(obj, "success"));

            return dict;
        }

        private static string? TryGetString(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var prop))
                return null;

            return prop.ValueKind switch
            {
                JsonValueKind.String => prop.GetString(),
                JsonValueKind.Number => prop.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => "",
                _ => prop.GetRawText()
            };
        }

        /// <summary>
        /// Bosta webhook endpoint for delivery status updates.
        /// </summary>
        [HttpPost("bosta")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public IActionResult BostaWebhook([FromBody] JsonElement payload)
        {
            // Bosta webhook processing will be implemented when needed
            // For now, log and return success
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Bosta webhook received.",
                StatusCode = 200
            });
        }
    }
}
