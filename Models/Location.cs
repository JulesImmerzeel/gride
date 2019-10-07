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
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public uint LocationID { get; set; }
		public string Name { get; set; }
		public string Street { get; set; }
		public int StreetNumber { get; set; }
		public string Aditions { get; set; }
		public string Postalcode { get; set; }
		public string City { get; set; }
		public string Country { get; set; } = "NetherLands";
	}
}
