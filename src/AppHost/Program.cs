var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloak", port: 8080)
    .WithRealmImport("./keycloak/eshopping-realm.json");

var mongo = builder.AddMongoDB("mongo");
var catalogDb = mongo.AddDatabase("catalog-db");

var elasticsearch = builder.AddElasticsearch("elasticsearch");

var catalogApi = builder.AddProject<Projects.Catalog_API>("catalog-api")
    .WithReference(catalogDb)
    .WithReference(elasticsearch)
    .WaitFor(catalogDb)
    .WaitFor(elasticsearch);
var basketApi = builder.AddProject<Projects.Basket_API>("basket-api");
var paymentApi = builder.AddProject<Projects.Payment_API>("payment-api");
var notificationWorker = builder.AddProject<Projects.Notification_Worker>("notification-worker");

var orderApi = builder.AddProject<Projects.Order_API>("order-api")
    .WithReference(paymentApi)
    .WithReference(notificationWorker);

builder.AddProject<Projects.Gateway_API>("gateway-api")
    .WithReference(catalogApi)
    .WithReference(basketApi)
    .WithReference(orderApi)
    .WithReference(keycloak);

builder.Build().Run();
