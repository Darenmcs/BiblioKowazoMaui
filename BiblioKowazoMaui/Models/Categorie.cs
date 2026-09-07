using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiblioKowazoMaui.Models
{
    // Classe Categorie = représente une catégorie de livres
    public class Categorie
    {
        // Clé primaire (identifiant unique)
        [Key]
        public int Id { get; set; }

        // Nom de la catégorie (obligatoire)
        [Required]

        // Longueur max = 100 caractères
        [StringLength(100)]
        public string Nom { get; set; } = "";

        // Description optionnelle (peut être vide)
        public string? Description { get; set; }

        // Relation avec LivreCategorie (table de liaison N-N)
        public virtual ICollection<LivreCategorie>? LivreCategories { get; set; }
    }
}