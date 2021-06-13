﻿using System;
using Moonglade.Data.Entities;

namespace Moonglade.Auditing
{
    public class AuditEntry
    {
        public BlogEventId EventId { get; set; }

        public EventType EventType { get; set; }

        public DateTime EventTimeUtc { get; set; }

        public string Username { get; set; }

        public string IpAddressV4 { get; set; }

        public string MachineName { get; set; }

        public string Message { get; set; }
    }
}
