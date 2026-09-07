# BiblioKowazo — Version MAUI

## Application de Gestion de Bibliothèque

**Technologies :**

* .NET MAUI (Multi-platform App UI)
* Entity Framework Core
* SQL Server

---

# 1. Diagramme des Classes

L'application est composée de **6 entités** avec 4 types de relations.

### Entités

* Auteur
* Editeur
* Livre
* FicheDetail
* Categorie
* LivreCategorie

---

# 2. Les Relations et Cardinalités

| Relation            | Cardinalité | Signification                      | OnDelete |
| ------------------- | ----------- | ---------------------------------- | -------- |
| Auteur → Livre      | 1 — N       | Un auteur écrit plusieurs livres   | CASCADE  |
| Editeur → Livre     | 1 — N       | Un éditeur publie plusieurs livres | RESTRICT |
| Livre ↔ FicheDetail | 1 — 1       | Un livre a une seule fiche détail  | CASCADE  |
| Livre ↔ Categorie   | N — M       | Via table LivreCategorie           | —        |

---

## Détails des relations

### Auteur → Livre (1-N)

* **CASCADE** : Supprimer un auteur supprime tous ses livres.

### Editeur → Livre (1-N)

* **RESTRICT** : Impossible de supprimer un éditeur s'il a des livres.

### Livre ↔ FicheDetail (1-1)

* **CASCADE** : Supprimer un livre supprime sa fiche.

### Livre ↔ Categorie (N-M)

* Table de jonction **LivreCategorie** avec clé composite.

---

# 3. Architecture de l'Application MAUI

## 3.1 Structure du projet

```
📁 BiblioKowazoMaui/
├── 📁 Data/
│   └── BiblioContext.cs
├── 📁 Models/
│   ├── Auteur.cs
│   ├── Editeur.cs
│   ├── Livre.cs
│   ├── FicheDetail.cs
│   ├── Categorie.cs
│   └── LivreCategorie.cs
├── 📁 Views/
│   ├── MainTabbedPage.xaml / .cs
│   ├── AuteursPage.xaml / .cs
│   ├── EditeursPage.xaml / .cs
│   ├── CategoriesPage.xaml / .cs
│   ├── LivresPage.xaml / .cs
│   └── RecherchePage.xaml / .cs
├── App.xaml / .cs
├── AppShell.xaml / .cs
└── MauiProgram.cs
```

## 3.2 Navigation

L'application utilise un **TabbedPage** comme conteneur principal. Chaque onglet est une **ContentPage** indépendante avec son propre fichier XAML et code-behind.

| Composant WPF            | Équivalent MAUI utilisé   |
| ------------------------ | ------------------------- |
| MetroWindow              | TabbedPage + ContentPages |
| TabControl               | TabbedPage                |
| DataGrid                 | CollectionView            |
| TextBox                  | Entry                     |
| TextBlock                | Label                     |
| ComboBox                 | Picker                    |
| StackPanel               | VerticalStackLayout       |
| GroupBox                  | Frame                     |
| StatusBar                | Label (en bas de page)    |
| MessageBox.Show()        | DisplayAlert()            |

---

# 4. Captures d'Écran de l'Application

## 4.1 Onglet Auteurs

Écran auteurs (./Medias/Ecran_auteurs.png)

## 4.2 Onglet Éditeurs

Écran éditeur (./Medias/Ecran_editeurs.png)

## 4.3 Onglet Catégories

Écran des catégories (./Medias/Ecran_categories.png)

## 4.4 Onglet Livres

Écran liste des livres avec Auteur/Éditeur (./Medias/Ecran_livres.png)

## 4.5 Onglet Recherche

Écran recherche multicritère avec résultats (./Medias/Ecran_recherches.png)

## 4.6 Message de confirmation

Écran message de confirmation CRUD via DisplayAlert (./Medias/Ecran_message_CRUD.png)

---

# 5. Fonctionnalités CRUD avec Intégrité des Données

## 5.1 Opérations CRUD

Pour chaque entité (Auteur, Livre, Catégorie, Éditeur), l'application permet :

* **Créer** — Ajouter un nouvel élément
* **Afficher** — Visualiser les données dans une CollectionView
* **Modifier** — Mettre à jour un élément sélectionné
* **Supprimer** — Retirer un élément avec gestion des dépendances

## 5.2 Exigences d'intégrité des données

### Vérification avant AJOUT

* Aucun doublon ne doit être inséré.
* L'application vérifie que l'élément n'existe pas déjà.
* Tous les champs obligatoires doivent être remplis.
* Pour les Livres : le titre, l'ISBN (minimum 10 caractères), le prix (nombre positif), la date, l'auteur et l'éditeur sont tous obligatoires.
* L'ISBN doit être unique dans la base.

### Vérification avant SUPPRESSION

* L'élément doit être sélectionné dans la CollectionView.
* Les dépendances sont gérées correctement :
   * Supprimer un **Auteur** → supprime d'abord tous ses Livres (CASCADE).
   * Supprimer une **Catégorie** → supprime les liens LivreCategorie associés.
   * Supprimer un **Éditeur** → bloqué si des Livres sont associés (RESTRICT).
   * Supprimer un **Livre** → supprime sa FicheDetail et ses LivreCategories.

### Vérification avant MODIFICATION

* L'élément doit être sélectionné.
* Les nouvelles valeurs doivent respecter les contraintes (champs obligatoires, ISBN unique en excluant l'élément en cours).

### Après chaque opération CRUD

* Recharger les données dans la CollectionView
* Afficher un message clair dans le Label de statut

---

# 6. Explication du Fonctionnement de la Recherche Multicritère

## 6.1 Présentation

La fonctionnalité de recherche multicritère permet à l'utilisateur de filtrer les livres de la bibliothèque selon plusieurs critères combinés. Les critères sont appliqués simultanément pour affiner les résultats.

---

## 6.2 Critères de recherche disponibles

| Critère    | Contrôle MAUI | Type de filtre | Description                                                               |
|------------|---------------|----------------|---------------------------------------------------------------------------|
| Titre      | Entry         | Partiel        | Recherche les livres dont le titre **contient** le texte saisi            |
| Auteur     | Picker        | Exact          | Filtre par auteur sélectionné (option "(Tous)" = pas de filtre)           |
| Éditeur    | Picker        | Exact          | Filtre par éditeur sélectionné (option "(Tous)" = pas de filtre)          |
| Catégorie  | Picker        | Relation N-M   | Filtre les livres appartenant à la catégorie via la table `LivreCategorie`|
| Date début | DatePicker    | Intervalle     | Livres publiés **à partir de** cette date                                 |
| Date fin   | DatePicker    | Intervalle     | Livres publiés **jusqu'à** cette date                                     |

---

## 6.3 Logique de recherche

### Principe : Filtres cumulatifs (ET)

Tous les critères renseignés sont combinés avec l'opérateur logique **ET**. Si un critère n'est pas renseigné, il est ignoré.

**Exemple :** Si l'utilisateur sélectionne Auteur = "ZOHOU" et Catégorie = "Roman", la recherche retourne uniquement les livres qui sont **à la fois** écrits par ZOHOU **et** dans la catégorie Roman.

### Implémentation technique

La recherche utilise `AsQueryable()` d'Entity Framework Core et ajoute des `.Where()` conditionnels :

```csharp
var query = db.Livres
    .Include(l => l.Auteur)
    .Include(l => l.Editeur)
    .Include(l => l.LivreCategories)
    .AsQueryable();

// Chaque filtre est ajouté seulement si le critère est renseigné
if (!string.IsNullOrWhiteSpace(titre))
    query = query.Where(l => l.Titre.Contains(titre));

if (auteur != null && auteur.Id != 0)
    query = query.Where(l => l.AuteurId == auteur.Id);

// Filtre N-M avec Any()
if (categorie != null && categorie.Id != 0)
    query = query.Where(l => l.LivreCategories.Any(lc => lc.CategorieId == categorie.Id));
```

---

## 6.4 Exemples de recherches

| Recherche souhaitée                      | Critères utilisés                                | Résultat attendu                          |
|------------------------------------------|--------------------------------------------------|-------------------------------------------|
| Livres de Louis Stephane Zohou           | Auteur = "ZOHOU"                                 | Tous les livres de cet auteur             |
| Romans publiés après 2015               | Catégorie = "Roman" + Date début = 01/01/2015    | Romans à partir de 2015                   |
| Livres Lacite contenant "Aventurier"     | Éditeur = "Lacite" + Titre = "Aventurier"        | Livres Lacite avec "Aventurier" dans le titre |
| Livres de philosophie                    | Catégorie = "Philosophie"                        | Livres classés en philosophie             |

---

## 6.5 Affichage des résultats

- Les résultats sont affichés dans une **CollectionView** avec un DataTemplate
- Le **nombre total** de livres trouvés est affiché dans le **Label de statut**
- Un bouton **Réinitialiser** permet d'effacer tous les critères et vider les résultats

---

# 7. Données de Test (Seed Data)

## Auteurs
| Id | Nom     | Prénom          |
|----|---------|-----------------|
| 1  | ZOHOU   | Louis Stephane  |
| 2  | MAHAD   | Wais            |
| 3  | KOUASSI | Konan Jeannot   |

## Éditeurs
| Id | Nom      | Adresse  |
|----|----------|----------|
| 1  | Lacite   | Abidjan  |
| 2  | Uottawa  | Djibouti |

## Catégories
| Id | Nom         | Description           |
|----|-------------|-----------------------|
| 1  | Roman       | Romans littéraires    |
| 2  | Poésie      | Recueils de poèmes    |
| 3  | Philosophie | Essais philosophiques |

## Livres
| Id | Titre                        | ISBN               | Prix  | Auteur  | Éditeur  |
|----|------------------------------|--------------------|-------|---------|----------|
| 1  | L'Aventurier des Mers        | 978-2-07-040850-4  | 12.50 | ZOHOU   | Lacite   |
| 2  | L'Informatique pour les Nuls | 978-2-07-036024-6  | 9.90  | MAHAD   | Lacite   |
| 3  | Le Millionnaire en Dollars   | 978-2-07-036002-4  | 6.90  | KOUASSI | Uottawa  |

---

# 8. Packages NuGet Requis

```
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
```

---

## Documentation — BiblioKowazo MAUI
