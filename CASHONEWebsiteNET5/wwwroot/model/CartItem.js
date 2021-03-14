function CartItem(data) {
    var instance = this;

    instance._datakey = data.ItemId;

    instance.Uid = ko.observable(data.ItemId);
    instance.OrderId = ko.observable(data.OrderId);
    instance.ItemId = ko.observable(data.ItemId);
    instance.ItemName = ko.observable(data.ItemName);
    instance.ItemCode = ko.observable(data.ItemCode);
    instance.AvailableQuantity = ko.observable(data.AvailableQuantity);
    instance.Quantity = ko.observable(data.Quantity);
    instance.Cost = ko.observable(data.Cost);
    instance.Discount = ko.observable((data.Discount === null || data.Discount === undefined) ? 0 : data.Discount);

    instance.PicturePath = ko.computed(function () {
        return '/home/getpicture/' + instance.ItemCode();
    });

    instance.LineTotal = ko.computed(function () {
        return (instance.Quantity() * instance.Cost()) - (instance.Quantity() * instance.Discount());
    });
}