using System;
using System.Threading;

public class Account
{
    private decimal balance;
    private object balanceLock = new object();

    public decimal Balance
    {
        get { return balance; }
    }

    public void Deposit(decimal amount)
    {
        lock (balanceLock)
        {
            balance += amount;
            Console.WriteLine($"Пополнено: {amount}. Новый баланс: {balance}");
            Monitor.Pulse(balanceLock);
        }
    }

    public void Withdraw(decimal amount)
    {
        lock (balanceLock)
        {
            while (balance < amount)
            {
                Console.WriteLine($"Ожидание пополнения баланса до {amount}...");
                Monitor.Wait(balanceLock);
            }

            balance -= amount;
            Console.WriteLine($"Снято: {amount}. Новый баланс: {balance}");
        }
    }
}

public class Program
{
    static void Main()
    {
        Account account = new Account();

        // Запуск потока пополнения баланса
        Thread depositThread = new Thread(() =>
        {
            Random random = new Random();
            while (true)
            {
                decimal amount = random.Next(1, 100);
                account.Deposit(amount);
                Thread.Sleep(random.Next(1000, 3000));
            }
        });
        depositThread.Start();

        // Снятие денег после достижения указанной суммы
        account.Withdraw(50);

        Console.WriteLine($"Остаток на балансе: {account.Balance}");

        Console.ReadLine();
    }
}
