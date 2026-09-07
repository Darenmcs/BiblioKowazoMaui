using BiblioKowazoMaui.Data;
using BiblioKowazoMaui.Views;

namespace BiblioKowazoMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Créer la DB + tables + seed data au démarrage
            using (var context = new BiblioContext())
            {
                context.Database.EnsureCreated();
            }

            // Page principale = TabbedPage avec 5 onglets
            MainPage = new MainTabbedPage();
        }
    }
}