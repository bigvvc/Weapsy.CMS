﻿using System;
using Weapsy.Framework.Queries;

namespace Weapsy.Reporting.Menus.Queries
{
    public class GetForAdmin : IQuery
    {
        public Guid SiteId { get; set; }
        public Guid Id { get; set; }
    }
}
