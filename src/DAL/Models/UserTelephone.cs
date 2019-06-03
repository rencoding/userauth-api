using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenesisTest.DAL.Models
{
    /// <summary>
    /// User Telephones
    /// </summary>
    public class UserTelephone
    {
        /// <summary>
        /// Id as Auto generated
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// User Id as Foreign Key
        /// </summary>
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Telephone number
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string TelephoneNumber { get; set; }

    }
}
