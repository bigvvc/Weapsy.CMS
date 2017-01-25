﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Weapsy.Mvc.Context;
using Weapsy.Reporting.Languages;
using Weapsy.Reporting.Pages;

namespace Weapsy.Extensions
{
    public class PageSlugRoute : IRouter
    {
        private readonly IRouter _router;

        public PageSlugRoute(IRouter router)
        {
            _router = router;
        }

        // under development/refactoring
        public async Task RouteAsync(RouteContext context)
        {
            string path = GetPath(context);

            var contextService = context.HttpContext.RequestServices.GetService<IContextService>();
            var languageFacade = context.HttpContext.RequestServices.GetService<ILanguageFacade>();
            var pageFacade = context.HttpContext.RequestServices.GetService<IPageFacade>();

            var pathParts = path.Split('/');
            var languageSlug = pathParts.Length > 1 ? pathParts[0] : path;
            var pageSlug = path;

            var site = contextService.GetCurrentSiteInfo();
            var languages = await languageFacade.GetAllActiveAsync(site.Id);
            var language = languages.FirstOrDefault(x => x.Url == languageSlug);
            Guid? pageId = null;

            if (language != null)
            {
                contextService.SetLanguageInfo(language);
                pageSlug = languageSlug == path ? string.Empty : path.Substring(languageSlug.Length + 1);
            }
            else
            {
                language = contextService.GetCurrentLanguageInfo();
            }

            if (!string.IsNullOrEmpty(pageSlug))
                pageId = pageFacade.GetIdBySlug(site.Id, pageSlug, language.Id);

            if (pageId == null && !string.IsNullOrEmpty(pageSlug))
                pageId = pageFacade.GetIdBySlug(site.Id, pageSlug);

            if (pageId == null && string.IsNullOrEmpty(pageSlug))
                pageId = site.HomePageId;

            if (pageId == null)
                return;

            var routeData = new RouteData(context.RouteData);
            routeData.Routers.Add(_router);
            routeData.Values["controller"] = "Home";
            routeData.Values["action"] = "Index";
            routeData.Values[ContextKeys.PageKey] = pageId;
            routeData.Values[ContextKeys.LanguageKey] = language?.Id ?? Guid.Empty;

            context.RouteData = routeData;

            await _router.RouteAsync(context);
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        private string GetPath(RouteContext context)
        {
            string path = context.HttpContext.Request.Path.Value;

            if (!string.IsNullOrEmpty(path) && path[0] == '/')
                path = path.Substring(1);

            return path;
        }
    }
}