using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcServiceTest.Database.Entities
{
    public class PersonEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Column("name", TypeName = "varchar"), Required(ErrorMessage = "The name of the person must be provide !"), MaxLength(80)]
        public string Name { get; set; }

        [Column("lastName", TypeName = "varchar"), Required(ErrorMessage = "The last name o the person must be provide !"), MaxLength(80)]
        public string LastName { get; set; }

        [Column("email",TypeName = "varchar"), Required(ErrorMessage = "The email of the person must be provide"), MaxLength(120)]
        public string Email { get; set; }

        [Column("age"), Required(ErrorMessage = "The age of the person must be provide")]
        public int Age { get; set; }
    }
}
