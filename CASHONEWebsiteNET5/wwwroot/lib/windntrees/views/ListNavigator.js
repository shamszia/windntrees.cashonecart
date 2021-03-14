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
/// Represents paging or listing hyper link.
/// </summary>
function NavLink(number, listsource) {
    var instance = this;

    /// <summary>
    /// ListSource, data member property.
    /// </summary>
    instance.ListSource = listsource;

    /// <summary>
    /// Number, data member property.
    /// </summary>
    instance.Number = number;

    /// <summary>
    /// List records based on list source.
    /// </summary>
    instance.list = function (options, fill) {

        if (instance.ListSource !== null && instance.ListSource !== undefined) {

            return instance.ListSource.list(options, fill);
        }
    };
}

/// <summary>
/// ListNavigator data structure used to render lists with scroll size.
/// </summary>
function ListNavigator(options) {
    var instance = this;

    /// <summary>
    /// Lists, data member array property.
    /// </summary>
    instance.Lists = [];

    /// <summary>
    /// CurrentList, data member array property.
    /// </summary>
    instance.CurrentList = (options.currentList !== null && options.currentList !== undefined) ? options.currentList : 1;

    /// <summary>
    /// ListSize, data member array property.
    /// </summary>
    instance.ListSize = (options.listSize !== null && options.listSize !== undefined) ? options.listSize : 10;

    /// <summary>
    /// TotalRecords, data member array property.
    /// </summary>
    instance.TotalRecords = (options.totalRecords !== null && options.totalRecords !== undefined) ? options.totalRecords : 0;

    /// <summary>
    /// ScrollSize, data member array property.
    /// </summary>
    instance.ScrollSize = (options.scrollSize !== null && options.scrollSize !== undefined) ? options.scrollSize : 10; // Number of pages dispalyed

    /// <summary>
    /// Calculates total number of pages or lists.
    /// </summary>
    instance.calculateTotalPages = function () {
        return Math.ceil(instance.TotalRecords / instance.ListSize);
    };

    /// <summary>
    /// Composes navigation or listing links.
    /// </summary>
    instance.composeLists = function (listsource) {
        instance.Lists = [];
        //find the pager scroll size offset to find min and max pages
        var pageOffset = Math.ceil(instance.ScrollSize / 2);
        var minPageNumber = options.currentList - pageOffset;
        var maxPageNumber = options.currentList + pageOffset;

        if (minPageNumber <= 0) {
            minPageNumber = 1;
        }

        if (minPageNumber + instance.ScrollSize > instance.calculateTotalPages()) {
            maxPageNumber = instance.calculateTotalPages();
        }
        else {
            maxPageNumber = minPageNumber + instance.ScrollSize;

            if (maxPageNumber > instance.calculateTotalPages()) {
                maxPageNumber = instance.calculateTotalPages();
            }
        }
        for (var i = minPageNumber; i <= maxPageNumber; i++) {

            var ls = (listsource !== null && listsource !== undefined) ? listsource : options.listsource;
            instance.Lists.push(new NavLink(i, ls));
        }
    };

    /// <summary>
    /// Gets navigation or listing array.
    /// </summary>
    instance.getLists = function () {
        return instance.Lists;
    };

    /// <summary>
    /// Instantiate new instance of ListNavigator.
    /// </summary>
    instance.newObject = function(options) {
        return new ListNavigator(options);
    };
    
    instance.composeLists();
}