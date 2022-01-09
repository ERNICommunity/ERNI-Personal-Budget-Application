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
                Key = "personal",
                SinglePerUser = true,
                IsTransferable = false
            },
            new BudgetType
            {
                Id = BudgetTypeEnum.CommunityBudget,
                Name = "Community budget",
                Key = "community",
                SinglePerUser = false,
                IsTransferable = true
            },
            new BudgetType
            {
                Id = BudgetTypeEnum.TeamBudget,
                Name = "Team budget",
                Key = "team",
                SinglePerUser = true,
                IsTransferable = false
            },
            new BudgetType
            {
                Id = BudgetTypeEnum.RecreationBudget,
                Name = "Recreation budget",
                Key = "recreation",
                SinglePerUser = true,
                IsTransferable = false
            }
        };

        public BudgetTypeEnum Id { get; init; }

        public string Name { get; init; } = null!;

        public string Key { get; init; } = null!;

        public bool SinglePerUser { get; init; }

        public bool IsTransferable { get; init; }
    }
}