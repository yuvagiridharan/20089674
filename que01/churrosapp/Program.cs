using System;
using System.Collections.Generic;

namespace ChurrosTruck
{
    /// <summary>
    /// Entry point – menu-driven console application for the Churros food truck.
    /// Uses a Queue<Order> as the primary data structure for order management.
    /// </summary>
    internal class Program
    {
        // ── Menu data – stored in a list of Churros objects ──────────────────
        private static  List<Churros> Menu = new List<Churros>
        {
            new Churros("Churros with plain sugar",     6.00),
            new Churros("Churros with cinnamon sugar",  6.00),
            new Churros("Churros with chocolate sauce", 8.00),
            new Churros("Churros with Nutella",         8.00),
        };

        // ── Order queue (FIFO – first placed, first delivered) ────────────────
        private static readonly Queue<Order> OrderQueue = new Queue<Order>();

        // ─────────────────────────────────────────────────────────────────────
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool running = true;

            while (running)
            {
                ShowMainMenu();
                string input = Console.ReadLine()?.Trim() ?? "";

                switch (input)
                {
                    case "1": PlaceOrderFlow();   break;
                    case "2": DeliverOrderFlow(); break;
                    case "0": running = false;    break;
                    default:
                        Console.WriteLine("\n  Invalid option. Please try again.");
                        Pause();
                        break;
                }
            }

            Console.WriteLine("\n  Goodbye! Thanks for visiting Delicious Churros. 🥐\n");
        }

        // ── Display main menu ─────────────────────────────────────────────────
        private static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine("                     🥐 Delicious Churros 🥐                 ");
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine();

            for (int i = 0; i < Menu.Count; i++)
                Console.WriteLine($"  {i + 1}. {Menu[i]}");

            Console.WriteLine();

            if (OrderQueue.Count > 0)
            {
                Console.WriteLine($"  📋 Orders in queue: {OrderQueue.Count}");
                Console.WriteLine();
            }

            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine("  Choose your option:");
            Console.WriteLine("    1. Place order");
            Console.WriteLine("    2. Deliver order");
            Console.WriteLine("    0. Exit");
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.Write("  > ");
        }

        // ── Place-order flow ──────────────────────────────────────────────────
        private static void PlaceOrderFlow()
        {
            Console.Clear();
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine("                        Place an Order                       ");
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine();

            // Show numbered menu items
            for (int i = 0; i < Menu.Count; i++)
                Console.WriteLine($"  {i + 1}. {Menu[i]}");

            Console.WriteLine("  0. Back");
            Console.WriteLine();
            Console.Write("  Select item (1-4): ");

            if (!int.TryParse(Console.ReadLine(), out int itemChoice) ||
                itemChoice < 0 || itemChoice > Menu.Count)
            {
                Console.WriteLine("\n  Invalid selection.");
                Pause(); return;
            }

            if (itemChoice == 0) return;

            Churros selected = Menu[itemChoice - 1];

            Console.Write("  Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
            {
                Console.WriteLine("\n  Invalid quantity.");
                Pause(); return;
            }

            // Create order object, place it and pay
            try
            {
                Order order = new Order(selected, qty);
                order.place_order();
                order.pay_bill(selected.Price);

                // Enqueue for delivery
                OrderQueue.Enqueue(order);

                Console.WriteLine($"\n  Your order number is: #{order.OrderNo}");
                Console.WriteLine("  Please wait – you will be called when your order is ready.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n  Error: {ex.Message}");
            }

            Pause();
        }

        // ── Deliver-order flow ────────────────────────────────────────────────
        private static void DeliverOrderFlow()
        {
            Console.Clear();
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine("                       Deliver Order                        ");
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine();

            if (OrderQueue.Count == 0)
            {
                Console.WriteLine("  No orders in the queue.");
                Pause(); return;
            }

            // Show all queued orders
            Console.WriteLine("  Current queue:");
            Console.WriteLine();
            foreach (Order o in OrderQueue)
                Console.WriteLine($"    {o}");

            Console.WriteLine();
            Console.WriteLine("  The next order to deliver is:");
            Order next = OrderQueue.Peek();
            Console.WriteLine($"    {next}");
            Console.WriteLine();
            Console.Write("  Confirm delivery? (Y/N): ");

            string confirm = Console.ReadLine()?.Trim().ToUpper() ?? "N";

            if (confirm == "Y")
            {
                Order delivered = OrderQueue.Dequeue();
                delivered.collect_order();
                Console.WriteLine($"\n  Orders remaining in queue: {OrderQueue.Count}");
            }
            else
            {
                Console.WriteLine("\n  Delivery cancelled.");
            }

            Pause();
        }

        // ── Helper – wait for keypress ────────────────────────────────────────
        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("  Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}