using CrystalDecisions.CrystalReports.Engine;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace FileService.Controllers
{
    public class FileController : ApiController
    {

        // POST api/file
        [HttpPost]
        [Route("api/upload")]

        public HttpResponseMessage UploadFiles()
        {
            string path = HttpContext.Current.Server.MapPath("~/uploads/reports");
            string PDF_Path = HttpContext.Current.Server.MapPath("~/uploads/PDF");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            //Fetch the File.
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

            //Fetch the File Name.
            string fileName = "RPTFILE"+ Path.GetExtension(postedFile.FileName);
            if (Directory.Exists(Path.Combine(path + fileName)))
                Directory.Delete(Path.Combine(path + fileName), true);
            //Save the File.
            postedFile.SaveAs(Path.Combine(path, fileName));
            var rd = new ReportDocument();

            rd.Load(Path.Combine(path , fileName));
            for (int index = 0; index < HttpContext.Current.Request.Form.Count; index++)
            {
                rd.SetParameterValue("@"+HttpContext.Current.Request.Form.Keys[index], HttpContext.Current.Request.Form[index]);
            }
            //rd.SetParameterValue(0, 3);
            //rd.SetParameterValue(1, "99428");
            //rd.Load(@"C:\\Users\Abdul Rafay\\Documents\\Projects\\.NetCore\\APIs\\CrystalReportWebAPI\\CrystalReportWebAPI\\Reports\\CustomerLedgerReport.rpt");
            MemoryStream ms = new MemoryStream();
            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Path.Combine(PDF_Path , "CustomerLedger.pdf"));

            string WorkingFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string InputFile = Path.Combine(PDF_Path , "CustomerLedger.pdf");
            string OutputFile = Path.Combine(PDF_Path , "CustomerLedger_Encyrpt.pdf");

            using (Stream input = new FileStream(InputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (Stream output = new FileStream(OutputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfReader reader = new PdfReader(input);
                    PdfEncryptor.Encrypt(reader, output, true, "1234", "1234", PdfWriter.ALLOW_PRINTING);
                }
            }
            if (Directory.Exists(InputFile))
                Directory.Delete(InputFile, true);
            //Send OK Response to Client.
            //return Request.CreateResponse(HttpStatusCode.OK, fileName);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(Path.Combine(PDF_Path  , "CustomerLedger.pdf"), FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            if (Directory.Exists(InputFile))
                Directory.Delete(InputFile, true);
            if (Directory.Exists(OutputFile))
                Directory.Delete(OutputFile, true);
            return response;
        }
        public static void DeleteDirectory(string directoryName, bool checkDirectiryExist)
        {
            DirectoryInfo RootDir = new DirectoryInfo(@"C:\somedirectory\");
            foreach (DirectoryInfo dir in RootDir.GetDirectories())
                DeleteDirectory(dir.FullName, true);
            if (Directory.Exists(directoryName))
                Directory.Delete(directoryName, true);
            else if (checkDirectiryExist)
                throw new SystemException("Directory you want to delete is not exist");
        }

    }
}
