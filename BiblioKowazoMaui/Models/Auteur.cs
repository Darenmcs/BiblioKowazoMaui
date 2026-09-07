namespace BiblioKowazoMaui.Models
{
    // Classe Auteur = représente un auteur dans la base de données
    public class Auteur
    {
        // Identifiant unique de l'auteur (clé primaire)
        public int Id { get; set; }

        // Nom de l'auteur (obligatoire, vide par défaut)
        public string Nom { get; set; } = "";

        // Prénom de l'auteur (optionnel grâce au ?)
        public string? Prenom { get; set; }

        // Liste des livres écrits par cet auteur (relation 1-N)
        public virtual ICollection<Livre>? Livres { get; set; }
    }
}