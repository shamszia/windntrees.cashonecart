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
/// InstanceExtender extends instances with common and useful functions.
/// </summary>
function InstanceExtender(options) {
    var instance = this;
    
    /// <summary>
    /// Extends instance with new instance creation functionality.
    /// </summary>
    instance.extendNewInstance = function (extoptions) {
        var instance = extoptions.instance;

        /// <summary>
        /// Integrates new instance functionality with provided instance.
        /// </summary>
        instance.newInstance = function (options) {
            
            var instanceOptions = (options !== null && options !== undefined) ? options : instance.newOptions();
            instanceOptions.instance = undefined;
            
            if (extoptions.newparameter !== null && extoptions.newparameter !== undefined) {

                if (Array.isArray(extoptions.newparameter)) {

                    var newInstances = [];
                    for (var i = 0; i < extoptions.newparameter.length; i++) {

                        if (instanceOptions.observer !== null && instanceOptions.observer !== undefined) {
                            instanceOptions.observer = extoptions.newparameter.newInstance();
                        }
                        
                        newInstances[i] = extoptions.newparameter[i].newInstance();
                    }

                    return new (Object.getPrototypeOf(instance)).constructor(newInstances);
                } else {
                    
                    if (instanceOptions.observer !== null && instanceOptions.observer !== undefined) {
                        
                        instanceOptions.observer = extoptions.newparameter.newInstance();
                    }
                    
                    instanceOptions.instance = undefined;
                    return new (Object.getPrototypeOf(instance)).constructor(instanceOptions);
                }
            }
            return new (Object.getPrototypeOf(instance)).constructor(instanceOptions);
        };
        
        /// <summary>
        /// Returns object instantiation options.
        /// </summary>
        instance.newOptions = function () {
            
            return extoptions.options;
        };
        
        return Object.create(instance);
    };
    
    /// <summary>
    /// Extends instance with field and related setter and getter functions.
    /// </summary>
    instance.extendField = function (options) {
        var instance = options.instance;

        if (options.field !== null && options.field !== undefined) {
            if (options.value !== null && options.value !== undefined) {                
                instance[options.field] = options.value;
            }
            
            /// <summary>
            /// Field setter function.
            /// </summary>
            instance["set" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1)] = function (object) {
                if (typeof (instance[options.field]) === 'function') {
                    instance[options.field](object);

                } else {
                    instance[options.field] = object;
                }
            };

            /// <summary>
            /// Field getter function.
            /// </summary>
            instance["get" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1)] = function () {
                if (typeof (instance[options.field]) === 'function') {
                    return instance[options.field]();

                } else {
                    return instance[options.field];
                }
            };


            /// <summary>
            /// Observable getter function.
            /// </summary>
            instance["getObservable" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1)] = function () {
                return instance[options.field];
            };


            /// <summary>
            /// Gets field stringified object.
            /// </summary>
            instance["get" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1) + "StringifiedJSONObject"] = function () {
                if (typeof (instance[options.field]) === 'function') {
                    return JSON.stringify(instance[options.field]());

                } else {
                    return JSON.stringify(instance[options.field]);
                }
            };

            /// <summary>
            /// Gets field JSON object.
            /// </summary>
            instance["get" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1) + "JSONObject"] = function () {
                if (typeof (instance[options.field]) === 'function') {
                    return JSON.parse(JSON.stringify(instance[options.field]()));

                } else {
                    return JSON.parse(JSON.stringify(instance[options.field]));
                }
            };
        }
        
        return instance;
    };

    /// <summary>
    /// Extends instance with field observer interface and related setter and getter functions.
    /// </summary>
    instance.extendFieldObserver = function (options) {
        var instance = options.instance;
        
        var observer = options.observer;

        if (options.field !== null && options.field !== undefined) {

            /// <summary>
            /// Field setter function.
            /// </summary>
            instance["set" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1)] = function (object) {
                observer["set" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1)](object);
            };

            /// <summary>
            /// Field getter function.
            /// </summary>
            instance["get" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1)] = function () {
                return observer["get" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1)]();
            };


            /// <summary>
            /// Observable getter function.
            /// </summary>
            instance["getObservable" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1)] = function () {
                return observer["getObservable" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1)]();
            };

            /// <summary>
            /// Gets field stringified object.
            /// </summary>
            instance["get" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1) + "StringifiedJSONObject"] = function () {
                return observer["get" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1) + "StringifiedJSONObject"]();
            };

            /// <summary>
            /// Gets field JSON object.
            /// </summary>
            instance["get" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1) + "JSONObject"] = function () {
                return observer["get" + options.field.substr(0, 1).toUpperCase() + options.field.substr(1, options.field.length - 1) + "JSONObject"]();
            };
        }
        
        return instance;
    };

    /// <summary>
    /// Extends instance with Object Interface.
    /// </summary>
    instance.extendObjectInterface = function (options) {
        var instance = options.instance;

        if (options.field !== null && options.field !== undefined) {

            /// <summary>
            /// Sets field with object.
            /// </summary>
            instance.setObject = function (object) {
                if (typeof (instance[options.field]) === 'function') {
                    instance[options.field](object);

                } else {
                    instance[options.field] = object;
                }
            };

            /// <summary>
            /// Gets field object.
            /// </summary>
            instance.getObject = function () {
                if (typeof (instance[options.field]) === 'function') {
                    return instance[options.field]();

                } else {
                    return instance[options.field];
                }
            };

            /// <summary>
            /// Gets field observer object.
            /// </summary>
            instance.getObserverObject = function () {
                if (typeof (instance[options.field]) === 'function') {
                    return instance[options.field]();

                } else {
                    return instance[options.field];
                }
            };

            /// <summary>
            /// Gets observable field.
            /// </summary>
            instance.getObservableObject = function () {
                return instance[options.field];
            };

            /// <summary>
            /// Gets stringified field object.
            /// </summary>
            instance.getStringifiedObject = function () {
                if (typeof (instance[options.field]) === 'function') {
                    return JSON.stringify(instance[options.field]());

                } else {
                    return JSON.stringify(instance[options.field]);
                }
            };

            /// <summary>
            /// Gets JSON field object.
            /// </summary>
            instance.getJSONObject = function () {
                if (typeof (instance[options.field]) === 'function') {
                    return JSON.parse(JSON.stringify(instance[options.field]()));

                } else {
                    return JSON.parse(JSON.stringify(instance[options.field]));
                }
            };
        }
        
        return Object.create(instance);
    };

    /// <summary>
    /// Extends observer with Observer Interface.
    /// </summary>
    instance.extendObserverInterface = function (options) {
        var instance = options.instance;
        
        var observer = options.observer;

        if (instance !== null && instance !== undefined) {

            if (observer !== null && observer !== undefined) {

                /// <summary>
                /// Sets object using observer interface.
                /// </summary>
                instance.setObject = function (object) {
                    observer.setObject(object);
                };

                /// <summary>
                /// Gets object using observer interface.
                /// </summary>
                instance.getObject = function () {
                    return observer.getObject();
                };

                /// <summary>
                /// Gets object using observer interface.
                /// </summary>
                instance.getObserverObject = function () {
                    return observer.getObject();
                };

                /// <summary>
                /// Gets observable object using observer interface.
                /// </summary>
                instance.getObservableObject = function () {
                    return observer.getObservableObject();
                };

                /// <summary>
                /// Gets stringified JSON object using observer interface.
                /// </summary>
                instance.getStringifiedObject = function () {
                    return observer.getStringifiedObject();
                };

                /// <summary>
                /// Gets JSON object using observer interface.
                /// </summary>
                instance.getJSONObject = function () {
                    return observer.getJSONObject();
                };

                /// <summary>
                /// Gets concrete observer type using observer interface.
                /// </summary>
                instance.getObserverType = function () {
                    return observer.getType();
                };

                /// <summary>
                /// Gets concrete observer using observer interface.
                /// </summary>
                instance.getObserver = function () {
                    return observer;
                };
            }
        }
        
        return Object.create(instance);
    };

    /// <summary>
    /// Extends instance with content information.
    /// </summary>
    instance.extendContentTypeObject = function (options) {
        var instance = options.instance;

        /// <summary>
        /// ContentType data member.
        /// </summary>
        instance.ContentType = options.contentType;


        /// <summary>
        /// Gets content object.
        /// </summary>
        instance.getContentTypeObject = function () {
            return instance.ContentType;
        };

        /// <summary>
        /// Gets content object prototype.
        /// </summary>
        instance.getContentTypeObjectPrototype = function () {
            if (instance.ContentType !== null && instance.ContentType !== undefined) {
                return Object.getPrototypeOf(instance.ContentType);
            }
            return null;
        };

        return Object.create(instance);
    };

    /// <summary>
    /// Extends instance with content observer.
    /// </summary>
    instance.extendContentObserver = function (options) {
        var instance = options.instance;
        
        var observer = options.observer;

        if (instance !== null && instance !== undefined) {
            if (observer !== null && observer !== undefined) {
                
                /**
                 * Gets content object using observer interface.
                 * 
                 * @returns {unresolved}
                 */
                instance.getContentTypeObject = function () {
                    return observer.getContentTypeObject();
                };
                
                /**
                 * Gets content object prototype using observer interface.
                 * 
                 * @returns {unresolved}
                 */
                instance.getContentTypeObjectPrototype = function () {
                    return observer.getContentTypeObjectPrototype();
                };
            }
        }
        
        return Object.create(instance);
    };
}
