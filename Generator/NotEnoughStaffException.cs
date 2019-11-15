using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Generator
{

	[Serializable]
	public class NotEnoughStaffException : Exception
	{
		// TODO: Make this exception a better representation of the exception that is called
		public NotEnoughStaffException() { }
		public NotEnoughStaffException(string message) : base(message) { }
		public NotEnoughStaffException(string message, Exception inner) : base(message, inner) { }
		protected NotEnoughStaffException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
