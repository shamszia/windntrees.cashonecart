function Cart(data) {
    var instance = this;

    instance._datakey = data.Uid;
    instance.Uid = ko.observable(data.Uid);
    instance.Title = ko.observable(data.Title);
    instance.Email = ko.observable(data.Email);
    instance.Cell = ko.observable(data.Cell);
    instance.Company = ko.observable(data.Company);
    instance.Address = ko.observable(data.Address);
    instance.City = ko.observable(data.City);
    instance.Country = ko.observable(data.Country);
    instance.Status = ko.observable(data.Status);
    instance.UserId = ko.observable(data.UserId);
}