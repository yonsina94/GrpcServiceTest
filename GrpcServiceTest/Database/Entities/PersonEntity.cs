using GrpcServiceTest.Database.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcServiceTest.Database.Entities
{
    [Table("persons")]
    public class PersonEntity: BaseEntity
    {

        [Column("name", TypeName = "varchar(80)"), Required(ErrorMessage = "The name of the person must be provide !"), MaxLength(80)]
        public string Name { get; set; }

        [Column("lastName", TypeName = "varchar(80)"), Required(ErrorMessage = "The last name o the person must be provide !"), MaxLength(80)]
        public string LastName { get; set; }

        [Column("email",TypeName = "varchar(120)"), Required(ErrorMessage = "The email of the person must be provide"), MaxLength(120)]
        public string Email { get; set; }

        [Column("age", TypeName ="int"), Required(ErrorMessage = "The age of the person must be provide")]
        public int Age { get; set; }
    }
}
