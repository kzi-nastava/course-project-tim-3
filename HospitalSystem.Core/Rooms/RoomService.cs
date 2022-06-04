namespace HospitalSystem.Core;

public class RoomService
{
    private IRoomRepository _roomRepo;

    public RoomService(RoomRepository roomRepo)
    {
        _roomRepo = roomRepo;
    }

    public IQueryable<Room> GetActive()
    {
        return _roomRepo.GetActive();
    }

    public void Delete(string location)
    {
        if (!_roomRepo.Delete(location))
        {
            throw new Exception("Nope, that key does not exist.");  // TODO: change except
        }
    }

    public void Insert(Room newRoom)
    {
        _roomRepo.Insert(newRoom);
    }

    public void UpsertInactive(Room newRoom)  // needs to be upsert, in case of later failure TODO: transaction make
    {
        newRoom.Active = false;
        _roomRepo.Upsert(newRoom, room => room.Location == newRoom.Location && 
            !room.Deleted && !room.Active);
    }

    public bool DoesExist(string location)
    {
        return _roomRepo.DoesExist(location);
    }

    public void Replace(Room replacing)
    {
        _roomRepo.Replace(replacing, room => room.Id == replacing.Id && !room.Deleted);
    }

    public void Activate(string location)
    {
        var room = _roomRepo.Get(location);
        room.Active = true;
        Replace(room);
    }

    public void Deactivate(string location)
    {
        // TODO: check if room still exists by this time... or stop delete if renovating
        var room = _roomRepo.Get(location);
        room.Active = false;
        Replace(room);
    }
}