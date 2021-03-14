/*  Copyright [2017-2020] [Invincible Technologies]
 *  
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *    
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

/// <summary>
/// CRUDController is a CRUD component that enables create, read, update, delete and list requests 
/// functionality using AJAX. All requests take a URI (address) with or without resulting 
/// content type object and produces response event notifications 
/// with resulting data or contents. CRUDController component operates on top 
/// of a CRUDSource and is responsible for data extraction and reporting to 
/// other components. Usually a CRUDController deals with a CRUDSource and a 
/// CRUDProcessor. 
/// </summary>
function CRUDController(options) {
    var instance = this;

    /// <summary>
    /// Key is unique identifier to differentiate between CRUD controllers and is optional.
    /// </summary>
    instance.Key = options !== undefined ? options.key : null;

    /// <summary>
    /// Processing is request status monitoring data member, true status value tells that request is in -process.
    /// </summary>
    instance.Processing = false;

    /// <summary>
    /// ResponseData is the response result received after successful request completion.
    /// </summary>
    instance.ResponseData = null;

    /// <summary>
    /// ResponseError is the response result received after failed request completion.
    /// </summary>
    instance.ResponseError = null;

    /// <summary>
    /// Gets controller key.
    /// </summary>
    instance.getKey = function () {
        return instance.Key;
    };
    
    /// <summary>
    /// Gets function construct type information.
    /// </summary>
    instance.getType = function () {
        return "CRUDController";
    };

    /// <summary>
    /// Gets request processing status.
    /// </summary>
    instance.processing = function () {
        return instance.Processing;
    };

    /// <summary>
    /// Gets requested response data.
    /// </summary>
    instance.responseData = function () {
        return instance.ResponseData;
    };

    /// <summary>
    /// Gets response error.
    /// </summary>
    instance.responseError = function () {
        return instance.ResponseError;
    };

    /// <summary>
    /// Checks whether request yields in response error.
    /// </summary>
    instance.isResponseError = function () {
        return instance.ResponseError !== null;
    };
    
    /// <summary>
    /// Sends ajax request.
    /// </summary>
    instance.sendRequest = function (options, callback) {

        //initialize headers and set __RequestVerificationToken for request authorization.
        var headers = (options.headers !== null && options.headers !== undefined) ? options.headers : {};
        headers['__RequestVerificationToken'] = $('[name=__RequestVerificationToken]').val();        

        if (options.eventData.target === 'CreateFileContent' || options.eventData.target === 'UpdateFileContent') {

            var uploadForm = (options.eventData.form !== null && options.eventData.form !== undefined) ? options.eventData.form : '__uploadform';

            if (document.forms[uploadForm] !== null && document.forms[uploadForm] !== undefined) {

                headers["Content-Disposition"] = "attachment; filename=" + document.forms[uploadForm]["upload"].value;

                try {

                    $(document.forms[uploadForm]).ajaxSubmit({
                        headers: headers,
                        dataType: (options.dataType !== null && options.dataType !== undefined) ? options.dataType : "json",
                        type: (options.method !== null && options.method !== undefined) ? options.method : "POST",
                        data: (options.data !== null && options.data !== undefined) ? options.data : null,
                        url: options.url,
                        contentType: 'multipart/form-data',
                        uploadProgress: function (event, position, total, percentComplete) {

                            instance.notify({ event: "progress.request.CRUD.WindnTrees", result: percentComplete, position: position, total: total, percentComplete: percentComplete });
                        },
                        success: function (data, textStatus, jqXHR) {

                            options.eventData.event = "after.request.CRUD.WindnTrees";
                            options.eventData.method = (options.method !== null && options.method !== undefined) ? options.method : "POST";

                            instance.notifyDone(data, textStatus, jqXHR, options.eventData);
                        },
                        error: function (data, textStatus, jqXHR) {

                            options.eventData.event = "after.request.CRUD.WindnTrees";
                            options.eventData.method = (options.method !== null && options.method !== undefined) ? options.method : "POST";

                            instance.notifyFail(data, textStatus, jqXHR, options.eventData);
                        },
                        always: function (data, textStatus, jqXHR) {
                            if (callback !== null && callback !== undefined) {
                                callback(data);
                            }
                        }
                    });

                } catch (e) {

                }
            }
        }
         else {

            $.ajax({
                headers: headers,
                dataType: (options.dataType !== null && options.dataType !== undefined) ? options.dataType : "json",
                type: (options.method !== null && options.method !== undefined) ? options.method : "POST",
                data: (options.data !== null && options.data !== undefined) ? options.data : null,
                url: options.url,
                contentType: (options.contentType !== null && options.contentType !== undefined) ? options.contentType : "application/json; charset=utf-8"
            })
                .done(function (data, textStatus, jqXHR) {

                    options.eventData.event = "after.request.CRUD.WindnTrees";
                    options.eventData.method = (options.method !== null && options.method !== undefined) ? options.method : "POST";

                    instance.notifyDone(data, textStatus, jqXHR, options.eventData);
                })
                .fail(function (data, textStatus, jqXHR) {

                    options.eventData.event = "after.request.CRUD.WindnTrees";
                    options.eventData.method = (options.method !== null && options.method !== undefined) ? options.method : "POST";

                    instance.notifyFail(data, textStatus, jqXHR, options.eventData);
                })
                .always(function (data, textStatus, jqXHR) {
                    if (callback !== null && callback !== undefined) {
                        callback(data);
                    }
                });
        }
    };

    /// <summary>
    /// Creates new content object using POST method and notify result in after event.
    /// </summary>
    instance.create = function (data, callback) {
        instance.Processing = true;
        instance.ResponseData = null;
        instance.ResponseError = null;

        var eventData = { 'event': 'before.request.CRUD.WindnTrees', 'key': instance.Key, 'request': (data.request !== null && data.request !== undefined) ? data.request : "create", 'target': data.target, 'form': data.form, 'data': data };
        var request = (data.target !== null && data.target !== undefined) ? data.target : (data.request !== null && data.request !== undefined) ? data.request : "create";
        
        instance.notify(eventData);
        
        instance.sendRequest({
            'headers': { 'W-Target': data.__target },
            'data': (typeof(data.content) === "string" ? data.content : JSON.stringify(data.content)),
            'url': data.uri + "/" + request,
            'eventData': eventData}, callback);
    };

    /// <summary>
    /// Read response based on a key value using GET method and notify content in 
    /// after event.
    /// </summary>
    instance.read = function (data, callback) {
        instance.Processing = true;
        instance.ResponseData = null;
        instance.ResponseError = null;

        var eventData = { 'event': 'before.request.CRUD.WindnTrees', 'key': instance.Key, 'request': (data.request !== null && data.request !== undefined) ? data.request : "read", 'target': data.target, 'form': data.form, 'data': data };
        var request = (data.target !== null && data.target !== undefined) ? data.target : (data.request !== null && data.request !== undefined) ? data.request : "read";

        instance.notify(eventData);

        instance.sendRequest({
            'headers': { 'W-Target': data.__target },
            'method': 'GET',
            'data': null,
            'url': encodeURI(data.uri + "/" + request + "/" + ((data.key !== null && data.key !== undefined) ? data.key : "")),
            'eventData': eventData
        }, callback);
    };

    /// <summary>
    /// Updates existing content object using POST method and notify result in after event.
    /// </summary>
    instance.update = function (data, callback) {
        instance.Processing = true;
        instance.ResponseData = null;
        instance.ResponseError = null;

        var eventData = { 'event': 'before.request.CRUD.WindnTrees', 'key': instance.Key, 'request': (data.request !== null && data.request !== undefined) ? data.request : "update", 'target': data.target, 'form': data.form, 'data': data };
        var request = (data.target !== null && data.target !== undefined) ? data.target : (data.request !== null && data.request !== undefined) ? data.request : "update";
        
        instance.notify(eventData);

        instance.sendRequest({
            'headers': { 'W-Target': data.__target },
            'data': (typeof(data.content) === "string" ? data.content : JSON.stringify(data.content)),
            'url': data.uri + "/" + request,
            'file': data.UploadField,
            'eventData': eventData}, callback);
    };

    /// <summary>
    /// Deletes existing content object using POST method and notify result in after event.
    /// </summary>
    instance.delete = function (data, callback) {
        instance.Processing = true;
        instance.ResponseData = null;
        instance.ResponseError = null;

        var eventData = { 'event': 'before.request.CRUD.WindnTrees', 'key': instance.Key, 'request': (data.request !== null && data.request !== undefined) ? data.request : "delete", 'target': data.target, 'form': data.form, 'data': data };
        var request = (data.target !== null && data.target !== undefined) ? data.target : (data.request !== null && data.request !== undefined) ? data.request : "delete";
        
        instance.notify(eventData);

        instance.sendRequest({
            'headers': { 'W-Target': data.__target },
            'data': (typeof(data.content) === "string" ? data.content : JSON.stringify(data.content)),
            'url': data.uri + "/" + request,
            'eventData': eventData}, callback);
    };

    /// <summary>
    /// List records based on key, keyword, list number and list size at 
    /// specified URI address and notify request data in before.request.CRUD.WindnTrees event. 
    /// After successfull processing, a list of records is notified in after.request.CRUD.WindnTrees event.
    /// </summary>
    instance.list = function (data, callback) {
        instance.Processing = true;
        instance.ResponseData = null;
        instance.ResponseError = null;

        var eventData = { 'event': 'before.request.CRUD.WindnTrees', 'key': instance.Key, 'request': (data.request !== null && data.request !== undefined) ? data.request : "list", 'target': data.target, 'form': data.form, 'data': data };

        var request = (data.target !== null && data.target !== undefined) ? data.target : (data.request !== null && data.request !== undefined) ? data.request : "list";

        instance.notify(eventData);

        if (data.method === "GET") {

            var url = data.uri + "/" + request;

            if (data.key !== null && data.key !== undefined) {
                url = (url + "/" + data.key);
            }

            if (data.source !== null && data.source !== undefined) {
                url = (url + "/" + data.source);
            }

            if (data.keyword !== null && data.keyword !== undefined) {
                url = (url + "/" + data.keyword);
            }

            if (data.page !== null && data.page !== undefined) {
                url = (url + "/" + data.page);
            }

            if (data.size !== null && data.size !== undefined) {
                url = (url + "/" + data.size);
            }

            instance.sendRequest({
                'headers': { 'W-Target': data.__target },
                'method': "GET",
                'data': null,
                'url': url,
                'eventData': eventData
            }, callback);

        } else {

            var queryObject = (data.query !== null && data.query !== undefined) ? data.query : { "key": data.key, "source": data.source, "keyword": data.keyword, "size": data.size, "page": data.page };

            instance.sendRequest({
                'headers': { 'W-Target': data.__target },
                'data': JSON.stringify(queryObject),
                'url': data.uri + "/" + request,
                'eventData': eventData
            }, callback);
        }
    };

    /// <summary>
    /// Makes event notification call.
    /// </summary>
    instance.notify = function (eventData) {
        $(instance).trigger(eventData.event, eventData);
    };
    
    /// <summary>
    /// Processes successfull response event and notify data in after.request.CRUD.WindnTrees event.
    /// </summary>
    instance.notifyDone = function (data, textStatus, jqXHR, eventData) {
        instance.Processing = false;
        instance.ResponseData = data;

        eventData.result = data;
        $(instance).trigger('after.request.CRUD.WindnTrees', eventData);
    };
    
    /// <summary>
    /// Processes failure response event and notify data in fail.request.CRUD.WindnTrees event.
    /// </summary>
    instance.notifyFail = function (jqXHR, textStatus, errorThrown, eventData) {
        instance.Processing = false;
        instance.ResponseError = errorThrown;

        eventData.result = jqXHR;
        $(instance).trigger('fail.request.CRUD.WindnTrees', eventData);
    };
}