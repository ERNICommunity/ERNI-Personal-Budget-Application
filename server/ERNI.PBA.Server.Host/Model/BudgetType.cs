using System.Collections.Generic;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class BudgetType
    {
        public static readonly IReadOnlyCollection<BudgetType> Types = new[]
        {
            new BudgetType {Id = BudgetTypeEnum.PersonalBudget,
                Name = "Personal budget",
                SinglePerUser = true,
                IsTransferable = false
            },
            new BudgetType {Id = BudgetTypeEnum.CommunityBudget,
                Name = "Community budget",
                SinglePerUser = false,
                IsTransferable = true
            },
            new BudgetType {Id = BudgetTypeEnum.TeamBudget,
                Name = "Team budget",
                SinglePerUser = false,
                IsTransferable = false
            },
            new BudgetType {Id = BudgetTypeEnum.RecreationBudget,
                Name = "Recreation budget",
                SinglePerUser = true,
                IsTransferable = false
            }
        };

        public BudgetTypeEnum Id { get; private set; }

        public string Name { get; private set; }

        public bool SinglePerUser { get; private set; }

        public bool IsTransferable { get; private set; }
    }
}
