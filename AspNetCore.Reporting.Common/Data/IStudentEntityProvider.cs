using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Reporting.Common.Data {
    public interface IStudentEntityProvider {
        DbSet<StudentIdentity> Students { get; set; }
    }
}
