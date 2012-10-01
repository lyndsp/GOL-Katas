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
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody]Civilisation value)
        {
            if (value == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            _nextCivilisationId++;

            value.Id = _nextCivilisationId;
            value.Generation = 0;

            Civilisations[_nextCivilisationId] = value;

            var response = Request.CreateResponse<Civilisation>(HttpStatusCode.Created, value);

            var uri = Url.Link("DefaultApi", new { id = value.Id });
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