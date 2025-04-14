namespace Caliber_Models.Models.SunPharma
{
    public class InboundModel
    {
        // Gets or sets the memo number.
        public string MemoNumber { get; set; } = string.Empty;

        //Gets or sets the memo date.

        public DateTime MemoDate { get; set; }

        //Gets or sets the item description.

        public string ItemDesc { get; set; } = string.Empty;

        //Gets or sets the item code.

        public string ItemCode { get; set; } = string.Empty;

        //Gets or sets the manufacture name.

        public string ManufactureName { get; set; } = string.Empty;

        // Gets or sets the manufacture code.

        public string ManufactureCode { get; set; } = string.Empty;

        //Gets or sets the supplier name.

        public string SupplierName { get; set; } = string.Empty;

        //Gets or sets the supplier code.

        public string SupplierCode { get; set; } = string.Empty;

        //Gets or sets the batch number.

        public string BatchNo { get; set; } = string.Empty;

        //Gets or sets the batch quantity.

        public decimal BatchQuantity { get; set; } = 0;

        // Gets or sets the batch unit of measure.

        public string BatchUOM { get; set; } = string.Empty;

        //Gets or sets the batch manufacture date.

        public DateTime BatchManufactureDate { get; set; }

        //Gets or sets the expiry date.

        public DateTime ExpiryDate { get; set; }

        //Gets or sets the retest date.

        public DateTime RetestDate { get; set; }

        //Gets or sets the ExpiryRetestIndicator.

        public string EX_RT_Indicator { get; set; } = string.Empty;

        //Gets or sets the sample number.

        public string SampleNo { get; set; } = string.Empty;

        //Gets or sets the sample quantity.

        public decimal SampleQty { get; set; } = 0;

        //Gets or sets the sample quantity unit of measure.

        public string SampleQtyUOM { get; set; } = string.Empty;

        //Gets or sets the sampling by.

        public string SamplingBy { get; set; } = string.Empty;

        //Gets or sets the inspection type.

        public string InspectionType { get; set; } = string.Empty;

        //Gets or sets the plant code.

        public string PlantCode { get; set; } = string.Empty;

        //Gets or sets the created by.

        public string CreatedBy { get; set; } = string.Empty;

        //Gets or sets the created on date.

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        //Gets or sets the total number of containers.

        public int TotalNoOfContainers { get; set; } = 0;

        //Gets or sets the vendor batch number.

        public string VendorBatchNo { get; set; } = string.Empty;


        //Gets or sets the GRN number.

        public string GRNNo { get; set; } = string.Empty;

        //Gets or sets the GRN date.

        public DateTime GRNDate { get; set; }

        //Gets or sets the GRN unit of measure.

        public string GRNUOM { get; set; } = string.Empty;

        //Gets or sets the GRN quantity.

        public decimal GRNQty { get; set; }

        //Gets or sets the downloaded on date.

        public DateTime DownloadedOn { get; set; } = DateTime.UtcNow;

    }

    public class SunparmaInboundResponseModel
    {
        //Gets or sets the return status.

        public int ReturnStatus { get; set; }

        //Gets or sets the key field.

        public string KeyField { get; set; } = string.Empty;
    }
}
