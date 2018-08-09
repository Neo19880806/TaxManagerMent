using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxManagerMent.Model
{
    [Table("TaxSetting")]
    class TaxSetting
    {
        //对应数据库唯一id
        [Column("Type")]
        [PrimaryKey]
        public string TYPE { get; set; }

    }
}
