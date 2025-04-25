using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CareerConnect.Models.DTOs
{
	public class ParsedCV
	{
		[JsonPropertyName("skills")]
		public List<string> Skills { get; set; }

		[JsonPropertyName("education")]
		public List<string> Education { get; set; }

		[JsonPropertyName("experience")]
		public List<string> Experience { get; set; }
	}
}
