using BiblioKowazoMaui.Data;
using BiblioKowazoMaui.Models;

namespace BiblioKowazoMaui.Views;

public partial class EditeursPage : ContentPage
{
    // Éditeur actuellement sélectionné dans la liste
    private Editeur? _selected;

    // Constructeur de la page
    public EditeursPage()
    {
        InitializeComponent();
    }

    // Quand la page apparaît à l’écran
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Charger(); // recharge la liste des éditeurs
    }

    // Charger les éditeurs depuis la base
    private void Charger()
    {
        using (var db = new BiblioContext())
            cvEditeurs.ItemsSource = db.Editeurs.ToList();
    }

    // Quand on sélectionne un éditeur dans la liste
    private void Cv_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        _selected = e.CurrentSelection.FirstOrDefault() as Editeur;

        // Remplir les champs avec les données sélectionnées
        if (_selected != null)
        {
            TxtNom.Text = _selected.Nom;
            TxtAdresse.Text = _selected.Adresse;
        }
    }

    // Ajouter un éditeur
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
            // Ajouter nouvel éditeur
            db.Editeurs.Add(new Editeur
            {
                Nom = TxtNom.Text,
                Adresse = TxtAdresse.Text
            });

            db.SaveChanges();
        }

        Charger(); // refresh liste

        // vider champs
        TxtNom.Text = "";
        TxtAdresse.Text = "";

        LblStatus.Text = "Éditeur ajouté";
    }

    // Modifier un éditeur
    private async void BtnModifier_Click(object s, EventArgs e)
    {
        // Vérifier sélection
        if (_selected == null)
        {
            await DisplayAlert("Erreur", "Sélectionnez un éditeur", "OK");
            return;
        }

        using (var db = new BiblioContext())
        {
            // retrouver éditeur en base
            var ed = db.Editeurs.Find(_selected.Id);

            // modifier données
            ed.Nom = TxtNom.Text;
            ed.Adresse = TxtAdresse.Text;

            db.SaveChanges();
        }

        Charger();
        LblStatus.Text = "Éditeur modifié";
    }

    // Supprimer un éditeur
    private async void BtnSupprimer_Click(object s, EventArgs e)
    {
        if (_selected == null) return;

        using (var db = new BiblioContext())
        {
            // vérifier si des livres existent avec cet éditeur
            if (db.Livres.Any(l => l.EditeurId == _selected.Id))
            {
                await DisplayAlert("Erreur",
                    "Impossible : des livres sont associés à cet éditeur",
                    "OK");
                return;
            }

            // supprimer éditeur
            var ed = db.Editeurs.Find(_selected.Id);
            db.Editeurs.Remove(ed);

            db.SaveChanges();
        }

        Charger();

        // vider interface
        TxtNom.Text = "";
        TxtAdresse.Text = "";
        _selected = null;

        LblStatus.Text = "Éditeur supprimé";
    }
}