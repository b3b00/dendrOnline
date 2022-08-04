---
id: f3v7nwkqkzq8hh048ra1lgv
title: Analyse
desc: ''
updated: 1659597467981
created: 1659597399733
---
# bootstrap

transformation d'un module monolithique en une série d'enregistrement dans une nouvelle table *SCRIPTS*
Cela nécessitera l'utilisation du parser et provoquera la perte des commentaires

# alternative 1 : edition par script unitaire

edition script / script dans une fenetre à part

## avantages

détection des changements gratuite.

## inconvénients

Changement du parcours de modélisation => demander avis BA.
commentaires ?

# alternative 2 : edition du module

edition dans le module

## avantages

pas de changement de parcours modélisation

## inconvénients

détection des scripts modifiés/ajoutés/supprimés ardue.

### utilisation du parser de script :

* complexité de la régénération du script texte ?

  ### utilisation de regex de détection des débuts de scripts

* `^(const )?([a-z]|[A-Z])+ :=`

* fragilité ?

* détection des changements aisée

* pas de perte des commentaires

    * comment rattacher les commentaires au bon script ?

Ce mécanisme peut être utilisé au moment du bootstrap pour extraire les scripts du module.

# les commentaires

L'utilisation du parser provoquera la perte des commentaires (à vérifier).

les commentaires seront perdu dans les cas

* alt 1
* alt 2 / utilisation du parser

Alternative 2 : L'utilisation d'une regex permet de conserver les commentaires dans les scripts.

### modifier le modele d'AST pour y insérer des commentaires ?

AST : Abre Syntaxique Abstrait

=> les noeuds sont définis dans le framework : 🤮

# Ordre des scripts

les modules sont organisés par "thèmes" logiques. Il faut le conserver ! ceci tend à rejetter la première alternative.
Il faudra également ajouter une meta-data `ordre` sur le script pour pouvoir reconstituer le module dans l'ordre.

exemples :

* découpage d'un script

* regroupement au sein de régions démarquées par :

```
//------------ région ------------
// nom de la règion
// ----------    
```

## opportunité

Modifier l'affichage dans l'éditeur pour limiter  l'affichage à un périmétre donné (ex : Fillon / allégement général ou CIBTP)

⚠️Ceci demanderait un gros travail de réorganisation des scripts en région de la part des modélisateurs toutefois. Il faudra vérifier que le coût du dev jsutifie le gain pour les modélisateurs.

Cette notion de région pourrait être construite :

* dynamiquement par analyse du code du module,
* par ajout d'une meta-data sur les scripts.

# Navigation dans le code

l'éditeur propose une fonction de navigation dans le script.
Le passage en blocs demandera un rework limité.
