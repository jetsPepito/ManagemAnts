# README

# Déploiement

## Base de donnée
Afin de pouvoir déployer le serveur et le client sur Azure nous avons tout d'abord créer une base de données sur notre portail Azure puis nous avons fais une migration du schema de notre base de données SSMS dans celle d'Azure grâce à la l'outil "Microsoft Data Migration Assistant".

Pour se faire, il faut ouvrir l'utilitaire de migration, créer un nouveau projet de migration, Choisir _Migration_ dans Project type et choisir le scope de migration puis appuyer sur __Create__. Une fois dans le projet de migration, il faut choisir le nom du serveur de la base de données SSMS et la méthode d'authentification puis ajouter l'option _Trust server certificate_, enfin __Connect__. Il faut ensuite choisir la database qui va être migrer.
Maintenant il faut choisir le serveur de la base de données Azure, cette information peut être obtenu sur le portail Azure dans la vue d'ensemble de la base de la donnée. Choisir ensuite _SQL Server Authentication_ dans le type d'authentification et entrer l'_username_ et le _password_ administrateur de la base de données. De même, ajouter l'option _Trust server certificate_ dans les propriétés de connexion. Choisir ensuite la base de données Azure dans les base de données proposés et valider sur __Next__. Séléctionner ensuite les tables qui doivent être migré puis valider en appuyant sur __Generate SQL script__. Enfin, pour finir, appuyer sur __Deploy schema__ pour valider le processus.

Une fois la migration effectuée, nous pouvons récupéerer la connection string dans Azure dans la vue d'ensemble, dans le champs _Chaînes de connexion_. Une fois la chaîne de connexion récupérée il faut la mettre dans le fichier appsettings.json du projet Serveur et le mettre dans le champs _ConnectionStrings.ManagemAnts_.

## Serveur
Pour ce qui est du serveur, nous avons créer une App Service sur Azure : 

Pour se faire nous créons une App Services dans le portail Azure, on arrive sur la page de création dans laquelle il faut préciser un Groupe de ressources, il faut en créer une si c'est la première fois, ensuite il faut choisir un nom pour l'instance d'App Services et modifier la pile d'execution en choisissant _.NET 5.0_ et choisir la region _West Europe_. Nous appuyons ensuite sur __Vérifier + Créer__ puis __Créer__.

On peut désormais publier notre projet grâce à Visual Studio 2019 en faisant comme suit :
Clic droit sur le projet -> Publier -> Cible Azure -> Azure App Service (Windows) -> Choisir l'App Service créée plus tôt -> Ignorer l'étape de Gestion d'API -> Publier (fichier XML)  -> __Publier__.

## Client
Pour le client, c'est exactement pareil que le serveur, création d'une app service sur Azure puis publication sur visual studio. Il faut au préalable avoir changer l'url pour accèder à l'API en mettant celle donnée par Azure pour le serveur (dans la vue d'ensemble du serveur). Cette modification est à faire dans le projet Client dans le fichier Utils/Client.cs