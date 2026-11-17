PortfolioApi

Dit is de backend van mijn portfoliosite. Je kunt er projecten en blogposts mee aanmaken, bekijken, wijzigen en verwijderen. Iedereen mag projecten en blogposts bekijken. Alleen een ingelogde Editor of Admin mag ze aanmaken, wijzigen en verwijderen.

Ik heb het gemaakt met .NET 10 en ASP.NET Core Web API, met controllers. Gebruikers en rollen worden beheerd met ASP.NET Core Identity.

De code is verdeeld in lagen:
Controllers - hier komen de HTTP-requests binnen (ook registreren en inloggen).
Services - hier zit de logica en hier worden entiteiten omgezet naar DTO's.
Data - hier staat de DbContext die met de database praat.
Models - hier staan de entiteiten en de DTO's.

De repository bevat twee projecten:
PortfolioApi - de API.
PortfolioApi.Tests - de xUnit-tests.

Database en migraties
De gegevens worden opgeslagen in SQL Server (LocalDB) met Entity Framework Core. De connectionstring staat in PortfolioApi/appsettings.json onder ConnectionStrings, met de naam Portfolio.

Voer de eerste keer deze commando's uit in de map PortfolioApi. Hiermee worden alle tabellen aangemaakt, ook die van Identity en de rollen Gast, Editor en Admin:
dotnet tool install --global dotnet-ef
dotnet ef database update

De API starten
Voer in de hoofdmap uit:
dotnet run --project PortfolioApi
Zet /swagger achter het adres dat in de terminal verschijnt.

Gebruikers en rollen
1. Registreer een gebruiker in Swagger met POST /api/auth/register. Elke nieuwe gebruiker krijgt de rol Gast.
2. Log in met POST /api/auth/login. Je krijgt een cookie terug.
3. Om een gebruiker Editor of Admin te maken, open je rol-toekennen.sql. Vul het e-mailadres en de rol (Editor of Admin) in en voer het script uit op de database PortfolioDb, bijvoorbeeld met de SQL Server-extensie in VS Code.
4. Log daarna opnieuw in, zodat de nieuwe rol in de cookie staat.

Er staan geen wachtwoorden in de repository. Gebruikers kiezen zelf hun wachtwoord bij het registreren.

Tests uitvoeren
Voer in de hoofdmap uit:
dotnet test
De tests gebruiken een aparte database (PortfolioDb_Test), ingesteld in PortfolioApi/appsettings.Testing.json. Deze database wordt bij elke testrun leeggemaakt en opnieuw aangemaakt, zodat de gewone database niet wordt aangeraakt.