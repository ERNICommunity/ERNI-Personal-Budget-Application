﻿using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class UpdateUserModel
    {
        public int Id { get; set; }

        public User Superior { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsSuperior { get; set; }

        public bool IsViewer { get; set; }

        public UserState State { get; set; }
    }
}