using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApexOMS_Web.Models
{
    [Table("DashboardData")]
    public class DashboardData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int shop_order_number { get; set; }

        public DateTime? cd_date { get; set; }
        public DateTime? pd_date { get; set; }

        [Required]
        public string cust_name { get; set; }

        [Required]
        public string sample_order_no { get; set; }

        [Required]
        public string approval { get; set; }

        public int? shipment_quantity { get; set; }
        public DateTime? shipment_date { get; set; }
        public string? knife_status { get; set; }
        public string? tech_sheet_status { get; set; }
        public string? last { get; set; }
    }
}