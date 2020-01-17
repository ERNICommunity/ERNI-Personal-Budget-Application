﻿using System.Collections.Generic;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class BudgetType
    {
        public static IReadOnlyCollection<BudgetType> Types = new[]
        {
            new BudgetType {Id = BudgetTypeEnum.PersonalBudget, Name = "Personal budget", SinglePerUser = true},
            new BudgetType {Id = BudgetTypeEnum.CommunityBudget, Name = "Community budget", SinglePerUser = false},
            new BudgetType {Id = BudgetTypeEnum.TeamBudget, Name = "Team budget", SinglePerUser = true}
        };

        public BudgetTypeEnum Id { get; set; }

        public string Name { get; set; }

        public bool SinglePerUser { get; set; }
    }
}