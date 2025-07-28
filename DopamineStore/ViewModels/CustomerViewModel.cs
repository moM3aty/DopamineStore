using System.Collections.Generic;

namespace DopamineStore.ViewModels
{
    public class CustomerViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}