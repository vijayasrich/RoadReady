using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface ICarExtraRepository
    {
        IEnumerable<CarExtra> GetAllCarExtras();
        CarExtra GetCarExtraById(int extraId);
        void AddCarExtra(CarExtra carExtra);
        void UpdateCarExtra(CarExtra carExtra);
        void DeleteCarExtra(int extraId);
    }
}
