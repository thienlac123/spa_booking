using DoAnSPA.Models;

namespace DoAnSPA.Repositories
{
    public interface IPhanHoiRepository
    {
        IEnumerable<PhanHoi> GetAll();
        PhanHoi GetById(int id);
        void Add(PhanHoi phanHoi);
        void Update(PhanHoi phanHoi);
        void Delete(int id);
    }

}
