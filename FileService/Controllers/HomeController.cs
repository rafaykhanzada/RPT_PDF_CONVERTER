using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace FileService.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            var rptParams = HttpContext.Request.QueryString;
            var jsonText = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Reports/" + rptParams[0] + "/config.json"));
            var config = JsonConvert.DeserializeObject<Item>(jsonText);
            string reportPath = HttpContext.Server.MapPath("~/Reports/" + rptParams[0] + "/");
            string path = HttpContext.Server.MapPath("~/uploads/reports");
            string PDF_Path = HttpContext.Server.MapPath("~/uploads/PDF");
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
                ViewBag.Report = rd;
                //if (rptParams.GetKey(3) == "IsPrint" && rptParams[3] == "true")
                //    PrintPdf(rd, config.printer);
            }
            return View();
        }
        public ActionResult Report(string Url)
        {
            ViewBag.Title = "Home Page";
            var rptParams = HttpContext.Request.QueryString;
            var jsonText = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Reports/" + rptParams[0] + "/config.json"));
            var config = JsonConvert.DeserializeObject<Item>(jsonText);
            string reportPath = HttpContext.Server.MapPath("~/Reports/" + rptParams[0] + "/");
            string path = HttpContext.Server.MapPath("~/uploads/reports");
            string PDF_Path = HttpContext.Server.MapPath("~/uploads/PDF");
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
                ViewBag.Report = rd;
                //if (rptParams.GetKey(3) == "IsPrint" && rptParams[3] == "true")
                //    PrintPdf(rd, config.printer);
            }
            return View();
        }
    }
}
