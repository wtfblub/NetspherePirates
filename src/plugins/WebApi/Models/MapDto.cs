using System.Collections.Generic;
using Netsphere;

namespace WebApi.Models
{
    public class MapDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<GameRule> GameRules { get; set; }
    }
}
