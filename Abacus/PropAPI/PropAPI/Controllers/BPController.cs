using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IdentityModel;
using PropAPI.Models;
using System.Security.Principal;
using PropAPI.SAP;


using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;

namespace PropAPI.Controllers
{
    [Authorize]
    public class BPController : ApiController
    {
        [Authorize]
        // GET: api/BP
        public List<BP> Get()
        {
            BPRepo SAPBP = new BPRepo();

            return SAPBP.getBP();
        }

        // GET: ap\i/BP/5
        [Authorize]

        public BP Get(string id)
        {
            BPRepo SAPBP = new BPRepo();
            BP _bp = SAPBP.getBP(id);
            return _bp;
        }

        // POST: api/BP
        [Authorize]

        public string Post([FromBody]BP value)
        {

            BPRepo SAPBP = new BPRepo();
            string result = SAPBP.PostBP(value);

            if (result == "OK")
            {
                return "Posted Successfully";
            }
            else
            {
                return "Error in posting BP " + result;
            }

        }

        // PUT: api/BP/5
        [Authorize]

        public string Put( [FromBody]BP value)
        {
            BPRepo SAPBP = new BPRepo();
            string result = SAPBP.Update(value);

            if (result == "OK")
            {
                return "Posted Successfully";
            }
            else
            {
                return "Error in updating BP " + result;
            }

        }

        // DELETE: api/BP/5
        [Authorize]

        public string Delete(string value)
        {
            BPRepo SAPBP = new BPRepo();
            string result = SAPBP.DELBp(value);

            if (result == "OK")
            {
                return "Removed Successfully";
            }
            else
            {
                return "Error in removing BP " + result;
            }
        }


      
    }
}
