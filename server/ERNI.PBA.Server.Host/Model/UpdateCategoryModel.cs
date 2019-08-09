namespace ERNI.PBA.Server.Host.Model
{
    public class UpdateCategoryModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool IsUrlNeeded { get; set; }
        public int? SpendLimit { get; set; }
        public string[] Email { get; set; }
    }
}
