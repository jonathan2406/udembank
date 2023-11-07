using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using udembankproject.Models;

namespace udembankproject.Controllers
{
    public static class LoansController
    {
        public static void RequestLoan(ObjectId userId)
        {
            string userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(userId);
            ObjectId? userAccountId = AccountController.GetAccountID(userAccountNumber);

            if (userAccountId == null)
            {
                AnsiConsole.MarkupLine("[red]User account not found[/]");
                return;
            }

            var userSavingGroups = GetSavingGroupsByUserId(userAccountId.Value);

            if (userSavingGroups.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]You are not part of any saving group.[/]");
                return;
            }

            int selectedGroupIndex = SelectSavingGroup(userSavingGroups);

            if (selectedGroupIndex == -1)
            {
                AnsiConsole.MarkupLine("[red]Invalid selection. Loan request canceled.[/]");
                return;
            }

            var selectedGroup = userSavingGroups[selectedGroupIndex];

            int loanAmount = GetLoanAmount(selectedGroup);

            if (loanAmount == -1)
            {
                AnsiConsole.MarkupLine("[red]Loan request canceled.[/]");
                return;
            }

            List<ObjectId> topContributors = GetTopContributors(selectedGroup.Contributions);

            double monthlyInterestRate = 0.03;
            if (topContributors.Contains(userAccountId.Value))
            {
                monthlyInterestRate -= 0.01;  
            }

            int loanTermMonths = GetLoanTerm();

            double totalInterest = loanAmount * monthlyInterestRate * loanTermMonths;

            DisplayLoanDetails(loanAmount, monthlyInterestRate, loanTermMonths, totalInterest);

            if (ConfirmLoanRequest())
            {
                UpdateSavingGroupAndUserAccount(selectedGroup, userAccountId.Value, loanAmount, monthlyInterestRate);
                CreateLoan(selectedGroup, userAccountId.Value, loanAmount, totalInterest, loanTermMonths); 
                AnsiConsole.MarkupLine("[yellow]Loan request successful.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Loan request canceled.[/]");
            }
        }
        private static int GetLoanTerm()
        {
            int loanTerm;

            do
            {
                loanTerm = AnsiConsole.Prompt(
                    new TextPrompt<int>("[yellow]Enter the loan term in months: [/]")
                        .PromptStyle(Style.Parse("green"))
                );

                if (loanTerm < 2)
                {
                    AnsiConsole.MarkupLine("[red]Loan term must be at least 2 months.[/]");
                }
            } while (loanTerm < 2);

            return loanTerm;
        }

        private static List<Savings_Group> GetSavingGroupsByUserId(ObjectId userId)
        {
            var filter = Builders<Savings_Group>.Filter.AnyEq(x => x.UsersID, userId);
            return Collections.GetSavingsGroupCollection().Find(filter).ToList();
        }

        private static int SelectSavingGroup(List<Savings_Group> availableGroups)
        {
            if (availableGroups.Count == 1)
            {
                // Si solo hay un grupo disponible, selección automática.
                return 0;
            }

            Console.WriteLine("Available Saving Groups for Loan:");

            for (int i = 0; i < availableGroups.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableGroups[i].Name}");
            }

            int selectedGroupIndex = -1;

            do
            {
                selectedGroupIndex = AnsiConsole.Prompt(
                    new TextPrompt<int>("[yellow]Select a saving group to request a loan from (1 to N): [/]")
                        .PromptStyle(Style.Parse("green"))
                ) - 1;

                if (selectedGroupIndex < 0 || selectedGroupIndex >= availableGroups.Count)
                {
                    AnsiConsole.MarkupLine("[red]Invalid selection. Please enter a valid group number.[/]");
                }
            } while (selectedGroupIndex < 0 || selectedGroupIndex >= availableGroups.Count);

            return selectedGroupIndex;
        }
        private static int GetLoanAmount(Savings_Group selectedGroup)
        {
            int loanAmount;

            do
            {
                loanAmount = AnsiConsole.Prompt(
                    new TextPrompt<int>("[yellow]Enter the loan amount: [/]")
                        .PromptStyle(Style.Parse("green"))
                );

                if (loanAmount <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Loan amount must be greater than 0.[/]");
                }
                else if (loanAmount > selectedGroup.Amount)
                {
                    AnsiConsole.MarkupLine("[red]Loan amount cannot exceed the total savings in the group.[/]");
                }
            } while (loanAmount <= 0 || loanAmount > selectedGroup.Amount);

            return loanAmount;
        }

        private static void DisplayLoanDetails(int loanAmount, double monthlyInterestRate, int minLoanTermMonths, double totalInterest)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Loan Details:[/]");
            AnsiConsole.MarkupLine($"[yellow]Loan Amount:[/] {loanAmount}");
            AnsiConsole.MarkupLine($"[yellow]Interest Rate:[/] {monthlyInterestRate * 100}% per month");
            AnsiConsole.MarkupLine($"[yellow]Minimum Loan Term:[/] {minLoanTermMonths} months");
            AnsiConsole.MarkupLine($"[yellow]Total Interest:[/] {totalInterest}");
            AnsiConsole.WriteLine();
        }

        private static bool ConfirmLoanRequest()
        {
            var confirmation = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Do you want to confirm the loan request?")
                    .AddChoices("Yes", "No")
            );

            return confirmation == "Yes";
        }

        private static void UpdateSavingGroupAmount(string groupId, int newAmount)
        {
            var filter = Builders<Savings_Group>.Filter.Eq(x => x.Id, groupId);
            var update = Builders<Savings_Group>.Update.Set(x => x.Amount, newAmount);
            Collections.GetSavingsGroupCollection().UpdateOne(filter, update);
        }
        private static void UpdateSavingGroupAndUserAccount(Savings_Group selectedGroup, ObjectId userAccountId, int loanAmount, double monthlyInterestRate)
        {
            UpdateSavingGroupAmount(selectedGroup.Id, selectedGroup.Amount - loanAmount);
            AccountController.AgregarMontoAlAccount(userAccountId, loanAmount);
        }

        public static List<Loans> GetLoansByAccount(ObjectId accountId)
        {
            var filter = Builders<Loans>.Filter.Eq(x => x.Id_Account, accountId);
            return Collections.GetLoansCollection().Find(filter).ToList();
        }

        private static void CreateLoan(Savings_Group selectedGroup, ObjectId userAccountId, int loanAmount, double totalInterest, int loanTermMonths)
        {
            var newLoan = new Loans
            {
                Interest_Percent = 3,
                Id_Account = userAccountId,
                Id_group = ObjectId.Parse(selectedGroup.Id),
                Amount = (int)(loanAmount + totalInterest),
                LoanTermMonths = loanTermMonths
            };

            Collections.GetLoansCollection().InsertOne(newLoan);
        }
        public static List<Loans> GetLoans()
        {
            return Collections.GetLoansCollection().Find(_ => true).ToList();
        }
        public static void ViewLoans(ObjectId userId)
        {
            var userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(userId);
            ObjectId? userAccountId = AccountController.GetAccountID(userAccountNumber);

            if (userAccountId == null)
            {
                AnsiConsole.MarkupLine("[red]User account not found[/]");
                return;
            }

            var loans = GetLoansByAccount(userAccountId.Value);

            if (loans.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No loans found.[/]");
                return;
            }

            var table = new Table()
                .Title("Loan Details")
                .BorderColor(Color.Green)
                .AddColumn("Loan ID", column => column.Alignment(Justify.Left))
                .AddColumn("Interest Percent", column => column.Alignment(Justify.Left))
                .AddColumn("Account ID", column => column.Alignment(Justify.Left))
                .AddColumn("Group ID", column => column.Alignment(Justify.Left))
                .AddColumn("Months", column => column.Alignment(Justify.Left))
                .AddColumn("Amount", column => column.Alignment(Justify.Left));


            foreach (var loan in loans)
            {
                table.AddRow(
                    loan.Id,
                    loan.Interest_Percent.ToString(),
                    loan.Id_Account.ToString(),
                    loan.Id_group.ToString(),
                    loan.LoanTermMonths.ToString(),
                    loan.Amount.ToString()
                );
            }

            AnsiConsole.Clear();
            AnsiConsole.Render(table);

            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }

        public static void RepayLoan(ObjectId userId)
        {
            string userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(userId);
            ObjectId? userAccountId = AccountController.GetAccountID(userAccountNumber);

            if (userAccountId == null)
            {
                AnsiConsole.MarkupLine("[red]User account not found[/]");
                return;
            }

            var userLoans = GetLoansByAccount(userAccountId.Value);

            if (userLoans.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]You have no outstanding loans.[/]");
                return;
            }

            var selectedLoan = SelectLoanToRepay(userLoans);

            int repaymentAmount = GetRepaymentAmount(selectedLoan);

            if (repaymentAmount == -1)
            {
                AnsiConsole.MarkupLine("[red]Loan repayment canceled.[/]");
                return;
            }

            if (ConfirmRepayment())
            {
                RepayLoanAmount(selectedLoan, repaymentAmount);
                AnsiConsole.MarkupLine("[yellow]Loan repayment successful.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Loan repayment canceled.[/]");
            }
        }
        private static Loans SelectLoanToRepay(List<Loans> userLoans)
        {
            Console.WriteLine("Your Outstanding Loans:");

            for (int i = 0; i < userLoans.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Loan Amount: {userLoans[i].Amount}");
            }

            int selectedLoanIndex;

            do
            {
                selectedLoanIndex = AnsiConsole.Prompt(
                    new TextPrompt<int>("[yellow]Select a loan to repay (1 to N): [/]")
                        .PromptStyle(Style.Parse("green"))
                ) - 1;

                if (selectedLoanIndex < 0 || selectedLoanIndex >= userLoans.Count)
                {
                    AnsiConsole.MarkupLine("[red]Invalid selection. Please enter a valid loan number.[/]");
                }
            } while (selectedLoanIndex < 0 || selectedLoanIndex >= userLoans.Count);

            return userLoans[selectedLoanIndex];
        }

        private static int GetRepaymentAmount(Loans selectedLoan)
        {
            int repaymentAmount;

            do
            {
                repaymentAmount = AnsiConsole.Prompt(
                    new TextPrompt<int>("[yellow]Enter the repayment amount: [/]")
                        .PromptStyle(Style.Parse("green"))
                );

                if (repaymentAmount <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Repayment amount must be greater than 0.[/]");
                }
                else if (repaymentAmount > selectedLoan.Amount)
                {
                    AnsiConsole.MarkupLine("[red]Repayment amount cannot exceed the loan amount.[/]");
                }
            } while (repaymentAmount <= 0 || repaymentAmount > selectedLoan.Amount);

            return repaymentAmount;
        }
        private static void RepayLoanAmount(Loans selectedLoan, int repaymentAmount)
        {
            // Deduct the repayment amount from the user's account
            AccountController.RestarMontoAlAccount(selectedLoan.Id_Account, repaymentAmount);

            int loanTermMonths = selectedLoan.LoanTermMonths;

            // Update the loan amount
            var filter = Builders<Loans>.Filter.Eq(x => x.Id, selectedLoan.Id);
            var newLoanAmount = selectedLoan.Amount - repaymentAmount;

            if (newLoanAmount == 0)
            {
                // If the loan amount is zero, delete the loan
                Collections.GetLoansCollection().DeleteOne(filter);
            }
            else
            {
                var update = Builders<Loans>.Update.Set(x => x.Amount, newLoanAmount);
                Collections.GetLoansCollection().UpdateOne(filter, update);
            }

            AnsiConsole.MarkupLine($"[yellow]You are paying a loan for {loanTermMonths} months.[/]");
        }

        private static bool ConfirmRepayment()
        {
            var confirmation = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Do you want to confirm the loan repayment?")
                    .AddChoices("Yes", "No")
            );

            return confirmation == "Yes";
        }

        public static void RequestLoanFromAnotherGroup(ObjectId userId)
        {
            // Obtén la cuenta del usuario logeado
            string userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(userId);
            ObjectId? userAccountId = AccountController.GetAccountID(userAccountNumber);

            if (userAccountId == null)
            {
                AnsiConsole.MarkupLine("[red]User account not found[/]");
                return;
            }

            // Obtén los grupos de ahorro del usuario
            var userSavingGroups = GetSavingGroupsByUserId(userAccountId.Value);

            if (userSavingGroups.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]You are not part of any saving group.[/]");
                return;
            }

            // Obtén todos los grupos de ahorro disponibles
            var allSavingGroups = GetAllSavingGroups();

            if (allSavingGroups.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No saving groups available for loan requests.[/]");
                return;
            }

            // Filtra los grupos de ahorro de los cuales la cuenta logeada no es miembro
            var availableGroups = allSavingGroups.Where(group => !userSavingGroups.Any(userGroup => userGroup.Id == group.Id)).ToList();

            if (availableGroups.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]You are a member of all available saving groups. You cannot request loans from other groups.[/]");
                return;
            }

            // Filtra los grupos de ahorro en los que al menos una cuenta de uno de los grupos del usuario esté presente
            var eligibleGroups = availableGroups
            .Where(group => userSavingGroups.Any(userGroup =>
            userGroup.UsersID.Any(userGroupUserId =>
            group.UsersID.Any(groupUserId => groupUserId.Equals(userGroupUserId))
             )
             ))
            .ToList();

            if (eligibleGroups.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]You don't have any eligible saving groups to request loans from.[/]");
                return;
            }

            // Selecciona un grupo de ahorro del cual solicitar el préstamo
            int selectedGroupIndex = SelectSavingGroup(eligibleGroups);

            if (selectedGroupIndex == -1)
            {
                AnsiConsole.MarkupLine("[red]Invalid selection. Loan request canceled.[/]");
                return;
            }

            var selectedGroup = eligibleGroups[selectedGroupIndex];

            bool isTopContributor = IsTopContributorInAnyGroup(userAccountId.Value, userSavingGroups);

            double monthlyInterestRate = isTopContributor ? 0.04 : 0.05; // 4% si es un principal contribuyente, 5% en otros casos

            int loanAmount = GetLoanAmount(selectedGroup);

            if (loanAmount == -1)
            {
                AnsiConsole.MarkupLine("[red]Loan request canceled.[/]");
                return;
            }

            int loanTermMonths = GetLoanTerm();

            double totalInterest = loanAmount * monthlyInterestRate * loanTermMonths;

            DisplayLoanDetails(loanAmount, monthlyInterestRate, loanTermMonths, totalInterest);

            if (ConfirmLoanRequest())
            {
                UpdateSavingGroupAndUserAccount(selectedGroup, userAccountId.Value, loanAmount, monthlyInterestRate);
                CreateLoan(selectedGroup, userAccountId.Value, loanAmount, totalInterest, loanTermMonths);
                AnsiConsole.MarkupLine("[yellow]Loan request successful.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Loan request canceled.[/]");
            }
        }
        private static List<Savings_Group> GetAllSavingGroups()
        {
            return Collections.GetSavingsGroupCollection().Find(_ => true).ToList();
        }

        private static List<ObjectId> GetTopContributors(Dictionary<ObjectId, int> contributions)
        {
            // Ordenar el diccionario de contribuciones por la cantidad contribuida (de mayor a menor)
            var sortedContributors = contributions.OrderByDescending(x => x.Value);

            int numberOfTopContributors = 1;
            var topContributors = sortedContributors.Take(numberOfTopContributors).Select(x => x.Key).ToList();

            return topContributors;
        }

        private static List<ObjectId> GetTopContributorsFromAnotherGroup(Dictionary<ObjectId, int> contributions, int numberOfTopContributors)
        {
            // Ordena el diccionario de contribuciones por la cantidad contribuida (de mayor a menor)
            var sortedContributors = contributions.OrderByDescending(x => x.Value);

            // Toma las cuentas de los principales contribuyentes
            var topContributors = sortedContributors.Take(numberOfTopContributors).Select(x => x.Key).ToList();

            return topContributors;
        }
        private static bool IsTopContributorInAnyGroup(ObjectId userId, List<Savings_Group> userSavingGroups)
        {
            foreach (var group in userSavingGroups)
            {
                List<ObjectId> topContributors = GetTopContributorsFromAnotherGroup(group.Contributions, 1);

                if (topContributors.Contains(userId))
                {
                    return true; // El usuario es un principal contribuyente en al menos un grupo
                }
            }

            return false; // El usuario no es un principal contribuyente en ninguno de los grupos
        }
    }
}
