using Models.ViewModels.DSAuth.Setup;
using Models.ViewModels.Identity;

namespace SharedLib.Interfaces
{
    public interface ICurrentDSUser
    {
        public VwDSUser VwDSUser { get; set; }
    }
}
