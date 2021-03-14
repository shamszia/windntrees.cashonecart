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
/// NewView provides data handling and communication capability using read and create calls to a hosted web service or API controller and gets typed content objects based on input object. NewView extends from ObjectView and allows creation of new objects.
/// </summary>
function NewView(options) {
    var instance = (options.instance !== null && options.instance !== undefined) ? options.instance : this;
    var extender = new InstanceExtender();
    
    if (options.instance === null || options.instance === undefined) {
        instance = extender.extendNewInstance({ 'instance': instance, 'options': options});
    }
    
    //extend from object view
    var extOptions = Object.create(options);
    extOptions.instance = instance;
    instance = ObjectView(extOptions);

    /// <summary>
    /// Gets the type of the function construct.
    /// </summary>
    instance.getType = function () {
        return "NewView";
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
    }
    
    if (options.instance !== null && options.instance !== undefined) {
        return Object.create(instance);
    }
    
    return instance;
}