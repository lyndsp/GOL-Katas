using System.Collections.Generic;

namespace GameOfLife.Web.Models
{
    public class Civilisation
    {
        public int Id;
        public int Generation;
        public IEnumerable<IEnumerable<short>> Cells;

        public void Evolve()
        {
            Generation++;
        }
    }
}