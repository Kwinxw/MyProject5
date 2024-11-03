namespace GAMEOVER
{
    public class Player
    {
        public string Name { get; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; }
        public Weapon Weapon { get; private set; }
        public AidKit Aid { get; private set; }
        public int Score { get; set; }

        public Player(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Score = 0;
        }

        public void EquipWeapon(Weapon weapon)
        {
            Weapon = weapon;
            Console.WriteLine($"Вам был ниспослан {Weapon.Name} ({Weapon.Damage}).");
        }

        public void AddAid(AidKit aid)
        {
            Aid = aid;
            Console.WriteLine($"Вам была дана аптечка: {Aid.Name} ({Aid.HealAmount}hp).");
        }

        public void UseAid()
        {
            if (Aid != null)
            {
                int healAmount = Math.Min(Aid.HealAmount, MaxHealth - CurrentHealth);
                CurrentHealth += healAmount;
                Console.WriteLine($"{Name} использует аптечку {Aid.Name} и восстанавливает {healAmount} здоровья.");
                Console.WriteLine($"У вас теперь {CurrentHealth}hp.");
                Aid = null;
            }
            else
            {
                Console.WriteLine($"{Name}, у вас нет аптечек!");
            }
        }

        public void Attack(Enemy enemy)
        {
            if (Weapon == null || Weapon.Durability <= 0)
            {
                Console.WriteLine($"{Name}, у вас нет оружия или оно сломано! Вы не можете атаковать.");
                return;
            }

            int damage = Weapon.Use();
            enemy.CurrentHealth -= damage;
            Console.WriteLine($"{Name} ударил противника {enemy.Name}. У противника {enemy.CurrentHealth}hp, у вас {CurrentHealth}hp.");

            if (enemy.CurrentHealth <= 0)
            {
                Console.WriteLine($"{enemy.Name} повержен!");
                Score += 100;
            }
        }
    }


    public class Weapon
    {
        public string Name { get; }
        public int Damage { get; }
        public int Durability { get; private set; }

        public Weapon(string name, int damage, int durability)
        {
            Name = name;
            Damage = damage;
            Durability = durability;
        }

        public int Use()
        {
            if (Durability > 0)
            {
                Durability--;
                return Damage;
            }
            else
            {
                Console.WriteLine($"{Name} сломано!");
                return 0;
            }
        }
    }

    public class AidKit
    {
        public string Name { get; }
        public int HealAmount { get; }

        public AidKit(string name, int healAmount)
        {
            Name = name;
            HealAmount = healAmount;
        }
    }

    public class Enemy
    {
        public string Name { get; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; }
        public Weapon Weapon { get; }

        public Enemy(string name, int health, Weapon weapon)
        {
            Name = name;
            CurrentHealth = health;
            MaxHealth = health;
            Weapon = weapon;
        }

        public int Attack()
        {
            return Weapon.Damage;
        }
    }

    public class Game
    {
        private static Random random = new Random();

        private static string GenerateRandomEnemyName()
        {
            string[] enemyNames = { "Демон", "Гоблин", "Волк", "Скелет", "Нежить" };
            return enemyNames[random.Next(enemyNames.Length)];
        }

        private static Weapon GenerateRandomWeapon()
        {
            string[] weaponNames = { "Лук", "Экскалибур", "Копье", "Удачливая палка", "Нож" };
            return new Weapon(weaponNames[random.Next(weaponNames.Length)], random.Next(5, 21), random.Next(10, 20));
        }

        private static Enemy GenerateRandomEnemy()
        {
            string name = GenerateRandomEnemyName();
            int health = random.Next(30, 101);

            Weapon weapon = GenerateRandomWeapon();

            return new Enemy(name, health, weapon);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать, воин!");
            Console.Write("Назови себя: ");
            string playerName = Console.ReadLine();

            Player player = new Player(playerName, 100);

            Weapon playerWeapon = GenerateRandomWeapon();
            AidKit playerAid = new AidKit("Средняя аптечка", 30);

            player.EquipWeapon(playerWeapon);
            player.AddAid(playerAid);

            while (player.CurrentHealth > 0)
            {

                Enemy enemy = GenerateRandomEnemy();
                Console.WriteLine($"{player.Name} встречает врага {enemy.Name} ({enemy.CurrentHealth}hp), у врага на поясе сияет оружие {enemy.Weapon.Name} ({enemy.Weapon.Damage}).");

                while (enemy.CurrentHealth > 0 && player.CurrentHealth > 0)
                {
                    Console.WriteLine("Что вы будете делать?");
                    Console.WriteLine("1. Ударить");
                    Console.WriteLine("2. Пропустить ход");
                    Console.WriteLine("3. Использовать аптечку");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            player.Attack(enemy);
                            if (enemy.CurrentHealth > 0)
                            {
                                player.CurrentHealth -= enemy.Attack();
                                Console.WriteLine($"Противник {enemy.Name} ударил вас! У вас осталось {player.CurrentHealth}hp.");
                            }
                            break;

                        case "2":
                            Console.WriteLine($"{player.Name} пропускает ход.");
                            if (enemy.CurrentHealth > 0)
                            {
                                player.CurrentHealth -= enemy.Attack();
                                Console.WriteLine($"Противник {enemy.Name} ударил вас! У вас осталось {player.CurrentHealth}hp.");
                            }
                            break;

                        case "3":
                            player.UseAid();
                            break;

                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте снова.");
                            break;
                    }

                    if (player.CurrentHealth <= 0)
                    {
                        Console.WriteLine("Вы погибли!");
                        break;
                    }
                }

                if (player.CurrentHealth > 0)
                {
                    Console.WriteLine($"Ваш счет: {player.Score}");
                }
            }
        }
    }
}
