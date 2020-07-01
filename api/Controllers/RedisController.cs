using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace api
{
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _database;
        public RedisController(IDatabase database)
        {
            _database = database;
        }

        [HttpGet("/{key}")]
        public string Consultar(string key)
        {
            return _database.StringGet(key);
        }

        [HttpPost("/")]
        public IActionResult Guardar([FromBody] KeyValuePair<string, string> kvp)
        {
            if (_database.KeyExists(kvp.Key))
                return Conflict("Ya existe la llave");
            return Ok(_database.StringSet(kvp.Key, kvp.Value));
        }
    }
}