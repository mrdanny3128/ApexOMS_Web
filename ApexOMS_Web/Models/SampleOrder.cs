using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApexOMS_Web.Models
{
    [Table("tbl_sample_order")]
    public class SampleOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sl { get; set; }

        public string? sample_order_no { get; set; }
        public string? cust_name { get; set; }
        public string? apex_sales_part { get; set; }
        public string? last { get; set; }
        public double? pair { get; set; }
        public string? size { get; set; }
        public string? sample_status { get; set; }
        public string? approval { get; set; }
        public DateTime? request_date { get; set; }
        public DateTime? delivery_date { get; set; }
        public DateTime? dispatch_date { get; set; }
        public string? season { get; set; }
        public int? season_year { get; set; }
        public double? productionQty { get; set; }
        public string? user_name { get; set; }
        public DateTime? entry_date { get; set; }
    }
}