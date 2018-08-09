using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxManagerMent.Model
{
    [Table("TaxDetail")]
    class TaxDetail
    {
        [Column("Id")]
        [PrimaryKey]
        [AutoIncrement()]
        public int Id { get; set; }

        [Column("Type")]
        [NotNull]
        public string Type { get; set; }

        [Column("Price")]
        [NotNull]
        public decimal Price { get; set; }

        [Column("Time")]
        [NotNull]
        public string Time { get; set; }

        [Column("Comments")]
        public string Comments { get; set; }
    }
}
