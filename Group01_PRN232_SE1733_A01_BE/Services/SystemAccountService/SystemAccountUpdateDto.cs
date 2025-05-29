using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SystemAccountService
{
    public class SystemAccountUpdateDto
    {
        public short AccountId { get; set; }  // cần để biết update đối tượng nào
        public string AccountName { get; set; } = null!;
        public string AccountEmail { get; set; } = null!;
        public int? AccountRole { get; set; }
        public string? AccountPassword { get; set; }  // password có thể không thay đổi nên cho nullable
    }
}
