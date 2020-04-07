﻿using ERNI.PBA.Server.Domain.Entities;

namespace ERNI.PBA.Server.Host.Model
{
    public class CreateBudgetRequest
    {
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public BudgetTypeEnum BudgetType { get; set; }
    }
}