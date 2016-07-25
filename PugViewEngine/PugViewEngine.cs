using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Pug.ViewEngine
{
    public class PugViewEngine : VirtualPathProviderViewEngine
    {
        public PugViewEngine()
        {
            ViewLocationFormats = new string[]
            {
                "~/Views/{1}/{0}.pug",
                "~/Views/Shares/{0}.pug"
            };

            PartialViewLocationFormats = new string[]
            {
                "~/Views/{1}/{0}.pug",
                "~/Views/Shares/{0}.pug"
            };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            var physicalpath = controllerContext.HttpContext.Server.MapPath(partialPath);
            return new PugView(physicalpath);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var physicalpath = controllerContext.HttpContext.Server.MapPath(viewPath);
            return new PugView(physicalpath);
        }
    }
}
