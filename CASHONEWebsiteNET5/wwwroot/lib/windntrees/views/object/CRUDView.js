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
/// CRUDView provides data handling and communication capability using create, read, update, delete and list calls to a hosted web service or API controller and gets typed content objects based on input object. CRUDView extends from SearchView and allows CRUD operations.
/// </summary>
function CRUDView(options) {
    var instance = (options.instance !== null && options.instance !== undefined) ? options.instance : this;
    var extender = new InstanceExtender();
    
    if (options.instance === null || options.instance === undefined) {
        instance = extender.extendNewInstance({ 'instance': instance, 'options': options});
    }
    
    //extend from search view
    var extOptions = Object.create(options);
    extOptions.instance = instance;
    extOptions.events = false;
    instance = SearchView(extOptions);

    /// <summary>
    /// Gets the type of view.
    /// </summary>
    instance.getType = function () {
        return 'CRUDView';
    };

    /// <summary>
    /// Format the content object time fields.
    /// </summary>
    instance.formatContent = function (options) {

        //get form object
        var content = (options.content !== null && options.content !== undefined) ? options.content : instance.getObserverInterface().getFormObject();

        //if update have time fields then format them in proper order.
        if (options.TimeFields !== null && options.TimeFields !== undefined) {

            if (Array.isArray(options.TimeFields)) {

                for (var i = 0; i < options.TimeFields.length; i++) {

                    //for each time field format time value. If the time field is a function then evalute and format otherwise just format it.

                    if (typeof (content[options.TimeFields[i]]) === "function") {

                        if (content[options.TimeFields[i]]() !== null && content[options.TimeFields[i]]() !== undefined) {

                            content[options.TimeFields[i]](moment(typeof (content[options.TimeFields[i]]) === "function" ? content[options.TimeFields[i]]() : content[options.TimeFields[i]]).format('YYYY-MM-DD HH:mm:ss'));
                        }
                    }
                    else {

                        if (content[options.TimeFields[i]] !== null && content[options.TimeFields[i]] !== undefined) {

                            content[options.TimeFields[i]] = moment(typeof (content[options.TimeFields[i]]) === "function" ? content[options.TimeFields[i]]() : content[options.TimeFields[i]]).format('YYYY-MM-DD HH:mm:ss');
                        }
                    }
                }

            } else {

                if (content[options.TimeFields] !== null && content[options.TimeFields] !== undefined) {

                    content[options.TimeFields] = moment(typeof (content[options.TimeFields]) === "function" ? content[options.TimeFields]() : content[options.TimeFields]).format('YYYY-MM-DD HH:mm:ss');
                }
            }

            content = instance.getObserverInterface().getStringifiedObject(content);

        } else {

            content = (options.content === null || options.content === undefined) ? (instance.getObserverInterface().getFormStringifiedObject()) : instance.getObserverInterface().getStringifiedObject(options.content);
        }

        if (options.target === 'CreateFileContent' || options.target === 'UpdateFileContent') {

            var uploadForm = (options.form !== null && options.form !== undefined) ? options.form : '__uploadform';

            if (document.forms[uploadForm] !== null && document.forms[uploadForm] !== undefined) {

                if (options.TimeFields !== null && options.TimeFields !== undefined) {

                    if (Array.isArray(options.TimeFields)) {

                        for (var i = 0; i < options.TimeFields.length; i++) {

                            if (document.forms[uploadForm][options.TimeFields[i]] !== null && document.forms[uploadForm][options.TimeFields[i]] !== undefined) {

                                //for each time input value format time value.
                                if (document.forms[uploadForm][options.TimeFields[i]].value !== null && document.forms[uploadForm][options.TimeFields[i]].value !== undefined) {

                                    document.forms[uploadForm][options.TimeFields[i]].value = moment((document.forms[uploadForm][options.TimeFields[i]]).value).format('YYYY-MM-DD HH:mm:ss');
                                }
                            }
                        }
                    } else {

                        document.forms[uploadForm][options.TimeFields].value = moment((document.forms[uploadForm][options.TimeFields]).value).format('YYYY-MM-DD HH:mm:ss');
                    }
                }
            }
        }

        return content;
    };

    /// <summary>
    /// Evaluates field against form content object.
    /// </summary>
    instance.evaluateOption = function (field) {

        var formObject = instance.getObserverInterface().getFormObject();

        if (formObject !== null && formObject !== undefined) {

            if (formObject[field] !== null && formObject[field] !== undefined) {

                field = ((typeof (formObject[field]) === "function") ? formObject[field]() : formObject[field]);
            }
        }

        return field;
    };

    /// <summary>
    /// Creates new content at designated URI address with input object.
    /// </summary>
    instance.create = function (options) {
        options = (options === null || options === undefined) ? {} : options;
        options.action = 'create';

        options.content = instance.formatContent(options);

        options.validate = (options.validate === null || options.validate === undefined) ? true : options.validate;
        options.placement = (options.placement === null || options.placement === undefined) ? 'first' : options.placement;
        instance.createOrUpdateOrDelete(options);
    };

    /// <summary>
    /// Updates existing content at designated URI address with input object.
    /// </summary>
    instance.update = function (options) {
        options = (options === null || options === undefined) ? {} : options;
        options.action = 'update';

        options.content = instance.formatContent(options);

        instance.createOrUpdateOrDelete(options);
    };

    /// <summary>
    /// Deletes exisiting content at designated URI address with input object.
    /// </summary>
    instance.delete = function (options) {
        options = (options === null || options === undefined) ? {} : options;
        options.action = 'delete';
        
        if (options.content !== null && options.content !== undefined) {
            
            try {
                if (options.content.getType() === 'DetailKNObserver' ||
                        options.content.getType() === 'DetailObserver') {

                    options.content = options.content.getRecord();
                }
            } catch (e) {
            }
        }

        options.content = instance.formatContent(options);

        //options.content = (options.content === null || options.content === undefined) ? (instance.getObserverInterface().getFormStringifiedObject()) : instance.getObserverInterface().getStringifiedObject(options.content);
        instance.createOrUpdateOrDelete(options);
    };

    if (instance.getObserverInterface() !== null &&
            instance.getObserverInterface() !== undefined) {

        /**
         * Observer create function definition.
         * 
         * @param {type} options 
         * @returns {undefined}
         */
        instance.getObserverInterface().create = function (options) {
            instance.create(options);
        };

        /**
         * Observer update function definition.
         * 
         * @param {type} options 
         * @returns {undefined}
         */
        instance.getObserverInterface().update = function (options) {
            instance.update(options);
        };

        /**
         * Observer delete function definition.
         * 
         * @param {type} record
         * @returns {undefined}
         */
        instance.getObserverInterface().delete = function (record) {

            if (record.content === null || record.content === undefined) {
                
                instance.delete({
                    'content': record
                });
                
            } else {
                //it is already composed with content and other options.

                instance.delete(record);
            }
        };
    }

    if (instance.getObserverObject() !== null &&
            instance.getObserverObject() !== undefined) {

        /**
         * Observer create function definition.
         * 
         * @param {type} options 
         * @returns {undefined}
         */
        instance.getObserverObject().create = function (options) {
            instance.create(options);
        };

        /**
         * Observer update function definition.
         * 
         * @param {type} options 
         * @returns {undefined}
         */
        instance.getObserverObject().update = function (options) {
            instance.update(options);
        };

        /**
         * Observer delete function definition.
         * 
         * @param {type} record
         * @returns {undefined}
         */
        instance.getObserverObject().delete = function (record) {

            if (record.content === null || record.content === undefined) {

                instance.delete({
                    'content': record
                });

            } else {
                //it is already composed with content and other options.

                instance.delete(record);
            }
        };
    }

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