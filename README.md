# Managem'Ants  ![ManagemAntsLogo](https://i.imgur.com/3RatAgp.png)

![forthebadge](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![forthebadge](https://img.shields.io/badge/HTML-239120?style=for-the-badge&logo=html5&logoColor=white)
![forthebadge](https://img.shields.io/badge/CSS-239120?&style=for-the-badge&logo=css3&logoColor=white)
![forthebadge](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black)


##### Managem'Ants est un site web de gestion projet.
Il permet de créer des projets, d'ajouter des collaborateurs à ces projets et d'assigner des tâches à ces collaborateurs.
On peut également y retrouver des statistiques détaillées sur chaque projet afin de visualiser leur avancement ou d'aider à leur gestion et à la répartition des tâches entre les différents collaborateurs.
##### Le site est accéssible [ici](https://managemantsclient.azurewebsites.net/).


## Sommaire

 - [Lancer le projet en local](#lancer-le-projet-en-local)
 - [Lancer la testsuite](#lancer-la-testsuite)
 - [Déploiement Azure](#déploiement-azure)
 - [Auteurs](#auteurs)
 - [License](#license)

# Lancer le projet en local

#### Ouvrir la solution dans visual studio
#### Créer la base de données locale
- Créer une nouvelle base de donnée locale appelée ``ManagemAntsDb``
#### Ajout des tables dans la base de données
- Clique droit sur le projet ``ManagemAntsDatabase`` de la solution > ``Comparaison de schémas...``
- Mettre la base de données locale dans la cible
- Comparer et mettre à jour
#### Relier le serveur locale à la base de donées locale
- Dans le fichier ``appsettings.json`` du projet ``ManagemAntsServer`` : Changer la ``connexionString`` afin qu'elle corresponde à la base de données locale
#### Relier le client au serveur
- S'assurer que la variable ``Url`` dans le fichier ``Utils/Client.cs`` du projet ``ManagemAntsClient`` est ègale à ``https://localhost:44352/api/``

#### Vous pouvez maintenant lancer le serveur et le client

# Lancer la testsuite
- Ouvrir la solution dans Visual studio
- Clic droit sur le projet ``ManagemAntsTest`` > ``Executer les tests``
_(Inutile de lancer le serveur ou le client pour lancer les tests)_

# Déploiement Azure

### Base de données
Afin de pouvoir déployer le serveur et le client sur Azure nous avons tout d'abord créé une base de données sur notre portail Azure puis nous avons fait une migration du schema de notre base de données SSMS dans celle d'Azure grâce à l'outil ``Microsoft Data Migration Assistant``.

#### Pour se faire, il faut ouvrir l'utilitaire de migration puis :
- Créer un nouveau projet de migration
- Choisir ``Migration`` dans ``Project type``
- Choisir le scope de migration puis appuyer sur __Create__
#### Une fois dans le projet de migration :
- Choisir le nom du serveur de la base de données SSMS et la méthode d'authentification
- Ajouter l'option ``Trust server certificate`` et appuyer sur __Connect__
- Choisir la database qui va être migrer
- Choisir le serveur de la base de données Azure _(cette information peut être obtenue sur le portail Azure dans la vue d'ensemble de la base de la donnée)_
- Choisir ensuite ``SQL Server Authentication`` dans le type d'authentification
- Entrer le ``username`` et le ``password`` administrateur de la base de données
- Ajouter l'option ``Trust server certificate`` dans les propriétés de connexion
- Choisir ensuite la base de données Azure dans les bases de données proposées et valider sur __Next__
- Séléctionner ensuite les tables qui doivent être migrées puis valider en appuyant sur __Generate SQL script__
- Appuyer sur __Deploy schema__ pour valider le processus.

Une fois la migration effectuée, nous pouvons récupérer la _connection string_ dans Azure dans la vue d'ensemble, dans le champs ``Chaînes de connexion``. Une fois la _connection string_ récupérée il faut la mettre dans le fichier ``appsettings.json`` du projet ``ManagemantsServeur`` et le mettre dans le champs ``ConnectionStrings.ManagemAnts``.

### Serveur
#### Pour ce qui est du serveur, nous avons créé une App Services sur Azure : 
- Créer une App Services dans le portail Azure
#### On arrive sur la page de création :
- Préciser un Groupe de ressources (en créer un si c'est la première fois)
- Choisir un nom pour l'instance d'App Services
- Modifier la pile d'execution en choisissant ``.NET 5.0``
- Choisir la région ``West Europe``
- Appuyer sur __Vérifier + Créer__ puis __Créer__.

On peut désormais publier notre projet grâce à Visual Studio 2019 en faisant comme suit :
Clic droit sur le projet ``ManagemantsServer`` -> Publier -> Cible Azure -> Azure App Service (Windows) -> Choisir l'App Service créée plus tôt -> Ignorer l'étape de Gestion d'API -> Publier (fichier XML)  -> __Publier__.

### Client
Pour le client, c'est exactement pareil que le serveur. En effet, il faut créer une ``app service`` sur Azure puis publier directement sur visual studio. Il faut au préalable avoir changé l'url pour accèder à l'API en mettant celle donnée par Azure pour le serveur (dans la vue d'ensemble du serveur). Cette modification est à faire dans le projet ``ManagemantsClient`` dans le fichier ``Utils/Client.cs``

# Auteurs
* **Louis Le Gatt** _alias_ [@jetsPepito](https://github.com/jetsPepito)
* **Corentin Le Guennec** _alias_ [@Guennator](https://github.com/Guennator)
* **Jérémie Zeitoun** _alias_ [@J3remiez](https://github.com/J3remiez)
* **Wassim Ajili** _alias_ [@Userleef](https://github.com/Userleef)

# License
Ceci est un projet réalisé dans le cadre de la majeure MTI à l'EPITA.
