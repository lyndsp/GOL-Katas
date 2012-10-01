using GameOfLife.Web.Controllers;
using GameOfLife.Web.Models;
using NUnit.Framework;

namespace GameOfLife.Web.UnitTests
{
    [TestFixture]
    public class GenerationControllerTests
    {
        [Test]
        public void PostReturnsNextGameState()
        {
            var controller = new GenerationController();

            var result = controller.Post(null);

            Assert.That(result, Is.TypeOf<GameState>());
        }
    }
}
