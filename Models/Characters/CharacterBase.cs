using Microsoft.Extensions.DependencyInjection;
using W8_assignment_template.Helpers;
using W8_assignment_template.Interfaces;

namespace W8_assignment_template.Models.Characters;

public abstract class CharacterBase : ICharacter
{
    public int HP { get; set; }
    public int Level { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }

    protected OutputManager OutputManager;
    private IRoom _currentRoom;

    // Explicit interface implementation for CurrentRoom to prevent access from outside the class
    // This is to ensure that the CurrentRoom property is only set by the Move method
    public IRoom CurrentRoom => _currentRoom;

    // Parameterless constructor for deserialization
    protected CharacterBase()
    {
        Name = string.Empty; 
        Type = string.Empty; 
        OutputManager = new OutputManager(); 
        _currentRoom = null!; 
    }

    protected CharacterBase(string name, string type, int level, int hp, IRoom startingRoom)
    {
        Name = name;
        Type = type;
        Level = level;
        HP = hp;
        OutputManager = new OutputManager(); 
        _currentRoom = startingRoom;
        _currentRoom.Enter();
    }

    public void Attack(ICharacter target)
    {
        if (target == null)
        {
            OutputManager.WriteLine("No target to attack.", ConsoleColor.Yellow);
            return;
        }

        // Attack logic
        OutputManager.WriteLine($"{Name} attacks {target.Name} with a chilling touch.", ConsoleColor.Blue);

        // Example damage logic with random variability
        Random random = new Random();
        int variability = random.Next(-3, 4); // Generates a number between -3 and 3
        int damage = 10 + variability; // Base damage is 10, with added variability
        target.HP -= damage;
        OutputManager.WriteLine($"{target.Name} takes {damage} damage and has {target.HP} HP left.", ConsoleColor.Red);

        // Check if the target's HP is 0 or less and remove them from the room
        if (target.HP <= 0)
        {
            // Check if the target is lootable and has treasure
            if (this is Player player && target is ILootable targetWithTreasure && !string.IsNullOrEmpty(targetWithTreasure.Treasure))
            {
                OutputManager.WriteLine($"{Name} takes {targetWithTreasure.Treasure} from {target.Name}", ConsoleColor.Blue);
                player.Gold += 10; // Assuming each treasure is worth 10 gold
                targetWithTreasure.Treasure = string.Empty; // Treasure is taken
                OutputManager.WriteLine($"{Name} now has {player.Gold} gold", ConsoleColor.Blue);
            }
            else if (this is Player playerWithGold && target is Player targetWithGold && targetWithGold.Gold > 0)
            {
                // we can't attack other players, but if we could we could take their gold here
                OutputManager.WriteLine($"{Name} takes gold from {target.Name}", ConsoleColor.Blue);
                playerWithGold.Gold += targetWithGold.Gold;
                targetWithGold.Gold = 0; // Gold is taken
            }

            if (target.CurrentRoom != null)
            {
                var room = target.CurrentRoom;
                room.RemoveCharacter(target);

                OutputManager.WriteLine($"{target.Name} has been defeated and removed from the room.", ConsoleColor.Green);
            }
        }
    }



    public void Move(IRoom? nextRoom)
    {
        if (nextRoom != null)
        {
            _currentRoom = nextRoom;
            OutputManager.WriteLine($"{Name} has entered {_currentRoom.Name}. {_currentRoom.Description}", ConsoleColor.Green);
            foreach (var character in _currentRoom.Characters)
            {
                OutputManager.WriteLine($"{character.Name} is here.", ConsoleColor.Red);
            }
        }
        else
        {
            OutputManager.WriteLine($"{Name} cannot move to the specified room.", ConsoleColor.Yellow);
        }
    }

    public void Move(string? direction)
    {
        var nextRoom = direction?.ToLower() switch
        {
            "north" => _currentRoom.North,
            "south" => _currentRoom.South,
            "east" => _currentRoom.East,
            "west" => _currentRoom.West,
            _ => null
        };

        Move(nextRoom);
    }

    // Method to set the OutputManager called by the CharacterBaseConverter so that the character can write to the console
    // This method is called after deserialization
    public void SetOutputManager(OutputManager outputManager)
    {
        OutputManager = outputManager;
    }

    public abstract void UniqueBehavior();
}
