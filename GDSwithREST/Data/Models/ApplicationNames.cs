namespace GDSwithREST.Data.Models
{
    public partial class ApplicationNames
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string Locale { get; set; } = null!;
        public string? Text { get; set; }

        public Applications Application { get; set; } = null!;
    }
}
