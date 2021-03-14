function Product(data) {
    var instance = this;

    instance._datakey = data.Uid;
    instance.Uid = ko.observable(data.Uid);
    instance.Reference = ko.observable(data.Reference);
    instance.UserId = ko.observable(data.UserId);
    instance.Category = ko.observable(data.Category);
    instance.Manufacturer = ko.observable(data.Manufacturer);
    instance.Code = ko.observable(data.Code);
    instance.Color = ko.observable(data.Color);
    instance.Description = ko.observable(data.Description);
    instance.Name = ko.observable(data.Name);
    instance.SecondTitle = ko.observable(data.SecondTitle);
    instance.Picture = ko.observable(data.Picture);
    instance.StockLevel = ko.observable(data.StockLevel);
    instance.PublishedPrice = ko.observable(data.PublishedPrice);
    instance.SalesPrice = ko.observable(data.SalesPrice);
    instance.PurchaseCost = ko.observable(data.PurchaseCost);
    instance.Discount = ko.observable(data.Discount);
    instance.Commission = ko.observable(data.Commission);
    instance.IncomeTax = ko.observable(data.IncomeTax);
    instance.SalesTax = ko.observable(data.SalesTax);
    instance.Published = ko.observable(data.Published);
    instance.Top = ko.observable(data.Top);
    instance.Favourite = ko.observable(data.Favourite);
    instance.AvailableQuantity = ko.observable(data.AvailableQuantity);

    instance.PicturePath = ko.computed(function () {
        return '/home/getpicture/' + instance.Code();
    });
    instance.RowVersion = ko.observable(data.RowVersion);

    instance.CartQuantity = ko.observable(1);
}