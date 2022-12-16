using System.Collections.Generic;
using System.Web.Mvc;

namespace AspNetCore.Reporting.Common.Models {
    public class LoginScreenModel {
        public string UserId { get; set; }
        public IList<SelectListItem> Users { get; set; }
    }
}
