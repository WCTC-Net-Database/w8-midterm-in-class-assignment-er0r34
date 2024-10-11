using W8_assignment_template.Data;
using W8_assignment_template.Helpers;
using W8_assignment_template.Interfaces;
using W8_assignment_template.Models.Characters;

namespace W8_assignment_template.Services;

public class GameEngine
{
    private readonly IContext _context;
    private readonly MapManager _mapManager;
    private readonly MenuManager _menuManager;
    private readonly OutputManager _outputManager;

    private readonly IRoomFactory _roomFactory;
    private ICharacter _player;
    private ICharacter _goblin;
    private ICharacter _ghost;
    private ICharacter _orc;
    private ICharacter _dragon;

    private List<IRoom> _rooms;

    public GameEngine(IContext context, IRoomFactory roomFactory, MenuManager menuManager, MapManager mapManager, OutputManager outputManager)
    {
        _roomFactory = roomFactory;
        _menuManager = menuManager;
        _mapManager = mapManager;
        _outputManager = outputManager;
        _context = context;

        _player = null!;
        _goblin = null!;
        _ghost = null!;
        _orc = null!;
        _dragon = null!;
        _rooms = new List<IRoom>();
    }

    public void Run()
    {
        if (_menuManager.ShowMainMenu())
        {
            SetupGame();
        }
    }

    private void AttackCharacter()
    {
        var monsters = _player.CurrentRoom.Characters.Where(c => c != _player).ToList();

        if (monsters.Count == 0)
        {
            _outputManager.WriteLine("No characters to attack.", ConsoleColor.Red);
            return;
        }

        if (monsters.Count == 1)
        {
            _player.Attack(monsters.First());
            return;
        }

        _outputManager.WriteLine("Which monster would you like to attack first?", ConsoleColor.Cyan);
        for (int i = 0; i < monsters.Count; i++)
        {
            _outputManager.WriteLine($"{i + 1}. {monsters[i].Name} ({monsters[i].Type})", ConsoleColor.Yellow);
        }

        _outputManager.Display();
        var input = Console.ReadLine();

        if (!int.TryParse(input, out int selectedIndex) || selectedIndex < 1 || selectedIndex > monsters.Count)
        {
            _outputManager.WriteLine("Invalid selection. No attack performed.", ConsoleColor.Red);
            return;
        }

        var firstTarget = monsters[selectedIndex - 1];
        monsters.RemoveAt(selectedIndex - 1);

        _player.Attack(firstTarget);

        while (firstTarget.HP > 0)
        {
            _outputManager.WriteLine($"Press enter to attack {firstTarget.Name} ({firstTarget.Type}) again.", ConsoleColor.Cyan);
            _outputManager.Display();
            Console.ReadLine();
            _player.Attack(firstTarget);
        }

        _player.CurrentRoom.RemoveCharacter(firstTarget);

        if (monsters.Count > 0)
        {
            _outputManager.WriteLine("Do you want to attack another monster? (yes/no)", ConsoleColor.Cyan);
            _outputManager.Display();
            var attackAnother = Console.ReadLine();

            if (attackAnother?.Trim().ToLower() == "yes")
            {
                _outputManager.WriteLine("Which monster would you like to attack next?", ConsoleColor.Cyan);
                for (int i = 0; i < monsters.Count; i++)
                {
                    _outputManager.WriteLine($"{i + 1}. {monsters[i].Name} ({monsters[i].Type})", ConsoleColor.Yellow);
                }

                _outputManager.Display();
                input = Console.ReadLine();

                if (!int.TryParse(input, out selectedIndex) || selectedIndex < 1 || selectedIndex > monsters.Count)
                {
                    _outputManager.WriteLine("Invalid selection. No attack performed.", ConsoleColor.Red);
                    return;
                }

                var secondTarget = monsters[selectedIndex - 1];
                monsters.RemoveAt(selectedIndex - 1);

                _player.Attack(secondTarget);

                while (secondTarget.HP > 0)
                {
                    _outputManager.WriteLine($"Press enter to attack {secondTarget.Name} ({secondTarget.Type}) again.", ConsoleColor.Cyan);
                    _outputManager.Display();
                    Console.ReadLine();
                    _player.Attack(secondTarget);
                }

                _player.CurrentRoom.RemoveCharacter(secondTarget);

                if (monsters.Count > 0)
                {
                    _outputManager.WriteLine("Do you want to attack another monster? (yes/no)", ConsoleColor.Cyan);
                    _outputManager.Display();
                    attackAnother = Console.ReadLine();

                    if (attackAnother?.Trim().ToLower() == "yes")
                    {
                        _outputManager.WriteLine("Which monster would you like to attack next?", ConsoleColor.Cyan);
                        for (int i = 0; i < monsters.Count; i++)
                        {
                            _outputManager.WriteLine($"{i + 1}. {monsters[i].Name} ({monsters[i].Type})", ConsoleColor.Yellow);
                        }

                        _outputManager.Display();
                        input = Console.ReadLine();

                        if (!int.TryParse(input, out selectedIndex) || selectedIndex < 1 || selectedIndex > monsters.Count)
                        {
                            _outputManager.WriteLine("Invalid selection. No attack performed.", ConsoleColor.Red);
                            return;
                        }

                        var thirdTarget = monsters[selectedIndex - 1];

                        _player.Attack(thirdTarget);

                        while (thirdTarget.HP > 0)
                        {
                            _outputManager.WriteLine($"Press enter to attack {thirdTarget.Name} ({thirdTarget.Type}) again.", ConsoleColor.Cyan);
                            _outputManager.Display();
                            Console.ReadLine();
                            _player.Attack(thirdTarget);
                        }

                        _player.CurrentRoom.RemoveCharacter(thirdTarget);
                    }
                    else
                    {
                        _outputManager.WriteLine("No further attacks performed.", ConsoleColor.Red);
                    }
                }
            }
            else
            {
                _outputManager.WriteLine("No further attacks performed.", ConsoleColor.Red);
            }
        }
    }




    private void GameLoop()
    {
        while (true)
        {
            _mapManager.DisplayMap();
            _outputManager.WriteLine("Choose an action:", ConsoleColor.Cyan);
            _outputManager.WriteLine("1. Move North");
            _outputManager.WriteLine("2. Move South");
            _outputManager.WriteLine("3. Move East");
            _outputManager.WriteLine("4. Move West");

            if (_player.CurrentRoom.Characters.Any(c => c != _player))
            {
                _outputManager.WriteLine("5. Attack");
            }

            _outputManager.WriteLine("6. Exit Game");

            _outputManager.Display();

            var input = Console.ReadLine();

            string? direction = null;
            switch (input)
            {
                case "1":
                    direction = "north";
                    break;
                case "2":
                    direction = "south";
                    break;
                case "3":
                    direction = "east";
                    break;
                case "4":
                    direction = "west";
                    break;
                case "5":
                    if (_player.CurrentRoom.Characters.Any(c => c != _player))
                    {
                        AttackCharacter();
                    }
                    else
                    {
                        _outputManager.WriteLine("No characters to attack.", ConsoleColor.Red);
                    }
                    break;
                case "6":
                    _outputManager.WriteLine("Exiting game...", ConsoleColor.Red);
                    _outputManager.Display();
                    Environment.Exit(0);
                    break;
                default:
                    _outputManager.WriteLine("Invalid selection. Please choose a valid option.", ConsoleColor.Red);
                    break;
            }

            if (!string.IsNullOrEmpty(direction))
            {
                _outputManager.Clear();
                _player.Move(direction);
                _mapManager.UpdateCurrentRoom(_player.CurrentRoom);
            }
        }
    }

    private void LoadMonsters()
    {
        _goblin = _context.Characters.OfType<Goblin>().FirstOrDefault() ?? new Goblin();
        _ghost = _context.Characters.OfType<Ghost>().FirstOrDefault() ?? new Ghost();
        _dragon = _context.Characters.OfType<Dragon>().FirstOrDefault() ?? new Dragon();
        _orc = _context.Characters.OfType<Orc>().FirstOrDefault() ?? new Orc();

        var random = new Random();
        var randomRoom = _rooms[random.Next(_rooms.Count)];
        if (_goblin != null) randomRoom.AddCharacter(_goblin);

        var randomRoom2 = _rooms[random.Next(_rooms.Count)];
        if (_dragon != null) randomRoom2.AddCharacter(_dragon);
        if (_orc != null) randomRoom2.AddCharacter(_orc);
    }

    private void SetupGame()
    {
        var startingRoom = SetupRooms();
        _mapManager.UpdateCurrentRoom(startingRoom);

        _player = _context.Characters.OfType<Player>().FirstOrDefault() ?? new Player();
        _player.Move(startingRoom);
        _outputManager.WriteLine($"{_player.Name} has entered the game.", ConsoleColor.Green);

        LoadMonsters();

        Thread.Sleep(1000);
        GameLoop();
    }

    private IRoom SetupRooms()
    {
        var entrance = _roomFactory.CreateRoom("entrance", _outputManager);
        var treasureRoom = _roomFactory.CreateRoom("treasure", _outputManager);
        var dungeonRoom = _roomFactory.CreateRoom("dungeon", _outputManager);
        var library = _roomFactory.CreateRoom("library", _outputManager);
        var armory = _roomFactory.CreateRoom("armory", _outputManager);
        var garden = _roomFactory.CreateRoom("garden", _outputManager);
        var bedroom = _roomFactory.CreateRoom("bedroom", _outputManager);
        var hall = _roomFactory.CreateRoom("hall", _outputManager);

        entrance.North = hall;
        entrance.West = library;
        entrance.East = garden;

        hall.South = entrance;
        hall.West = bedroom;
        hall.North = treasureRoom;

        bedroom.East = hall;

        treasureRoom.South = hall;
        treasureRoom.West = dungeonRoom;

        dungeonRoom.East = treasureRoom;

        library.East = entrance;
        library.South = armory;

        armory.North = library;

        garden.West = entrance;

        _rooms = new List<IRoom> { entrance, treasureRoom, dungeonRoom, library, armory, garden, hall, bedroom };

        return entrance;
    }
}
