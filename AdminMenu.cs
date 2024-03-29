﻿using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            builder
                .Add(T["Configuration"], configuration => configuration
                    .Add(T["Greenhouse"], settings => settings
                        .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = Constants.GroupId })
                        .Permission(Permissions.ManageGreenhouseSettings)
                        .LocalNav()
                    ));

            return Task.CompletedTask;
        }
    }
}
