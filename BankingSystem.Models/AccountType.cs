namespace BankingSystem.Models
{
    public class AccountType
    {
        public int AccountTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumBalance { get; set; }
    }
}

