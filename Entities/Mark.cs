namespace StudentManagementApi.Entities {
    public class Mark {
        public int Id { get; set; }
        public Guid StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
