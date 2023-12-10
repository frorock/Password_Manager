using System;
using System.Collections.Generic;
using System.Text;

namespace Password.Models
{
    public class UserData
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Birth { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string IV { get; set; }

    }

}

