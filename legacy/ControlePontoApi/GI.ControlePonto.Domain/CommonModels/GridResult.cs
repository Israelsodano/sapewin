using System.Collections.Generic;
namespace GI.ControlePonto.Domain.CommonModels
{
    public class GridResult<T> where T : class
    {
        public int pages { get; set; }
        public int count { get; set; }
        public int page { get; set; }
        public IEnumerable<T> list { get; set; }
    }
}