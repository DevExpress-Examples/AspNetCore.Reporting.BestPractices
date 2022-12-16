using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Reporting.Common.Data {
    public interface IReportEntityProvider {
        DbSet<ReportEntity> Reports { get; set; }
    }
}
