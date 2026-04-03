TP .NET - Gestionnaire de Livres

Voici mon rendu pour le projet de dévelopement desktop en C#. L'objectif était de faire une application de gestion de bibliothèque avec WPF tout en essayant de bien respecter l'architecture MVVM.

Fonctionalités terminées (Parties 1 et 2)

Je me suis surtout concentré sur les deux premières parties pour m'assurer d'avoir une base propre et qui marche.

    L'interface WPF est complètement liée au ViewModel, il n'y a pas de code de logique dans la vue.
    L'affichage dans le DataGrid et les petites statistiques (total des livres, lus, et le pourcentage) se mettent à jour tout seuls.
    La base de données SQLite fonctionne pour l'ajout, la modification et la suppression. J'ai d'ailleurs passé pas mal de temps bloqué sur la modification à cause d'un DataGrid qui ne voulait pas séléctionner les lignes correctement, mais c'est réglé.

Partie 3 (MAUI) et Bonus

Pour être honnête je n'ai pas réussis la partie 3 sur MAUI. 
J'ai essayé de configurer le projet mais j'ai eu plein d'erreurs de compilation incompréhensibles liées au SDK Windows et à des manifestes MSIX introuvables. J'ai perdu beaucoup de temps à essayer de réparer la build... j'ai finis par abandonner cette partie pour fignoler mon WPF.

Par manque de temps, je n'ai pas non plus fait les bonus à la fin du sujet.

Comment tester le projet

    Ouvrez la solution avec Visual Studio.
    Vérifiez que le projet WPF est bien définit comme projet de démarrage par défaut.
    Lancez l'application. La base de données SQLite (livres.db) va se créer toute seule dans le dossier d'exécution au premier lancement.

Raphaël
