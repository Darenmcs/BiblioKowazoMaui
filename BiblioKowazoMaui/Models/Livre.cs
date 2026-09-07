using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioKowazoMaui.Models
{
    // Classe Livre = élément principal (le cœur du projet)
    public class Livre
    {
        // Clé primaire
        [Key]
        public int Id { get; set; }

        // Titre du livre (obligatoire, max 200 caractères)
        [Required]
        [StringLength(200)]
        public string Titre { get; set; } = "";

        // ISBN (optionnel, max 20 caractères)
        public string? ISBN { get; set; }

        // Date de publication (optionnelle)
        public DateTime? DatePublication { get; set; }

        // Prix du livre (optionnel)
        public decimal? Prix { get; set; }

        // Clé étrangère vers Auteur
        [ForeignKey("Auteur")]
        public int AuteurId { get; set; }

        // Navigation vers l'auteur (relation 1-N)
        public virtual Auteur? Auteur { get; set; }

        // Clé étrangère vers Editeur
        [ForeignKey("Editeur")]
        public int EditeurId { get; set; }

        // Navigation vers l'éditeur (relation 1-N)
        public virtual Editeur? Editeur { get; set; }

        // Relation 1-1 avec FicheDetail
        public virtual FicheDetail? FicheDetail { get; set; }

        // Relation N-N avec Categorie via table intermédiaire
        public virtual ICollection<LivreCategorie>? LivreCategories { get; set; }
    }
}