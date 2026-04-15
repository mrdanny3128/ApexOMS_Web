using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApexOMS_Web.Models
{
    [Table("tbl_article_last")]
    public class ArticleLast
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string apex_sales_part { get; set; }
        public string? old_apex_article { get; set; }
        public string Last { get; set; }
        public int status { get; set; } // Matches the INT in your SQL
        public string? user_id { get; set; }
        public string? last_part_code { get; set; }
        public DateTime? entry_date { get; set; }

        // ADD THESE BACK with [NotMapped] to stop the errors
        // This tells EF Core: "These exist in code, but don't look for them in SQL yet"
       
        public string? shoe_style { get; set; }
       
        public string? gender { get; set; }
        
        public string? season { get; set; }
        
        public int? season_year { get; set; }
    }
}