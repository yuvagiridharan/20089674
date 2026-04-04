namespace ChurrosTruck
{
    /// Represents a Churros menu item with a name and price.
    public class Churros
    {
        
        private string _name = string.Empty;
        private double _price;

    
        public string Name
        {
            get { return _name; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Churros name cannot be empty data.");
                _name = value;
            }
        }

        public double Price
        {
            get { return _price; }
            private set
            {
                if (value < 0)
                    throw new ArgumentException("Price cannot be negative insert.");
                _price = value;
            }
        }

        // Constructor
        public  Churros(string name, double price)
        {
            Name = name;
            Price = price;
        }

        // Returns a formatted string description of the menu item
        public override string ToString()
        {
            return $"{Name}: €{Price:F2}";
        }
    }
}