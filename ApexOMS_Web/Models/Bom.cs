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
        public string? ELEMENT { get; set; }
        public string? MATERIAL { get; set; }
        public string? MATERIALDESC { get; set; }
        public string? UOM { get; set; } // Unit of Measure
        public decimal? QUANTITY { get; set; }
        public decimal? RATE { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string? user_id { get; set; }
    }
}