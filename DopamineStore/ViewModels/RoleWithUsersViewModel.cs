using System.Collections.Generic;

namespace DopamineStore.ViewModels
{
    public class RoleWithUsersViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<string> UserNames { get; set; }
    }
}