using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Texts
{
    public static string WelcomeMessage = "Welcome to the Text Adventure Game!";
    public static string MovePrompt = "Choose your action:\n1. Move North\n2. Move South\n3. Move East";
    public static string MonsterAppears = "A wild monster appears with {0} HP!";
    public static string AttackPrompt = "Choose your action:\n1. Attack\n2. Defend\n3. Run";
    public static string RunAway = "You run away from the battle!";
    public static string CantRunAway = "You can't run away right now. Choose another action.";
    public static string InvalidInput = "Invalid input. Please try again.";
    public static string CounterattackMessage = "The monster attacks and deals {0} damage!";
    public static string DefeatedByMonster = "You were defeated by the monster. Game over.";
    public static string DefeatedMonster = "You defeated the monster and gained experience!";
}

class Item
{
    public string Name { get; set; }

    public Item(string name)
    {
        Name = name;
    }
}

class Sword : Item
{
    public int Damage { get; set; }

    public Sword(string name, int damage) : base(name)
    {
        Damage = damage;
    }
}

class Shield : Item
{
    public int Defense { get; set; }

    public Shield(string name, int defense) : base(name)
    {
        Defense = defense;
    }
}

class Monster
{
    public int HP { get; set; }
    public int Damage { get; set; }

    public Monster(int hp, int damage)
    {
        HP = hp;
        Damage = damage;
    }

    public virtual int Attack(Player player)
    {
        int damageDealt = player.TakeDamage(Damage);
        return damageDealt;
    }
}

class Goblin : Monster
{
    public Goblin() : base(50, 10)
    {
    }

    public override int Attack(Player player)
    {
        int damageDealt = base.Attack(player);
        return damageDealt;
    }
}

class Dragon : Monster
{
    public Dragon() : base(200, 30)
    {
    }

    public override int Attack(Player player)
    {
        int damageDealt = base.Attack(player);
        return damageDealt;
    }
}

class Player
{
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public List<string> Inventory { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExperienceToNextLevel { get; set; }
    public Sword EquippedSword { get; set; }
    public Shield EquippedShield { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public Player()
    {
        HP = 100;
        MaxHP = 100;
        Inventory = new List<string>();
        Level = 1;
        Experience = 0;
        ExperienceToNextLevel = 100;
        X = 0;
        Y = 0;
    }

    public int TakeDamage(int damage)
    {
        int actualDamage = Math.Max(0, damage);
        HP -= actualDamage;
        return actualDamage;
    }

    public bool IsAlive()
    {
        return HP > 0;
    }

    public void LevelUp()
    {
        Level++;
        Experience -= ExperienceToNextLevel;
        ExperienceToNextLevel = CalculateExperienceToNextLevel();
        MaxHP += 10;

        Console.WriteLine($"Congratulations! You reached level {Level}!");
    }

    private int CalculateExperienceToNextLevel()
    {
        return (int)(ExperienceToNextLevel * 1.5);
    }

    public void AddToInventory(string item)
    {
        Inventory.Add(item);
        Console.WriteLine($"Added {item} to your inventory!");
    }

    public async Task RegenerateHP()
    {
        while (true)
        {
            await Task.Delay(5000); // Asynchronous delay every 5 seconds

            if (HP < MaxHP)
            {
                HP = Math.Min(MaxHP, HP + (int)(MaxHP * 0.05));
                Console.WriteLine($"Your HP regenerated to {HP}!");

                if (HP < 100 && Inventory.Contains("HP Potion"))
                {
                    UsePotion();
                }
            }
        }
    }

    private void UsePotion()
    {
        if (Inventory.Contains("HP Potion"))
        {
            Inventory.Remove("HP Potion");
            HP = Math.Min(MaxHP, HP + 25);
            Console.WriteLine($"You used an HP Potion and healed 25 HP. Your HP is now {HP}!");
        }
        else
        {
            Console.WriteLine("You don't have an HP Potion in your inventory.");
        }
    }

    public int Attack(Monster monster)
    {
        int playerDamageDealt = EquippedSword != null ? EquippedSword.Damage : 5; // Default damage without a sword

        // Add a level-dependent bonus to the player's damage
        int levelBonus = Level * 5;

        playerDamageDealt += levelBonus;

        monster.HP -= playerDamageDealt;

        Console.WriteLine($"You attack and deal {playerDamageDealt} damage to the monster!");

        if (monster.HP <= 0)
        {
            Console.WriteLine($"You defeated the monster and gained experience!");
            Experience += 10; // Add a fixed amount of experience points for defeating a monster

            if (Experience >= ExperienceToNextLevel)
            {
                LevelUp();
            }
        }

        return playerDamageDealt;
    }

    public void Defend()
    {
        // Implement player's defense logic if needed
    }
}

class GameManager
{
    public static Random random = new Random();
}

class TextAdventureGame
{
    static Player player = new Player();
    static bool isGameOver = false;

    static async Task Main()
    {
        Console.WriteLine(Texts.WelcomeMessage);

        // Start the HP regeneration loop in the background
        Task regenerationTask = player.RegenerateHP();

        while (!isGameOver) // Use the game over flag as the loop condition
        {
            Console.WriteLine($"You are at position ({player.X}, {player.Y}).");

            Console.WriteLine($"Your HP: {player.HP}");
            Console.WriteLine($"Level: {player.Level}");
            Console.WriteLine($"Experience: {player.Experience}/{player.ExperienceToNextLevel}");
            Console.WriteLine($"Inventory: {string.Join(", ", player.Inventory)}");

            Console.WriteLine(Texts.MovePrompt);

            char userInput = Console.ReadKey().KeyChar;
            Console.WriteLine(); // Move to the next line after reading the key.

            HandleInput(userInput);
        }
    }

    static void HandleInput(char input)
    {
        switch (input)
        {
            case '1':
                Move(0, 1); // Move North
                break;
            case '2':
                Move(0, -1); // Move South
                break;
            case '3':
                Move(1, 0); // Move East
                break;
            default:
                Console.WriteLine(Texts.InvalidInput);
                break;
        }

        if (GameManager.random.Next(1, 6) == 1)
        {
            Console.WriteLine(Texts.MonsterAppears);

            Monster monster = CreateRandomMonster();
            FightMonster(monster);
        }

        if (GameManager.random.Next(1, 11) == 1)
        {
            Console.WriteLine("You encounter a friendly NPC!");

            if (GameManager.random.Next(1, 3) == 1)
            {
                GetSword();
            }
            else
            {
                GetShield();
            }
        }

        // 1 in 10 chance to spawn an HP potion
        if (GameManager.random.Next(1, 11) == 1)
        {
            Console.WriteLine("You found an HP potion!");
            player.AddToInventory("HP Potion");
        }
    }

    static void Move(int deltaX, int deltaY)
    {
        player.X += deltaX;
        player.Y += deltaY;

        Console.WriteLine($"You moved to ({player.X}, {player.Y}).");
    }

    static void FightMonster(Monster monster)
    {
        Console.WriteLine(string.Format(Texts.MonsterAppears, monster.HP));

        while (player.IsAlive() && monster.HP > 0)
        {
            Console.WriteLine(Texts.AttackPrompt);

            char userInput = Console.ReadKey().KeyChar;
            Console.WriteLine(); // Move to the next line after reading the key.

            switch (userInput)
            {
                case '1':
                    int playerDamageDealt = player.Attack(monster);
                    break;
                case '2':
                    player.Defend(); // Implement defense logic
                    break;
                case '3':
                    if (player.HP < 20)
                    {
                        Console.WriteLine(Texts.RunAway);
                        return; // Player did not defeat the monster, but they can escape
                    }
                    else
                    {
                        Console.WriteLine(Texts.CantRunAway);
                        break;
                    }
                default:
                    Console.WriteLine(Texts.InvalidInput);
                    break;
            }

            if (monster.HP > 0)
            {
                int monsterDamageDealt = monster.Attack(player);
                Console.WriteLine($"The monster attacks and deals {monsterDamageDealt} damage to you.");
            }

            // Check player's HP after each round
            if (!player.IsAlive())
            {
                Console.WriteLine(Texts.DefeatedByMonster);
                Console.WriteLine("Game Over! Your HP reached 0.");
                isGameOver = true; // Set the game over flag
                return; // Exit the fight loop
            }
        }

        if (!player.IsAlive())
        {
            Console.WriteLine(Texts.DefeatedByMonster);
            Console.WriteLine("Game Over! Your HP reached 0.");
            isGameOver = true; // Set the game over flag
        }
        else
        {
            Console.WriteLine(Texts.DefeatedMonster);
        }
    }

    static Monster CreateRandomMonster()
    {
        if (GameManager.random.Next(1, 3) == 1)
        {
            return new Goblin();
        }
        else
        {
            return new Dragon();
        }
    }

    static void GetSword()
    {
        int swordDamage = GameManager.random.Next(5, 16);
        Console.WriteLine($"The NPC gives you a sword! It deals {swordDamage} extra damage in battles.");

        Sword sword = new Sword("Sword", swordDamage);
        player.EquippedSword = sword;
    }

    static void GetShield()
    {
        int shieldDefense = GameManager.random.Next(5, 16);
        Console.WriteLine($"The NPC gives you a shield! It absorbs {shieldDefense} damage in battles.");

        Shield shield = new Shield("Shield", shieldDefense);
        player.EquippedShield = shield;
    }
}
