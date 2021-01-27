using System;

namespace AspNetCore.Reporting.Common.Models {
    public class ExportResult {
        byte[] documentBytes;
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string ExportOperationId { get; set; }
        public string ContentDisposition { get; set; }
        public DateTime TimeStamp { get; private set; }
        public ExportResult() {
            TimeStamp = DateTime.UtcNow;
        }
        public void AssignBytes(byte[] data) {
            documentBytes = data;
        }
        public byte[] GetBytes() {
            return documentBytes;
        }
    }
}
