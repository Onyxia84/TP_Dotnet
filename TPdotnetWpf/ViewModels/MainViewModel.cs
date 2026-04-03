using TPdotnetWpf.Data;
using TPdotnetWpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace TPdotnetWpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly LivreRepository _repository = new();

        public ObservableCollection<Livre> Livres { get; } = new();

        private Livre? _livreSelectionne;
        public Livre? LivreSelectionne
        {
            get => _livreSelectionne;
            set
            {
                SetProperty(ref _livreSelectionne, value);
                if (value != null) ChargerDansFormulaire(value);
            }
        }

        private string _titre = "";
        public string Titre
        {
            get => _titre;
            set => SetProperty(ref _titre, value);
        }

        private string _auteur = "";
        public string Auteur
        {
            get => _auteur;
            set => SetProperty(ref _auteur, value);
        }

        private string _anneeTexte = "";
        public string AnneeTexte
        {
            get => _anneeTexte;
            set => SetProperty(ref _anneeTexte, value);
        }

        private string _genre = "Autre";
        public string Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        private bool _lu;
        public bool Lu
        {
            get => _lu;
            set => SetProperty(ref _lu, value);
        }

        private string _termeRecherche = "";
        public string TermeRecherche
        {
            get => _termeRecherche;
            set
            {
                SetProperty(ref _termeRecherche, value);
                Rechercher();
            }
        }

        private int _totalLivres;
        public int TotalLivres
        {
            get => _totalLivres;
            private set => SetProperty(ref _totalLivres, value);
        }

        private int _livresLus;
        public int LivresLus
        {
            get => _livresLus;
            private set => SetProperty(ref _livresLus, value);
        }

        private double _pourcentageLus;
        public double PourcentageLus
        {
            get => _pourcentageLus;
            private set => SetProperty(ref _pourcentageLus, value);
        }

        public List<string> Genres { get; } =
            new() { "Roman", "SF", "Fantasy", "Policier", "Autre" };

        public ICommand AjouterCommand { get; }
        public ICommand ModifierCommand { get; }
        public ICommand SupprimerCommand { get; }
        public ICommand ViderCommand { get; }
        public ICommand ExporterCsvCommand { get; }

        public MainViewModel()
        {
            AjouterCommand    = new RelayCommand(_ => Ajouter(),   _ => PeutAjouter());
            ModifierCommand   = new RelayCommand(_ => Modifier(),  _ => PeutModifier());
            SupprimerCommand  = new RelayCommand(_ => Supprimer(), _ => LivreSelectionne != null);
            ViderCommand      = new RelayCommand(_ => ViderFormulaire());
            ExporterCsvCommand = new RelayCommand(_ => ExporterCsv());

            ChargerTout();
        }

        private void ChargerTout()
        {
            var livres = string.IsNullOrWhiteSpace(_termeRecherche)
                ? _repository.GetAll()
                : _repository.GetByRecherche(_termeRecherche);

            Livres.Clear();
            foreach (var l in livres)
                Livres.Add(l);

            RecalculerStatistiques();
        }

        private void Rechercher()
        {
            var resultats = string.IsNullOrWhiteSpace(_termeRecherche)
                ? _repository.GetAll()
                : _repository.GetByRecherche(_termeRecherche);

            Livres.Clear();
            foreach (var l in resultats)
                Livres.Add(l);

            RecalculerStatistiques();
        }

        private void Ajouter()
        {
            if (!Valider(out var erreurs))
            {
                AfficherErreurs(erreurs);
                return;
            }

            var livre = new Livre
            {
                Titre  = Titre.Trim(),
                Auteur = Auteur.Trim(),
                Annee  = int.Parse(AnneeTexte.Trim()),
                Genre  = Genre,
                Lu     = Lu
            };

            _repository.Add(livre);
            ChargerTout();
            ViderFormulaire();
        }

        private void Modifier()
        {
            if (LivreSelectionne == null) return;

            if (!Valider(out var erreurs))
            {
                AfficherErreurs(erreurs);
                return;
            }

            LivreSelectionne.Titre  = Titre.Trim();
            LivreSelectionne.Auteur = Auteur.Trim();
            LivreSelectionne.Annee  = int.Parse(AnneeTexte.Trim());
            LivreSelectionne.Genre  = Genre;
            LivreSelectionne.Lu     = Lu;

            _repository.Update(LivreSelectionne);
            ChargerTout();
            ViderFormulaire();
        }

        private void Supprimer()
        {
            if (LivreSelectionne == null) return;

            var result = MessageBox.Show(
                $"Voulez-vous vraiment supprimer :\n\n« {LivreSelectionne.Titre} » de {LivreSelectionne.Auteur} ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _repository.Delete(LivreSelectionne.Id);
                ChargerTout();
                ViderFormulaire();
            }
        }

        private void ExporterCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Titre;Auteur;Annee;Genre;Lu");

            foreach (var l in _repository.GetAll())
                sb.AppendLine($"{l.Titre};{l.Auteur};{l.Annee};{l.Genre};{l.Lu}");

            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "livres.csv");

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

            MessageBox.Show($"Export réussi !\n\nFichier : {path}",
                "Export CSV", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void ChargerDansFormulaire(Livre l)
        {
            Titre      = l.Titre;
            Auteur     = l.Auteur;
            AnneeTexte = l.Annee.ToString();
            Genre      = l.Genre;
            Lu         = l.Lu;
        }

        private void ViderFormulaire()
        {
            Titre             = "";
            Auteur            = "";
            AnneeTexte        = "";
            Genre             = "Autre";
            Lu                = false;
            _livreSelectionne = null;
            OnPropertyChanged(nameof(LivreSelectionne));
        }

        private void RecalculerStatistiques()
        {
            // On compte sur la totalité de la BDD, pas seulement le filtre affiché
            var tous = _repository.GetAll();
            TotalLivres    = tous.Count;
            LivresLus      = tous.Count(l => l.Lu);
            PourcentageLus = TotalLivres == 0
                ? 0
                : Math.Round((double)LivresLus / TotalLivres * 100, 1);
        }

        private bool Valider(out List<string> erreurs)
        {
            erreurs = new List<string>();

            if (Titre.Trim().Length < 2)
                erreurs.Add("• Titre : minimum 2 caractères.");

            if (Auteur.Trim().Length < 2)
                erreurs.Add("• Auteur : minimum 2 caractères.");

            if (!int.TryParse(AnneeTexte.Trim(), out int a)
                || a < 1800 || a > DateTime.Now.Year)
                erreurs.Add($"• Année : entier entre 1800 et {DateTime.Now.Year}.");

            if (string.IsNullOrWhiteSpace(Genre))
                erreurs.Add("• Genre : veuillez sélectionner un genre.");

            return erreurs.Count == 0;
        }

        private static void AfficherErreurs(List<string> erreurs) =>
            MessageBox.Show(
                "Veuillez corriger les erreurs suivantes :\n\n" + string.Join("\n", erreurs),
                "Champs invalides",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

        private bool PeutAjouter()  => LivreSelectionne == null;
        private bool PeutModifier() => LivreSelectionne != null;
    }
}
