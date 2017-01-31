using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiService.Models;


namespace WebApiService.Controllers
{
    public class WordController : ApiController
    {
        public IHttpActionResult GetWordById(string wordId)
        {
            var word = ParseController.GetWordFromId(wordId);
            if (word == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Ok(word);
        }
        public IHttpActionResult GetIdByName(string name)
        {
            var result = ParseController.GetIDFromWord(name);
            if (result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Ok(result);
        }
    }
}
