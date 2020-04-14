﻿using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.Employees
{
    public interface IGetEmployeeCodeQuery : IQuery<AdUserOutputModel[]>
    {
    }
}