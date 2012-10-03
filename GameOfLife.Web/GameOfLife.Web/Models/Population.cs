using System.Collections.Generic;

namespace GameOfLife.Web.Models
{
    public class Population
    {
        public IEnumerable<IEnumerable<short>> Cells;
    }
}