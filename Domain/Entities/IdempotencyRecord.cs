using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Entities
{
    public class IdempotencyRecord
    {
        public Guid Id { get; private set; }
        public string Key { get; private set; } = string.Empty;
        public string Path { get; private set; } = string.Empty;
        public string Response { get; private set; } = string.Empty;
        public int StatusCode { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private IdempotencyRecord() { }

        public IdempotencyRecord(string key, string path, string response, int statusCode)
        {
            Id = Guid.NewGuid();
            Key = key;
            Path = path;
            Response = response;
            StatusCode = statusCode;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
