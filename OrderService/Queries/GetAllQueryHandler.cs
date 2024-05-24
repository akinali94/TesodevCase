using MongoDB.Driver;
using OrderService.Configs;
using OrderService.Models;
using OrderService.V1.Models.QueryModels;

namespace OrderService.Queries;

public class GetAllQueryHandler
{
    private readonly DbContext _database;

    public GetAllQueryHandler(DbContext database)
    {
        _database = database;
    }

    public async Task<List<Order>> Handle(GetAllQuery query)
    {
        return await _database.Orders.Find(o => true).ToListAsync();
    }
}