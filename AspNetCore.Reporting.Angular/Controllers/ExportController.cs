using AspNetCore.Reporting.Common.Models;
using AspNetCore.Reporting.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Reporting.Common.Controllers {
    public class ExportController : ControllerBase {
        [HttpGet]
        public ActionResult GetExportResult([FromServices] IExportResultProvider exportResultProvider, string token, string fileName) {
            ExportResult exportResult;
            if(!exportResultProvider.TryGetExportResult(token, out exportResult)) {
                return new NotFoundResult();
            }
            var fileResult = File(exportResult.GetBytes(), exportResult.ContentType);
            if(exportResult.ContentDisposition != System.Net.Mime.DispositionTypeNames.Inline) {
                fileResult.FileDownloadName = exportResult.FileName;
            }

            return fileResult;
        }
    }
}
