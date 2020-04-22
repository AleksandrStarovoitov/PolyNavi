using SQLite;
using System;

namespace Polynavi.Common.Models
{
    public abstract class Entity
    {
        [PrimaryKey]
        public Guid Db_Id { get; set; }
    }
}
