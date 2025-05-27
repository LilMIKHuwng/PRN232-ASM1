using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.TagServices
{
    public class TagCreateDto
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string Note { get; set; }
    }
}
