using System;
using System.Net;

namespace TagReader.Message
{
    public class RfidTag
    {
        public string TagId { get; set; }
        public double Price { get; set; }
        public string Item { get; set; }
        public override string ToString()
        {
            return $"{TagId} - {Item} - {Price}";
        }
    }
}
