﻿namespace ERNI.PBA.Server.Host.Model
{
    public class UpdateRequestModel
    {
        public int Id { get; set; }

        public System.DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }
    }
}
