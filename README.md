# 🐇 Bunny Hop

An *experimental* Hopfield network implementation


## Prerequisites

To run Bunny Hop along with a F# notebook you will need:

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Polyglot Notebook VS Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode)

## Installation

1. `cd src`
1. `dotnet restore` to install the dependencies.
1. `dotnet tool restore` to install the tools.
1. `dotnet build` to create the Bunny Hop library.

## Notebook usage

### MNIST

1. Download [the MNIST dataset provided in a easy-to-use CSV format](https://www.kaggle.com/datasets/oddrationale/mnist-in-csv).
1. Extract the two CSV files from the zip archive and place them in a folder named `datasets` inside the `notebooks` folder.
