using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("FirstName")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("LastName")]
        public string LastName { get; set; }
        [Required]
        [DisplayName("Biography")]
        [StringLength(250)]
        public string Bio { get; set; }

        public virtual IList<Book> Books { get; set; }
    }
}
