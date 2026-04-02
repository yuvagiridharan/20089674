namespace ChurrosTruck
{
  
    public class Order
    {
      
        private static int _nextOrderNo = 1;  

        private string _orderDetails = string.Empty; 
        private int    _quantity;
        private double _bill;
        private bool   _isCollected;

      
       public int OrderNo { get; private set; }

  
        public string OrderDetails
        {
            get { return _orderDetails; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    Console.WriteLine("order details cannot be empty...");
                _orderDetails = value;
            }
        }

      
        public int Quantity
        {
            get { return _quantity; }
            private set
            {
                if (value <= 0)
                    Console.WriteLine("Error: Quantity must be at least 1.");
                     Console.WriteLine("You entered: " + value);
                     Console.WriteLine("Please enter a number greater than zero.");
                    _quantity = value;
               
            }
        }

       
      public double Bill
{
    
    get
    {
        return _bill;
    }

    private set
    {
        _bill = value;

        Console.WriteLine("Bill has been set to: €" + _bill);
    }
}
       
        public bool IsCollected
        {
            get { return _isCollected; }
            private set { _isCollected = value; }
        }

   
        /// <param name="item">The Churros item selected.</param>
        /// <param name="quantity">Number of portions.</param>
        public Order(Churros item, int quantity)
        {
            if (item == null)
                Console.WriteLine("Error: item cannot be empty.");
                Console.WriteLine("Please select a valid churros item.");

            OrderNo      = _nextOrderNo++;
            OrderDetails = item.Name;
            Quantity     = quantity;
            Bill         = 0;          
            IsCollected  = false;
        }

    
        public void place_order()
        {
            Console.WriteLine();
            Console.WriteLine("┌─────────────────────────────────────────┐");
            Console.WriteLine("│             ORDER CONFIRMED              │");
            Console.WriteLine("├─────────────────────────────────────────┤");
            Console.WriteLine($"│  Order No : #{OrderNo,-4}                      │");
            Console.WriteLine($"│  Item     : {OrderDetails,-29}│");
            Console.WriteLine($"│  Quantity : {Quantity,-29}│");
            Console.WriteLine("│                                         │");
            Console.WriteLine("│  Please proceed to payment.             │");
            Console.WriteLine("└─────────────────────────────────────────┘");
        }

      
        /// <param name="unitPrice">Price per portion in euro.</param>
      
        public double pay_bill(double unitPrice)
        {
            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            Bill = unitPrice * Quantity;

            Console.WriteLine();
            Console.WriteLine("┌─────────────────────────────────────────┐");
            Console.WriteLine("│               PAYMENT RECEIPT           │");
            Console.WriteLine("├─────────────────────────────────────────┤");
            Console.WriteLine($"│  Order No : #{OrderNo,-4}                      │");
            Console.WriteLine($"│  Item     : {OrderDetails,-29}│");
            Console.WriteLine($"│  Qty      : {Quantity,-29}│");
            Console.WriteLine($"│  Unit €   : {unitPrice,-29:F2}│");
            Console.WriteLine($"│  TOTAL    : €{Bill,-28:F2}│");
            Console.WriteLine("│                                         │");
            Console.WriteLine("│  Payment received. Thank you!           │");
            Console.WriteLine("└─────────────────────────────────────────┘");

            return Bill;
        }

   
        public void collect_order()
        {
            IsCollected = true;
            Console.WriteLine();
            Console.WriteLine($"  ✔  Order #{OrderNo} ({OrderDetails} x{Quantity}) collected. Enjoy!");
        }

     
        public override string ToString()
        {
            return $"#{OrderNo,-4} | {OrderDetails,-30} | Qty: {Quantity} | €{Bill:F2}";
        }

   
        internal static void ResetCounter() => _nextOrderNo = 1;
    }
}