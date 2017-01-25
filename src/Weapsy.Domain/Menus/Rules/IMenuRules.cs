﻿using System;
using Weapsy.Infrastructure.Domain;

namespace Weapsy.Domain.Menus.Rules
{
    public interface IMenuRules : IRules<Menu>
    {
        bool IsMenuIdUnique(Guid id);
        bool IsMenuNameValid(string name);
        bool IsMenuNameUnique(Guid siteId, string name);
    }
}
