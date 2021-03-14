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
/// SearchView provides data handling and communication capability using read and list calls to a hosted web service or API controller and gets typed content objects based on input object. SearchView extends from ObjectView and allows reading and listing.
/// </summary>
function SearchView(options) {
    var instance = (options.instance !== null && options.instance !== undefined) ? options.instance : this;
    var extender = new InstanceExtender();
    
    if (options.instance === null || options.instance === undefined) {
        instance = extender.extendNewInstance({ 'instance': instance, 'options': options});
    }
    
    //extend from object view
    var extOptions = Object.create(options);
    extOptions.instance = instance;
    extOptions.events = false;
    instance = ObjectView(extOptions);

    /// <summary>
    /// Gets the type of view.
    /// </summary>
    instance.getType = function () {
        return 'SearchView';
    };

    /// <summary>
    /// Clears (resets) observer records list and view.
    /// </summary>
    instance.clearRecords = function () {

        if (instance.getObserverInterface() !== null && instance.getObserverInterface() !== undefined) {

            if (instance.getObserverInterface() !== null && instance.getObserverInterface() !== undefined) {

                if (instance.getObserverInterface().getType() === 'ListObserver'
                        || instance.getObserverInterface().getType() === 'ListKNObserver') {

                    instance.getObserverInterface().clearList();
                    instance.getObserverInterface().displayProcessing(false);

                } else {

                    instance.getObserverInterface().clearListRecordsView();
                    instance.getObserverInterface().displayProcessing(false);
                    instance.getObserverInterface().fillListRecordsView({
                        'page': 1,
                        'responseData': null,
                        'records': instance.getObserverInterface().getRecords(),
                        'immediateRecords': true
                    });
                }
            }
        }
    };

    /// <summary>
    /// Lists records based on provided keyword (via observer keyword property) and page number.
    /// </summary>
    instance.list = function (options, fill) {

        var _keyword = null;
        if (instance.getObserverInterface() !== null && instance.getObserverInterface() !== undefined) {
            instance.getObserverInterface().displayProcessingActivity();
            _keyword = instance.getObserverInterface().getKeyword();
        }
        
        if (typeof (options) === "object") {

            var _options = Object.create(options);

            if (options.contentType !== null && options.contentType !== undefined) {
                _options.contentType = instance.extendContentMethods(_options);
            }
            
            _options.scopeObject = options.scopeObject;

            _options.key = (options.key !== null && options.key !== undefined) ? options.key : instance.getObjectKey();
            _options.uri = (options.uri !== null && options.uri !== undefined) ? options.uri : instance.URI;
            _options.keyword = (options.keyword !== null && options.keyword !== undefined) ? options.keyword : _keyword;
            _options.fill = fill;

            instance.getCRUDProcessor().list(_options);
        }
        else {

            instance.getCRUDProcessor().list({
                'uri': instance.URI,
                'key': instance.getObjectKey(),
                'keyword': instance.getObserverInterface().getKeyword(),
                'size': instance.getObserverInterface().getListSize(),
                'page': options,
                'fill': fill
            });
        }
    };

    /// <summary>
    /// Loads view and related lists.
    /// </summary>
    instance.load = function (options, fill) {

        instance.LoadFields(options);
        instance.list(1, fill);
    };
    
    if (instance.getObserverInterface() !== null &&
            instance.getObserverInterface() !== undefined) {
        
        /** 
         * Observer clearRecords function definition.
         * 
         * @returns {undefined}
         */
        instance.getObserverInterface().clearRecords = function () {
            instance.clearRecords();
        };

        /** 
         * Observer list function definition.
         * 
         * @param {type} page
         * @returns {undefined}
         */
        instance.getObserverInterface().list = function (page) {
            instance.list(page);
        };

        /** 
         * Observer load function definition.
         * 
         * @param {type} options
         * @returns {undefined}
         */
        instance.getObserverInterface().load = function (options, fill) {
            instance.load(options, fill);
        };
    }
    
    if (instance.getObserverObject() !== null &&
            instance.getObserverObject() !== undefined) {
        
        /** 
         * Observer clearRecords function definition.
         * 
         * @returns {undefined}
         */
        instance.getObserverObject().clearRecords = function () {
            instance.clearRecords();
        };

        /** 
         * Observer list function definition.
         * 
         * @param {type} page
         * @returns {undefined}
         */
        instance.getObserverObject().list = function (page) {
            instance.list(page);
        };

        /** 
         * Observer load function definition.
         * 
         * @param {type} options
         * @returns {undefined}
         */
        instance.getObserverObject().load = function (options, fill) {
            instance.load(options, fill);
        };
    }

    /// <summary>
    /// Error processing and presenting event subscription.
    /// </summary>
    instance.presentErrors = function (event, eventData) {

        eventData.event = "errors.before.rendering.view.CRUD.WindnTrees";
        instance.notify(eventData);

        if (eventData.data.callback !== null &&
                eventData.data.callback !== undefined) {

            eventData.data.callback(eventData.result);
        } else {
            
            if (instance.getObserverInterface() !== null && instance.getObserverInterface() !== undefined) {
                
                instance.getObserverInterface().displayClearActivity();
                instance.getObserverInterface().setErrors(instance.getErrors());
            }
        }

        eventData.event = "errors.after.rendering.view.CRUD.WindnTrees";
        instance.notify(eventData);
    };

    /// <summary>
    /// Record processing and presenting event subscription.
    /// </summary>
    instance.presentRecord = function (event, eventData) {

        eventData.event = "record.before.rendering.view.CRUD.WindnTrees";
        instance.notify(eventData);
        
        if (eventData !== null && eventData !== undefined) {

            if (eventData.code !== null && eventData.code !== undefined) {

                if (eventData.code.length > 0) {
                    instance.setAuthorizationCode(eventData.code);

                    if (eventData.data.codeField !== null && eventData.data.codeField !== undefined) {

                        $('[name=' + eventData.data.codeField + ']').val(eventData.code);
                    }
                    else {

                        $('[name=__RequestVerificationToken]').val(eventData.code);
                    }
                }
            }
        }

        if (eventData.data.callback !== null &&
                eventData.data.callback !== undefined) {

            eventData.observerType = "callback";
            eventData.data.callback(eventData.result);
        } else {
            
            if (eventData.request === 'read') {
                
                if (instance.getObserverInterface() !== null && instance.getObserverInterface() !== undefined) {

                    //attach observer type information within event
                    eventData.observerType = instance.getObserverInterface().getType();

                    instance.getObserverInterface().displaySuccessActivity();
                    instance.getObserverInterface().displayFormSuccessActivity();

                    instance.getObserverInterface().setRecord(eventData.result);

                    if (instance.getObserverInterface().getType() === "CRUDObserver" || instance.getObserverInterface().getType() === "CRUDKNObserver") {

                        if (eventData.result !== null && eventData.result !== undefined) {

                            instance.getObserverInterface().setFormObject(eventData.result);
                        }
                        else {
                            
                            instance.getObserverInterface().displayFormNoRecordActivity();
                        }
                    }
                }
                return;
            }
            
            if (eventData.data !== null &&
                    eventData.data !== undefined) {
                
                if (eventData.data.detailEntity !== null &&
                        eventData.data.detailEntity !== undefined) {

                    eventData.entityType = "detailEntity";

                    var detailEntity = eventData.data.detailEntity;
                    detailEntity.setRecord(eventData.result);
                    
                    if (instance.getObserverInterface() !== null &&
                        instance.getObserverInterface() !== undefined) {


                        if (eventData.data.messageType !== null &&
                            eventData.data.messageType !== undefined) {

                            if (eventData.data.messageType === 'brief') {

                                instance.getObserverInterface().displaySaved();

                            } else {

                                instance.getObserverInterface().displaySuccessActivity();
                                instance.getObserverInterface().displayFormSuccessActivity();
                            }

                        } else {

                            instance.getObserverInterface().displaySuccessActivity();
                            instance.getObserverInterface().displayFormSuccessActivity();
                        }

                        //attach observer type information within event
                        eventData.observerType = instance.getObserverInterface().getType();

                        instance.getObserverInterface().updateView({
                        'requestData': eventData.data,
                        'resetForm': eventData.data.resetForm,
                        'refObject': eventData.data.refObject,
                        'refActions': eventData.data.refActions,
                        'refInputs': eventData.data.refInputs,
                        'action': eventData.request,
                        'resultRecord': detailEntity,
                        'placement': (options.placement !== null && options.placement !== undefined) ? options.placement : 'first'
                    });
                    }
                    
                    return;
                }
            }

            if (instance.getObserverInterface() !== null &&
                instance.getObserverInterface() !== undefined) {
                
                if (eventData.data.messageType !== null &&
                    eventData.data.messageType !== undefined) {

                    if (eventData.data.messageType === 'brief') {

                        instance.getObserverInterface().displaySaved();

                    } else {

                        instance.getObserverInterface().displaySuccessActivity();
                        instance.getObserverInterface().displayFormSuccessActivity();
                    }

                } else {

                    instance.getObserverInterface().displaySuccessActivity();
                    instance.getObserverInterface().displayFormSuccessActivity();
                }

                //attach observer type information within event
                eventData.observerType = instance.getObserverInterface().getType();

                var record = eventData.result;

                instance.getObserverInterface().updateView({
                    'requestData': eventData.data,
                    'resetForm': eventData.data.resetForm,
                    'refObject': eventData.data.refObject,
                    'refActions': eventData.data.refActions,
                    'refInputs': eventData.data.refInputs,
                    'action': eventData.request,
                    'resultRecord': record,
                    'placement': (options.placement !== null && options.placement !== undefined) ? options.placement : 'first'
                });
            }
        }

        eventData.event = "record.after.rendering.view.CRUD.WindnTrees";
        instance.notify(eventData);
    };

    /// <summary>
    /// Multiple records processing and presenting event subscription.
    /// </summary>
    instance.presentRecords = function (event, eventData) {

        eventData.event = "records.before.rendering.view.CRUD.WindnTrees";
        instance.notify(eventData);

        if (eventData !== null && eventData !== undefined) {
            if (eventData.code !== null && eventData.code !== undefined) {

                if (eventData.code.length > 0) {
                    instance.setAuthorizationCode(eventData.code);

                    if (eventData.data.codeField !== null && eventData.data.codeField !== undefined) {

                        $('[name=' + eventData.data.codeField + ']').val(eventData.code);
                    }
                    else {

                        $('[name=__RequestVerificationToken]').val(eventData.code);
                    }
                }
            }
        }

        var listNumber = 1;
        var listSize = 10;

        //update page or list number from event data in a conventional
        //non-query object find call
        if (eventData.data.page !== null && eventData.data.page !== undefined) {

            listNumber = eventData.data.page;
        }
        else if (eventData.data.query !== null && eventData.data.query !== undefined) {

            //otherwise extract page and size values from query object
            if (eventData.data.query.page !== null && eventData.data.query.page !== undefined) {

                listNumber = eventData.data.query.page;
            }

            if (eventData.data.query.size !== null && eventData.data.query.size !== undefined) {

                listSize = eventData.data.query.size;
            }
        }


        if (eventData.data.callback !== null &&
                eventData.data.callback !== undefined) {

            eventData.observerType = "callback";

            eventData.data.callback(eventData.result);
        } else {

            if (instance.getObserverInterface() !== null && instance.getObserverInterface() !== undefined) {

                //attach observer type information within event
                eventData.observerType = instance.getObserverInterface().getType();

                var records = eventData.result;

                if (instance.getObserverInterface().getType() === "ListObserver" ||
                        instance.getObserverInterface().getType() === "ListKNObserver") {
                    
                    instance.getObserverInterface().fillList({'objects': records});

                } else {


                    if (eventData.data.messageType !== null &&
                        eventData.data.messageType !== undefined) {

                        if (eventData.data.messageType === 'brief') {

                            instance.getObserverInterface().displaySaved();

                        } else {

                            instance.getObserverInterface().displayProcessing(false);
                            instance.getObserverInterface().displaySuccessActivity();
                        }

                    } else {

                        instance.getObserverInterface().displayProcessing(false);
                        instance.getObserverInterface().displaySuccessActivity();
                    }

                    if (eventData.data.fill !== null && eventData.data.fill !== undefined) {

                        if (eventData.data.fill === 'continue') {

                            var currentList = instance.getObserverInterface().getCurrentList();
                            var existingRecords = instance.getObserverInterface().getRecords();
                            var newRecords = eventData.result;

                            if (eventData.data.page <= currentList) {
                                var listSize = instance.getObserverInterface().getListSize();
                                var listCount = instance.getObserverInterface().getRecords().length;

                                var startIndex = listSize * (eventData.data.page - 1);
                                startIndex = startIndex < 0 ? 0 : startIndex;

                                var deleteCount = listCount - startIndex;
                                deleteCount = deleteCount < 0 ? 0 : deleteCount;

                                existingRecords.splice(startIndex, deleteCount);
                            }

                            for (var i = 0; i < newRecords.length; i++) {
                                existingRecords.push(newRecords[i]);
                            }

                            //instance.getObserverInterface().setRecords(existingRecords);

                            instance.getObserverInterface().fillListRecordsView({
                                'page': listNumber,
                                'responseData': instance.getCRUDProcessor().responseData(),
                                'records': existingRecords,
                                'fill': 'continue',
                                'messageType': eventData.data.messageType
                            });

                        } else {

                            instance.getObserverInterface().fillListRecordsView({
                                'page': listNumber,
                                'responseData': instance.getCRUDProcessor().responseData(),
                                'records': records,
                                'messageType': eventData.data.messageType
                            });
                        }

                    } else {

                        instance.getObserverInterface().fillListRecordsView({
                            'page': listNumber,
                            'responseData': instance.getCRUDProcessor().responseData(),
                            'records': records,
                            'messageType': eventData.data.messageType
                        });
                    }
                }
            }
        }

        eventData.event = "records.after.rendering.view.CRUD.WindnTrees";
        instance.notify(eventData);
    };

    /// <summary>
    /// Presents request failure.
    /// </summary>
    instance.presentFailRequest = function (event, eventData) {

        eventData.event = "fail.before.rendering.view.CRUD.WindnTrees";
        instance.notify(eventData);

        if (eventData.data.callback !== null &&
                eventData.data.callback !== undefined) {

            eventData.data.callback(eventData.result);
        } else {

            if (instance.getObserverInterface() !== null &&
                instance.getObserverInterface() !== undefined) {

                if (eventData.data.messageType !== null &&
                    eventData.data.messageType !== undefined) {

                    if (eventData.data.messageType === 'brief') {

                        instance.getObserverInterface().displayFailed();

                    } else {

                        instance.getObserverInterface().displayFailureActivity();
                    }

                } else {

                    instance.getObserverInterface().displayFailureActivity();
                }
            }
        }
        eventData.event = "fail.after.rendering.view.CRUD.WindnTrees";
        instance.notify(eventData);
    };

    /// <summary>
    /// Subscribe CRUDProcessor events.
    /// </summary>
    instance.subscribeEvents = function (eventsInstance) {
        eventsInstance = (eventsInstance !== null && eventsInstance !== undefined) ? eventsInstance : instance;
        
        $(instance.getCRUDProcessor()).on('errors.processor.CRUD.WindnTrees', eventsInstance.presentErrors);
        $(instance.getCRUDProcessor()).on('record.processor.CRUD.WindnTrees', eventsInstance.presentRecord);
        $(instance.getCRUDProcessor()).on('records.processor.CRUD.WindnTrees', eventsInstance.presentRecords);
        $(instance.getCRUDProcessor()).on('fail.processor.CRUD.WindnTrees', eventsInstance.presentFailRequest);
    };

    /// <summary>
    /// Subscribe CRUDProcessor events.
    /// </summary>
    instance.unSubscribeEvents = function (eventsInstance) {
        
        eventsInstance = (eventsInstance !== null && eventsInstance !== undefined) ? eventsInstance : instance;
        
        $(instance.getCRUDProcessor()).off('errors.processor.CRUD.WindnTrees', eventsInstance.presentErrors);
        $(instance.getCRUDProcessor()).off('record.processor.CRUD.WindnTrees', eventsInstance.presentRecord);
        $(instance.getCRUDProcessor()).off('records.processor.CRUD.WindnTrees', eventsInstance.presentRecords);
        $(instance.getCRUDProcessor()).off('fail.processor.CRUD.WindnTrees', eventsInstance.presentFailRequest);
    };
    
    if (options.events !== null && options.events !== undefined) {
        if (options.events) {
            instance.subscribeEvents();
        }
    } else {
        instance.subscribeEvents();
    }
    
    if (options.instance !== null && options.instance !== undefined) {
        return Object.create(instance);
    }
    
    return instance;
}