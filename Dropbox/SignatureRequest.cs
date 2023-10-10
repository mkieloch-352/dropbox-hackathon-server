namespace webapi.Dropbox
{
    public class SignatureRequest
    {
        public string RequestorName { get; set; }
        public string RequestorEmail { get; set; }
        public string RequestorRole { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public string SignerRole { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public string TemplateId { get; set; }
    }
}
