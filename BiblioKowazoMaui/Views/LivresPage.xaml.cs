using Microsoft.EntityFrameworkCore;
using BiblioKowazoMaui.Data;
using BiblioKowazoMaui.Models;

namespace BiblioKowazoMaui.Views;

public partial class LivresPage : ContentPage
{
    // Livre sélectionné dans la liste
    private Livre? _selected;

    // Listes pour remplir les Pickers
    private List<Auteur> _auteurs = new();
    private List<Editeur> _editeurs = new();

    // Constructeur
    public LivresPage()
    {
        InitializeComponent();
    }

    // Quand la page apparaît
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Charger(); // recharge données
    }

    // Charger livres + auteurs + éditeurs
    private void Charger()
    {
        using (var db = new BiblioContext())
        {
            // Charger les livres avec Auteur et Editeur (Include = relations)
            cvLivres.ItemsSource =
                db.Livres
                  .Include(l => l.Auteur)
                  .Include(l => l.Editeur)
                  .ToList();

            // Charger auteurs pour Picker
            _auteurs = db.Auteurs.ToList();
            PickerAuteur.ItemsSource = _auteurs;
            PickerAuteur.ItemDisplayBinding = new Binding("Nom");

            // Charger éditeurs pour Picker
            _editeurs = db.Editeurs.ToList();
            PickerEditeur.ItemsSource = _editeurs;
            PickerEditeur.ItemDisplayBinding = new Binding("Nom");
        }
    }

    // Quand on sélectionne un livre
    private void Cv_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        _selected = e.CurrentSelection.FirstOrDefault() as Livre;

        if (_selected != null)
        {
            TxtTitre.Text = _selected.Titre;
            TxtISBN.Text = _selected.ISBN;
            TxtPrix.Text = _selected.Prix?.ToString();
            DpDate.Date = _selected.DatePublication ?? DateTime.Today;

            // sélectionner auteur et éditeur correspondants
            PickerAuteur.SelectedItem =
                _auteurs.FirstOrDefault(a => a.Id == _selected.AuteurId);

            PickerEditeur.SelectedItem =
                _editeurs.FirstOrDefault(e2 => e2.Id == _selected.EditeurId);
        }
    }

    // Ajouter un livre
    private async void BtnAjouter_Click(object s, EventArgs e)
    {
        // Vérifications des champs obligatoires
        if (string.IsNullOrWhiteSpace(TxtTitre.Text))
        { await DisplayAlert("Erreur", "Le titre est obligatoire", "OK"); return; }

        if (string.IsNullOrWhiteSpace(TxtISBN.Text))
        { await DisplayAlert("Erreur", "L'ISBN est obligatoire", "OK"); return; }

        if (TxtISBN.Text.Length < 10)
        { await DisplayAlert("Erreur", "ISBN invalide (min 10 caractères)", "OK"); return; }

        if (string.IsNullOrWhiteSpace(TxtPrix.Text))
        { await DisplayAlert("Erreur", "Le prix est obligatoire", "OK"); return; }

        // conversion prix
        if (!decimal.TryParse(TxtPrix.Text, out decimal prix))
        { await DisplayAlert("Erreur", "Prix invalide", "OK"); return; }

        if (prix < 0)
        { await DisplayAlert("Erreur", "Prix négatif interdit", "OK"); return; }

        // vérifier sélection auteur / éditeur
        var auteur = PickerAuteur.SelectedItem as Auteur;
        var editeur = PickerEditeur.SelectedItem as Editeur;

        if (auteur == null)
        { await DisplayAlert("Erreur", "Sélectionnez un auteur", "OK"); return; }

        if (editeur == null)
        { await DisplayAlert("Erreur", "Sélectionnez un éditeur", "OK"); return; }

        using (var db = new BiblioContext())
        {
            // vérifier ISBN unique
            if (db.Livres.Any(l => l.ISBN == TxtISBN.Text))
            {
                await DisplayAlert("Erreur", "ISBN déjà existant", "OK");
                return;
            }

            // ajouter livre
            db.Livres.Add(new Livre
            {
                Titre = TxtTitre.Text,
                ISBN = TxtISBN.Text,
                Prix = prix,
                DatePublication = DpDate.Date,
                AuteurId = auteur.Id,
                EditeurId = editeur.Id
            });

            db.SaveChanges();
        }

        Charger();
        Vider();
        LblStatus.Text = "Livre ajouté avec succès";
    }

    // Modifier un livre
    private async void BtnModifier_Click(object s, EventArgs e)
    {
        if (_selected == null)
        { await DisplayAlert("Erreur", "Sélectionnez un livre", "OK"); return; }

        // mêmes validations que ajout
        if (string.IsNullOrWhiteSpace(TxtTitre.Text))
        { await DisplayAlert("Erreur", "Titre obligatoire", "OK"); return; }

        if (!decimal.TryParse(TxtPrix.Text, out decimal prix))
        { await DisplayAlert("Erreur", "Prix invalide", "OK"); return; }

        var auteur = PickerAuteur.SelectedItem as Auteur;
        var editeur = PickerEditeur.SelectedItem as Editeur;

        if (auteur == null || editeur == null)
        {
            await DisplayAlert("Erreur", "Sélection auteur/éditeur obligatoire", "OK");
            return;
        }

        using (var db = new BiblioContext())
        {
            // vérifier ISBN unique sauf lui-même
            if (db.Livres.Any(l => l.ISBN == TxtISBN.Text && l.Id != _selected.Id))
            {
                await DisplayAlert("Erreur", "ISBN déjà utilisé", "OK");
                return;
            }

            var livre = db.Livres.Find(_selected.Id);

            if (livre != null)
            {
                livre.Titre = TxtTitre.Text;
                livre.ISBN = TxtISBN.Text;
                livre.Prix = prix;
                livre.DatePublication = DpDate.Date;
                livre.AuteurId = auteur.Id;
                livre.EditeurId = editeur.Id;

                db.SaveChanges();
            }
        }

        Charger();
        LblStatus.Text = "Livre modifié avec succès";
    }

    // Supprimer un livre
    private async void BtnSupprimer_Click(object s, EventArgs e)
    {
        if (_selected == null) return;

        using (var db = new BiblioContext())
        {
            // supprimer relations N-N
            db.LivreCategories.RemoveRange(
                db.LivreCategories.Where(lc => lc.LivreId == _selected.Id));

            // supprimer fiche détail
            var fiche = db.FicheDetails
                          .FirstOrDefault(f => f.LivreId == _selected.Id);

            if (fiche != null)
                db.FicheDetails.Remove(fiche);

            // supprimer livre
            var livre = db.Livres.Find(_selected.Id);
            db.Livres.Remove(livre);

            db.SaveChanges();
        }

        Charger();
        Vider();
        LblStatus.Text = "Livre supprimé";
    }

    // vider formulaire
    private void Vider()
    {
        TxtTitre.Text = "";
        TxtISBN.Text = "";
        TxtPrix.Text = "";

        PickerAuteur.SelectedIndex = -1;
        PickerEditeur.SelectedIndex = -1;

        _selected = null;
        cvLivres.SelectedItem = null;
    }
}