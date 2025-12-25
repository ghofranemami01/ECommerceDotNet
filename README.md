# MEMIETTE - Site e-commerce de pâtisserie

Bienvenue sur **MEMIETTE**, un site web de vente en ligne d'articles de pâtisserie, développé par **Mami Ghofrane** avec ASP.NET.

---

## 🌟 Description du projet

MEMIETTE est une plateforme e-commerce permettant aux utilisateurs de :
- Explorer et acheter des produits de pâtisserie artisanale.
- Ajouter des articles au panier et gérer leurs commandes.
- Suivre le statut de leurs commandes.
- Profiter d'une interface moderne et responsive.

Le site intègre également un espace **administrateur** pour :
- Gérer les commandes (voir, modifier le statut, supprimer).
- Gérer les produits et les variantes.
- Suivre les stocks en temps réel.

---

## 🛠️ Technologies utilisées

- **Backend** : ASP.NET Core MVC
- **Frontend** : Bootstrap 5, HTML5, CSS3, JavaScript
- **Base de données** : SQL Server
- **Gestion des dépendances et versionnage** : Git & GitHub
- **Authentification** : Identity ASP.NET (Rôles Admin et Manager)

---

## 📂 Structure du projet

MEMIETTE/
├─ WebApplication2.sln
├─ WebApplication2/
│ ├─ Controllers/
│ │ └─ ProductController.cs
│ │ └─ OrderController.cs
│ ├─ Models/
│ │ └─ Product.cs
│ │ └─ Order.cs
│ ├─ Views/
│ │ └─ Product/
│ │ └─ Order/
│ └─ wwwroot/
│ └─ css/
│ └─ js/
│ └─ images/
└─ README.md    

---

## 📖 Fonctionnalités principales

### Pour les utilisateurs
- Parcourir les produits et leurs variantes.
- Ajouter, modifier ou supprimer des articles du panier.
- Passer une commande et suivre son statut.
- Interface responsive pour mobile et desktop.

### Pour les administrateurs / managers
- Visualiser la liste des commandes.
- Mettre à jour le statut des commandes (En attente, Expédiée, Livrée, Annulée).
- Supprimer des commandes et gérer le stock des produits.
- Gestion des produits et variantes.
