using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;

namespace _01_startup_app.Controllers;

[ApiController]
[Route("[controller]")]
public class Mensagens : ControllerBase
{
    private readonly ILogger<Mensagens> _logger;
    private readonly IChatCompletionService _chatCompletionService;

    public Mensagens(ILogger<Mensagens> logger, IChatCompletionService chatCompletionService)
    {
        _logger = logger;
        _chatCompletionService = chatCompletionService;
    }

    public record MessageRequest(string Prompt);
    public record MessageResponse(string Response);

    [HttpPost("send")]
    public async Task<ActionResult<MessageResponse>> SendAsync([FromBody] MessageRequest request)
    {
        try
        {
            _logger.LogInformation("Processing message: {Prompt}", request.Prompt);
            
            var response = await _chatCompletionService.GetChatMessageContentAsync(request.Prompt);
            
            if (response?.Content == null)
            {
                return StatusCode(500, new MessageResponse("No response content received"));
            }
            
            return Ok(new MessageResponse(response.Content));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            return StatusCode(500, new MessageResponse("Error processing your request"));
        }
    }
}
