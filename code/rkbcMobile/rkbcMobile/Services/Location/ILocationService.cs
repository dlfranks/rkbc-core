using System.Threading.Tasks;

namespace rkbcMobile.Services.Location
{
    public interface ILocationService
    {
        Task UpdateUserLocation(rkbcMobile.Models.Location.Location newLocReq, string token);
    }
}
