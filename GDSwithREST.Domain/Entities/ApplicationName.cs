namespace GDSwithREST.Domain.Entities
{
    public sealed class ApplicationName
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string Locale { get; set; } = null!;
        public string? Text { get; set; }

        public Application Application { get; set; } = null!;
    }
}
