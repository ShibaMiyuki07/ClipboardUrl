🎧 ClipboardUrl
------------
ClipboardUrl est un logiciel Windows en C# qui détecte automatiquement les liens YouTube copiés dans le presse-papiers et télécharge l'audio en MP3 grâce à YoutubeExplode et FFmpeg. L’outil est léger, rapide, et s'exécute discrètement en arrière-plan.

✨ Fonctionnalités
------------
* 📋 Surveillance du presse-papiers pour détecter les liens YouTube

* 🎥 Support des vidéos individuelles et des playlists

* 🔄 Téléchargement automatique de l'audio via YoutubeExplode

* 🎵 Conversion en MP3 via FFmpeg

* 💾 Enregistrement local dans un dossier configurable

🛠️ Prérequis
------------
Windows 10 ou supérieur

.NET 6+ ou version compatible

FFmpeg installé et accessible via le PATH (ou fourni avec l'appli)

🚀 Installation & Exécution
------------
Cloner le projet
```sh
git clone https://github.com/ShibaMiyuki07/ClipboardUrl.git
cd ClipboardUrl
```
Ouvrir dans Visual Studio

Ouvrir ClipboardUrl.sln

Restaurer les dépendances NuGet (YoutubeExplode, etc.)

Compiler et lancer l'application

F5 ou Ctrl+F5 dans Visual Studio

📋 Utilisation
------------
Lancez ClipboardUrl

Copiez un lien YouTube (vidéo ou playlist)

L’application :

- détecte le lien dans le presse-papiers

- télécharge l’audio via YoutubeExplode

- convertit en .mp3 avec FFmpeg

- enregistre dans un dossier comme Downloads/ClipboardUrl

📂 Dossier de sortie
------------
Les fichiers MP3 sont enregistrés dans :

```
ClipboardUrl/
└── Downloads/
    └── <nom_video>.mp3
```

⚠️ Légalité
------------
Ce logiciel est destiné à un usage personnel et éducatif. Le téléchargement de contenu soumis à des droits d’auteur peut enfreindre les conditions d’utilisation de YouTube. Utilisez-le de manière responsable.

🤝 Contributions
------------
Les contributions sont les bienvenues !

Fork du projet

Créez une branche (feature/ma-fonctionnalite)

Faites un PR

📄 Licence
------------
MIT – libre pour usage personnel ou modification.
