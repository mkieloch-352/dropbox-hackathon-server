using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;

namespace webapi.OpenAI
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromptController : ControllerBase
    {
        private readonly OpenAIAPI _api;

        public PromptController(OpenAIAPI api)
        {
            _api = api;
        }

        [HttpPost]
        public async Task<IActionResult> PromptAsync([FromBody] PromptRequest request)
        {
            var chat = _api.Chat.CreateConversation();
            chat.AppendSystemMessage("You provide sample contracts that are straightforward to read and follow with minimal legal jargon based on user input.");
            chat.AppendUserInput(request.Prompt);
            var response = await chat.GetResponseFromChatbotAsync();
            return Ok(new PromptResponse { Response = response });
        }
    }
}
