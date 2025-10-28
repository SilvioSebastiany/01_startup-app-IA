using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel.ChatCompletion;

namespace _01_startup_app.Controllers;

[ApiController]
[Route("[controller]")]
public class Mensagens : ControllerBase
{
    private readonly IChatCompletionService _chatCompletionService;

    public Mensagens(IChatCompletionService chatCompletionService)
    {
        _chatCompletionService = chatCompletionService;
    }

    public record MessageRequest(string Prompt);
    public record MessageResponse(string Response);

    private ChatHistory History { get; set; } = [];
    private bool IsBusy { get; set; }

    [HttpPost("send")]
    public async Task<ActionResult<MessageResponse>> SendAsync([FromBody] MessageRequest request)
    {
        IsBusy = true;

        History.AddUserMessage("Você é um assistente virtual especializado em ajudar usuários com suas dúvidas. " +
                                "Nós somos uma escola de programação online com foco em tecnologia Microsoft." +
                                "Tente responder sempre de forma clara e resumida." +
                                "As respostas deves ser de facil entendimento.");

        History.AddUserMessage(request.Prompt);

        var response = await _chatCompletionService.GetChatMessageContentAsync(History);
        History.Add(response);

        IsBusy = false;

        if (response?.Content == null)
            return StatusCode(500, new MessageResponse("No response content received"));

        return Ok(new MessageResponse(response.Content));
    }
}
