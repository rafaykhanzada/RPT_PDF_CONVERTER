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
                bool encrypt = rptParams[2].ToLower() == "true";
                //Fetch the File Name.
                string fileName = reportPath + rptParams[1] + ".rpt";

                var rd = new ReportDocument();

                rd.Load(fileName);


                rd.SetDatabaseLogon(config.user, config.password, config.host, config.db);

                for (int index = 4; index < rptParams.Count; index++)
                {
                    rd.SetParameterValue("@" + rptParams.Keys[index], rptParams[index]);
                }

                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Path.Combine(PDF_Path, rptParams[1] + ".pdf"));

                // Encrypt PDF
                if (encrypt)
                {
                    string InputFile = Path.Combine(PDF_Path, rptParams[1] + ".pdf");
                    string OutputFile = Path.Combine(PDF_Path, rptParams[1] + "_Encyrpt.pdf");

                    using (Stream input = new FileStream(InputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (Stream output = new FileStream(OutputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            PdfReader reader = new PdfReader(input);
                            PdfEncryptor.Encrypt(reader, output, true, rptParams[3], rptParams[3], PdfWriter.ALLOW_PRINTING);
                        }
                    }
                }

                //Send OK Response to Client.
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                if(encrypt)
                    response.Content = new StreamContent(new FileStream(Path.Combine(PDF_Path, rptParams[1] +  "_Encyrpt.pdf"), FileMode.Open, FileAccess.Read));
                else
                  response.Content = new StreamContent(new FileStream(Path.Combine(PDF_Path, rptParams[1] +  ".pdf"), FileMode.Open, FileAccess.Read));
                //response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline");
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                if(encrypt)
                    response.Content.Headers.ContentDisposition.FileName = rptParams[1] + "_Encyrpt.pdf";
                else    
                    response.Content.Headers.ContentDisposition.FileName = rptParams[1] + ".pdf";
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
