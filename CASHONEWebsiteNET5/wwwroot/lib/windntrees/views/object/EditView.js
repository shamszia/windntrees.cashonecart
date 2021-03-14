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
/// EditView provides data handling and communication capability using read and update calls to a hosted webservice or API controller and gets typed content objects based on input object. EditView extends ObjectView and allows editing of existing objects.
/// </summary>
function EditView(options) {
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
        return "EditView";
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
    
    if (instance.getObserverInterface() !== null &&
            instance.getObserverInterface() !== undefined) {
        
        /** 
         * Observer create function definition.
         * 
         * @param {type} options
         * @returns {undefined}
         */
        instance.getObserverInterface().update = function (options) {
            instance.update(options);
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
        instance.getObserverObject().update = function (options) {
            instance.update(options);
        };

        /** 
         * Observer view getType() function definition.
         * 
         * @param {type} options
         * @returns {undefined}
         */
        instance.getObserverObject().getViewType = function (options) {
            return instance.getType();
        };
    }
    
    if (options.instance !== null && options.instance !== undefined) {
        return Object.create(instance);
    }
    
    return instance;
}