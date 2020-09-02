using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCTBank.Entity
{
    [Table("Credit")]
    public class Credit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal LoanAmount { get; set; }
        public int Age { get; set; }
        public string HasHouse { get; set; }
        public int UsedCredits { get; set; }
        public string HasPhone { get; set; }
        public string HasCredit { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
