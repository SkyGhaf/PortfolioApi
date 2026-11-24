PortfolioApi

Dit is de backend van mijn portfoliosite. Je kunt er projecten en blogposts mee aanmaken, bekijken, wijzigen en verwijderen. Iedereen mag projecten en blogposts bekijken. Alleen een ingelogde Editor of Admin mag ze aanmaken en wijzigen. Verwijderen mag alleen een Editor of Admin die ook de claim MagVerwijderen heeft.

Ik heb het gemaakt met .NET 10 en ASP.NET Core Web API, met controllers. Gebruikers en rollen worden beheerd met ASP.NET Core Identity.

De code is verdeeld in lagen:
Controllers - hier komen de HTTP-requests binnen (ook registreren, inloggen en /me).
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

Wachtwoordregels
De wachtwoordregels staan in PortfolioApi/Program.cs bij AddIdentity: een wachtwoord moet minimaal 8 tekens hebben, met een hoofdletter, een kleine letter, een cijfer en een speciaal teken. Elk e-mailadres mag maar één keer gebruikt worden.

Gebruikers, rollen en claims
1. Registreer een gebruiker in Swagger met POST /api/auth/register. Elke nieuwe gebruiker krijgt de rol Gast.
2. Log in met POST /api/auth/login. Je krijgt een cookie terug.
3. Met GET /api/auth/me zie je met welk e-mailadres en welke rollen je bent ingelogd.
4. Om een gebruiker Editor of Admin te maken, open je rol-toekennen.sql. Vul het e-mailadres en de rol in en voer het script uit op de database PortfolioDb, bijvoorbeeld met de SQL Server-extensie in VS Code.
5. Om een Editor of Admin ook te laten verwijderen, open je claim-toekennen.sql. Vul het e-mailadres in en voer het script uit. De gebruiker krijgt dan de claim MagVerwijderen.
6. Log daarna opnieuw in, zodat de nieuwe rol of claim in de cookie staat.

Er staan geen wachtwoorden in de repository. Gebruikers kiezen zelf hun wachtwoord bij het registreren.

Tests uitvoeren
Voer in de hoofdmap uit:
dotnet test
De tests gebruiken een aparte database (PortfolioDb_Test), ingesteld in PortfolioApi/appsettings.Testing.json. Deze database wordt bij elke testrun leeggemaakt en opnieuw aangemaakt, zodat de gewone database niet wordt aangeraakt.