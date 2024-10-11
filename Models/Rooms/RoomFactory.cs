using W8_assignment_template.Helpers;
using W8_assignment_template.Interfaces;

namespace W8_assignment_template.Models.Rooms;

public class RoomFactory : IRoomFactory
{
    // DONE You can add more rooms here
    public IRoom CreateRoom(string roomType, OutputManager outputManager)
    {
        switch (roomType.ToLower())
        {
            case "treasure":
                return new Room("Treasure Room", "Filled with gold and treasures.", outputManager);
            case "dungeon":
                return new Room("Dungeon", "Dark and damp, with echoes of past prisoners.", outputManager);
            case "entrance":
                return new Room("Entrance Hall", "A large room with high ceilings.", outputManager);
            case "library":
                return new Room("Library", "Shelves filled with ancient books.", outputManager);
            case "armory":
                return new Room("Armory", "Weapons and armor line the walls.", outputManager);
            case "garden":
                return new Room("Garden", "A peaceful garden with blooming flowers.", outputManager);
            case "bedroom":
                return new Room("Master Bedroom", "A glorious bedroom with a large 4 poster bed.", outputManager);
            case "hall":
                return new Room("Great hall", "A large and great hall, meant for diplomacy and dining", outputManager);
            default:
                return new Room("Generic Room", "A simple room.", outputManager);
        }
    }
}
