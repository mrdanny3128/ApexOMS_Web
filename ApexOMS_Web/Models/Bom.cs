using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApexOMS_Web.Models
{
    [Table("tbl_BOM")]
    public class Bom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sl { get; set; }

        public string? BOM_NO { get; set; }
        public string? APEX_SALES_PART { get; set; }
        public string? LAST { get; set; }
        public string? REVISION { get; set; } // nchar(10) in your screenshot

        // Match the 'float' type in SQL
        public double? QUANTITY { get; set; }
        public double? basequantity { get; set; }

        // Match the 'money' type in SQL
        public decimal? RATE { get; set; }

        public string? ELEMENT { get; set; }
        public string? MATERIALDESC { get; set; }
        public string? UOM { get; set; }
        public int? ELEMENT_ID { get; set; }

        public DateTime? CREATEDATE { get; set; }
        public string? user_id { get; set; }
    }
}