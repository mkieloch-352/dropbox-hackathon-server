namespace webapi.Contracts
{
    public class ContractPromptRequest
    {
        public Guid ContractId { get; set; }
        public string Prompt { get; set; }
    }
}
