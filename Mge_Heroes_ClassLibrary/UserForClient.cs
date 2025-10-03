using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mge_Heroes_ClassLibrary
{
    public class UserForClient
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string EmailHash { get; set; }
        public string Password { get; set; }
    }
}
