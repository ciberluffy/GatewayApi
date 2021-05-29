using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusalaSoft.GatewayApi.Model
{
    public class Gateway
    {
        public Gateway() {
            USN = Guid.NewGuid().ToString();
        }

        [Key]
        public string USN { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", 
            ErrorMessage = "The Address must be \"d.d.d.d\" with d in range 0-255")]
        public string Address { get; set; }

        [MaxLength(10)]
        public virtual ICollection<Device> Devices { get; set; }
    }
}
