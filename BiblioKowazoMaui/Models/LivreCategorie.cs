namespace BiblioKowazoMaui.Models
{
    // Classe de liaison entre Livre et Categorie (relation N-N)
    public class LivreCategorie
    {
        // Clé étrangère vers Livre
        public int LivreId { get; set; }

        // Navigation vers le livre associé
        public virtual Livre? Livre { get; set; }

        // Clé étrangère vers Categorie
        public int CategorieId { get; set; }

        // Navigation vers la catégorie associée
        public virtual Categorie? Categorie { get; set; }
    }
}