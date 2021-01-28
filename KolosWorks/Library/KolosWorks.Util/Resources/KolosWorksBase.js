var KolosWorksBase = {
    Class: function (baseClass, JsonObject) {
        for (var key in JsonObject) {
            var extraObj = {
                value: JsonObject[key],
                enumerable: true,
                writable: true
            }

            JsonObject[key] = extraObj;
        }

        result = Object.create(baseClass, JsonObject);

        if (result.construct) {
            this.result = result;
            this.doAjax('OnFrontEndConstruct', '{}', this.getConstructCallback, this.getConstructCallback, this);
        }

        return result;
    },
    // path and type are optional
    doAjax: function (path, method, data, successCallback, failureCallback, type, context) {
        var defaultType = 'POST';
        var defaultPath = location.pathname;
        if (defaultPath == '/') {
            defaultPath = "/Default.aspx";
        }

        if (type === undefined && context === undefined) {
            context = failureCallback;
            type = defaultType;
            failureCallback = successCallback;
            successCallback = data;
            data = method;
            method = path;
            path = defaultPath;
        }
        else if (context === undefined && typeof failureCallback === 'function') {
            context = type;
            type = defaultType;
        }
        else if (context === undefined && typeof failureCallback === 'string') {
            var fc = failureCallback.toUpperCase();
            if (fc === 'POST' || fc === 'GET' || fc === 'PUT' || fc === 'PATCH' || fc === 'DELETE') {
                context = type;
                type = fc;
                failureCallback = successCallback;
                successCallback = data;
                data = method;
                method = path;
                path = defaultPath;
            }
        }
        else if (type !== undefined && typeof type === 'string') {
            type = type.toUpperCase();
        }

        $.ajax({
            type: type,
            url: path + '/' + method,
            data: data,
            contentType: 'application/json; charset=utf-8',
            success: this.createDelegate(context, successCallback),
            failure: this.createDelegate(context, failureCallback)
        });
    },
    getConstructCallback: function (data) {
        if (data.d) {
            data = data.d;
        }
        this.constructData = data;
        $(document).ready(this.createDelegate(this, this.doConstruct));
    },
    doConstruct: function (eventArgs) {
        this.result.construct(this.constructData);

        delete this.result;
        delete this.constructData;
        delete this.__proto__.getConstructCallback;       
        delete this.__proto__.doConstruct;
    },
    createDelegate:  function (object, method) {
        var shim = function () {
            method.apply(object, arguments);
        }
        return shim;
    }
}
KolosWorksBase.PageControl = {
    PageStuff: function (something) {
        return something;
    }
}

var KolosWorks;
if (typeof KolosWorks === 'undefined') {
    KolosWorks = {};
    KolosWorks.Util = Object.create(KolosWorksBase, {});
}