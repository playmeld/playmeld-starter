# Playmeld Starter

The template comes with configuration and dependencies needed to quickstart a project

- Recommended Unity version is `2023.3.4f1`
- Uses URP template
- Git configurations based on [unity-git-setup](https://github.com/playmeld/unity-git-setup)

## Prerequisites

### 1. install Unity Hub

Go to [Unity's download page](https://unity.com/download) and download Unity Hub for your operating system

### 2. Create Unity Id

Go to [Unity ID page](https://id.unity.com) and create an account

### 3. Install Unity 2023.3.4f1

- Open Unity Hub and login.
- Go to "Installs" section
- Click on "Install Editor" button
- Click on "Archive"
- Click on "download archive" link
- Find "Unity 2022.3.4" and click "Unity Hub" button
- A new window should open in Unity Hub with the selected Unity version, please install it. Select Visual Studio, iOS and Android build support options.

## Getting started

There are 2 options to create a new project from this template:

### 1. Download zip

If you are not familiar with git, use this option.

- Click on `<> Code` button and select "Download ZIP"
- unarchive zip on your computer
- In Unity Hub Go to "Projects" section
- Click on an arrow next to "Open" button and choose "Add project from disk"

### 2. Clone repository

If you are familiar with git, use this option.

- `$ git clone https://github.com/playmeld/playmeld-starter.git example`
- Install [Git LFS]((https://git-lfs.com/)) if you don't have it
- Update git remote

```
$ git remote rename origin seed
# git remote add origin <your-git-origin>
```

- Make sure to update the path to `UnityYAMLMerge` in `.gitconfig`
- Copy pre-commit file into .git/hooks folder from [unity-git-setup](https://github.com/playmeld/unity-git-setup#5-setup-pre-commit-hook) repository
- [Optional] in text editor open `ProjectSettings.asset` and update `productName`, `metroPackageName` and `metroApplicationDescription`
- In Unity Hub Go to "Projects" section
- Click on an arrow next to "Open" button and choose "Add project from disk"

## How to update

To be up-to-date with the changes in the starter project, you can pull and merge updates from the "upstream" repository back into your project by running:

```
$ git fetch seed                # Fetch Node.js Starter Kit (upstream) repository
$ git checkout main             # Switch to the main branch (or, master branch)
$ git merge seed/main           # Merge upstream/master into the local branch
```
