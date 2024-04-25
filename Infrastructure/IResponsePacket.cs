using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
	public interface IResponsePacket
	{
		public bool Status { get; set; }
		public string? Message { get; set; }
	}
}
