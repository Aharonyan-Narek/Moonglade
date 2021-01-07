﻿using System;

namespace Moonglade.Model
{
    public class PageSegment
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string RouteName { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreateTimeUtc { get; set; }
    }
}
