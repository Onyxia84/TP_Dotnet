using TPdotnetWpf.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;

namespace TPdotnetWpf.Data
{
    public class LivreRepository
    {
        
        private readonly string _connectionString;

        public LivreRepository()
        {
            var dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "livres.db");
            _connectionString = $"Data Source={dbPath}";
            InitialiserBase();
        }

        
        public void InitialiserBase()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS livres (
                    id     INTEGER PRIMARY KEY AUTOINCREMENT,
                    titre  TEXT    NOT NULL,
                    auteur TEXT    NOT NULL,
                    annee  INTEGER,
                    genre  TEXT    NOT NULL DEFAULT 'Autre',
                    lu     INTEGER NOT NULL DEFAULT 0
                );";
            cmd.ExecuteNonQuery();
        }

        
        public List<Livre> GetAll()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, titre, auteur, annee, genre, lu FROM livres ORDER BY id;";
            return LireLivres(cmd);
        }

        public List<Livre> GetByRecherche(string terme)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, titre, auteur, annee, genre, lu
                FROM   livres
                WHERE  LOWER(titre)  LIKE @terme
                    OR LOWER(auteur) LIKE @terme
                ORDER BY id;";
            cmd.Parameters.AddWithValue("@terme", $"%{terme.ToLower()}%");
            return LireLivres(cmd);
        }

        
        public void Add(Livre livre)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO livres (titre, auteur, annee, genre, lu)
                VALUES (@titre, @auteur, @annee, @genre, @lu);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@titre",  livre.Titre);
            cmd.Parameters.AddWithValue("@auteur", livre.Auteur);
            cmd.Parameters.AddWithValue("@annee",  livre.Annee);
            cmd.Parameters.AddWithValue("@genre",  livre.Genre);
            cmd.Parameters.AddWithValue("@lu",     livre.Lu ? 1 : 0);
            livre.Id = (int)(long)cmd.ExecuteScalar()!;
        }

        
        public void Update(Livre livre)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE livres
                SET    titre  = @titre,
                       auteur = @auteur,
                       annee  = @annee,
                       genre  = @genre,
                       lu     = @lu
                WHERE  id = @id;";
            cmd.Parameters.AddWithValue("@titre",  livre.Titre);
            cmd.Parameters.AddWithValue("@auteur", livre.Auteur);
            cmd.Parameters.AddWithValue("@annee",  livre.Annee);
            cmd.Parameters.AddWithValue("@genre",  livre.Genre);
            cmd.Parameters.AddWithValue("@lu",     livre.Lu ? 1 : 0);
            cmd.Parameters.AddWithValue("@id",     livre.Id);
            cmd.ExecuteNonQuery();
        }

        
        public void Delete(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM livres WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        
        private static List<Livre> LireLivres(SqliteCommand cmd)
        {
            var liste = new List<Livre>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Livre
                {
                    Id     = reader.GetInt32(0),
                    Titre  = reader.GetString(1),
                    Auteur = reader.GetString(2),
                    Annee  = reader.GetInt32(3),
                    Genre  = reader.GetString(4),
                    Lu     = reader.GetInt32(5) == 1
                });
            }
            return liste;
        }
    }
}
