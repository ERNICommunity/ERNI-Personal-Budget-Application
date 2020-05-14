using System;
using System.Collections.Generic;
using System.Text;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;

namespace ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets
{
    public interface IDeleteBudgetCommand : ICommand<int>
    {
    }
}
