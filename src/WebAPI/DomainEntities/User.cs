using System;
using System.Collections.Generic;

namespace GenesisTest.WebAPI.DomainEntities
{
    public class User
    {
        /// <summary>
        /// User Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Hashed Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Last updated date
        /// </summary>
        public DateTime? LastUpdatedOn { get; set; }

        /// <summary>
        /// Last Login date (in case of creation, it will be the same as creation date)
        /// </summary>
        public DateTime? LastLoginOn { get; set; }

        /// <summary>
        /// JWT Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Telephone Numbers
        /// </summary>
        public IEnumerable<string> TelephoneNumbers { get; set; }

        public User()
        {
            TelephoneNumbers = new List<string>() { };
            Token = string.Empty;
        }
    }
}
