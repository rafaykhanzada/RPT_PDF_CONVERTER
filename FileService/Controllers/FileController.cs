using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
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
        [Route("report")]

        public HttpResponseMessage UploadFiles()
        {
            var rptParams = HttpContext.Current.Request.QueryString;
            var jsonText = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Reports/" + rptParams[0] +"/config.json"));
            var config = JsonConvert.DeserializeObject<Item>(jsonText);
            string reportPath = HttpContext.Current.Server.MapPath("~/Reports/" + rptParams[0] + "/");
            string path = HttpContext.Current.Server.MapPath("~/uploads/reports");
            string PDF_Path = HttpContext.Current.Server.MapPath("~/uploads/PDF");
            string output = rptParams[2];
            //Fetch the File Name.
            string fileName = reportPath + rptParams[1] + ".rpt";
            //pdf, xlsx, docx
            string media_type = "";
            using (ReportDocument rd = new ReportDocument())
            {
                rd.Load(fileName);
                rd.SetDatabaseLogon(config.user, config.password, config.host, config.db);

                for (int index = 3; index < rptParams.Count; index++)
                {
                    rd.SetParameterValue("@" + rptParams.Keys[index], rptParams[index]);
                }

                MemoryStream ms = new MemoryStream();
                var exportType = output == "docx" ? CrystalDecisions.Shared.ExportFormatType.WordForWindows : output == "xlsx" ? CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook : CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
                media_type = output == "docx" ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" : output == "xlsx" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";

                rd.ExportToDisk(exportType, Path.Combine(PDF_Path, rptParams[1] + "." + output));
                
                //if (rptParams.GetKey(3) == "IsPrint" && rptParams[3] == "true")
                //    PrintPdf(rd, config.printer);
            }
            #region Encrypt
            //string InputFile = Path.Combine(PDF_Path, rptParams[1] +"."+ output);
            //string OutputFile = Path.Combine(PDF_Path, rptParams[1] +"_Encyrpt.pdf");

            //using (Stream input = new FileStream(InputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            //    using (Stream output = new FileStream(OutputFile, FileMode.Create, FileAccess.Write, FileShare.None))
            //    {
            //        PdfReader reader = new PdfReader(input);
            //        PdfEncryptor.Encrypt(reader, output, true, "1234", "1234", PdfWriter.ALLOW_PRINTING);
            //    }
            //}
            #endregion

            //Send OK Response to Client.
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(Path.Combine(PDF_Path, rptParams[1] + "."+ output), FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue(output == "pdf" ? "inline":"attachment");
            response.Content.Headers.ContentDisposition.FileName = rptParams[1]+ "."+ output;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(media_type);
            return response;
        }
        //Print PDF
        public void PrintPdf(ReportDocument reportDocument,string printerName)
        {
            // load the .rpt file
            // reportDocument.Load(@"C:\path\to\report.rpt");

            // set the printer name
            reportDocument.PrintOptions.PrinterName = printerName;

            // set the duplex mode
            reportDocument.PrintOptions.PrinterDuplex = PrinterDuplex.Simplex;

            // set the paper orientation
            reportDocument.PrintOptions.PaperOrientation = PaperOrientation.Portrait;

            // print the report to the printer
            reportDocument.PrintToPrinter(1, true, 0, 0);
        }

    }

    public class Item
    {
        public string user { get; set; }
        public string password { get; set; }
        public string host { get; set; }
        public string db { get; set; }
        public string printer { get; set; }
    }

}
