using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class TextAdventureGame
{
    static int playerX = 0;
    static int playerY = 0;
    static int playerHP = 100;
    static int maxPlayerHP = 100;
    static int playerDamage = 10;
    static int playerDefense = 0;
    static int potionHealAmount = 25;
    static List<string> inventory = new List<string>(); // Player's inventory
    static Random random = new Random();

    static async Task Main()
    {
        Console.WriteLine("Welcome to the Text Adventure Game!");

        // Start the HP regeneration loop in the background
        Task regenerationTask = RegenerateHP();

        while (true)
        {
            Console.WriteLine($"You are at position ({playerX}, {playerY}).");

            Console.WriteLine($"Your HP: {playerHP}");
            Console.WriteLine($"Inventory: {string.Join(", ", inventory)}");

            Console.WriteLine("Choose your action:");
            Console.WriteLine("1. Move North");
            Console.WriteLine("2. Move South");
            Console.WriteLine("3. Move East");

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
                Console.WriteLine("Invalid input. Please try again.");
                break;
        }

        if (random.Next(1, 6) == 1)
        {
            Console.WriteLine("Oh no! You've encountered a monster!");

            if (FightMonster())
            {
                Console.WriteLine("You defeated the monster and gained experience!");
            }
            else
            {
                Console.WriteLine("You were defeated by the monster. Game over.");
                Environment.Exit(0);
            }
        }

        if (random.Next(1, 11) == 1)
        {
            Console.WriteLine("You encounter a friendly NPC!");

            if (random.Next(1, 3) == 1)
            {
                GetSword();
            }
            else
            {
                GetShield();
            }
        }

        // 1 in 10 chance to spawn an HP potion
        if (random.Next(1, 11) == 1)
        {
            Console.WriteLine("You found an HP potion!");
            AddToInventory("HP Potion");
        }
    }

    static void Move(int deltaX, int deltaY)
    {
        playerX += deltaX;
        playerY += deltaY;

        Console.WriteLine($"You moved to ({playerX}, {playerY}).");
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

    static bool FightMonster()
    {
        int monsterHP = random.Next(50, 101);

        Console.WriteLine($"A wild monster appears with {monsterHP} HP!");

        while (playerHP > 0 && monsterHP > 0)
        {
            Console.WriteLine($"Your HP: {playerHP} | Monster HP: {monsterHP}");
            Console.WriteLine("Choose your action:");
            Console.WriteLine("1. Attack");
            Console.WriteLine("2. Defend");
            Console.WriteLine("3. Run");

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
                        Console.WriteLine("You run away from the battle!");
                        return false; // Player didn't defeat the monster
                    }
                    else
                    {
                        Console.WriteLine("You can't run away right now. Choose another action.");
                        break;
                    }
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }

            if (monsterHP > 0)
            {
                if (userInput != '3') // Monster only attacks if the player didn't run
                {
                    int monsterDamage = random.Next(5, 16);
                    playerHP -= Math.Max(0, monsterDamage - playerDefense);
                    Console.WriteLine($"The monster attacks and deals {Math.Max(0, monsterDamage - playerDefense)} damage!");
                }
            }

            if (playerHP <= 0)
            {
                Console.WriteLine("You were defeated by the monster.");
                return false;
            }
        }

        Console.WriteLine("You defeated the monster!");
        return true;
    }

    static void Attack(ref int monsterHP)
    {
        int damageDealt = random.Next(playerDamage - 5, playerDamage + 6);
        int monsterDamage = Math.Min(monsterHP, random.Next(1, 26)); // Ensure monsterDamage doesn't exceed monster's remaining HP

        monsterHP -= damageDealt;
        playerHP -= Math.Max(0, monsterDamage - playerDefense);

        Console.WriteLine($"You attack and deal {damageDealt} damage to the monster!");
        Console.WriteLine($"The monster counterattacks and deals {Math.Max(0, monsterDamage - playerDefense)} damage to you!");
    }

    static void Defend()
    {
        int monsterDamage = random.Next(1, 26);
        int parryChance = random.Next(1, 11);

        if (parryChance <= 1)
        {
            Console.WriteLine("You successfully parry the monster's attack!");
        }
        else
        {
            playerHP -= Math.Max(0, monsterDamage - playerDefense / 2);
            Console.WriteLine($"You defend and take only {Math.Max(0, monsterDamage - playerDefense / 2)} damage from the monster's attack.");
        }
    }

    static void GetSword()
    {
        int swordDamage = random.Next(5, 16);
        Console.WriteLine($"The NPC gives you a sword! It deals {swordDamage} extra damage in battles.");

        playerDamage += swordDamage;
    }

    static void GetShield()
    {
        int shieldDefense = random.Next(5, 16);
        Console.WriteLine($"The NPC gives you a shield! It absorbs {shieldDefense} damage in battles.");

        playerDefense += shieldDefense;
    }

    static void AddToInventory(string item)
    {
        inventory.Add(item);
        Console.WriteLine($"Added {item} to your inventory!");
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
