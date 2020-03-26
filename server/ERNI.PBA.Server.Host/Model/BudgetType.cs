using ERNI.PBA.Server.DataAccess.Model;
using System.Collections.Generic;

namespace ERNI.PBA.Server.Host.Model
{
    public class BudgetType
    {
        public static readonly IReadOnlyCollection<BudgetType> Types = new[]
        {
            new BudgetType {Id = BudgetTypeEnum.PersonalBudget, Name = "Personal budget", SinglePerUser = true},
            new BudgetType {Id = BudgetTypeEnum.CommunityBudget, Name = "Community budget", SinglePerUser = false},
            new BudgetType {Id = BudgetTypeEnum.TeamBudget, Name = "Team budget", SinglePerUser = false},
            new BudgetType {Id = BudgetTypeEnum.RecreationBudget, Name = "Recreation budget", SinglePerUser = true}
        };

        public BudgetTypeEnum Id { get; private set; }

        public string Name { get; private set; }

        public bool SinglePerUser { get; private set; }
    }
}
