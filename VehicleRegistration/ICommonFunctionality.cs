using Models.ViewModels.VehicleRegistration.Setup;
using System.Collections.Generic;
using System.Linq;

namespace VehicleRegistration
{
    public interface ICommonFunctionality
    {
        long GetBusinessTableLevelAccess(object typeName);
    }

    public class CommonFunctionality : ICommonFunctionality
    {
        readonly List<VwBusinessTableAccessLevel> businessTableAccessLevels;

        public CommonFunctionality(List<VwBusinessTableAccessLevel> businessTableAccessLevels)
        {
            this.businessTableAccessLevels = businessTableAccessLevels;
        }

        public long GetBusinessTableLevelAccess(object obj)
        {
            if (obj is null)
                return -1;

            var objTypeName = obj.GetType().Name;

            var businessTableAccessLevel = this.businessTableAccessLevels.FirstOrDefault(x => objTypeName.Contains(x.BusinessTableName));

            if (businessTableAccessLevel is null)
                return -1;

            return businessTableAccessLevel.AccessLevel;
        }
    }
}
