namespace QuestBook_AdminPanel_SzM
{
    public class UserModel
    {
        public UserModel(Guid id, string name, int rep, string gmail, string role, int warnings, bool isSuspended)
        {
            Id = id;
            Name = name;
            Rep = rep;
            Gmail = gmail;
            Role = role;
            Warnings = warnings;
            IsSuspended = isSuspended;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Rep { get; set; }
        public string Gmail { get; set; }
        public string Role { get; set; }
        public int Warnings { get; set; }
        public bool IsSuspended { get; set; }

    }
}