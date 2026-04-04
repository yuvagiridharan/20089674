import socket
import json
import re
from rich.console import Console
from rich.panel import Panel
from rich.table import Table
from rich.text import Text
from rich import box
import questionary
from questionary import Style


console = Console()

#configuration
HOST: str = "127.0.0.1"
PORT: int = 65432
BUFFER_SIZE: int = 4096

# Custom questionary style
PROMPT_STYLE = Style(
    [
        ("qmark",        "fg:#00aaff bold"),   # the '?' marker
        ("question",     "bold"),
        ("answer",       "fg:#00cc66 bold"),
        ("pointer",      "fg:#00aaff bold"),
        ("highlighted",  "fg:#00aaff bold"),
        ("selected",     "fg:#00cc66"),
        ("instruction",  "fg:#888888 italic"),
        ("text",         ""),
        ("disabled",     "fg:#555555 italic"),
    ]
)


def validate_pps(value):
    # Remove extra spaces from start and end
    pps = value.strip()

    # Check if pps number is empty
    if len(pps) == 0:
        return "PPS number cannot be empty"

    # Check total length is between 8 and 9 characters
    # 7 digits + 1 or 2 letters = 8 or 9 characters
    if len(pps) < 8 or len(pps) > 9:
        return "PPS number must be 8 or 9 characters, e.g. 1234567A"

    # Check first 7 characters are all numbers
    for i in range(7):
        if not pps[i].isdigit():
            return "First 7 characters must be numbers, e.g. 1234567A"

    # Check remaining characters are letters
    for i in range(7, len(pps)):
        if not pps[i].isalpha():
            return "Last 1 or 2 characters must be letters, e.g. 1234567A"

    # PPS number passed all checks
    return True


def validate_name(value: str) -> bool | str:
    """Validate that a full name contains only letters and spaces.

    Parameters
    ----------
    value : str
        The raw name string entered by the user.

    Returns
    -------
    bool | str
        ``True`` if valid, or an error message string if invalid.
    """
    if not re.match(r"^[A-Za-z\s]+$", value.strip()) or len(value.strip()) < 2:
        return "Name must contain letters only (spaces allowed, min 2 characters)"
    return True


def validate_address(value: str) -> bool | str:
    """Validate that an address is not too short to be meaningful.

    Parameters
    ----------
    value : str
        The raw address string entered by the user.

    Returns
    -------
    bool | str
        ``True`` if valid, or an error message string if invalid.
    """
    if len(value.strip()) < 5:
        return "Address must be at least 5 characters"
    return True


def validate_license(value: str) -> bool | str:
    """Validate that a driving licence field is not blank.

    Parameters
    ----------
    value : str
        The raw licence string entered by the user.

    Returns
    -------
    bool | str
        ``True`` if valid, or an error message string if invalid.
    """
    if not value.strip():
        return "Driving licence number cannot be empty"
    return True

#Input collection and display functions
def collect_customer_info() -> dict:
    """Interactively collect all required EasyDrive registration fields.

    Uses questionary for interactive prompts with inline validation so
    the user sees errors immediately without re-entering all fields.

    Returns
    -------
    dict
        A dictionary with keys ``name``, ``address``, ``pps_number``, and
        ``driving_license``, ready to be serialised and sent to the server.
    """
    console.print(
        Panel(
            Text("EasyDrive Car Rental", justify="center", style="bold white"),
            subtitle="[dim]Customer Registration Form[/dim]",
            border_style="cyan",
            padding=(1, 4),
        )
    )
    console.print("  Please fill in your details below.\n")

    name: str = questionary.text(
        "Full Name:",
        validate=validate_name,
        style=PROMPT_STYLE,
    ).ask()

    address: str = questionary.text(
        "Home Address:",
        validate=validate_address,
        style=PROMPT_STYLE,
    ).ask()

    pps_number: str = questionary.text(
        "PPS Number:",
        validate=validate_pps,
        style=PROMPT_STYLE,
        instruction="(format: 1234567A)",
    ).ask()

    driving_license: str = questionary.text(
        "Driving Licence No.:",
        validate=validate_license,
        style=PROMPT_STYLE,
    ).ask()

    # questionary returns None if the user hits Ctrl+C
    if any(v is None for v in [name, address, pps_number, driving_license]):
        console.print("\n  Registration cancelled.")
        raise SystemExit(0)

    return {
        "name":            name.strip(),
        "address":         address.strip(),
        "pps_number":      pps_number.strip().upper(),
        "driving_license": driving_license.strip().upper(),
    }


def show_summary(customer_data: dict) -> None:
    """Display a formatted table of the collected registration details.

    Parameters
    ----------
    customer_data : dict
        The customer information dict from :func:`collect_customer_info`.

    Returns
    -------
    None
    """
    table = Table(
        title="Registration Summary",
        box=box.SIMPLE_HEAVY,
        header_style="bold cyan",
        show_header=True,
        show_lines=False,
        padding=(0, 2),
    )
    table.add_column("Field",  style="dim",   min_width=20)
    table.add_column("Value",  style="white", min_width=30)

    labels = {
        "name":            "Full Name",
        "address":         "Home Address",
        "pps_number":      "PPS Number",
        "driving_license": "Driving Licence No.",
    }
    for key, label in labels.items():
        table.add_row(label, customer_data.get(key, ""))

    console.print()
    console.print(table)

#Network communication functions
def send_registration(customer_data: dict) -> dict:
    """Open a TCP connection to the server and transmit the registration data.

    Parameters
    ----------
    customer_data : dict
        The customer information dict from :func:`collect_customer_info`.

    Returns
    -------
    dict
        The parsed JSON response from the server.

    Raises
    ------
    ConnectionRefusedError
        If the server is not reachable at ``HOST:PORT``.
    TimeoutError
        If the connection or data transfer times out.
    """
    payload: bytes = json.dumps(customer_data).encode("utf-8")

    with console.status(f"  Connecting to server {HOST}:{PORT} ...", spinner="dots"):
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client_sock:
            client_sock.settimeout(10)
            client_sock.connect((HOST, PORT))
            client_sock.sendall(payload)
            raw_response: bytes = client_sock.recv(BUFFER_SIZE)

    return json.loads(raw_response.decode("utf-8"))


def display_response(response: dict) -> None:
    """Print the server response in a formatted panel.

    Parameters
    ----------
    response : dict
        The parsed response dict from :func:`send_registration`.

    Returns
    -------
    None
    """
    console.print()
    if response.get("status") == "success":
        reg_no = response.get("registration_number", "N/A")
        content = Text.assemble(
            ("Registration Successful\n\n", "bold green"),
            (response.get("message", ""), "white"),
            ("\n\nRegistration Number: ", "dim"),
            (reg_no, "bold green"),
            ("\n\nPlease keep this number safe.", "dim italic"),
        )
        console.print(Panel(content, border_style="green", padding=(1, 4)))
    else:
        content = Text.assemble(
            ("Registration Failed\n\n", "bold red"),
            ("Reason: ", "dim"),
            (response.get("message", "Unknown error"), "white"),
        )
        console.print(Panel(content, border_style="red", padding=(1, 4)))

#Entry point
def main() -> None:
    """Entry point for the EasyDrive client application.

    Orchestrates: collect input -> confirm -> send to server -> display result.
    Handles network errors gracefully with plain error messages.

    Returns
    -------
    None
    """
    try:
        customer_data: dict = collect_customer_info()

        show_summary(customer_data)

        confirmed: bool = questionary.confirm(
            "Submit this registration?",
            default=True,
            style=PROMPT_STYLE,
        ).ask()

        if not confirmed:
            console.print("\n  Registration cancelled. Goodbye!\n")
            return

        response: dict = send_registration(customer_data)
        display_response(response)

    except ConnectionRefusedError:
        console.print(
            "\n  [bold red]Connection refused.[/bold red] "
            "Make sure Que3_server.py is running first.\n"
        )
    except TimeoutError:
        console.print(
            "\n  [bold red]Connection timed out.[/bold red] "
            "The server is not responding. Try again.\n"
        )
    except KeyboardInterrupt:
        console.print("\n\n  Registration interrupted. Goodbye!\n")
    except SystemExit:
        pass
    except Exception as exc:  # noqa: BLE001
        console.print(f"\n  [bold red]Unexpected error:[/bold red] {exc}\n")


if __name__ == "__main__":
    main()