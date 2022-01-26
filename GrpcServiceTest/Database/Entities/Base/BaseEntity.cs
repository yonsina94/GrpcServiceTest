using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcServiceTest.Database.Entities.Base
{
    public interface IBaseEntity
    {
        [Key]
        Guid Id { get; set; }

        [Column("createdAt")]
        DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        DateTime UpdatedAt { get; set; }
    }
    public abstract class BaseEntity : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
