using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioKowazoMaui.Models
{
    // Cette classe correspond à la table "Editeurs" dans la base
    [Table("Editeurs")]
    public class Editeur
    {
        // Clé primaire (identifiant unique)
        [Key]
        public int Id { get; set; }

        // Nom de l'éditeur (obligatoire)
        [Required]

        // Max 100 caractères
        [StringLength(100)]
        public string Nom { get; set; } = "";

        // Adresse (optionnelle, max 200 caractères)
        [StringLength(200)]
        public string? Adresse { get; set; }

        // Liste des livres publiés par cet éditeur (relation 1-N)
        public virtual ICollection<Livre>? Livres { get; set; }
    }
}