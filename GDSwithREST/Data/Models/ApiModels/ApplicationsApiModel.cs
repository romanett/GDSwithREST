namespace GDSwithREST.Data.Models.ApiModels
{
    public class ApplicationsApiModel
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationUri { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;
        public int ApplicationType { get; set; }
        public string ProductUri { get; set; } = null!;

        //Certificate?
    }
}
