namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class BlobStorageSettings
    {
        public const string SectionName = "Storage";

        public string AttachmentDataContainerName { get; set; } = null!;
    }
}