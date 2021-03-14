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
/// Locale message to represent localized messages in a key/value store.
/// </summary>
function LocaleMessage(key, value) {
    var instance = this;

    /// <summary>
    /// key data member.
    /// </summary>
    instance.key = key;

    /// <summary>
    /// value data member.
    /// </summary>
    instance.value = value;

    /// <summary>
    /// Gets the key.
    /// </summary>
    instance.getKey = function () {
      return instance.key;  
    };


    /// <summary>
    /// Sets the key.
    /// </summary>
    instance.setKey = function (data) {
        instance.key = data;
    };

    /// <summary>
    /// Gets the value.
    /// </summary>
    instance.getValue = function () {
        return instance.value;
    };

    /// <summary>
    /// Sets the value.
    /// </summary>
    instance.setValue = function (data) {
        instance.value = data;
    };
}