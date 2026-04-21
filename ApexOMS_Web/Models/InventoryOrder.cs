using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApexOMS_Web.Models
{
    [Table("tbl_invent_order")]
    public class InventoryOrder
    {
        [Key]
        public int sl { get; set; }
        public int? order_id { get; set; }
        public string? cust_name { get; set; }
        public string? style_name { get; set; }
        public string? part_color { get; set; }
        public int? totalqty { get; set; }
        public string? order_status { get; set; }
        public DateTime? order_receive_date { get; set; }
    }
}