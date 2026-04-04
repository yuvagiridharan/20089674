using System;

namespace ChurrosTruck
{
    // Unit test class for Order
    // Tests the pay_bill() method 
    public class OrderTests
    {
        
        private static int passed = 0;
        private static int failed = 0;

      
        public static void RunAllTests()
        {
          
            Console.WriteLine("       Running Order Unit Tests          ");
          

            // Run each test one by one
            Test_PayBill_CorrectTotalForSingleItem();
            Test_PayBill_CorrectTotalForMultipleQuantity();
            Test_PayBill_ZeroQuantityGivesZeroBill();
            Test_PayBill_NegativePriceThrowsException();
            Test_PayBill_ReturnsCorrectValue();

           
            Console.WriteLine($"  Tests Passed : {passed}");
            Console.WriteLine($"  Tests Failed : {failed}");
        }

        // Test 1 - Check bill is correct for single item
        private static void Test_PayBill_CorrectTotalForSingleItem()
        {
            // Arrange - create a churros item and order
            Churros item = new Churros("Churros with plain sugar", 6.00);
            Order order = new Order(item, 1);

          
            double result = order.pay_bill(6.00);

            // Assert - check result is correct
            if (result == 6.00)
            {
                Console.WriteLine("PASS - Test1: Single item bill is correct (6.00)");
                passed++;
            }
            else
            {
                Console.WriteLine($"FAIL - Test1: Expected 6.00 but got {result}");
                failed++;
            }

            
            Order.ResetCounter();
        }

        // Test 2 - Check bill is correct for multiple quantity
        private static void Test_PayBill_CorrectTotalForMultipleQuantity()
        {
            
            Churros item = new Churros("Churros with Nutella", 8.00);
            Order order = new Order(item, 3);

           
            double result = order.pay_bill(8.00);

            // Assert - 3 x 8.00 = 24.00
            if (result == 24.00)
            {
                Console.WriteLine("PASS - Test2: Multiple quantity bill is correct (24.00)");
                passed++;
            }
            else
            {
                Console.WriteLine($"FAIL - Test2: Expected 24.00 but got {result}");
                failed++;
            }

            Order.ResetCounter();
        }

        // Test 3 - Check bill is zero when quantity is zero
        private static void Test_PayBill_ZeroQuantityGivesZeroBill()
        {
            // Arrange
            Churros item = new Churros("Churros with chocolate sauce", 8.00);
            Order order = new Order(item, 1);

            // Act - pay bill with zero unit price
            double result = order.pay_bill(0.00);

            // Assert - 1 x 0.00 = 0.00
            if (result == 0.00)
            {
                Console.WriteLine("PASS - Test3: Zero price gives zero bill (0.00)");
                passed++;
            }
            else
            {
                Console.WriteLine($"FAIL - Test3: Expected 0.00 but got {result}");
                failed++;
            }

            Order.ResetCounter();
        }

        // Test 4 - Check negative price throws exception
        private static void Test_PayBill_NegativePriceThrowsException()
        {
            // Arrange
            Churros item = new Churros("Churros with cinnamon sugar", 6.00);
            Order order = new Order(item, 1);

            // Act and Assert
            try
            {
                // This should throw an exception
                order.pay_bill(-5.00);

                // If we reach here the test failed
                Console.WriteLine("FAIL - Test4: Expected exception for negative price");
                failed++;
            }
            catch (ArgumentException)
            {
                // Exception was thrown as expected
                Console.WriteLine("PASS - Test4: Negative price throws ArgumentException");
                passed++;
            }

            Order.ResetCounter();
        }

        // Test 5 - Check pay_bill returns the correct value
        private static void Test_PayBill_ReturnsCorrectValue()
        {
            // Arrange - create order with quantity 2
            Churros item = new Churros("Churros with plain sugar", 6.00);
            Order order = new Order(item, 2);

            // Act - pay bill for 2 items at 6.00 each
            double result = order.pay_bill(6.00);

            // Assert - 2 x 6.00 = 12.00
            if (result == 12.00)
            {
                Console.WriteLine("PASS - Test5: pay_bill returns correct value (12.00)");
                passed++;
            }
            else
            {
                Console.WriteLine($"FAIL - Test5: Expected 12.00 but got {result}");
                failed++;
            }

            Order.ResetCounter();
        }
    }
}