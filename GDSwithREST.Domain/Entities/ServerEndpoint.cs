namespace GDSwithREST.Domain.Entities
{
    public sealed record ServerEndpoint
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string DiscoveryUrl { get; set; } = null!;

        public Application Application { get; set; } = null!;
    }
}
