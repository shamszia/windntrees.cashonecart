function OrderItem(data) {
    var instance = this;

    instance._datakey = data.Uid;
    instance.Uid = ko.observable(data.Uid);
    instance.OrderId = ko.observable(data.OrderId);
    instance.ItemId = ko.observable(data.ItemId);
    instance.ItemName = ko.observable(data.ItemName);
    instance.ItemCode = ko.observable(data.ItemCode);
    instance.Quantity = ko.observable(data.Quantity);
    instance.Cost = ko.observable(data.Cost);
    instance.Discount = ko.observable((data.Discount === null || data.Discount === undefined) ? 0 : data.Discount);
    instance.RowVersion = ko.observable(data.RowVersion);
}