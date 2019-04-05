# Projet 3ème année Cycle science de l'ingénieur - ConstellationChatiereConnectee

## Constellation ?

Constellation est une plate-forme pour orchestrer et interconnecter vos programmes et vos périphériques. Il permet la communication en temps réel entre eux et vers l'extérieur.
Pour en savoir plus sur constellation: http://www.myconstellation.io/

## Présentation du système

Les animaux de compagnies sont des membres à part entière de nos familles, nous voulons ce qu’il y a de mieux pour eux. Cependant, il est parfois difficile de répondre à leurs besoins quand nous ne sommes pas présents ou que ceux-ci n’arrivent pas à décider ce qu’ils souhaitent faire. Dans ces deux cas, ils doivent attendre après nous malgré notre absence ou notre lassitude.
Avec la trappe connectée, ces situations n’ont plus lieu d’être puisque celle-ci répondra aux besoins de nos amis à quatre pattes dans l’instant sans nécessiter notre intervention. Elle les éclairera lors de leurs sorties nocturnes et vous tiendra informé de leur présence ou non à l’intérieur. Elle est programmable, selon vos conditions, permettant leur passage ou les bloquant selon l’horaire.
Cependant, si vous souhaitez reprendre la main sur la trappe, il vous est possible de la commander à distance grâce à son interface web simple d’utilisation.

## Structure du projet

    |--- sites                               # Interface web permettant de controller la trappe
    |    |--- css                            # dossier des fichiers css
    |    |--- img                            # dossier des images du site
    |    |--- js
    |         |--- constellation.js          # Connecte l'application web au serveur constellation
    |    |--- index.html
    |
    |--- packagesConstellation               # data receive by sensors
    |    |--- Chatiere                       # contient le code pour connecter la trappe au serveur constellation
    |         |--- Chatiere.ino              # code arduino pour connecter la trappe au serveur constellation
    |
    |--- ConstellationMyBrain                # contient l'ensemble des fichiers nécessaire au bon fonctionnement du serveur constellation
    |    |--- Properties
    |    |--- App.config
    |    |--- PackageInfo.xml
    |    |--- packages.config
    |    |--- Program.cs                     # contient les fonctions qui permettent de gérer les applications connectées à la trappe
    |
    |--- ESP8266.jpg                         # ESP8266 Pin Mapping
    |--- README.md
    |--- .gitignore
