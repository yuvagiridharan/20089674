using System;
using System.Collections.Generic;


class Program
{
    static Dictionary<int, Element> LoadElements()
    {
        return new Dictionary<int, Element>
        {
            { 1,  new Element(1,  "Hydrogen",   "H",  "Nonmetal",        "Lightest and most abundant element in the universe") },
            { 2,  new Element(2,  "Helium",     "He", "Noble Gas",       "Colourless inert gas, second lightest element") },
            { 3,  new Element(3,  "Lithium",    "Li", "Alkali Metal",    "Lightest solid metal, used in batteries") },
            { 4,  new Element(4,  "Beryllium",  "Be", "Alkaline Earth",  "Hard lightweight metal used in aerospace") },
            { 5,  new Element(5,  "Boron",      "B",  "Metalloid",       "Used in glass and detergents") },
            { 6,  new Element(6,  "Carbon",     "C",  "Nonmetal",        "Basis of all known life on Earth") },
            { 7,  new Element(7,  "Nitrogen",   "N",  "Nonmetal",        "Makes up 78% of Earth's atmosphere") },
            { 8,  new Element(8,  "Oxygen",     "O",  "Nonmetal",        "Essential for respiration and combustion") },
            { 9,  new Element(9,  "Fluorine",   "F",  "Halogen",         "Most reactive and electronegative element") },
            { 10, new Element(10, "Neon",       "Ne", "Noble Gas",       "Inert gas used in bright electric signs") },
            { 11, new Element(11, "Sodium",     "Na", "Alkali Metal",    "Highly reactive metal, found in table salt") },
            { 12, new Element(12, "Magnesium",  "Mg", "Alkaline Earth",  "Lightweight metal used in alloys") },
            { 13, new Element(13, "Aluminium",  "Al", "Post-transition", "Most abundant metal in Earth's crust") },
            { 14, new Element(14, "Silicon",    "Si", "Metalloid",       "Key material in semiconductors and glass") },
            { 15, new Element(15, "Phosphorus", "P",  "Nonmetal",        "Essential for DNA and energy transfer in cells") },
            { 16, new Element(16, "Sulfur",     "S",  "Nonmetal",        "Yellow solid used in fertilisers and gunpowder") },
            { 17, new Element(17, "Chlorine",   "Cl", "Halogen",         "Used in water purification and PVC plastics") },
            { 18, new Element(18, "Argon",      "Ar", "Noble Gas",       "Third most abundant gas in Earth's atmosphere") },
            { 19, new Element(19, "Potassium",  "K",  "Alkali Metal",    "Essential mineral for nerve and muscle function") },
            { 20, new Element(20, "Calcium",    "Ca", "Alkaline Earth",  "Key component of bones, teeth, and shells") },
            { 21, new Element(21, "Scandium",   "Sc", "Transition Metal","Lightweight metal used in aerospace alloys") },
            { 22, new Element(22, "Titanium",   "Ti", "Transition Metal","Strong, lightweight, corrosion-resistant metal") },
            { 23, new Element(23, "Vanadium",   "V",  "Transition Metal","Used in steel alloys and rechargeable batteries") },
            { 24, new Element(24, "Chromium",   "Cr", "Transition Metal","Used in stainless steel and chrome plating") },
            { 25, new Element(25, "Manganese",  "Mn", "Transition Metal","Used in steel production and dry cell batteries") },
            { 26, new Element(26, "Iron",       "Fe", "Transition Metal","Most used metal, core component of steel") },
            { 27, new Element(27, "Cobalt",     "Co", "Transition Metal","Used in lithium batteries and blue pigments") },
            { 28, new Element(28, "Nickel",     "Ni", "Transition Metal","Used in coins, stainless steel, and batteries") },
            { 29, new Element(29, "Copper",     "Cu", "Transition Metal","Excellent conductor used in wiring and plumbing") },
            { 30, new Element(30, "Zinc",       "Zn", "Transition Metal","Used in galvanising steel and in batteries") },
        };
    }

    static void Main(string[] args)
{
    Dictionary<int, Element> table = LoadElements();

    Console.WriteLine("Hi there! Happy to help!");
    Console.WriteLine("--------------------------------------------------");

    string again = "y";

    while ( again == "y")
    {
        Console.Write("\nProvide atomic number of the element (1-30): ");

        string input = Console.ReadLine() ??"";

        // validate input is a number
        if (!int.TryParse(input, out int atomicNumber))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            continue;
        }

        // validate number is in range
        if (atomicNumber < 1 || atomicNumber > 30)
        {
            Console.WriteLine("Please enter a number between 1 and 30.");
            continue;
        }

        // lookup and display
        Console.WriteLine("--------------------------------------------------");
        table[atomicNumber].Display();
        Console.WriteLine("--------------------------------------------------");

        // ask to continue
        Console.Write("\nDo you want to know more elements [y/n]? ");
        again = Console.ReadLine()?.Trim().ToLower() ?? "n";

        // validate y/n input
        while (again != "y" && again != "n")
        {
            Console.Write("Please enter y or n: ");
            again = Console.ReadLine()?.Trim().ToLower() ?? "n";
        }
    }

    Console.WriteLine("\nThanks!");
}
}
