using HospitalSystem.Core;
using HospitalSystem.Core.Rooms;
using HospitalSystem.Core.Equipment;

namespace HospitalSystem.Tests;

public static class RoomGenerator
{
    public static void GenerateRoomsAndEquipment(Hospital hospital)
    {
        for (int i = 0; i < 10; i++)
        {
            if (i % 3 == 0)
            {   
                StockRooms(hospital, i);
            } 
            else if (i % 3 == 1)
            {
                OperationRooms(hospital, i);
            } 
            else
            {
                CheckupRooms(hospital, i);
            }
        }
    }

    private static void StockRooms(Hospital hospital, int i)
    {
        var newRoom = new Room("90" + i, "NA" + i, RoomType.STOCK);
        hospital.RoomService.Insert(newRoom);
        for (int j = 0; j < 4; j++)
        {
            var newEquipmentBatch = new EquipmentBatch(newRoom.Location, "scalpel", 3, EquipmentType.OPERATION);
            hospital.EquipmentService.Add(newEquipmentBatch);
        }
    }

    private static void OperationRooms(Hospital hospital, int i)
    {
        var newRoom = new Room("10" + i, "NA" + i, RoomType.OPERATION);
        hospital.RoomService.Insert(newRoom);
    }

    private static void CheckupRooms(Hospital hospital, int i)
    {
        var newRoom = new Room("55" + i, "NA" + i, RoomType.CHECKUP);
        hospital.RoomService.Insert(newRoom);
        var newEquipmentBatch = new EquipmentBatch(newRoom.Location, "syringe", 10, EquipmentType.CHECKUP);
        hospital.EquipmentService.Add(newEquipmentBatch);
        var newEquipmentBatch2 = new EquipmentBatch(newRoom.Location, "bandage", 10, EquipmentType.CHECKUP);
        hospital.EquipmentService.Add(newEquipmentBatch2);
    }
}
