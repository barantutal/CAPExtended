using System.Data;
using Dapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sample.RabbitMQ.PostgreSql.Controllers;

[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ICapPublisher _capBus;

    public ValuesController(ICapPublisher capPublisher)
    {
        _capBus = capPublisher;
    }
    
    [Route("AdonetWithTransaction")]
    public IActionResult AdonetWithTransaction([FromServices] AppDbContext dbContext)
    {
        using (var connection = dbContext.Database.GetDbConnection())
        {
            using (var transaction = connection.BeginTransaction(_capBus, autoCommit: false))
            {
                connection.Execute("INSERT INTO \"public\".\"Persons\" (\"Name\") VALUES ('test')", transaction: (IDbTransaction)transaction.DbTransaction);

                _capBus.Publish("sample.rabbitmq.postgresql", "test5");

                transaction.Commit();
            }
        }

        return Ok();
    }

    [NonAction]
    [CapSubscribe("sample.rabbitmq.postgresql")]
    public void Subscriber(string content, string c2)
    {
        throw new Exception("...");
        Console.WriteLine($"Consume time: {DateTime.Now} \r\n   --> " +content);
    }
}