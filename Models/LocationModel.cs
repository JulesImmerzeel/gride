using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class LocationModel
	{
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
