using System.Linq.Expressions;

namespace HospitalSystem.Core;

public interface IRoomRepository
{
    public IQueryable<Room> GetAll();

    public void Insert(Room room);

    public bool Delete(string location);

    public Room Get(string location);  // NOTE: Expects existing

    public void Replace(Room room, Expression<Func<Room, bool>> filter);

    public void Upsert(Room room, Expression<Func<Room, bool>> filter);
}