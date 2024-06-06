namespace AspNetCore.Reporting.MVC.Reports {
    partial class MyEnrollmentsReport {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.label1 = new DevExpress.XtraReports.UI.XRLabel();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.label9 = new DevExpress.XtraReports.UI.XRLabel();
            this.label8 = new DevExpress.XtraReports.UI.XRLabel();
            this.label7 = new DevExpress.XtraReports.UI.XRLabel();
            this.label6 = new DevExpress.XtraReports.UI.XRLabel();
            this.label5 = new DevExpress.XtraReports.UI.XRLabel();
            this.label4 = new DevExpress.XtraReports.UI.XRLabel();
            this.label3 = new DevExpress.XtraReports.UI.XRLabel();
            this.label2 = new DevExpress.XtraReports.UI.XRLabel();
            this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            this.table1 = new DevExpress.XtraReports.UI.XRTable();
            this.tableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.tableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.odsEnrollments = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.odsStudentDetails = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.rpShowTimestamp = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.table1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.odsEnrollments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.odsStudentDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.label1});
            this.TopMargin.HeightF = 135.4167F;
            this.TopMargin.Name = "TopMargin";
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo1});
            this.BottomMargin.Name = "BottomMargin";
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.label9,
            this.label8,
            this.label7,
            this.label6,
            this.label5,
            this.label4,
            this.label3,
            this.label2});
            this.Detail.Expanded = false;
            this.Detail.HeightF = 114.5833F;
            this.Detail.Name = "Detail";
            // 
            // DetailReport
            // 
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1});
            this.DetailReport.DataSource = this.odsEnrollments;
            this.DetailReport.Level = 0;
            this.DetailReport.Name = "DetailReport";
            // 
            // label1
            // 
            this.label1.Font = new DevExpress.Drawing.DXFont("Segoe UI Semibold", 20.25F, DevExpress.Drawing.DXFontStyle.Bold, DevExpress.Drawing.DXGraphicsUnit.Point, new DevExpress.Drawing.DXFontAdditionalProperty[] {
            new DevExpress.Drawing.DXFontAdditionalProperty("GdiCharSet", ((byte)(0)))});
            this.label1.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 76.37501F);
            this.label1.Multiline = true;
            this.label1.Name = "label1";
            this.label1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label1.SizeF = new System.Drawing.SizeF(630F, 49.04166F);
            this.label1.StylePriority.UseFont = false;
            this.label1.Text = "Enrollments Report";
            // 
            // pageInfo1
            // 
            this.pageInfo1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Visible", "?rpShowTimestamp")});
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.pageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfo1.SizeF = new System.Drawing.SizeF(650F, 23F);
            // 
            // label9
            // 
            this.label9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EnrollmentDate]")});
            this.label9.LocationFloat = new DevExpress.Utils.PointFloat(261.4583F, 79.00003F);
            this.label9.Multiline = true;
            this.label9.Name = "label9";
            this.label9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label9.SizeF = new System.Drawing.SizeF(336.4583F, 23F);
            this.label9.Text = "label9";
            this.label9.TextFormatString = "{0:d}";
            // 
            // label8
            // 
            this.label8.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 79.00003F);
            this.label8.Multiline = true;
            this.label8.Name = "label8";
            this.label8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label8.SizeF = new System.Drawing.SizeF(238.5416F, 23F);
            this.label8.Text = "Enrollment Date:";
            // 
            // label7
            // 
            this.label7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[LastName]")});
            this.label7.LocationFloat = new DevExpress.Utils.PointFloat(261.4583F, 56.00001F);
            this.label7.Multiline = true;
            this.label7.Name = "label7";
            this.label7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label7.SizeF = new System.Drawing.SizeF(336.4583F, 23F);
            this.label7.Text = "label7";
            // 
            // label6
            // 
            this.label6.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 56.00001F);
            this.label6.Multiline = true;
            this.label6.Name = "label6";
            this.label6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label6.SizeF = new System.Drawing.SizeF(238.5416F, 23F);
            this.label6.Text = "Last Name:";
            // 
            // label5
            // 
            this.label5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FirstMidName]")});
            this.label5.LocationFloat = new DevExpress.Utils.PointFloat(261.4583F, 32.99999F);
            this.label5.Multiline = true;
            this.label5.Name = "label5";
            this.label5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label5.SizeF = new System.Drawing.SizeF(336.4584F, 23F);
            this.label5.Text = "label5";
            // 
            // label4
            // 
            this.label4.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 32.99999F);
            this.label4.Multiline = true;
            this.label4.Name = "label4";
            this.label4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label4.SizeF = new System.Drawing.SizeF(238.5416F, 23F);
            this.label4.Text = "Name:";
            // 
            // label3
            // 
            this.label3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[StudentID]")});
            this.label3.LocationFloat = new DevExpress.Utils.PointFloat(261.4583F, 10.00001F);
            this.label3.Multiline = true;
            this.label3.Name = "label3";
            this.label3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label3.SizeF = new System.Drawing.SizeF(336.4583F, 23F);
            this.label3.StylePriority.UseTextAlignment = false;
            this.label3.Text = "label3";
            this.label3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // label2
            // 
            this.label2.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 10.00001F);
            this.label2.Multiline = true;
            this.label2.Name = "label2";
            this.label2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label2.SizeF = new System.Drawing.SizeF(238.5416F, 23F);
            this.label2.Text = "ID:";
            // 
            // Detail1
            // 
            this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.table1});
            this.Detail1.HeightF = 35.00001F;
            this.Detail1.Name = "Detail1";
            // 
            // table1
            // 
            this.table1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
            this.table1.Name = "table1";
            this.table1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.table1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableRow1});
            this.table1.SizeF = new System.Drawing.SizeF(650F, 25F);
            // 
            // tableRow1
            // 
            this.tableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tableCell1,
            this.tableCell2,
            this.tableCell3});
            this.tableRow1.Name = "tableRow1";
            this.tableRow1.Weight = 11.5D;
            // 
            // tableCell1
            // 
            this.tableCell1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EnrollmentID]")});
            this.tableCell1.Multiline = true;
            this.tableCell1.Name = "tableCell1";
            this.tableCell1.Text = "tableCell1";
            this.tableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.tableCell1.Weight = 0.13324174818101819D;
            // 
            // tableCell2
            // 
            this.tableCell2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CourseTitle]")});
            this.tableCell2.Multiline = true;
            this.tableCell2.Name = "tableCell2";
            this.tableCell2.Text = "tableCell2";
            this.tableCell2.Weight = 0.4381868232475532D;
            // 
            // tableCell3
            // 
            this.tableCell3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Grade]")});
            this.tableCell3.Multiline = true;
            this.tableCell3.Name = "tableCell3";
            this.tableCell3.Text = "tableCell3";
            this.tableCell3.Weight = 0.2857142857142857D;
            // 
            // odsEnrollments
            // 
            this.odsEnrollments.DataMember = "GetEnrollments";
            this.odsEnrollments.DataSource = typeof(global::AspNetCore.Reporting.Common.Services.MyEnrollmentsReportRepository);
            this.odsEnrollments.Name = "odsEnrollments";
            // 
            // odsStudentDetails
            // 
            this.odsStudentDetails.DataMember = "GetStudentDetails";
            this.odsStudentDetails.DataSource = typeof(global::AspNetCore.Reporting.Common.Services.MyEnrollmentsReportRepository);
            this.odsStudentDetails.Name = "odsStudentDetails";
            // 
            // rpShowTimestamp
            // 
            this.rpShowTimestamp.Description = "Show Timestamp";
            this.rpShowTimestamp.Name = "rpShowTimestamp";
            this.rpShowTimestamp.Type = typeof(bool);
            this.rpShowTimestamp.ValueInfo = "False";
            // 
            // MyEnrollmentsReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail,
            this.DetailReport});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.odsStudentDetails,
            this.odsEnrollments});
            this.DataSource = this.odsStudentDetails;
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.Margins = new DevExpress.Drawing.DXMargins(100F, 100F, 135.4167F, 100F);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.rpShowTimestamp});
            this.RequestParameters = false;
            this.Version = "23.2";
            ((System.ComponentModel.ISupportInitialize)(this.table1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.odsEnrollments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.odsStudentDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.XRLabel label1;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRLabel label9;
        private DevExpress.XtraReports.UI.XRLabel label8;
        private DevExpress.XtraReports.UI.XRLabel label7;
        private DevExpress.XtraReports.UI.XRLabel label6;
        private DevExpress.XtraReports.UI.XRLabel label5;
        private DevExpress.XtraReports.UI.XRLabel label4;
        private DevExpress.XtraReports.UI.XRLabel label3;
        private DevExpress.XtraReports.UI.XRLabel label2;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        private DevExpress.XtraReports.UI.DetailBand Detail1;
        private DevExpress.XtraReports.UI.XRTable table1;
        private DevExpress.XtraReports.UI.XRTableRow tableRow1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell2;
        private DevExpress.XtraReports.UI.XRTableCell tableCell3;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource odsEnrollments;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource odsStudentDetails;
        private DevExpress.XtraReports.Parameters.Parameter rpShowTimestamp;
    }
}
