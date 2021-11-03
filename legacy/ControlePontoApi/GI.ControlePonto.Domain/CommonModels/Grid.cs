
using Newtonsoft.Json.Linq;

namespace GI.ControlePonto.Domain.CommonModels
{
    public class Grid
    {
        public int page { get; set; }

        public int range { get; set; } = 10;

        public JObject filter { get; set; }
    }
}