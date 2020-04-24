using SQLite;

namespace Polynavi.Common.Models
{
    public abstract class Entity
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Db_Id { get; set; }
    }
}
