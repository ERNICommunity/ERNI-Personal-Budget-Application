﻿namespace ERNI.PBA.Server.Host.Commands.Requests
{
    public class ApproveRequestCommand : CommandBase<bool>
    {
        public int RequestId { get; set; }
    }
}