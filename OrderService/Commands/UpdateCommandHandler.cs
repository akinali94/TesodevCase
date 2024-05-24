using MongoDB.Driver;
using OrderService.Configs;
using OrderService.Models;
using OrderService.V1.Models.CommandModels;

namespace OrderService.Commands;

public class UpdateCommandHandler
{
    private readonly DbContext _database;

    public UpdateCommandHandler(DbContext database)
    {
        _database = database;
    }

    public async Task<bool> Handle(string id, UpdateCommand command)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        var update = Builders<Order>.Update
            .Set(o => o.CustomerId, command.CustomerId)
            .Set(o => o.Quantity, command.Quantity)
            .Set(o => o.Address, command.Addresses)
            .Set(o => o.Products, command.Products)
            .Set(o => o.UpdatedAt, DateTime.Now);

        var result = await _database.Orders.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }
}