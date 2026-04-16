using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApexOMS_Web.Models
{
    [Table("tbl_user")]
    public class User
    {
        [Key]
        public int sl { get; set; }

        public string? user_id { get; set; }
        public string? user_name { get; set; }
        public string? user_pass { get; set; }

        // This was missing! Add this line:
        public int? status { get; set; }

        public string? Role { get; set; }
        public int? active { get; set; }
    }
}