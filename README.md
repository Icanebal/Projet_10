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

### Microservices
- **MediLabo.Identity.API** : Authentification et gestion des utilisateurs
- **MediLabo.Patients.API** : Gestion des patients (SQL Server)
- **MediLabo.Notes.API** : Gestion des notes médicales (MongoDB)
- **MediLabo.Assessments.API** : Évaluation du risque diabète
- **MediLabo.Gateway** : API Gateway (Ocelot)
- **MediLabo.Web** : Interface utilisateur (ASP.NET Core MVC)

### Bases de données
- **SQL Server** : Patients, Identity
- **MongoDB** : Notes médicales (NoSQL)

---

## 🛠️ Technologies utilisées

- **Backend** : ASP.NET Core 9.0
- **Frontend** : ASP.NET Core MVC, Bootstrap
- **API Gateway** : Ocelot
- **Authentification** : ASP.NET Identity + JWT
- **Bases de données** : 
  - SQL Server (Entity Framework Core)
  - MongoDB (EF Core MongoDB Provider)
- **Conteneurisation** : Docker, Docker Compose
- **Langages** : C# 13, HTML, CSS, JavaScript

---

## ✅ Prérequis

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (version 4.0+)
- [.NET SDK 9.0](https://dotnet.microsoft.com/download) (pour développement local uniquement)
- 8 GB RAM minimum
- 10 GB espace disque

---

## 🚀 Installation et lancement

### 1. Cloner le repository
```bash
git clone https://github.com/ton-username/MediLabo.git
cd MediLabo
```

### 2. Lancer l'application avec Docker
```bash
docker-compose up --build
```

**Temps de démarrage** : 5-10 minutes (première fois)

### 3. Vérifier que tous les conteneurs sont démarrés
```bash
docker ps
```

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

### Bases de données
- **SQL Server** : `localhost:1433` (user: sa, password: YourStrong@Passw0rd)
- **MongoDB** : `localhost:27018`

---

## 👤 Comptes et données de test

### Compte administrateur
- **Email** : admin@medilabo.com
- **Mot de passe** : Admin123!

### Patients de test

| ID | Nom | Prénom | Date de naissance | Risque attendu |
|----|-----|--------|-------------------|----------------|
| 1 | TestNone | Test | 31/12/1966 | Aucun |
| 2 | TestBorderline | Test | 24/06/1945 | Risque limité |
| 3 | TestInDanger | Test | 18/06/2004 | Danger |
| 4 | TestEarlyOnset | Test | 28/06/2002 | Apparition précoce |

### Niveaux de risque

1. **Aucun** : 0-1 déclencheur
2. **Risque limité** : 2-5 déclencheurs + âge > 30 ans
3. **Danger** : 3-7 déclencheurs selon âge/sexe
4. **Apparition précoce** : 5+ déclencheurs (H<30) / 7+ (F<30) / 8+ (>30)

**Termes déclencheurs** : Hémoglobine A1C, Microalbumine, Taille, Poids, Fumeur, Anormal, Cholestérol, Vertiges, Rechute, Réaction, Anticorps

---

## 📁 Structure du projet
```
MediLabo/
├── MediLabo.sln
├── docker-compose.yml
├── README.md
├── MediLabo.Common/              # Classes partagées
├── MediLabo.Identity.API/         # Authentification
│   ├── Dockerfile
│   └── appsettings.Docker.json
├── MediLabo.Patients.API/         # Gestion patients
│   ├── Dockerfile
│   └── appsettings.Docker.json
├── MediLabo.Notes.API/            # Gestion notes
│   ├── Dockerfile
│   └── appsettings.Docker.json
├── MediLabo.Assessments.API/      # Évaluation risque
│   ├── Dockerfile
│   └── appsettings.Docker.json
├── MediLabo.Gateway/              # API Gateway (Ocelot)
│   ├── Dockerfile
│   ├── ocelot.json
│   └── ocelot.Docker.json
└── MediLabo.Web/                  # Frontend MVC
    ├── Dockerfile
    └── appsettings.Docker.json
```

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

### Recommandations d'amélioration

#### 🔧 Images Docker
```dockerfile
# Passer de debian à alpine
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
# Réduction : ~200 MB → ~40 MB
```

#### 🔧 Limites ressources
```yaml
services:
  patients-api:
    deploy:
      resources:
        limits:
          cpus: '0.5'
          memory: 512M
```

#### 🔧 Healthcheck optimisé
```yaml
healthcheck:
  interval: 30s  # Au lieu de 10s
```

#### 🔧 Logs en production
```json
"LogLevel": {
    "Default": "Warning"  // Au lieu de Information
}
```

#### 🔧 Compression HTTP
```csharp
builder.Services.AddResponseCompression();
```

### Impact environnemental

**Estimation de réduction** (avec améliorations) :
- **Taille images** : -60% (alpine)
- **Mémoire** : -30% (limites ressources)
- **CPU** : -20% (healthcheck, logs)
- **Réseau** : -50% (compression)

### Ressources
- [Green Software Foundation](https://greensoftware.foundation/)
- [Microsoft Sustainability](https://learn.microsoft.com/en-us/azure/architecture/framework/sustainability/)
- [Référentiel Green IT (CNUMR)](https://www.greenit.fr/)

---

## 🔮 Améliorations futures

- [ ] Pagination des listes (patients, notes)
- [ ] Export PDF des rapports d'évaluation
- [ ] Notifications email (risque élevé détecté)
- [ ] Tableau de bord statistiques
- [ ] Tests d'intégration automatisés
- [ ] CI/CD avec GitHub Actions
- [ ] Déploiement Azure Container Instances

---

## 👨‍💻 Auteur

**[Ton Nom]**
- Formation : Développeur d'application C# .NET
- Date : Novembre 2025
- Contact : [ton-email@example.com]

---

## 📄 Licence

Ce projet est développé dans le cadre d'une formation OpenClassrooms.

---

**🎉 Merci d'avoir consulté ce projet !**
