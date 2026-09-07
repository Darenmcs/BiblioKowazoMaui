using Microsoft.EntityFrameworkCore;
using BiblioKowazoMaui.Models;

namespace BiblioKowazoMaui.Data
{
    // Classe principale pour gérer la base de données
    public class BiblioContext : DbContext
    {
        // Constructeur vide
        public BiblioContext() { }

        // Constructeur avec options (souvent utilisé avec injection de dépendance)
        public BiblioContext(DbContextOptions<BiblioContext> options)
            : base(options) { }

        // Tables de la base de données
        public DbSet<Auteur> Auteurs { get; set; }
        public DbSet<Editeur> Editeurs { get; set; }
        public DbSet<Livre> Livres { get; set; }
        public DbSet<FicheDetail> FicheDetails { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<LivreCategorie> LivreCategories { get; set; }

        // Configuration de la connexion à SQL Server
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(
                @"Server=DINA-QUATRE;Database=BiblioKowazoMauiDb;Trusted_Connection=True;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relation 1-1 : un Livre a une seule FicheDetail
            modelBuilder.Entity<Livre>()
                .HasOne(l => l.FicheDetail)
                .WithOne(f => f.Livre)
                .HasForeignKey<FicheDetail>(f => f.LivreId)
                .OnDelete(DeleteBehavior.Cascade); // si Livre supprimé => FicheDetail aussi

            // Relation 1-N : un Auteur peut avoir plusieurs Livres
            modelBuilder.Entity<Livre>()
                .HasOne(l => l.Auteur)
                .WithMany(a => a.Livres)
                .HasForeignKey(l => l.AuteurId)
                .OnDelete(DeleteBehavior.Cascade); // suppression en cascade

            // Relation 1-N : un Editeur peut avoir plusieurs Livres
            modelBuilder.Entity<Livre>()
                .HasOne(l => l.Editeur)
                .WithMany(e => e.Livres)
                .HasForeignKey(l => l.EditeurId)
                .OnDelete(DeleteBehavior.Restrict); // interdit de supprimer si utilisé

            // Relation N-N avec table intermédiaire LivreCategorie
            modelBuilder.Entity<LivreCategorie>()
                .HasKey(lc => new { lc.LivreId, lc.CategorieId }); // clé composite

            modelBuilder.Entity<LivreCategorie>()
                .HasOne(lc => lc.Livre)
                .WithMany(l => l.LivreCategories)
                .HasForeignKey(lc => lc.LivreId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LivreCategorie>()
                .HasOne(lc => lc.Categorie)
                .WithMany(c => c.LivreCategories)
                .HasForeignKey(lc => lc.CategorieId)
                .OnDelete(DeleteBehavior.Restrict);

            // ISBN doit être unique (pas de doublon)
            modelBuilder.Entity<Livre>()
                .HasIndex(l => l.ISBN)
                .IsUnique();

            // Ajouter des données de départ
            SeedData(modelBuilder);
        }

        // Méthode pour insérer des données au début (seed)
        private static void SeedData(ModelBuilder mb)
        {
            // Auteurs
            mb.Entity<Auteur>().HasData(
                new Auteur { Id = 1, Nom = "ZOHOU", Prenom = "Louis Stephane" },
                new Auteur { Id = 2, Nom = "MAHAD", Prenom = "Wais" },
                new Auteur { Id = 3, Nom = "KOUASSI", Prenom = "Konan Jeannot" });

            // Editeurs
            mb.Entity<Editeur>().HasData(
                new Editeur { Id = 1, Nom = "Lacite", Adresse = "Abidjan" },
                new Editeur { Id = 2, Nom = "Uottawa", Adresse = "Djibouti" });

            // Categories
            mb.Entity<Categorie>().HasData(
                new Categorie { Id = 1, Nom = "Roman", Description = "Romans littéraires" },
                new Categorie { Id = 2, Nom = "Poésie", Description = "Recueils de poèmes" },
                new Categorie { Id = 3, Nom = "Philosophie", Description = "Essais philosophiques" });

            // Livres
            mb.Entity<Livre>().HasData(
                new Livre
                {
                    Id = 1,
                    Titre = "L'Aventurier des Mers",
                    ISBN = "978-2-07-040850-4",
                    Prix = 12.50m,
                    DatePublication = new DateTime(2015, 1, 1),
                    AuteurId = 1,
                    EditeurId = 1
                },
                new Livre
                {
                    Id = 2,
                    Titre = "L'Informatique pour les Nuls",
                    ISBN = "978-2-07-036024-6",
                    Prix = 9.90m,
                    DatePublication = new DateTime(2018, 1, 1),
                    AuteurId = 2,
                    EditeurId = 1
                },
                new Livre
                {
                    Id = 3,
                    Titre = "Le Millionnaire en Dollars",
                    ISBN = "978-2-07-036002-4",
                    Prix = 6.90m,
                    DatePublication = new DateTime(2020, 1, 1),
                    AuteurId = 3,
                    EditeurId = 2
                });

            // Liaison Livre - Categorie
            mb.Entity<LivreCategorie>().HasData(
                new LivreCategorie { LivreId = 1, CategorieId = 1 },
                new LivreCategorie { LivreId = 2, CategorieId = 1 },
                new LivreCategorie { LivreId = 3, CategorieId = 1 },
                new LivreCategorie { LivreId = 3, CategorieId = 3 });
        }
    }
}