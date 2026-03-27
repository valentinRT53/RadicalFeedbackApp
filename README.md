RADICALFEEDBACK
Application de gestion des experts et des retours utilisateurs
WinUI 3  •  C#  •  MVVM  •  MySQL

Présentation
RadicalFeedbackApp est une application de bureau développée en WinUI 3 avec le pattern MVVM. Elle permet aux experts et administrateurs de gérer les avis, signalements, disponibilités et abonnements liés à la plateforme RadicalFeedback.
L'application est connectée à une base de données MySQL et propose une interface moderne dark/rouge avec un système de rôles complet.

Stack technique
Technologie	Usage
WinUI 3	Framework UI natif Windows
C# / .NET	Langage et runtime
MVVM (CommunityToolkit)	Pattern architectural
MySQL + MySql.Data	Base de données et connecteur
Bebas Neue / DM Sans	Typographie de l'interface


Architecture MVVM
Le projet respecte strictement le pattern MVVM avec 4 couches distinctes :

Couche	Fichiers	Rôle
Model	Models/*.cs	Miroir des tables BDD
Service	Data/*Service.cs	Requêtes SQL et accès BDD
ViewModel	ViewModels/*.cs	Logique métier et état UI
View	Views/*.xaml	Affichage et binding

Flux d'une donnée
BDD → Service → ViewModel (ObservableCollection) → View (Binding XAML)

Structure du projet
Dossier	Contenu
Data/	Database.cs, ConnexionService.cs, *Service.cs
Helpers/	Session.cs (gestion utilisateur connecté)
Models/	Utilisateur, Avis, Signalement, Abonnement, Specialisation...
ViewModels/	Un ViewModel par entité principale
Views/	Pages XAML + LoginWindow + MainWindow


Système de rôles
L'application gère 3 rôles avec des accès différenciés :

Rôle	Accès	Pages visibles
Admin	Complet	Toutes les pages
Expert	Filtré sur ses données	Avis, Signalements, Agenda
Utilisateur	Site Symfony uniquement	Aucune (hors appli)

Session.cs
La session est gérée via une classe statique accessible partout dans l'application sans injection de dépendances.
Session.IdUtilisateur  →  ID de l'utilisateur connecté
Session.Role           →  Rôle (Admin / Expert)
Session.EstAdmin       →  true si Admin
Session.EstExpert      →  true si Expert

Fonctionnalités
Gestion des utilisateurs (Admin)
    • Liste avec recherche, tri et filtre par rôle
    • Ajouter un utilisateur avec login/mdp
    • Modifier les informations et spécialisations (experts)
    • Réinitialiser le mot de passe avec double vérification
    • Supprimer un utilisateur avec confirmation
Avis
    • Vue expert : note moyenne + liste des avis reçus
    • Vue admin : menu déroulant pour voir les avis par expert
    • Affichage du nom de l'utilisateur ayant laissé l'avis
Signalements
    • Signalement d'un expert ou d'une conversation
    • Filtres par expert et par catégorie
    • Panel détail avec affichage de la conversation si applicable
    • Suppression depuis la vue admin
Agenda
    • Grille hebdomadaire par créneaux horaires (9h-18h)
    • Expert : coche/décoche ses disponibilités
    • Admin : menu déroulant pour voir l'agenda de chaque expert
    • Données sauvegardées en BDD en temps réel
Abonnements (Admin)
    • Liste des abonnements avec nombre d'abonnés
    • Voir les utilisateurs liés à chaque abonnement
    • Ajouter et supprimer des abonnements
Spécialisations (Admin)
    • Page dédiée à la gestion des spécialisations
    • Ajouter, modifier, supprimer une spécialisation
    • Assigner plusieurs spécialisations à un expert

Base de données
Table	Description
UTILISATEUR	Tous les profils (admin, expert, utilisateur)
CONNEXION	Login/mdp des admins et experts
ROLE / OBTENIR	Gestion des rôles
AVIS	Avis laissés par les utilisateurs sur les experts
SIGNALEMENT	Signalements d'experts ou de conversations
DISPONIBILITE	Créneaux horaires des experts
ABONNEMENT	Plans tarifaires
SPECIALISATION	Domaines d'expertise
EXPERT_SPECIALISATION	Liaison experts ↔ spécialisations
CONVERSATION / MESSAGE	Gérés côté Symfony


Installation
Prérequis
    • Visual Studio 2022 avec workload WinUI / Windows App SDK
    • MySQL Server en local
    • .NET 6 ou supérieur
Configuration
Dans Data/Database.cs, renseigne ta chaîne de connexion :
server=localhost;database=radical_feedback;user=root;password=;
Comptes de test
Login	Mot de passe	Rôle
admin	admin123	Admin
alice	password1	Expert
bob	password2	Expert
carla	password3	Expert


Projet scolaire — RadicalFeedbackApp
WinUI 3 • C# • MVVM • MySQL
