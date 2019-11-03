using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace TVProgViewer
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css",
                "~/Content/themes/base/*.css",
                "~/Content/jsTree/themes/default/style.css",
                "~/Content/ui.jqgrid.css",
                "~/Content/Site.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/tvprogviewerscripts")
                .Include("~/Scripts/jquery-{version}.js",
                    "~/Scripts/modernizr-{version}.js",
                    "~/Scripts/jquery-ui-{version}.js",
                    "~/Scripts/jquery.validate.js",
                    "~/Scripts/jquery.validate.unobtrusive.js",
                    "~/Scripts/jsTree3/jstree.js",
                    "~/Scripts/free-jqGrid/i18n/min/grid.locale-ru.js",
                    "~/Scripts/free-jqGrid/jquery.jqgrid.js",
                    "~/Scripts/free-jqGrid/jquery.jqgrid.src.js"
                    ));
            BundleTable.EnableOptimizations = true;
        }
    }
}