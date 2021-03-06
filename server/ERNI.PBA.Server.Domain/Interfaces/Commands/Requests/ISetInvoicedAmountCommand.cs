﻿using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Domain.Interfaces.Commands.Requests
{
    public interface ISetInvoicedAmountCommand : ICommand<(int requestId, SetInvoicedAmountModel model)>
    {
    }
}