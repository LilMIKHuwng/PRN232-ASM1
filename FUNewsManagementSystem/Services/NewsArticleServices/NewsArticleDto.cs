using Repositories.Models;
using Services.TagServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.NewsArticleServices
{
	public class NewsArticleDto
	{
		public string NewsArticleId { get; set; }
		public string NewsTitle { get; set; }
		public string Headline { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string NewsContent { get; set; }
		public string NewsSource { get; set; }
		public short? CategoryId { get; set; }
		public bool? NewsStatus { get; set; }
		public short? CreatedById { get; set; }
		public short? UpdatedById { get; set; }
		public DateTime? ModifiedDate { get; set; }

		public List<TagDto> Tags { get; set; }
	}
}
