function Order(data) {
    var instance = this;
    instance._datakey = data.Uid;
    instance.Uid = ko.observable(data.Uid);
    instance.OrderTime = ko.observable(data.OrderTime);
    instance.OrderNo = ko.observable(data.OrderNo);
    instance.OrderType = ko.observable(data.OrderType);

    instance.Title = ko.observable(data.Title);
    instance.Title.extend({
        required: true,
        maxLength: 100
    });
    instance.Company = ko.observable(data.Company);
    instance.Company.extend({
        required: true,
        maxLength: 100
    });
    instance.Address = ko.observable(data.Address);
    instance.Address.extend({
        required: true,
        maxLength: 200
    });
    instance.City = ko.observable(data.City);
    instance.City.extend({
        required: true,
        maxLength: 100
    });
    instance.Country = ko.observable(data.Country);
    instance.Country.extend({
        required: true,
        maxLength: 100
    });
    instance.PostalCode = ko.observable(data.PostalCode);
    instance.PostalCode.extend({
        required: true,
        maxLength: 10
    });

    instance.Email = ko.observable(data.Email);
    instance.Email.extend({
        required: true,
        maxLength: 127
    });

    instance.Cell = ko.observable(data.Cell);
    instance.Cell.extend({
        required: true,
        maxLength: 15
    });
    
    instance.Status = ko.observable(data.Status);
    instance.UserId = ko.observable(data.UserId);

    instance.SecretWord = ko.observable(data.SecretWord);
    instance.SecretWord.extend({
        required: true,
        maxLength: 20
    });

    instance.ConfirmSecretWord = ko.observable(data.ConfirmSecretWord);
    instance.ConfirmSecretWord.extend({
        required: true,
        maxLength: 20,
        message: "Secret and confirm secret word are required and must match."
    });

    instance.RowVersion = ko.observable(data.RowVersion); 

    instance.OrderItems = [];
}