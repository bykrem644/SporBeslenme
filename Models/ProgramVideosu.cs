namespace SporBeslenmeWeb.Models
{
    public class ProgramVideosu
    {
        public int Id { get; set; }

        // Hangi programa ait olduğunu belirten bağlantı (Foreign Key)
        public int AntrenmanProgramiId { get; set; }
        public AntrenmanProgrami AntrenmanProgrami { get; set; } = null!;

        public string VideoBaslik { get; set; }=string.Empty;    
        public string Aciklama { get; set; } = string.Empty; 
            public string VideoYolu { get; set; } = string.Empty;    
        public int Sira { get; set; }
    }
}