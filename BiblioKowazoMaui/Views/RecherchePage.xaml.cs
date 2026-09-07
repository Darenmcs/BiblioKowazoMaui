using Microsoft.EntityFrameworkCore;
using BiblioKowazoMaui.Data;
using BiblioKowazoMaui.Models;

namespace BiblioKowazoMaui.Views;

public partial class RecherchePage : ContentPage
{
    // Listes utilisées pour remplir les filtres (Pickers)
    private List<Auteur> _auteurs = new();
    private List<Editeur> _editeurs = new();
    private List<Categorie> _categories = new();

    // Constructeur
    public RecherchePage()
    {
        InitializeComponent();
    }

    // Quand la page s'affiche
    protected override void OnAppearing()
    {
        base.OnAppearing();

        using (var db = new BiblioContext())
        {
            // Charger les auteurs
            _auteurs = db.Auteurs.ToList();

            // Option "Tous"
            _auteurs.Insert(0, new Auteur { Id = 0, Nom = "(Tous)" });

            PickerAuteur.ItemsSource = _auteurs;
            PickerAuteur.ItemDisplayBinding = new Binding("Nom");
            PickerAuteur.SelectedIndex = 0;

            // Charger les éditeurs
            _editeurs = db.Editeurs.ToList();
            _editeurs.Insert(0, new Editeur { Id = 0, Nom = "(Tous)" });

            PickerEditeur.ItemsSource = _editeurs;
            PickerEditeur.ItemDisplayBinding = new Binding("Nom");
            PickerEditeur.SelectedIndex = 0;

            // Charger les catégories
            _categories = db.Categories.ToList();
            _categories.Insert(0, new Categorie { Id = 0, Nom = "(Toutes)" });

            PickerCategorie.ItemsSource = _categories;
            PickerCategorie.ItemDisplayBinding = new Binding("Nom");
            PickerCategorie.SelectedIndex = 0;
        }
    }

    // Bouton RECHERCHER
    private void BtnRechercher_Click(object s, EventArgs e)
    {
        using (var db = new BiblioContext())
        {
            // base de requête (LINQ)
            var query = db.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Editeur)
                .Include(l => l.LivreCategories)
                .AsQueryable();

            // filtre par titre
            if (!string.IsNullOrWhiteSpace(TxtTitre.Text))
                query = query.Where(l => l.Titre.Contains(TxtTitre.Text));

            // filtre auteur
            var auteur = PickerAuteur.SelectedItem as Auteur;
            if (auteur != null && auteur.Id != 0)
                query = query.Where(l => l.AuteurId == auteur.Id);

            // filtre éditeur
            var editeur = PickerEditeur.SelectedItem as Editeur;
            if (editeur != null && editeur.Id != 0)
                query = query.Where(l => l.EditeurId == editeur.Id);

            // filtre catégorie (relation N-N)
            var categorie = PickerCategorie.SelectedItem as Categorie;
            if (categorie != null && categorie.Id != 0)
                query = query.Where(l =>
                    l.LivreCategories.Any(lc => lc.CategorieId == categorie.Id));

            // filtre date début
            if (DpDateDebut.Date != DateTime.Today)
                query = query.Where(l => l.DatePublication >= DpDateDebut.Date);

            // filtre date fin
            if (DpDateFin.Date != DateTime.Today)
                query = query.Where(l => l.DatePublication <= DpDateFin.Date);

            // exécuter la requête
            var resultats = query.ToList();

            // afficher résultats
            cvResultats.ItemsSource = resultats;
            LblStatus.Text = $"{resultats.Count} livre(s) trouvé(s)";
        }
    }

    // Bouton RESET
    private void BtnReset_Click(object s, EventArgs e)
    {
        // vider tous les champs
        TxtTitre.Text = "";
        PickerAuteur.SelectedIndex = 0;
        PickerEditeur.SelectedIndex = 0;
        PickerCategorie.SelectedIndex = 0;

        DpDateDebut.Date = DateTime.Today;
        DpDateFin.Date = DateTime.Today;

        cvResultats.ItemsSource = null;
        LblStatus.Text = "Recherche réinitialisée";
    }
}