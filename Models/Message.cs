﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class Message
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public ulong MessageID { get; set; }
		[Key]
		[ForeignKey("Employee")]
		public uint EmployeeID { get; set; }
		[Required]
		[StringLength(2000)]
		public string Text { get; set; }
		public DateTime Time { get; set; } = DateTime.Now;
	}
}