using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace Domain.Models
{
	public class StaticFile
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Extension { get; set; }
		public string Path { get; set; }
		public Guid? HolderId { get; set; }
	}
}
