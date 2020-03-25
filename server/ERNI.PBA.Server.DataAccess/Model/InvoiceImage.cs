using System;
using System.Collections.Generic;
using System.Text;

namespace ERNI.PBA.Server.DataAccess.Model
{
    public class InvoiceImage
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int RequestId { get; set; }

        public Request Request { get; set; }

        public byte[] Data { get; set; }
    }
}
