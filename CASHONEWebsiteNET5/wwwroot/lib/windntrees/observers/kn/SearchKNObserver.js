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
/// SearchKNObserver provide reading and listing of record(s) from a http service.
/// </summary>
function SearchKNObserver(options) {
    var instance = (options.instance !== null && options.instance !== undefined) ? options.instance : this;
    var extender = new InstanceExtender();
    
    if (options.instance === null || options.instance === undefined) {
        instance = extender.extendNewInstance({ 'instance': instance, 'options': options });
    }
    
    var activityOptions = Object.create(options);
    activityOptions.instance = instance;
    instance = ActivityKNObserver(activityOptions);
    
    
    /// <summary>
    /// Record, data member property.
    /// </summary>
    instance.Record = ko.observable(null);

    /// <summary>
    /// Records, data member property.
    /// </summary>
    instance.Records = ko.observableArray([]);

    /// <summary>
    /// RecordCount, data member property.
    /// </summary>
    instance.RecordCount = ko.observable(0);

    /// <summary>
    /// SelectedRecord, data member property.
    /// </summary>
    instance.SelectedRecord = ko.observable(null);

    /// <summary>
    /// SelectedRecordIndex, data member property.
    /// </summary>
    instance.SelectedRecordIndex = -1;

    /// <summary>
    /// ListNavigator, data member property.
    /// </summary>
    instance.ListNavigator = ko.observable(new ListNavigator({ 'currentList': 1, 'listSize': 1, 'totalRecords': 0, 'scrollSize': 1, 'listsource': instance.ListSource }));

    /// <summary>
    /// ListingScrollSize, data member property.
    /// </summary>
    instance.ListingScrollSize = ko.observable(5);

    /// <summary>
    /// ListSize, data member property.
    /// </summary>
    instance.ListSize = ko.observable(10);

    /// <summary>
    /// CurrentList, data member property.
    /// </summary>
    instance.CurrentList = ko.observable(1);

    /// <summary>
    /// ListSource, data member property.
    /// </summary>
    instance.ListSource = options.listsource;
    
    /// <summary>
    /// Gets the type of function construct.
    /// </summary>
    instance.getType = function () {
        return "SearchKNObserver";
    };

    /// <summary>
    /// Sets record.
    /// </summary>
    instance.setRecord = function (record) {
        instance.Record(record);
    };

    /// <summary>
    /// Gets record.
    /// </summary>
    instance.getRecord = function () {
        return instance.Record();
    };

    /// <summary>
    /// Gets observable record.
    /// </summary>
    instance.getObservableRecord = function () {
        return instance.Record;
    };

    /// <summary>
    /// Sets list of records.
    /// </summary>
    instance.setRecords = function(data) {
        instance.Records(data);
    };

    /// <summary>
    /// Gets list of records.
    /// </summary>
    instance.getRecords = function() {
        return instance.Records();
    };

    /// <summary>
    /// Gets observable list of records.
    /// </summary>
    instance.getObservableRecords = function() {
        return instance.Records;
    };

    /// <summary>
    /// Sets record count.
    /// </summary>
    instance.setRecordCount = function(data) {
        instance.RecordCount(data);
    };

    /// <summary>
    /// Gets record count.
    /// </summary>
    instance.getRecordCount = function() {
        return instance.RecordCount();
    };

    /// <summary>
    /// Gets observable record count.
    /// </summary>
    instance.getObservableRecordCount = function() {
        return instance.RecordCount;
    };

    /// <summary>
    /// Sets selected record in detail observer.
    /// </summary>
    instance.setSelectedRecord = function (data) {
        instance.SelectedRecord(data);
    };

    /// <summary>
    /// Gets selected record from detail observer.
    /// </summary>
    instance.getSelectedRecord = function () {
        return instance.SelectedRecord();
    };

    /// <summary>
    /// Gets observable selected record.
    /// </summary>
    instance.getObservableSelectedRecord = function () {
        return instance.SelectedRecord;
    };

    /// <summary>
    /// Sets selected record index.
    /// </summary>
    instance.setSelectedRecordIndex = function(data) {
        instance.SelectedRecordIndex(data);
    };

    /// <summary>
    /// Gets selected record index.
    /// </summary>
    instance.getSelectedRecordIndex = function () {
        return instance.SelectedRecordIndex();
    };

    /// <summary>
    /// Gets observable selected record index.
    /// </summary>
    instance.getObservableSelectedRecordIndex = function () {
        return instance.SelectedRecordIndex;
    };

    /// <summary>
    /// Sets navigator data object.
    /// </summary>
    instance.setListNavigator = function (navigator) {
        instance.ListNavigator(navigator);
    };

    /// <summary>
    /// Gets list navigator object.
    /// </summary>
    instance.getListNavigator = function () {
        return instance.ListNavigator();
    };

    /// <summary>
    /// Gets observable list navigator object.
    /// </summary>
    instance.getObservableListNavigator = function () {
        return instance.ListNavigator;
    };

    /// <summary>
    /// Sets listing scroll size.
    /// </summary>
    instance.setListingScrollSize = function(data) {
        instance.ListingScrollSize(data);
    };

    /// <summary>
    /// Gets listing scroll size.
    /// </summary>
    instance.getListingScrollSize = function () {
        return instance.ListingScrollSize();
    };

    /// <summary>
    /// Gets observable listing scroll size.
    /// </summary>
    instance.getObservableListingScrollSize = function () {
        return instance.ListingScrollSize;
    };

    /// <summary>
    /// Sets list size.
    /// </summary>
    instance.setListSize = function(data) {
        instance.ListSize(data);
    };

    /// <summary>
    /// Gets list size.
    /// </summary>
    instance.getListSize = function () {
        return instance.ListSize();
    };

    /// <summary>
    /// Gets observable list size.
    /// </summary>
    instance.getObservableListSize = function () {
        return instance.ListSize;
    };

    /// <summary>
    /// Sets current list.
    /// </summary>
    instance.setCurrentList = function(data) {
        instance.CurrentList(data);
    };

    /// <summary>
    /// Gets current list number.
    /// </summary>
    instance.getCurrentList = function () {
        return instance.CurrentList();
    };

    /// <summary>
    /// Sets list source.
    /// </summary>
    instance.setListSource = function (value) {
        instance.ListSource = value;
    };

    /// <summary>
    /// Gets list source.
    /// </summary>
    instance.getListSource = function () {
        return instance.ListSource;
    };

    /// <summary>
    /// Gets observable current list object.
    /// </summary>
    instance.getObservableCurrentList = function () {
        return instance.CurrentList;
    };

    /// <summary>
    /// Gets indexed stringified text from indexed JSON object.
    /// </summary>
    instance.getIndexedStringifiedObject = function (index) {
        if (index === undefined) {
            if (instance.SelectedRecordIndex >= 0) {
                return ko.toJSON(instance.Records[instance.SelectedRecordIndex]);
            }
        } else {
            if (index >= 0) {
                return ko.toJSON(instance.Records[index]);
            }
        }
    };

    /// <summary>
    /// Gets indexed JSON object from indexed stringified object.
    /// </summary>
    instance.getIndexedJSONObject = function(index) {
        return JSON.parse(instance.getIndexedStringifiedObject(index));
    };

    /// <summary>
    /// Gets stringified text from selected data record.
    /// </summary>
    instance.getSelectedStringifiedObject = function() {
        return ko.toJSON(instance.SelectedRecord());
    };

    /// <summary>
    /// Gets JSON object from selected data record.
    /// </summary>
    instance.getSelectedJSONObject = function() {
        return JSON.parse(ko.toJSON(instance.SelectedRecord()));
    };

    /// <summary>
    /// Gets stringified text from provided immediate data object.
    /// </summary>
    instance.getStringifiedObject = function(data) {
        return ko.toJSON(data);
    };

    /// <summary>
    /// Gets JSON object from provided immediate data object.
    /// </summary>
    instance.getJSONObject = function(data) {
        return JSON.parse(ko.toJSON(data));
    };

    /// <summary>
    /// Select a record based on index value.
    /// </summary>
    instance.selectRecord = function (options) {
        
        var selectedRecord = null;
        if (options.index !== null && options.index !== undefined) {
            selectedRecord = instance.Records()[options.index];
        } else if (options.record !== null && options.record !== undefined) {
            selectedRecord = options.record;
        }
        
        if (selectedRecord.getType() === 'DetailKNObserver'
                || selectedRecord.getType() === 'DetailObserver') {
            
            instance.setRecord(selectedRecord.getRecord());
        } else {
            
            instance.setRecord(selectedRecord);
        }
    };

    /// <summary>
    /// Resets list of records, error list and record count views.
    /// </summary>
    instance.clearListRecordsView = function () {
        instance.Records([]);
        instance.Errors([]);
        instance.RecordCount(0);
    };

    /// <summary>
    /// Fills list of records and display in associated views.
    /// </summary>
    instance.fillListRecordsView = function (data) {

        if (data.clearRecords !== null && data.clearRecords !== undefined) {
            if (data.clearRecords) {
                instance.clearListRecordsView();
            }
        } else {
            instance.clearListRecordsView();
        }
        
        if (data.records !== null && data.records !== undefined) {

            instance.Records(data.records);
        }

        if (data.immediateRecords !== null && data.immediateRecords !== undefined) {
            if (data.immediateRecords) {
                if (data.records !== null && data.records !== undefined) {
                    instance.RecordCount(data.records.length);
                    var pagesNavigator = new ListNavigator({ 'currentList': instance.CurrentList(), 'listSize': instance.ListSize(), 'totalRecords': data.records.length, 'scrollSize': instance.ListingScrollSize(), 'listsource': instance.ListSource });
                    instance.ListNavigator(pagesNavigator);
                    instance.CurrentList(data.page);

                    if (data.messageType !== null &&
                        data.messageType !== undefined) {

                        if (data.messageType === 'brief') {

                            instance.displaySaved();

                        } else {

                            instance.ResultMessage(instance.getMessageRepository().get("form.found.text") + " " + data.records.length + " " + instance.getMessageRepository().get("form.records.text") + " " + instance.getMessageRepository().get("form.displayingPage.text") + " " + instance.CurrentList() + " " + instance.getMessageRepository().get("form.of.text") + " " + (instance.ListNavigator().calculateTotalPages() == 0 ? 1 : instance.ListNavigator().calculateTotalPages()) + " " + instance.getMessageRepository().get("form.totalPages.text"));
                        }
                    } else {

                        instance.ResultMessage(instance.getMessageRepository().get("form.found.text") + " " + data.records.length + " " + instance.getMessageRepository().get("form.records.text") + " " + instance.getMessageRepository().get("form.displayingPage.text") + " " + instance.CurrentList() + " " + instance.getMessageRepository().get("form.of.text") + " " + (instance.ListNavigator().calculateTotalPages() == 0 ? 1 : instance.ListNavigator().calculateTotalPages()) + " " + instance.getMessageRepository().get("form.totalPages.text"));
                    }
                }
            }
        } else {
            
            //total, currentList, listSize
            data.responseData = typeof (data.responseData) === "string" ? JSON.parse(data.responseData) : data.responseData;

            if (data.responseData !== null && data.responseData !== undefined) {

                if (data.responseData.contents !== null && data.responseData.contents !== undefined) {

                    instance.RecordCount(data.responseData.total);
                    var pagesNavigator = new ListNavigator({ 'currentList': instance.CurrentList(), 'listSize': instance.ListSize(), 'totalRecords': data.responseData.total, 'scrollSize': instance.ListingScrollSize(), 'listsource': instance.ListSource});
                    instance.ListNavigator(pagesNavigator);
                    instance.CurrentList(data.page);

                    if (data.messageType !== null &&
                        data.messageType !== undefined) {

                        if (data.messageType === 'brief') {

                            instance.displaySaved();

                        } else {

                            instance.ResultMessage(instance.getMessageRepository().get("form.found.text") + " " + data.responseData.total + " " + instance.getMessageRepository().get("form.records.text") + " " + instance.getMessageRepository().get("form.displayingPage.text") + " " + instance.CurrentList() + " " + instance.getMessageRepository().get("form.of.text") + " " + (instance.ListNavigator().calculateTotalPages() == 0 ? 1 : instance.ListNavigator().calculateTotalPages()) + " " + instance.getMessageRepository().get("form.totalPages.text"));
                        }
                    } else {

                        instance.ResultMessage(instance.getMessageRepository().get("form.found.text") + " " + data.responseData.total + " " + instance.getMessageRepository().get("form.records.text") + " " + instance.getMessageRepository().get("form.displayingPage.text") + " " + instance.CurrentList() + " " + instance.getMessageRepository().get("form.of.text") + " " + (instance.ListNavigator().calculateTotalPages() == 0 ? 1 : instance.ListNavigator().calculateTotalPages()) + " " + instance.getMessageRepository().get("form.totalPages.text"));
                    }
                }
            }
        }
    };

    /// <summary>
    /// Composes navigator observable object based on records, total records and current list.
    /// </summary>
    instance.composeNavigator = function (options) {
        
        if (options.responseData !== null && options.responseData !== undefined) {
            if (options.responseData.contents !== null && options.responseData.contents !== undefined) {
                
                instance.CurrentList(options.currentList);
                instance.RecordCount(options.responseData.total);

                var pagesNavigator = new ListNavigator({ 'currentList': instance.CurrentList(), 'listSize': instance.ListSize(), 'totalRecords': options.responseData.total, 'scrollSize': instance.ListingScrollSize(), 'listsource': instance.ListSource });
                instance.ListNavigator(pagesNavigator);
                instance.ResultMessage(instance.getMessageRepository().get("form.found.text") + " " + options.responseData.total + " " + instance.getMessageRepository().get("form.records.text") + " " + instance.getMessageRepository().get("form.displayingPage.text") + " " + instance.CurrentList() + " " + instance.getMessageRepository().get("form.of.text") + " " + (instance.ListNavigator().calculateTotalPages() == 0 ? 1 : instance.ListNavigator().calculateTotalPages()) + " " + instance.getMessageRepository().get("form.totalPages.text"));
            }
        }
    };
    
    if (options.instance !== null && options.instance !== undefined) {
        return Object.create(instance);
    }
    
    return instance;
};


