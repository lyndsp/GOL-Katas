using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using GameOfLife.Web.Controllers;
using GameOfLife.Web.Models;
using NUnit.Framework;

namespace GameOfLife.Web.UnitTests
{
    [TestFixture]
    public class GenerationControllerTests
    {
        private GenerationController _controller;
        private Population _population;

        [SetUp]
        public void SetupController()
        {
            _controller = new GenerationController();
            SetupControllerForTests(_controller);

            var cells = new Collection<IEnumerable<short>>
                    {
                        new short[] {0, 1, 0},
                        new short[] {1, 1, 0},
                        new short[] {0, 1, 0},
                    };

            _population = new Population() { Cells = cells };
        }

        [Test]
        public void PostCreatesCivilisation()
        {
            var response = _controller.Post(_population);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void PostReturnsCivilisationsLocation()
        {
            var response = _controller.Post(_population);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            Assert.That(response.Headers.Location.ToString(), Is.Not.Empty);
        }

        [Test]
        public void PostCreatesNewCivilisationResource()
        {
            var firstId = GetResourceId( _controller.Post(_population) );
            var secondId = GetResourceId( _controller.Post(_population) );

            Assert.That(firstId, Is.Not.EqualTo(secondId));
        }

        [Test]
        public void GetReturnsNotFoundForIncorrectCivilisationId()
        {
            var exception = Assert.Throws<HttpResponseException>(() => _controller.Get(int.MaxValue));

            Assert.That(exception.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));            
        }

        [Test]
        public void GetReturnsTheCorrectCivilisation()
        {
            var resourceId = GetResourceId(_controller.Post(_population));
            
            var response = _controller.Get(resourceId);

            Assert.That(response.Id, Is.EqualTo(resourceId));
        }

        [Test]
        public void PutThrowsNotFoundForIncorrectCivilisationId()
        {
            var exception = Assert.Throws<HttpResponseException>(() => _controller.Put(int.MaxValue, 1));

            Assert.That(exception.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void PutEvolvesTheCorrectCivilisation()
        {
            var resourceId = GetResourceId(_controller.Post(_population));

            var response = _controller.Put(resourceId, 1);

            Assert.That(response.Id, Is.EqualTo(resourceId));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        public void PutUpdatesCivilisationGenerationCount(int evolutions, int expectedGenerationCount)
        {
            var resourceId = GetResourceId(_controller.Post(_population));

            var response = _controller.Put(resourceId, evolutions);

            Assert.That(response.Generation, Is.EqualTo(expectedGenerationCount));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        public void SuccessivePutsUpdateCivilisationGenerationCount(int iterationCount, int expectedGenerationCount)
        {
            var resourceId = GetResourceId(_controller.Post(_population));

            var iterations = new int[iterationCount];

            var response = new Civilisation();

            foreach (var i in iterations)
            {
                response = _controller.Put(resourceId, 1);                
            }

            Assert.That(response.Generation, Is.EqualTo(expectedGenerationCount));
        }

        private static void SetupControllerForTests(ApiController controller)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/generation");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "generation" } });

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            controller.Request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
        }

        private static int GetResourceId(HttpResponseMessage response)
        {
            var uriSegments = response.Headers.Location.Segments;
            
            return int.Parse(uriSegments[uriSegments.Length - 1]);            
        }
    }
}
