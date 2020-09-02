using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCTBank.Entity
{
    [Table("HGSTransaction")]
    public class HGSTransaction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PlateNumber { get; set; }
        public string WebOrMobil { get; set; }
        public decimal LoadBalance { get; set; }
        public DateTime LoadDate { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual HGS Hgs { get; set; }
        public List<HGS> HGSes { get; set; }

    }
}
