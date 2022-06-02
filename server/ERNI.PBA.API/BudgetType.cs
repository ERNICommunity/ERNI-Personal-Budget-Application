namespace ERNI.PBA.API
{
    public class BudgetType
    {
        public BudgetTypeEnum Id { get; init; }

        public string Name { get; init; } = null!;

        public string Key { get; init; } = null!;

        public bool SinglePerUser { get; init; }

        public bool IsTransferable { get; init; }
    }
}