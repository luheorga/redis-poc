using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<string> Consultar(string key)
        {
            return await _database.StringGetAsync(key);
        }

        [HttpPost("/")]
        public async Task<IActionResult> Guardar([FromBody] KeyValuePair<string, string> kvp)
        {
            if (await _database.KeyExistsAsync(kvp.Key))
                return Conflict("Ya existe la llave");

            return Ok(await _database.StringSetAsync(kvp.Key, kvp.Value));
        }

        [HttpPost("/Hash/{key}")]
        public async Task GuardarHash(string key, [FromBody] List<KeyValuePair<string, string>> campos)
        {
            var camposHash = campos.Select(kvp => new HashEntry(kvp.Key, kvp.Value));
            await _database.HashSetAsync(key, camposHash.ToArray());
        }


        [HttpGet("/Hash/{key}")]
        public async Task<Dictionary<string, string>> ObtenerHash(string key)
        {
            var entries = await _database.HashGetAllAsync(key);
            return entries.ToStringDictionary();
        }
    }
}