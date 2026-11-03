PortfolioApi



Dit is de backend van mijn portfoliosite. Je kunt er projecten en blogposts mee aanmaken, bekijken, wijzigen en verwijderen.



Ik heb het gemaakt met .NET 10 en ASP.NET Core Web API. Ik heb gekozen voor controllers.



De code is verdeeld in lagen:

Controllers - hier komen de HTTP-requests binnen.

Services - hier zit de logica en hier worden entiteiten omgezet naar DTO's.

Data - hier staat de DbContext die met de database praat.

Models - hier staan de entiteiten en de DTO's.



De gegevens worden opgeslagen in SQL Server (LocalDB) met Entity Framework Core. De connectionstring staat in appsettings.json onder ConnectionStrings, met de naam Portfolio.



De database maak ik aan met een migratie. Als je het project voor het eerst start, voer je deze twee commando's uit:

dotnet tool install --global dotnet-ef

dotnet ef database update



Daarna start je de applicatie met:

dotnet run



Zet /swagger achter het adres dat in de terminal verschijnt. Dan kun je alle endpoints testen.

