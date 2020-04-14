﻿using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Extensions
{
    public static class UserExtensions
    {
        public static UserModel ToModel(this User user)
        {
            return new UserModel
            {
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                IsSuperior = user.IsSuperior,
                IsViewer = user.IsViewer,
                FirstName = user.FirstName,
                LastName = user.LastName,
                State = user.State,
                Superior = user.Superior != null
                    ? new SuperiorModel
                    {
                        Id = user.Superior.Id,
                        FirstName = user.Superior.FirstName,
                        LastName = user.Superior.LastName,
                    }
                    : null
            };
        }
    }
}