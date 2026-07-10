### Defra Core

A core library to containing common functionality for Defra dotnet projects.

### Run Cake Files locally

If you want to test the dotnet cake build locally you can run the following command:

```bash
dotnet cake ./build/build.cake --target=Default --product_name=Defra.Core --solution_file_name=../Core.slnx --package_version=0.1.0
```

### Test Github action

Install `act` to run the github action locally.

To Install act on Linux

```sh
brew install act
```

To run github action 

```sh
act -W .github/workflows/build.yml 
```