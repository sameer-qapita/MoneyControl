using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyControl.Models
{
    public class Transcation
    {
        [Key]
        public int TranscationId { get; set; }

        //CategoryId
        [Range(1,int.MaxValue,ErrorMessage ="Select a Category")]
        public int categoryId { get; set; }
        public category? category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than zero")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public String? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.Now ;

        [NotMapped]
        public String? CategoryTitleWithIcon {
            get
            {
                return category==null?"":category.Icon+" "+category.Title;
            } 
        
        }

        [NotMapped]
        public String? FormattedAmount
        {
            get
            {
                return ((category == null || category.Type== "Expense")? " - ": " + " ) + Amount.ToString("C0");
            }

        }

    }
}
