namespace Caliber_Models.Models.BioPharma
{
    public class InboundModel
    {
        public int Insp_Receipt_ID { get; set; }

        public string InspectionLot { get; set; } = string.Empty;

        public DateTime Manufacture_Date { get; set; }

        public DateTime Expr_Date { get; set; }

        public string Customer_Name { get; set; } = string.Empty;

        public string Customer_No { get; set; } = string.Empty;

        public DateTime GRN_Date { get; set; }

        public string GRN_No { get; set; } = string.Empty;

        public decimal GRN_Qty { get; set; }

        public string Item_Desc { get; set; } = string.Empty;

        public string Item_code { get; set; } = string.Empty;

        public string Manufacture_Name { get; set; } = string.Empty;

        public string Manufacturer_No { get; set; } = string.Empty;

        public string Vendor_Name { get; set; } = string.Empty;

        public string Vendor_No { get; set; } = string.Empty;

        public string No_Of_Containers { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public string UOM { get; set; } = string.Empty;

        public string Vendor_Lot_No { get; set; } = string.Empty;

        public string Batch_No { get; set; } = string.Empty;

        public int Retest_Period { get; set; }

        public int Retest_Flag { get; set; }



    }

    public class BiopharmaInboundResponseModel
    {
        /// <summary>
        /// Gets or sets the return status.
        /// </summary>
        public int ReturnStatus { get; set; }

        /// <summary>
        /// Gets or sets the key field.
        /// </summary>
        public string KeyField { get; set; } = string.Empty;
    }
}
