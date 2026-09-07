using BiblioKowazoMaui.Data;
using BiblioKowazoMaui.Models;

namespace BiblioKowazoMaui.Views;

public partial class CategoriesPage : ContentPage
{
    // Catégorie actuellement sélectionnée dans la liste
    private Categorie? _selected;

    // Constructeur de la page
    public CategoriesPage()
    {
        InitializeComponent();
    }

    // Quand la page apparaît à l’écran
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Charger(); // recharge la liste
    }

    // Charger les catégories depuis la base
    private void Charger()
    {
        using (var db = new BiblioContext())
            cvCategories.ItemsSource = db.Categories.ToList();
    }

    // Quand on sélectionne une catégorie dans la liste
    private void Cv_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        _selected = e.CurrentSelection.FirstOrDefault() as Categorie;

        // Remplir les champs avec les données sélectionnées
        if (_selected != null)
        {
            TxtNom.Text = _selected.Nom;
            TxtDescription.Text = _selected.Description;
        }
    }

    // Ajouter une catégorie
    private async void BtnAjouter_Click(object s, EventArgs e)
    {
        // Vérifier si le nom est vide
        if (string.IsNullOrWhiteSpace(TxtNom.Text))
        {
            await DisplayAlert("Erreur", "Le nom est obligatoire", "OK");
            return;
        }

        using (var db = new BiblioContext())
        {
            // Ajouter la nouvelle catégorie
            db.Categories.Add(new Categorie
            {
                Nom = TxtNom.Text,
                Description = TxtDescription.Text
            });

            db.SaveChanges();
        }

        Charger(); // rafraîchir la liste

        // vider les champs
        TxtNom.Text = "";
        TxtDescription.Text = "";

        LblStatus.Text = "Catégorie ajoutée";
    }

    // Modifier une catégorie
    private async void BtnModifier_Click(object s, EventArgs e)
    {
        // Vérifier sélection
        if (_selected == null) return;

        using (var db = new BiblioContext())
        {
            // retrouver catégorie en base
            var c = db.Categories.Find(_selected.Id);

            // modifier les champs
            c.Nom = TxtNom.Text;
            c.Description = TxtDescription.Text;

            db.SaveChanges();
        }

        Charger();
        LblStatus.Text = "Catégorie modifiée";
    }

    // Supprimer une catégorie
    private async void BtnSupprimer_Click(object s, EventArgs e)
    {
        if (_selected == null) return;

        using (var db = new BiblioContext())
        {
            // supprimer les liens LivreCategorie (relation N-N)
            db.LivreCategories.RemoveRange(
                db.LivreCategories.Where(lc => lc.CategorieId == _selected.Id));

            // supprimer la catégorie
            var c = db.Categories.Find(_selected.Id);
            db.Categories.Remove(c);

            db.SaveChanges();
        }

        Charger();

        // vider interface
        TxtNom.Text = "";
        TxtDescription.Text = "";
        _selected = null;

        LblStatus.Text = "Catégorie supprimée";
    }
}