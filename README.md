# MediLabo Solutions - Système de Détection du Diabète de Type 2

## 📋 Table des matières
1. [Description du projet](#description)
2. [Architecture](#architecture)
3. [Technologies utilisées](#technologies)
4. [Prérequis](#prérequis)
5. [Installation et lancement](#installation)
6. [Accès à l'application](#accès)
7. [Comptes et données de test](#données-test)
8. [Structure du projet](#structure)
9. [Endpoints API](#endpoints)
10. [Green Code - Bonnes pratiques](#green-code)
11. [Améliorations futures](#améliorations)
12. [Auteur](#auteur)

---

## 📖 Description du projet

Application médicale développée pour **MediLabo Solutions** permettant d'identifier les patients à risque de diabète  de type 2.

### Objectif
Aider les médecins à détecter précocement les risques de diabète en analysant automatiquement les notes médicales et les informations des patients.

### Fonctionnalités
- ✅ Gestion des patients (CRUD)
- ✅ Gestion des notes médicales (CRUD)
- ✅ Évaluation automatique du risque diabète (4 niveaux)
- ✅ Authentification sécurisée (JWT)
- ✅ Architecture microservices conteneurisée

---

## 🏗️ Architecture

### Schéma global
<img width="4168" height="3684" alt="Architecture MediLabo" src="https://github.com/user-attachments/assets/5290b9b8-2a5a-48d5-8d82-62cf5729338f" />

---

## 🛠️ Technologies utilisées

- **Backend** : ASP.NET Core 9.0
- **Frontend** : ASP.NET Core MVC, Razor, Bootstrap
- **API Gateway** : Ocelot
- **Authentification** : ASP.NET Identity + JWT Bearer
- **Bases de données** : 
  - SQL Server (Entity Framework Core)
  - MongoDB (EF Core MongoDB Provider)
- **Conteneurisation** : Docker, Docker Compose
- **Langages** : C# 13, HTML, CSS, JavaScript

---

## 🚀 Installation et lancement

### 1. Cloner le repository

### 2. Lancer l'application avec Docker
Vous devriez voir 8 conteneurs en cours d'exécution.

---

## 🌐 Accès à l'application

### Interface utilisateur
**URL** : http://localhost:5002

### API Gateway
**URL** : http://localhost:5000

### Endpoints individuels (développement)
- Identity API : http://localhost:5003
- Patients API : http://localhost:5001
- Notes API : http://localhost:5004
- Assessments API : http://localhost:5005

---

## 👤 Compte de test

### Compte administrateur
- **Email** : admin@medilabo.com
- **Mot de passe** : Admin123!

---

## 🔌 Endpoints API

### Identity API (`/api/auth` et `/api/users`)
- `POST /api/auth/login` : Connexion (retourne JWT token)
- `GET /api/users` : Liste des utilisateurs
- `DELETE /api/users/{id}` : Supprimer un utilisateur

### Patients API (`/api/patients`)
- `GET /api/patients` : Liste des patients
- `GET /api/patients/{id}` : Détails d'un patient
- `POST /api/patients` : Créer un patient
- `PUT /api/patients/{id}` : Modifier un patient
- `DELETE /api/patients/{id}` : Supprimer un patient

### Notes API (`/api/notes`)
- `GET /api/notes/patient/{patientId}` : Notes d'un patient
- `POST /api/notes` : Créer une note
- `PUT /api/notes/{id}` : Modifier une note
- `DELETE /api/notes/{id}` : Supprimer une note

### Assessments API (`/api/assessments`)
- `GET /api/assessments/{patientId}` : Évaluation du risque diabète

**Note** : Toutes les APIs (sauf `/api/auth/login`) nécessitent un JWT token dans le header `Authorization: Bearer <token>`

---

## 🌱 Green Code - Bonnes pratiques

### Principes appliqués

#### ✅ Architecture & Conception
- **Microservices découplés** : Scalabilité ciblée
- **Principe YAGNI** : Aucune fonctionnalité superflue
- **Normalisation 3NF** : Bases de données optimisées

#### ✅ Docker
- **Multi-stage builds** : Images légères (SDK séparé du runtime)
- **HTTP entre conteneurs** : Pas de chiffrement inutile
- **Volumes persistants** : Conservation des données

#### ✅ Code .NET
- **Async/Await** : Libération des threads lors des I/O
- **Using statements** : Libération immédiate des ressources
- **Injection de dépendances** : Gestion optimisée du cycle de vie

#### ✅ Base de données
- **Entity Framework Core** : Requêtes optimisées (LINQ)
- **Connection pooling** : Réutilisation des connexions
- **Index sur colonnes recherchées** : Performances accrues

### Ressources
- [Green Software Foundation](https://greensoftware.foundation/)
- [Microsoft Sustainability](https://learn.microsoft.com/en-us/azure/architecture/framework/sustainability/)
- [Référentiel Green IT (CNUMR)](https://www.greenit.fr/)
