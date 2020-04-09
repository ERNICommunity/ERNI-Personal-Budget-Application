﻿using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets
{
    public interface ICreateBudgetCommand : ICommand<CreateBudgetRequest, bool>
    {
    }
}
