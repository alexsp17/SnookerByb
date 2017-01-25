using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
    public enum LocationServicesStatusEnum
    {
        Unknown = 0,
        Ok = 1,
        NotAuthorized = 2
    }

    public interface ILocationService
    {
        void RequestLocationAsync(EventHandler done, bool askPermissionIfNecessary);
		Location GetLastKnownLocationQuickly();

        Location Location { get; }
        DateTime? TimeUpdated { get; }
        string FailureInfo { get; }
        LocationServicesStatusEnum LocationServicesStatus { get; }
    }
}
