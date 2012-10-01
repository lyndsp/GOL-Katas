using System.Web.Http;
using GameOfLife.Web.Models;

namespace GameOfLife.Web.Controllers
{
    public class GenerationController : ApiController
    {
        public GameState Post([FromBody]string value)
        {
            return new GameState();
        }
    }
}