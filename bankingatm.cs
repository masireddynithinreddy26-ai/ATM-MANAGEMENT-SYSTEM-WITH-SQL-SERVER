using System;
using System.Data.SqlClient;

namespace ATMApp
{
    class Program
    {
        static string conStr = @"Data Source=MASIREDDYNITHN\MSSQLSERVER01;Initial Catalog=AtmManagement;Integrated Security=True";

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("\n===== ATM MANAGEMENT SYSTEM =====");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Choose: ");
                int ch = int.Parse(Console.ReadLine());

                switch (ch)
                {
                    case 1: Register(); break;
                    case 2:
                        int ac = Login();
                        if (ac != -1)
                            UserMenu(ac);
                        else
                            Console.WriteLine("Invalid Account No or PIN");
                        break;
                    case 3: return;
                }
            }
        }

        static void Register()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            Console.Write("Account No: ");
            int ac = int.Parse(Console.ReadLine());
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("PIN: ");
            int pin = int.Parse(Console.ReadLine());
            Console.Write("Balance: ");
            double bal = double.Parse(Console.ReadLine());

            SqlCommand cmd = new SqlCommand(
              "INSERT INTO Accounts VALUES(@a,@n,@p,@b)", con);
            cmd.Parameters.AddWithValue("@a", ac);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@p", pin);
            cmd.Parameters.AddWithValue("@b", bal);
            cmd.ExecuteNonQuery();

            con.Close();
            Console.WriteLine("Account Created Successfully!");
        }

        static int Login()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            Console.Write("Account No: ");
            int ac = int.Parse(Console.ReadLine());
            Console.Write("PIN: ");
            int pin = int.Parse(Console.ReadLine());

            SqlCommand cmd = new SqlCommand(
             "SELECT AccountNo FROM Accounts WHERE AccountNo=@a AND Pin=@p", con);
            cmd.Parameters.AddWithValue("@a", ac);
            cmd.Parameters.AddWithValue("@p", pin);

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                return ac;
            }
            con.Close();
            return -1;
        }

        static void UserMenu(int ac)
        {
            while (true)
            {
                Console.WriteLine("\n1. View Balance");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Mini Statement");
                Console.WriteLine("5. Logout");
                Console.Write("Choose: ");
                int ch = int.Parse(Console.ReadLine());

                switch (ch)
                {
                    case 1: ViewBalance(ac); break;
                    case 2: Deposit(ac); break;
                    case 3: Withdraw(ac); break;
                    case 4: MiniStatement(ac); break;
                    case 5: return;
                }
            }
        }

        static void ViewBalance(int ac)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand(
             "SELECT Balance FROM Accounts WHERE AccountNo=@a", con);
            cmd.Parameters.AddWithValue("@a", ac);

            double bal = Convert.ToDouble(cmd.ExecuteScalar());
            Console.WriteLine("Balance = " + bal);

            con.Close();
        }

        static void Deposit(int ac)
        {
            Console.Write("Amount: ");
            double amt = double.Parse(Console.ReadLine());

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand(
             "UPDATE Accounts SET Balance = Balance + @amt WHERE AccountNo=@a", con);
            cmd.Parameters.AddWithValue("@amt", amt);
            cmd.Parameters.AddWithValue("@a", ac);
            cmd.ExecuteNonQuery();

            SqlCommand tr = new SqlCommand(
             "INSERT INTO Transactions(AccountNo,Type,Amount) VALUES(@a,'Deposit',@amt)", con);
            tr.Parameters.AddWithValue("@a", ac);
            tr.Parameters.AddWithValue("@amt", amt);
            tr.ExecuteNonQuery();

            con.Close();
            Console.WriteLine("Deposit Successful!");
        }

        static void Withdraw(int ac)
        {
            Console.Write("Amount: ");
            double amt = double.Parse(Console.ReadLine());

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand balcmd = new SqlCommand(
              "SELECT Balance FROM Accounts WHERE AccountNo=@a", con);
            balcmd.Parameters.AddWithValue("@a", ac);
            double bal = Convert.ToDouble(balcmd.ExecuteScalar());

            if (amt > bal)
            {
                Console.WriteLine("Insufficient Balance!");
                con.Close();
                return;
            }

            SqlCommand cmd = new SqlCommand(
              "UPDATE Accounts SET Balance = Balance - @amt WHERE AccountNo=@a", con);
            cmd.Parameters.AddWithValue("@amt", amt);
            cmd.Parameters.AddWithValue("@a", ac);
            cmd.ExecuteNonQuery();

            SqlCommand tr = new SqlCommand(
             "INSERT INTO Transactions(AccountNo,Type,Amount) VALUES(@a,'Withdraw',@amt)", con);
            tr.Parameters.AddWithValue("@a", ac);
            tr.Parameters.AddWithValue("@amt", amt);
            tr.ExecuteNonQuery();

            con.Close();
            Console.WriteLine("Please collect your cash.");
        }

        static void MiniStatement(int ac)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand(
             "SELECT Type,Amount,TDate FROM Transactions WHERE AccountNo=@a", con);
            cmd.Parameters.AddWithValue("@a", ac);

            SqlDataReader dr = cmd.ExecuteReader();
            Console.WriteLine("\n--- Mini Statement ---");

            while (dr.Read())
            {
                Console.WriteLine(dr["Type"] + " - " + dr["Amount"] + " - " + dr["TDate"]);
            }
            con.Close();
        }
    }
}
