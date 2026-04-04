import socket
import json
import uuid
import logging
from datetime import datetime
from tinydb import TinyDB, Query

# Server settings
HOST = "127.0.0.1"
PORT = 65432
DB_PATH = "easydrive_customers.json"
BUFFER_SIZE = 4096

# Setup logging to show server activity
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s  [SERVER]  %(levelname)s  %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S",
)
logger = logging.getLogger(__name__)


# Open and return the database
def get_database():
    return TinyDB(DB_PATH)


# Save customer to database and return registration number
def save_customer(db, customer_data):
    # Generate unique registration number
    registration_number = str(uuid.uuid4()).upper()[:12]

    # Add extra info to customer data
    customer_data["registration_number"] = registration_number
    customer_data["registered_at"] = datetime.now().isoformat(timespec="seconds")

    # Get customers table and insert record
    customers_table = db.table("customers")
    customers_table.insert(customer_data)

    # Log the saved customer
    logger.info(
        "Saved customer %s with registration number %s",
        customer_data.get("name"),
        registration_number
    )

    return registration_number


# Check if customer already exists using PPS number
def customer_exists(db, pps_number):
    customers_table = db.table("customers")
    Customer = Query()

    # Search for matching PPS number
    results = customers_table.search(Customer.pps_number == pps_number)

    # Return True if found, False if not found
    if len(results) > 0:
        return True
    return False


# Print all customers in database
def print_db_table(db):
    customers_table = db.table("customers")
    records = customers_table.all()

    # Check if database is empty
    if not records:
        print("No customers registered yet.")
        return

    # Print each customer record
    print("\n--- Registered Customers ---")
    for r in records:
        print("Name            :", r.get("name", ""))
        print("PPS             :", r.get("pps_number", ""))
        print("Reg No.         :", r.get("registration_number", ""))
        print("Registered At   :", r.get("registered_at", ""))
       


# Handle one client connection
def handle_client(conn, addr, db):
    print(f"New connection from {addr[0]}:{addr[1]}")
    logger.info("Connection accepted from %s:%s", addr[0], addr[1])

    with conn:
        try:
            # Receive data from client
            raw = conn.recv(BUFFER_SIZE)

            # Check if data is empty
            if not raw:
                logger.warning("Empty data received - closing connection.")
                return

            # Convert received bytes to dictionary
            customer_data = json.loads(raw.decode("utf-8"))
            print(f"Registration request for: {customer_data.get('name')}")

            # Check if customer already registered
            if customer_exists(db, customer_data.get("pps_number", "")):
                # Send error response back to client
                response = {
                    "status": "error",
                    "message": "A customer with this PPS number is already registered.",
                }
                print(f"Rejected - duplicate PPS {customer_data.get('pps_number')}")
                logger.warning(
                    "Duplicate PPS %s rejected.",
                    customer_data.get("pps_number")
                )

            else:
                # Save customer and get registration number
                reg_number = save_customer(db, customer_data)

                # Send success response back to client
                response = {
                    "status": "success",
                    "registration_number": reg_number,
                    "message": (
                        f"Welcome to EasyDrive, {customer_data.get('name')}! "
                        f"Your registration number is {reg_number}."
                    ),
                }
                print(f"Saved - assigned registration number {reg_number}")

            # Send response to client
            conn.sendall(json.dumps(response).encode("utf-8"))

        except json.JSONDecodeError as exc:
            # Handle invalid data format
            logger.error("Invalid data received from %s: %s", addr, exc)
            error_response = {
                "status": "error",
                "message": "Invalid data format received."
            }
            conn.sendall(json.dumps(error_response).encode("utf-8"))

        except Exception as exc:
            # Handle any other unexpected errors
            logger.exception("Unexpected error: %s", exc)


# Start the server
def start_server():
    # Open database
    db = get_database()

    # Print server info
    print("=" * 40)
    print("   EasyDrive Registration Server")
    print("=" * 40)
    print(f"Host      : {HOST}")
    print(f"Port      : {PORT}")
    print(f"Database  : {DB_PATH}")
    print(f"Started   : {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print("Waiting for connections ...\n")

    # Create TCP server socket
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server_sock:
        # Allow reuse of address
        server_sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

        # Bind to host and port
        server_sock.bind((HOST, PORT))

        # Start listening for connections
        server_sock.listen(5)

        try:
            # Keep accepting connections forever
            while True:
                conn, addr = server_sock.accept()
                handle_client(conn, addr, db)

        except KeyboardInterrupt:
            # Handle Ctrl+C shutdown
            print("\nServer shutting down. Goodbye!")
            print_db_table(db)
            logger.info("Server stopped.")

        finally:
            # Always close database when done
            db.close()


if __name__ == "__main__":
    start_server()