# PugViewEngine

This is a view engine for ASP.NET MVC using the [Pug](https://github.com/pugjs/pug) (formerly Jade) template engine for NodeJS.

As of now, it only supports MVC 5. MVC 6 (ASP.Net Core) support is planned, but [Edge.js](https://github.com/tjanczuk/edge) (which PugViewEngine uses to execute Pug) is still working on .Net Core support.

## Installation/Building From Source

A Nuget package will be created soon.

### Building From Source

__Requirements__
* Visual Studio 2015 (the [Community Edition](https://www.visualstudio.com/en-us/visual-studio-homepage-vs.aspx) should suffice)
* [NPM Task Runner](https://visualstudiogallery.msdn.microsoft.com/8f2f2cbc-4da5-43ba-9de2-c9d08ade4941) plugin for Visual Studio (used to get NPM packages)

Additional configurations may work, but this is all I have tested so far. I hope to support MonoDevelop on OS X and Linux, and the dotnet cli once .Net Core support is added.

## Setup

In your `Global.asax.cs` file, add the following line at the end of the `Application_Start()` method:

    ViewEngines.Engines.Add(new PugViewEngine.PugViewEngine());

When processing views, MVC will use Pug for views with the `.pug` extension and Razor for `.cshtml`. If you want to remove Razor entirely:

    ViewEngines.Engines.Clear(); // call this first to clear all view engines
    ViewEngines.Engines.Add(new PugViewEngine.PugViewEngine()); // then this to add PugViewEngine

## Usage

Just create views as you normally would, just using the `.pug` extension instead of `.cshtml`, and Pug syntax instead of Razor syntax.

Within the view, you will have 3 JavaScript objects available: `model`, `viewBag`, and `html`.

### Model

`model` is your the model passed to the view. If your C# code looked like this:

    public class PersonModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    ....

    // in PersonController
    public ActionResult View()
    {
        PersonModel model = new PersonModel{
            FirstName = "John",
            LastName = "Doe",
            Age = 35
        };

        return View(model);
    }

in your view you can do something like:

    // in Views/Person/View.pug
    h2 Person
    ul
      li First Name: #{model.FirstName}
      li Last Name: #{model.LastName}
      li Age: #{model.Age}

and the resulting HTML will be:

    <h2>Person</h2>
    <ul>
        <li>First Name: John</li>
        <li>Last Name: Doe</li>
        <li>Age: 35</li>
    </ul>

### ViewBag

You can use `viewBag` the same way. You can set the value in C#:

    ViewBag.Message = "This is our message.";

and in the Pug view:

    h2 Message:
    p= viewBag.Message

resulting in:

    <h2>Message:</h2>
    <p>This is our message.</p>

### Html

`html` provide various helpers, similar to `@Html` in Razor views. In fact, most of the Pug `html` helper functions call Razor's `@Html` helper behind the scenes.

Here are all the helpers that are implemented so far:

* `action` - Renders a child action
* _That's it right now, more to come soon. Pull requests welcome._

## Html Helpers

### `action`

    action(actionName, controllerName, routeValues)

Renders a child action. Calls `HtmlHelper.Action()` behind the scenes. The view engine the child view uses doesn't matter, this helper will insert the rendered HTML.

`actionName` and `controllerName` are strings, while `routeValues` is a JavaScript object. `controllerName` can be `null` if the child action is in the same controller as the current action.

    div.nav-bar= html.action("NavBar", "Common", null)
    ...
    div.article= html.action("Article", null, {articleId: 4})

The first example would render the `NavBar()` action in the `CommonController` controller class. The second example would render the `Article(int articleId)` action in the same controller the current action is in, passing a value of 4 in the `articleId` parameter.
