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

class GameManager
{
    public static Random random = new Random();
}

class TextAdventureGame
{
    static int playerX = 0;
    static int playerY = 0;
    static int playerHP = 100;
    static int maxPlayerHP = 100;
    static int potionHealAmount = 25;
    static List<string> inventory = new List<string>(); // Player's inventory
    static int playerLevel = 1;
    static int experiencePoints = 0;
    static int experienceToNextLevel = 100;

    static async Task Main()
    {
        Console.WriteLine(Texts.WelcomeMessage);

        // Start the HP regeneration loop in the background
        Task regenerationTask = RegenerateHP();

        while (true)
        {
            Console.WriteLine($"You are at position ({playerX}, {playerY}).");

            Console.WriteLine($"Your HP: {playerHP}");
            Console.WriteLine($"Level: {playerLevel}");
            Console.WriteLine($"Experience: {experiencePoints}/{experienceToNextLevel}");
            Console.WriteLine($"Inventory: {string.Join(", ", inventory)}");

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

            if (FightMonster())
            {
                Console.WriteLine("You defeated the monster and gained experience!");
            }
            else
            {
                Console.WriteLine(Texts.DefeatedByMonster);
                Environment.Exit(0);
            }
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
            AddToInventory("HP Potion");
        }
    }

    static bool FightMonster()
    {
        int monsterHP = GameManager.random.Next(50, 101);
        int experiencePointsForDefeatingMonster = GameManager.random.Next(10, 21);

        Console.WriteLine(string.Format(Texts.MonsterAppears, monsterHP));

        while (playerHP > 0 && monsterHP > 0)
        {
            Console.WriteLine(string.Format(Texts.AttackPrompt));

            char userInput = Console.ReadKey().KeyChar;
            Console.WriteLine(); // Move to the next line after reading the key.

            switch (userInput)
            {
                case '1':
                    Attack(ref monsterHP);
                    break;
                case '2':
                    Defend();
                    break;
                case '3':
                    if (playerHP < 20)
                    {
                        Console.WriteLine(Texts.RunAway);
                        return false; // Player didn't defeat the monster
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

            if (monsterHP <= 0)
            {
                Console.WriteLine($"You defeated the monster and gained {experiencePointsForDefeatingMonster} experience points!");
                experiencePoints += experiencePointsForDefeatingMonster;

                if (experiencePoints >= experienceToNextLevel)
                {
                    LevelUp();
                }

                return true;
            }

            if (monsterHP > 0 && userInput != '3' && ShouldCounterAttack()) // Check for counterattack
            {
                int monsterDamage = GameManager.random.Next(5, 16);
                playerHP -= Math.Max(0, monsterDamage);
                Console.WriteLine(string.Format(Texts.CounterattackMessage, Math.Max(0, monsterDamage)));
            }

            if (playerHP <= 0)
            {
                Console.WriteLine(Texts.DefeatedByMonster);
                return false;
            }
        }

        Console.WriteLine(Texts.DefeatedMonster);
        return true;
    }

    static bool ShouldCounterAttack()
    {
        // Adjust the counterattack chance as needed (e.g., 30% chance)
        return GameManager.random.Next(1, 11) <= 3;
    }


    static void LevelUp()
    {
        playerLevel++;
        experiencePoints -= experienceToNextLevel;
        experienceToNextLevel = CalculateExperienceToNextLevel();
        maxPlayerHP += 10;

        Console.WriteLine($"Congratulations! You reached level {playerLevel}!");
    }

    static int CalculateExperienceToNextLevel()
    {
        return (int)(experienceToNextLevel * 1.5);
    }

    static void Attack(ref int monsterHP)
    {
        int damageDealt = GameManager.random.Next(5, 16);
        int monsterDamage = Math.Min(monsterHP, GameManager.random.Next(1, 26));

        monsterHP -= damageDealt;
        playerHP -= Math.Max(0, monsterDamage);

        Console.WriteLine($"You attack and deal {damageDealt} damage to the monster!");
        Console.WriteLine($"The monster counterattacks and deals {Math.Max(0, monsterDamage)} damage!");
    }

    static void Defend()
    {
        int monsterDamage = GameManager.random.Next(1, 26);
        int parryChance = GameManager.random.Next(1, 11);

        if (parryChance <= 1)
        {
            Console.WriteLine("You successfully parry the monster's attack!");
        }
        else
        {
            playerHP -= Math.Max(0, monsterDamage / 2);
            Console.WriteLine($"You defend and take only {Math.Max(0, monsterDamage / 2)} damage from the monster's attack.");
        }
    }

    static void GetSword()
    {
        int swordDamage = GameManager.random.Next(5, 16);
        Console.WriteLine($"The NPC gives you a sword! It deals {swordDamage} extra damage in battles.");

        // Modify the player's damage based on the sword received
    }

    static void GetShield()
    {
        int shieldDefense = GameManager.random.Next(5, 16);
        Console.WriteLine($"The NPC gives you a shield! It absorbs {shieldDefense} damage in battles.");

        // Modify the player's defense based on the shield received
    }

    static void AddToInventory(string item)
    {
        inventory.Add(item);
        Console.WriteLine($"Added {item} to your inventory!");
    }

    static async Task RegenerateHP()
    {
        while (true)
        {
            await Task.Delay(5000); // Asynchronous delay every 5 seconds

            if (playerHP < maxPlayerHP)
            {
                playerHP = Math.Min(maxPlayerHP, playerHP + (int)(maxPlayerHP * 0.05));
                Console.WriteLine($"Your HP regenerated to {playerHP}!");

                if (playerHP < 100 && inventory.Contains("HP Potion"))
                {
                    UsePotion();
                }
            }
        }
    }

    static void Move(int deltaX, int deltaY)
    {
        playerX += deltaX;
        playerY += deltaY;

        Console.WriteLine($"You moved to ({playerX}, {playerY}).");
    }

    static void UsePotion()
    {
        if (inventory.Contains("HP Potion"))
        {
            inventory.Remove("HP Potion");
            playerHP = Math.Min(maxPlayerHP, playerHP + potionHealAmount);
            Console.WriteLine($"You used an HP Potion and healed {potionHealAmount} HP. Your HP is now {playerHP}!");
        }
        else
        {
            Console.WriteLine("You don't have an HP Potion in your inventory.");
        }
    }
}
