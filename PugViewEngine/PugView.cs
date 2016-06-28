using EdgeJs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace PugViewEngine
{
    public class PugView : IView
    {
        private string _path;
        private static string _jsSource;
        private HtmlHelper _html;
        private IDictionary<string, Func<string>> _viewHelperQueue;

        public PugView(string path)
        {
            _path = path;
            _viewHelperQueue = new Dictionary<string, Func<string>>();
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            _html = new HtmlHelper(viewContext, new ViewPage());

            object model = viewContext.ViewData.Model;
            object viewBag = viewContext.ViewData;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            var actionHelperMethod = (Func<dynamic, Task<Object>>)( async(input) =>
            {
                string action = input.action as string;
                string controller = input.controller as string;

                RouteValueDictionary routeValues;

                if (input.routeValues == null)
                    routeValues = new RouteValueDictionary();
                else
                    routeValues = new RouteValueDictionary(input.routeValues);

                string guid = "{" + Guid.NewGuid().ToString() + "}";

                _viewHelperQueue.Add(guid,
                    () =>
                    {
                        return _html.Action(action, controller, routeValues).ToString();
                    });

                return guid;
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            if (string.IsNullOrEmpty(_jsSource))
                _jsSource = File.ReadAllText(viewContext.HttpContext.Server.MapPath("~/bin/pugViewEngine.js"));
            var func = Edge.Func(_jsSource);

            var task = func(new
            {
                path = _path,
                viewBag = viewBag,
                model = model,
                methods = new
                {
                    action = actionHelperMethod
                }
            });

            string pugResult = task.Result.ToString();

            foreach(var helper in _viewHelperQueue)
            {
                string helperResult = helper.Value();
                pugResult = pugResult.Replace(
                    helper.Key,
                    helperResult);
            }


            writer.Write(pugResult);
        }
    }
}
