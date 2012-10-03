using System.Collections.Generic;

namespace GameOfLife.Web.Models
{
    public class Civilisation : Population
    {
        public int Id;
        public int Generation;

        public void Evolve()
        {
            Generation++;
        }
    }
}