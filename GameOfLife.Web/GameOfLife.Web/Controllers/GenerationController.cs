using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using GameOfLife.Web.Models;

namespace GameOfLife.Web.Controllers
{
    public class GenerationController : ApiController
    {
        private static readonly IDictionary<int, Civilisation> Civilisations = new ConcurrentDictionary<int, Civilisation>();
        private static int _nextCivilisationId;

        public GenerationController()
        {
            // Provide a sample civilisation at id 0.
            var cells = new Collection<IEnumerable<short>>
                    {
                        new short[] {0, 1, 0},
                        new short[] {1, 1, 0},
                        new short[] {0, 1, 0},
                    };

            Civilisations[0] = new Civilisation {Cells = cells};
        }

        public Civilisation Get(int id)
        {
            if (!Civilisations.ContainsKey(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Civilisations[id];
        }

        /// <summary>
        /// Remember to set the content type header: Content-Type: application/json
        /// Post data in json is: {"Cells":[[1,1,1],[1,1,0],[1,1,1]]}
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody]Population population)
        {
            if (population == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            _nextCivilisationId++;

            var civilisation = new Civilisation { Cells = population.Cells, Generation = 0, Id = _nextCivilisationId };

            Civilisations[_nextCivilisationId] = civilisation;

            var response = Request.CreateResponse<Civilisation>(HttpStatusCode.Created, civilisation);

            var uri = Url.Link("DefaultApi", new { id = civilisation.Id });
            response.Headers.Location = new Uri(uri);

            return response;
        }

        /// <summary>
        /// Receives request in the form http://<base-uri/>/api/generation/<id/>?evolutions=<evolutions/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="evolutions"></param>
        /// <returns></returns>
        public Civilisation Put(int id, int evolutions)
        {
            if (!Civilisations.ContainsKey(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var civilisation = Civilisations[id];

            for (var i = 0; i < evolutions; i++)
            {
                civilisation.Evolve();                
            }

            return civilisation;
        }
    }
}