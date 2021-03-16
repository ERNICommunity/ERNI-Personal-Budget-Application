using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models
{
    public class BudgetType
    {
        public static readonly IReadOnlyCollection<BudgetType> Types = new[]
        {
            new BudgetType
            {
                Id = BudgetTypeEnum.PersonalBudget,
                Name = "Personal budget",
                SinglePerUser = true,
                IsTransferable = false
            },
            new BudgetType
            {
                Id = BudgetTypeEnum.CommunityBudget,
                Name = "Community budget",
                SinglePerUser = false,
                IsTransferable = true
            },
            new BudgetType
            {
                Id = BudgetTypeEnum.TeamBudget,
                Name = "Team budget",
                SinglePerUser = true,
                IsTransferable = false
            },
            new BudgetType
            {
                Id = BudgetTypeEnum.RecreationBudget,
                Name = "Recreation budget",
                SinglePerUser = true,
                IsTransferable = false
            }
        };

        public BudgetTypeEnum Id { get; init; }

        public string Name { get; init; } = null!;

        public bool SinglePerUser { get; init; }

        public bool IsTransferable { get; init; }
    }
}