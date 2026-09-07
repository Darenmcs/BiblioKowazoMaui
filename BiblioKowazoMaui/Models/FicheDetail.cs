using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioKowazoMaui.Models
{
    // Classe FicheDetail = informations supplémentaires d’un livre
    public class FicheDetail
    {
        // Clé primaire
        [Key]
        public int Id { get; set; }

        // Résumé du livre (optionnel)
        public string? Resume { get; set; }

        // Nombre de pages (optionnel)
        public int? NombrePages { get; set; }

        // Langue du livre (max 50 caractères)
        [StringLength(50)]
        public string? Langue { get; set; }

        // Clé étrangère vers Livre
        [ForeignKey("Livre")]
        public int LivreId { get; set; }

        // Navigation vers le livre associé (relation 1-1)
        public virtual Livre? Livre { get; set; }
    }
}