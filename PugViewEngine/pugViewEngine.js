var pug = require('pug');
return function (data, callback) {
    var html = {};
    html.action = function(action, controller, routeValues){
        var actionResult;
        data.methods.action({
            action: action,
            controller: controller,
            routeValues: routeValues
        }, function(error, result){
            actionResult = result;
        });
        
        while(actionResult === undefined) {
            require('deasync').runLoopOnce();
        }
        return actionResult;
    }

    callback(null, pug.renderFile(data.path, { 
        viewBag: data.viewBag, 
        model: data.model,
        modelErrors: data.modelErrors,
        html: html,
        Enumerable: require('linq')
    }));
}