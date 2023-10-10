using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenAI_API;
using webapi.Models;
using webapi.OpenAI;

namespace webapi.Contracts
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly OpenAIAPI _api;

        public ContractController(DataContext context, IOptions<AppSettings> appSettings, OpenAIAPI api)
        {
            _context = context;
            _appSettings = appSettings;
            _api = api;
        }

        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetContractAsync(Guid id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            return Ok(contract);
        }

        [HttpPost]
        [Route("prompt")]
        public async Task<IActionResult> PromptAsync([FromBody] ContractPromptRequest request)
        {
            var contract = await _context.Contracts.FindAsync(request.ContractId);
            var chat = _api.Chat.CreateConversation();
            chat.AppendSystemMessage($"You will analyze the contract based on the provided questions from the user and provide feedback for the following contract: {contract.Content}");
            chat.AppendUserInput(request.Prompt);
            var response = await chat.GetResponseFromChatbotAsync();
            return Ok(new PromptResponse { Response = response });
        }
    }
}
