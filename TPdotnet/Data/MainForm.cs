using TPdotnet.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TPdotnet.Data
{
    public class MainForm : Form
    {
        
        private TextBox txtTitre = null!;
        private TextBox txtAuteur = null!;
        private TextBox txtAnnee = null!;
        private ComboBox cmbGenre = null!;
        private CheckBox chkLu = null!;
        private Button btnAjouter = null!;
        private Button btnModifier = null!;
        private Button btnSupprimer = null!;
        private ListBox lstLivres = null!;
        private Label lblTitre = null!, lblAuteur = null!, lblAnnee = null!,
                      lblGenre = null!, lblTitle = null!;

        
        private readonly List<Livre> _livres = new();
        private int _nextId = 1;
        private Livre? _livreEnEdition = null;

        
        public MainForm()
        {
            InitialiserInterface();
            ChargerDonneesDemo();
        }

        private void InitialiserInterface()
        {
            
            Text = "Gestionnaire de Livres — WinForms";
            Size = new Size(860, 560);
            MinimumSize = new Size(860, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 245, 250);
            Font = new Font("Segoe UI", 9.5f);

            
            var pnlForm = new Panel
            {
                Location = new Point(16, 16),
                Size = new Size(360, 480),
                BackColor = Color.White,
                Padding = new Padding(16)
            };
            pnlForm.Paint += PanelPaint_RoundedBorder;

            lblTitle = new Label
            {
                Text = "Ajouter / Modifier un livre",
                Location = new Point(16, 16),
                Size = new Size(328, 28),
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 80)
            };

            
            lblTitre = CreateLabel("Titre *", 16, 58);
            txtTitre = CreateTextBox(16, 80);

            
            lblAuteur = CreateLabel("Auteur *", 16, 118);
            txtAuteur = CreateTextBox(16, 140);

           
            lblAnnee = CreateLabel("Année *", 16, 178);
            txtAnnee = CreateTextBox(16, 200);

            
            lblGenre = CreateLabel("Genre *", 16, 238);
            cmbGenre = new ComboBox
            {
                Location = new Point(16, 260),
                Size = new Size(328, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f)
            };
            cmbGenre.Items.AddRange(new object[] { "Roman", "SF", "Fantasy", "Policier", "Autre" });

            
            chkLu = new CheckBox
            {
                Text = "Lu ?",
                Location = new Point(16, 304),
                Size = new Size(100, 24),
                Font = new Font("Segoe UI", 9.5f)
            };

            
            btnAjouter = CreateButton("Ajouter", 16, 348, Color.FromArgb(34, 197, 94));
            btnModifier = CreateButton("Modifier", 176, 348, Color.FromArgb(59, 130, 246));
            btnModifier.Enabled = false;

            btnAjouter.Click += BtnAjouter_Click;
            btnModifier.Click += BtnModifier_Click;

            pnlForm.Controls.AddRange(new Control[]
            {
                lblTitle, lblTitre, txtTitre,
                lblAuteur, txtAuteur,
                lblAnnee, txtAnnee,
                lblGenre, cmbGenre,
                chkLu,
                btnAjouter, btnModifier
            });

            // ── Panel liste (droite) ──────────────────────────────────
            var pnlListe = new Panel
            {
                Location = new Point(396, 16),
                Size = new Size(440, 480),
                BackColor = Color.White
            };
            pnlListe.Paint += PanelPaint_RoundedBorder;

            var lblListeTitle = new Label
            {
                Text = "Bibliothèque",
                Location = new Point(16, 16),
                Size = new Size(300, 28),
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 80)
            };

            lstLivres = new ListBox
            {
                Location = new Point(16, 52),
                Size = new Size(408, 380),
                Font = new Font("Segoe UI", 9.5f),
                BorderStyle = BorderStyle.None,
                ItemHeight = 24
            };
            lstLivres.MouseDoubleClick += LstLivres_DoubleClick;
            lstLivres.SelectedIndexChanged += LstLivres_SelectedIndexChanged;

            btnSupprimer = CreateButton("Supprimer", 16, 440, Color.FromArgb(239, 68, 68));
            btnSupprimer.Enabled = false;
            btnSupprimer.Click += BtnSupprimer_Click;

            pnlListe.Controls.AddRange(new Control[]
            {
                lblListeTitle, lstLivres, btnSupprimer
            });

            Controls.AddRange(new Control[] { pnlForm, pnlListe });
        }

        
        private static Label CreateLabel(string text, int x, int y) =>
            new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(328, 20),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 80, 120)
            };

        private static TextBox CreateTextBox(int x, int y) =>
            new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(328, 28),
                Font = new Font("Segoe UI", 9.5f),
                BorderStyle = BorderStyle.FixedSingle
            };

        private static Button CreateButton(string text, int x, int y, Color color) =>
            new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(152, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

        private static void PanelPaint_RoundedBorder(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel p) return;
            using var pen = new Pen(Color.FromArgb(220, 220, 235), 1.5f);
            e.Graphics.DrawRectangle(pen, 1, 1, p.Width - 2, p.Height - 2);
        }

       
        private void ChargerDonneesDemo()
        {
            AjouterLivre(new Livre { Titre = "Le Seigneur des Anneaux", Auteur = "Tolkien", Annee = 1954, Genre = "Fantasy", Lu = true });
            AjouterLivre(new Livre { Titre = "1984", Auteur = "Orwell", Annee = 1949, Genre = "SF", Lu = false });
            AjouterLivre(new Livre { Titre = "Germinal", Auteur = "Zola", Annee = 1885, Genre = "Roman", Lu = true });
            AjouterLivre(new Livre { Titre = "Sherlock Holmes", Auteur = "Conan Doyle", Annee = 1892, Genre = "Policier", Lu = false });
        }

        
        private bool Valider(out List<string> erreurs)
        {
            erreurs = new List<string>();

            if (txtTitre.Text.Trim().Length < 2)
                erreurs.Add("• Titre : minimum 2 caractères non vides.");

            if (txtAuteur.Text.Trim().Length < 2)
                erreurs.Add("• Auteur : minimum 2 caractères non vides.");

            if (!int.TryParse(txtAnnee.Text.Trim(), out int annee)
                || annee < 1800
                || annee > DateTime.Now.Year)
                erreurs.Add($"• Année : entier entre 1800 et {DateTime.Now.Year}.");

            if (cmbGenre.SelectedIndex < 0)
                erreurs.Add("• Genre : veuillez sélectionner un genre.");

            return erreurs.Count == 0;
        }

        
        private void BtnAjouter_Click(object? sender, EventArgs e)
        {
            if (!Valider(out var erreurs))
            {
                MessageBox.Show(
                    "Veuillez corriger les erreurs suivantes :\n\n" + string.Join("\n", erreurs),
                    "Champs invalides",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var livre = new Livre
            {
                Titre = txtTitre.Text.Trim(),
                Auteur = txtAuteur.Text.Trim(),
                Annee = int.Parse(txtAnnee.Text.Trim()),
                Genre = cmbGenre.SelectedItem!.ToString()!,
                Lu = chkLu.Checked
            };

            AjouterLivre(livre);
            ViderFormulaire();
        }

        private void BtnModifier_Click(object? sender, EventArgs e)
        {
            if (_livreEnEdition == null) return;

            if (!Valider(out var erreurs))
            {
                MessageBox.Show(
                    "Veuillez corriger les erreurs suivantes :\n\n" + string.Join("\n", erreurs),
                    "Champs invalides",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _livreEnEdition.Titre = txtTitre.Text.Trim();
            _livreEnEdition.Auteur = txtAuteur.Text.Trim();
            _livreEnEdition.Annee = int.Parse(txtAnnee.Text.Trim());
            _livreEnEdition.Genre = cmbGenre.SelectedItem!.ToString()!;
            _livreEnEdition.Lu = chkLu.Checked;

            RafraichirListe();
            ViderFormulaire();

            MessageBox.Show("Livre modifié avec succès !", "Succès",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSupprimer_Click(object? sender, EventArgs e)
        {
            if (lstLivres.SelectedItem is not Livre livre) return;

            var result = MessageBox.Show(
                $"Voulez-vous vraiment supprimer :\n\n« {livre.Titre} » de {livre.Auteur} ?",
                "Confirmation de suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _livres.Remove(livre);
                RafraichirListe();
                ViderFormulaire();
            }
        }

        
        private void LstLivres_DoubleClick(object? sender, MouseEventArgs e)
        {
            if (lstLivres.SelectedItem is not Livre livre) return;

            _livreEnEdition = livre;

            txtTitre.Text = livre.Titre;
            txtAuteur.Text = livre.Auteur;
            txtAnnee.Text = livre.Annee.ToString();
            cmbGenre.SelectedItem = livre.Genre;
            chkLu.Checked = livre.Lu;

            btnAjouter.Enabled = false;
            btnModifier.Enabled = true;

            
            Text = $"Gestionnaire de Livres — Édition : {livre.Titre}";
        }

        private void LstLivres_SelectedIndexChanged(object? sender, EventArgs e)
        {
            btnSupprimer.Enabled = lstLivres.SelectedIndex >= 0;
        }

        
        private void AjouterLivre(Livre livre)
        {
            livre.Id = _nextId++;
            _livres.Add(livre);
            lstLivres.Items.Add(livre);
        }

        private void RafraichirListe()
        {
            lstLivres.Items.Clear();
            foreach (var l in _livres)
                lstLivres.Items.Add(l);
        }

        private void ViderFormulaire()
        {
            txtTitre.Clear();
            txtAuteur.Clear();
            txtAnnee.Clear();
            cmbGenre.SelectedIndex = -1;
            chkLu.Checked = false;
            _livreEnEdition = null;
            btnAjouter.Enabled = true;
            btnModifier.Enabled = false;
            btnSupprimer.Enabled = false;
            Text = "Gestionnaire de Livres — WinForms";
        }
    }
}
