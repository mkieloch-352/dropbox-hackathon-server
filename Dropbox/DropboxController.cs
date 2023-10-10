using Dropbox.Sign.Api;
using Dropbox.Sign.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using webapi.Models;

namespace webapi.Dropbox
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropboxController : ControllerBase
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly DataContext _context;
        private readonly EmbeddedApi _embeddedApi;
        private readonly SendGridClient _sendGridClient;
        private readonly SignatureRequestApi _signatureRequestApi;
        private readonly TemplateApi _templateApi;

        public DropboxController(IOptions<AppSettings> appSettings, DataContext context, EmbeddedApi embeddedApi, SendGridClient sendGridClient, SignatureRequestApi signatureRequestApi, TemplateApi templateApi)
        {
            _appSettings = appSettings;
            _context = context;
            _embeddedApi = embeddedApi;
            _sendGridClient = sendGridClient;
            _signatureRequestApi = signatureRequestApi;
            _templateApi = templateApi;
        }

        [HttpPost]
        [Route("template")]
        public async Task<IActionResult> CreateTemplateAsync([FromBody] TemplateRequest request)
        {
            ChromePdfRenderer renderer = new ChromePdfRenderer();
            PdfDocument pdf = renderer.RenderHtmlAsPdf(request.Content);

            List<Stream> files = new List<Stream>
            {
                pdf.Stream
            };

            var signerRoles = new List<SubTemplateRole>();
            var roles = request.Roles.Split(",");

            foreach (var role in roles)
            {
                var signerRole = new SubTemplateRole(role);
                signerRoles.Add(signerRole);
            }

            TemplateCreateEmbeddedDraftRequest templateRequest = new TemplateCreateEmbeddedDraftRequest(
                testMode: true,
                clientId: _appSettings.Value.DropboxSignClientId,
                files: files,
                /*message: request.Message,
                subject: request.Subject,
                title: request.Title,*/
                signerRoles: signerRoles
            );

            var result = await _templateApi.TemplateCreateEmbeddedDraftAsync(templateRequest);
            var contract = new Contract
            {
                Content = request.Content,
                EditUrl = result.Template?.EditUrl,
                TemplateId = result.Template?.TemplateId
            };
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return Ok(new TemplateResponse { 
                EditUrl = result.Template.EditUrl,
                TemplateId = result.Template.TemplateId
            });
        }

        [HttpPost]
        [Route("sign")]
        public async Task<IActionResult> CreateSignatureRequestAsync([FromBody] SignatureRequest request)
        {
            SignatureRequestCreateEmbeddedWithTemplateRequest signatureRequest = new SignatureRequestCreateEmbeddedWithTemplateRequest(
                clientId: _appSettings.Value.DropboxSignClientId,
                templateIds: new List<string> { request.TemplateId},
                title: request.Title,
                subject: request.Subject,
                message: request.Message,
                signers: new List<SubSignatureRequestTemplateSigner> { new SubSignatureRequestTemplateSigner(
                    role: request.SignerRole,
                    name: request.SignerName,
                    emailAddress: request.SignerEmail
                ), new SubSignatureRequestTemplateSigner(
                    role: request.RequestorRole,
                    name: request.RequestorName,
                    emailAddress: request.RequestorEmail
                )},
                testMode: true
            );

            var response = await _signatureRequestApi.SignatureRequestCreateEmbeddedWithTemplateAsync(signatureRequest);
            
            if (response.SignatureRequest != null)
            {
                var embeddedResponse = await _embeddedApi.EmbeddedSignUrlAsync(response.SignatureRequest.Signatures.FirstOrDefault(x => x.SignerRole == request.SignerRole).SignatureId);

                if (embeddedResponse.Embedded != null)
                {
                    var contract = await _context.Contracts.FirstOrDefaultAsync(x => x.TemplateId == request.TemplateId);

                    if (contract != null)
                    {
                        contract.RequestorName = request.RequestorName;
                        contract.RequestorEmail = request.RequestorEmail;
                        contract.SignerName = request.SignerName;
                        contract.SignerEmail = request.SignerEmail;
                        contract.SignatureUrl = embeddedResponse.Embedded.SignUrl;
                        contract.ModifiedDate = DateTime.UtcNow;
                        await _context.SaveChangesAsync();

                        var httpRequest = HttpContext.Request;
                        string signatureUrl = $"{_appSettings.Value.ClientUrl}/contract/sign/?contractId={contract.Id}";
                        var message = new SendGridMessage
                        {
                            From = new EmailAddress("info@getbindr.com", request.RequestorName),
                            Subject = $"Signature Request From {request.RequestorName}",
                            HtmlContent = $"Your contract is ready to be signed <a href=\"{signatureUrl}\" target=\"_blank\">here</a>",
                        };

                        message.AddTo(request.SignerEmail, request.SignerName);

                        var messageResponse = await _sendGridClient.SendEmailAsync(message);

                        if (messageResponse.IsSuccessStatusCode)
                        {
                            string contractUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/contract/{contract.Id}";
                            return Ok(new SignatureResponse { ContractId = contract.Id, ContractUrl = contractUrl });
                        }
                    }
                }
            }

            return BadRequest();
        }
    }
}
