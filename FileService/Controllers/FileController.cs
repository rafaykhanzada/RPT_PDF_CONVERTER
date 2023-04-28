using CrystalDecisions.CrystalReports.Engine;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Lifetime;
using System.Web;
using System.Web.Http;

namespace FileService.Controllers
{
    public class FileController : ApiController
    {

        // POST api/file
        [HttpGet]
        [Route("api/upload")]

        public HttpResponseMessage UploadFiles()
        {
            try
            {
                //"?Folder=Dawlance&Report=CustomerLedgerReport&Encrypt={encrypt}&Password={dealer.DealerCode}&Requestid={RequestId}&dealercode={dealer.DealerCode}&LedgerCoID={comId.Id}"
                var rptParams = HttpContext.Current.Request.QueryString;
                var jsonText = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Reports/" + rptParams[0] + "/config.json"));
                var config = JsonConvert.DeserializeObject<Item>(jsonText);
                string reportPath = HttpContext.Current.Server.MapPath("~/Reports/" + rptParams[0] + "/");
                string PDF_Path = HttpContext.Current.Server.MapPath("~/uploads/PDF");
                //Fetch the File Name.
                string fileName = reportPath + rptParams[1] + ".rpt";

                var rd = new ReportDocument();

                rd.Load(fileName);


                rd.SetDatabaseLogon(config.user, config.password, config.host, config.db);

                for (int index = 2; index < rptParams.Count; index++)
                {
                    rd.SetParameterValue("@" + rptParams.Keys[index], rptParams[index]);
                }

                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Path.Combine(PDF_Path, rptParams[1] + "_" + rptParams[2] + ".pdf"));

                

                //Send OK Response to Client.
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(Path.Combine(PDF_Path, rptParams[1] + "_" + rptParams[2] + ".pdf"), FileMode.Open, FileAccess.Read));
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline");
                //response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = rptParams[1]+"_"+ rptParams[2] + ".pdf";
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);

            }
        }
    }

    public class Item
    {
        public string user { get; set; }
        public string password { get; set; }
        public string host { get; set; }
        public string db { get; set; }
    }

}
