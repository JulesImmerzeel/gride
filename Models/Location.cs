using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class Location
	{
		public int LocationID { get; set; }
        [Required]
		public string Name { get; set; }
        [StringLength(100)]
		public string Street { get; set; }
        [Display(Name="Street Number")]
		public int StreetNumber { get; set; }
		public string Additions { get; set; }
		public string Postalcode { get; set; }
		public string City { get; set; }
		public string Country { get; set; } = "NetherLands";
        public uint EmployeeModelID { get; set; }

    }
}
