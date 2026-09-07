using BiblioKowazoMaui.Data;
using BiblioKowazoMaui.Models;

namespace BiblioKowazoMaui.Views;

public partial class AuteursPage : ContentPage
{
    // Auteur actuellement sélectionné dans la liste
    private Auteur? _selected;

    // Constructeur de la page
    public AuteursPage()
    {
        InitializeComponent();
    }

    // Appelé quand la page apparaît à l’écran
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Charger(); // recharge la liste des auteurs
    }

    // Charger les auteurs depuis la base de données
    private void Charger()
    {
        using (var db = new BiblioContext())
        {
            cvAuteurs.ItemsSource = db.Auteurs.ToList();
        }
    }

    // Quand on sélectionne un auteur dans la liste
    private void CvAuteurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // récupérer l’auteur sélectionné
        _selected = e.CurrentSelection.FirstOrDefault() as Auteur;

        // afficher ses données dans les champs
        if (_selected != null)
        {
            TxtNom.Text = _selected.Nom;
            TxtPrenom.Text = _selected.Prenom;
        }
    }

    // Ajouter un auteur
    private async void BtnAjouter_Click(object sender, EventArgs e)
    {
        // Vérifier si le nom est vide
        if (string.IsNullOrWhiteSpace(TxtNom.Text))
        {
            await DisplayAlert("Erreur", "Le nom est obligatoire", "OK");
            return;
        }

        using (var db = new BiblioContext())
        {
            // Vérifier si l’auteur existe déjà
            if (db.Auteurs.Any(a => a.Nom == TxtNom.Text && a.Prenom == TxtPrenom.Text))
            {
                await DisplayAlert("Erreur", "Cet auteur existe déjà", "OK");
                return;
            }

            // Ajouter nouvel auteur
            db.Auteurs.Add(new Auteur
            {
                Nom = TxtNom.Text,
                Prenom = TxtPrenom.Text
            });

            db.SaveChanges();
        }

        Charger(); // recharger la liste
        Vider();   // vider les champs
        LblStatus.Text = "Auteur ajouté";
    }

    // Modifier un auteur
    private async void BtnModifier_Click(object sender, EventArgs e)
    {
        // Vérifier si un auteur est sélectionné
        if (_selected == null)
        {
            await DisplayAlert("Erreur", "Sélectionnez un auteur", "OK");
            return;
        }

        using (var db = new BiblioContext())
        {
            // retrouver l’auteur dans la base
            var auteur = db.Auteurs.Find(_selected.Id);

            // modifier ses données
            auteur.Nom = TxtNom.Text;
            auteur.Prenom = TxtPrenom.Text;

            db.SaveChanges();
        }

        Charger();
        LblStatus.Text = "Auteur modifié";
    }

    // Supprimer un auteur
    private async void BtnSupprimer_Click(object sender, EventArgs e)
    {
        if (_selected == null) return;

        // confirmation utilisateur
        bool ok = await DisplayAlert("Confirmation",
            "Supprimer cet auteur et tous ses livres ?", "Oui", "Non");

        if (ok)
        {
            using (var db = new BiblioContext())
            {
                // récupérer tous les livres de l’auteur
                var livres = db.Livres.Where(l => l.AuteurId == _selected.Id).ToList();

                // supprimer les dépendances de chaque livre
                foreach (var livre in livres)
                {
                    db.LivreCategories.RemoveRange(
                        db.LivreCategories.Where(lc => lc.LivreId == livre.Id));

                    var fiche = db.FicheDetails.FirstOrDefault(f => f.LivreId == livre.Id);
                    if (fiche != null)
                        db.FicheDetails.Remove(fiche);
                }

                // supprimer les livres
                db.Livres.RemoveRange(livres);

                // supprimer l’auteur
                var auteur = db.Auteurs.Find(_selected.Id);
                db.Auteurs.Remove(auteur);

                db.SaveChanges();
            }

            Charger();
            Vider();
            LblStatus.Text = "Auteur supprimé";
        }
    }

    // Vider les champs du formulaire
    private void Vider()
    {
        TxtNom.Text = "";
        TxtPrenom.Text = "";

        _selected = null;
        cvAuteurs.SelectedItem = null;
    }
}