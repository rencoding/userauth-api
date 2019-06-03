using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenesisTest.DAL.Models
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        /// <summary>
        /// Id 
        /// </summary>
        [Key]
        [Column(TypeName = "UNIQUEIDENTIFIER")]
        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [EmailAddress]
        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(200)")] 
        public string Password { get; set; }

        /// <summary>
        /// Created date time
        /// </summary>
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Last updated date time
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedOn { get; set; }

        /// <summary>
        /// Last login date time
        /// </summary>
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime LastLoginOn { get; set; }

        /// <summary>
        /// JWT Token
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(4000)")]
        public string Token { get; set; }
        
        /// <summary>
        /// Telephone numbers 
        /// </summary>
        public virtual List<UserTelephone> UserTelephones { get; set; }
    }
}
