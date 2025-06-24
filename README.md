# Kartverket2025

Kartverketprosjekt – En fullstack webapplikasjon for innmelding og behandling av kartfeil

---

## Innholdsfortegnelse

- [Oversikt](#oversikt)
- [Funksjonalitet](#funksjonalitet)
  - [Brukerroller](#brukerroller)
  - [Kartinnmelding](#kartinnmelding)
  - [Saksbehandling](#saksbehandling)
  - [Visning og oversikt](#visning-og-oversikt)
- [Oppsett og installasjon](#oppsett-og-installasjon)
  - [Krav](#krav)
  - [Installasjonstrinn](#installasjonstrinn)
  - [Docker-bruk](#docker-bruk)
  - [Miljøvariabler](#miljøvariabler)
- [Arkitektur og struktur](#arkitektur-og-struktur)
- [Teknologier](#teknologier)
- [Lisens](#lisens)

---

## Oversikt

Dette prosjektet er en fullstack-applikasjon inspirert av Kartverket sine behov og tilsvarende studentprosjekter, hvor formålet er å kunne melde inn, administrere og behandle feil i norske kartdata. Systemet lar brukere rapportere feil, og saksbehandlere kan følge opp og oppdatere status, alt gjennom et moderne og brukervennlig grensesnitt.

## Funksjonalitet

### Brukerroller

- **Innmelder (Map User):** Kan registrere seg, logge inn, og melde inn kartfeil med beskrivelse og geodata. Kan se og filtrere sine egne innmeldinger.
- **Saksbehandler (Case Handler):** Kan se alle innmeldinger, tildele seg saker, oppdatere, og endre status. Kan sortere og filtrere rapporter etter status/kartlag, og søke i innmeldinger.
- **Systemadministrator:** Har oversikt over alle brukere og kan slette brukere fra et eget adminpanel.

### Kartinnmelding

- Rapportering av feil kan gjøres via kartet, hvor brukeren kan angi område via GeoJSON og fylle inn beskrivelse, tittel, kommune/fylke m.m.
- Støtte for forhåndsvisning av innmelding før innsending.
- Hver innmelding får status og prioritet.

### Saksbehandling

- Saksbehandlere kan tildele seg saker i status "Pending", oppdatere status, og eventuelt slette saker.
- Kun saksbehandler tilknyttet en sak kan utføre endringer etter behandling er startet.
- Alle roller benytter autentisering via ASP.NET Identity.

### Visning og oversikt

- **Tabellvisning:** Filtrer og sorter egne/alle innmeldinger. Søk funksjon for nøkkelord.
- **Kartvisning:** Viser innmeldinger som markører fra GeoJSON, grupperes etter zoomnivå.

## Oppsett og installasjon

### Krav

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Visual Studio 2022, Rider, eller annen .NET-kompatibel IDE
- MariaDB (Docker container anbefalt)
- (For Mac: egen databasecontainer må startes manuelt, se under)

### Installasjonstrinn

1. Klon dette repositoriet:
   ```sh
   git clone https://github.com/luckyme28/Kartverket2025.git
   ```

2. Konfigurer connection string i `appsettings.json` (se etter `MariaDbConnection`).

3. Kjør Entity Framework migrasjoner for å sette opp databasen:
   ```sh
   dotnet ef database update
   ```

4. Start applikasjonen og databasen:
   - På Windows kan du bruke Docker Compose for å starte begge containere.
   - På Mac må du starte MariaDB-container manuelt og kjøre appen fra IDE.

5. Logg inn med seedede testbrukere:
   - Saksbehandler: `casehandler@test.com` / `CaseHandler@123`
   - Innmelder: `submitter@test.com` / `Submitter@123`
   - Admin: `Sysadmin@test.com` / `Test@123`

### Docker-bruk

Applikasjonen og databasen kjøres i separate containere via Docker Compose. For Mac kan det være nødvendig å kjøre MariaDB-container manuelt:

```sh
docker run --name mariadb \
  -e MYSQL_ROOT_PASSWORD=kartverket \
  -e MYSQL_DATABASE=KartverketDb \
  -p 3307:3306 \
  -v mariadb_data:/var/lib/mysql \
  -d mariadb:latest
```

### Miljøvariabler

I `appsettings.json` må følgende settes:
- `MariaDbConnection`: Connection string til MariaDB-databasen

## Arkitektur og struktur

Prosjektet følger MVC-arkitektur og repository pattern for å skille logikk, datatilgang og presentasjon. 

**Viktige kataloger/klasser:**
- `Controllers/`: Inneholder bl.a. `MapReportController` for kartinnmelding og saksbehandling.
- `Models/DomainModels/`: Domene-modeller som `MapReportModel`.
- `Repositories/`: F.eks. `MapReportRepository` for CRUD-operasjoner mot DB.
- `Data/`: `ApplicationDbContext` for EF/Identity.
- `Migrations/`: Generert kode for databasemigreringer.
- `Views/`: Razor views for frontend.

**Kort om flyt:**
- Brukeren melder inn feil via webskjema/kart.
- Data lagres via repository til MariaDB gjennom EF.
- Saksbehandler kan endre status, tildele seg saker, og følge opp.
- All tilgang og funksjon er rollebasert og sikret via ASP.NET Identity.

## Teknologier

- **ASP.NET Core MVC 8.0** – Backend og autentisering
- **C#** – Hovedspråk backend
- **MariaDB** – Relasjonsdatabase (via Docker)
- **Entity Framework Core** – ORM, datamigrering og seed-data
- **Docker** – Containerisering
- **JavaScript, HTML, CSS** – Frontend og kartvisning
- **Razor Views** – Dynamisk visning

## Lisens

MIT-lisens

---

> Inspirert av andre studentprosjekter for Kartverket. Utviklet og vedlikeholdt av [luckyme28](https://github.com/luckyme28).
