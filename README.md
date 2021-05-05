# README

# Déploiement

## Base de donnée
Afin de pouvoir déployer le serveur et le client sur Azure nous avons tout d'abord créer une base de donnée sur notre portail Azure puis nous avons fais une migration du schema de notre base de données dans celle d'Azure grâce à la l'outil "Microsoft Data Migration Assistant". Une fois la migration effectuée, nous avons pu récupérer la connexion string donnée dans par Azure pour la mettre dans notre serveur.

## Serveur
Pour ce qui est du serveur, nous avons créer une App Service sur Azure dans laquelle nous avons publier le projet serveur à l'aide de Visuat Studio :
Clic droit sur le projet -> Publier -> Cible Azure -> Azure App Service (Windows) -> App Service créée plus tôt -> Pas de Gestion d'API -> Publier (fichier XML)  -> Publier.

## Client
Pour le client, c'est exactement pareil que le serveur, création d'une app service sur Azure puis publication sur visual studio. Il faut au préalable avoir changer l'url pour accèder à l'API en mettant celle donnée par Azure pour le serveur.