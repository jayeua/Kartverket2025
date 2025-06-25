# Kartverket2025

Kartverketprosjekt – En fullstack webapplikasjon for innmelding og behandling av kartfeil
## Innholdsfortegnelse

- [Oversikt](#oversikt)
- [Eksempler på bruk](#eksempler-på-bruk)
  - [For innmeldere (Map User)](#for-innmeldere-map-user)
  - [For saksbehandlere (Case Handler)](#for-saksbehandlere-case-handler)
  - [For systemadministrator](#for-systemadministrator)
- [Oppsett og installasjon](#oppsett-og-installasjon)
- [Arkitektur og struktur](#arkitektur-og-struktur)
- [Systemstruktur](#systemstruktur)
- [Teknologier](#teknologier)
- [Lisens](#lisens)

---

## Oversikt

Dette prosjektet er en fullstack-applikasjon inspirert av Kartverket sine behov og tilsvarende studentprosjekter, hvor formålet er å kunne melde inn, administrere og behandle feil i norske kartdata. Systemet lar brukere rapportere feil, og saksbehandlere kan følge opp og oppdatere status, alt gjennom et moderne og brukervennlig grensesnitt.

---

## Eksempler på bruk

### For innmeldere (Map User)

1. **Logg inn eller registrer deg**  
   ![Registrer Map User](https://github.com/user-attachments/assets/5840aef8-2061-4bf4-ab4f-2dadcb15ef9a)
2. **Hjemmeside for Map User**  
   ![Map User-hjem](https://github.com/user-attachments/assets/4e17ab4c-9bcb-4c9b-a75b-b48255d34ad2)
3. **Meld inn kartfeil via kart og skjema**  
   ![Legg til rapport (Map User)](https://github.com/user-attachments/assets/35ab037e-73c9-4c90-b06a-d92385115f12)
4. **Forhåndsvis og bekreft innmelding**  
   ![Forhåndsvis rapport (Map User)](https://github.com/user-attachments/assets/099a1b92-95f1-43b2-beef-0fb460139f45)
5. **Se historikk over egne innmeldinger**  
   ![Map User rapporthistorikk](https://github.com/user-attachments/assets/d35ea96c-16ec-4829-aac1-5f692e6a8c4f)
6. **Motta bekreftelse på innmelding**  
   ![Innmelding suksess (Map User)](https://github.com/user-attachments/assets/72e2d434-46ef-476d-843b-78b0f92fa75b)

---

### For saksbehandlere (Case Handler)

1. **Logg inn eller registrer deg som saksbehandler**  
   ![Registrer Case Handler](https://github.com/user-attachments/assets/be3d3fcd-6009-4c31-b72c-72591b92dc90)
2. **Hjemmeside for Case Handler**  
   ![Case Handler-hjem](https://github.com/user-attachments/assets/4e62bd4d-d829-4a3a-9eab-3a4f696fa17d)
3. **Se oversikt over alle rapporter**  
   ![Alle rapporter (Case Handler)](https://github.com/user-attachments/assets/9f5e1081-f612-4deb-ace0-853e340443bb)
4. **Behandle/oppdatere/slette rapporter**  
   ![Slett rapport (Case Handler)](https://github.com/user-attachments/assets/6362e534-46bf-4ba9-84c6-c7050ec559bf)
   ![Se rapport (Case Handler)](https://github.com/user-attachments/assets/b3af82ca-faab-48d7-8e3f-3c6296f66749)

---

### For systemadministrator

1. **Logg inn som admin**  
   ![Login-side](https://github.com/user-attachments/assets/8e51b534-df7e-473d-89cf-bb7b3779693e)
2. **Admin-hjemmeside og brukerliste**  
   ![Systemadmin-hjem](https://github.com/user-attachments/assets/ddc861ee-eba1-46d6-a3b5-e28fa13f2310)
   ![Alle brukere (Systemadmin)](https://github.com/user-attachments/assets/ec0f5131-18b9-4354-bab6-545f49a45ab4)

---

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
   https://github.com/jayeua/Kartverket2025.git
   ```

2. Konfigurer connection string i `appsettings.json` (se etter `MariaDbConnection`).

3. Kjør Entity Framework migrasjoner for å sette opp databasen:
   ```sh
   dotnet ef database update
   ```

4. Start applikasjonen og databasen:
   - På Windows kan du bruke Docker Compose for å starte begge containere.
   - På Mac må du starte MariaDB-container manuelt og kjøre appen fra IDE.

5. Logg inn med seeds testbrukere:
   - Saksbehandler: `casehandler@test.com` / `casehandler123@`
   - Innmelder: `mapuser@test.com` / `mapuser123@`
   - Admin: `systemadmin@test.com` / `systemadmin123@`

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

---

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

### Systemstruktur

| **Komponent**         | **Beskrivelse**                                                                                   |
|----------------------|---------------------------------------------------------------------------------------------------|
| **kartverket2025**   | Hovedprosjektet som inneholder hele applikasjonen.                                                |
| **Data**             | Inneholder `ApplicationDbContext` og håndterer tilkobling mot MariaDB, migrasjoner og seed-data.  |
| **Controllers**      | Inneholder alle kontrollere (f.eks. `MapReportController`) som styrer flyten mellom view og modell.|
| **Models/DomainModels** | Lagrer domene- og datamodeller (f.eks. `MapReportModel`) som brukes på tvers av applikasjonen.  |
| **Models/ViewModels** | Inneholder view-modeller som brukes for å presentere og flytte data mellom backend og frontend.   |
| **Repositories**     | Implementerer repository pattern for datatilgang og CRUD-operasjoner (f.eks. `MapReportRepository`).|
| **Services**         | Inneholder tjenester som sender e-post, autentisering mm., og koordinerer samspillet mellom lagene.|
| **Migrations**       | Inneholder automatisk generert kode for Entity Framework migrasjoner.                             |
| **wwwroot**          | Inneholder statiske filer som JavaScript, CSS og bilder.                                          |
| **Views**            | Razor views som presenterer data i brukergrensesnittet.                                           |

---

## Teknologier

- **ASP.NET Core MVC 8.0** – Backend og autentisering
- **C#** – Hovedspråk backend
- **MariaDB** – Relasjonsdatabase (via Docker)
- **Entity Framework Core** – ORM, datamigrering og seed-data
- **Docker** – Container
- **JavaScript, HTML, CSS** – Frontend og kartvisning
- **Razor Views** – Dynamisk visning

---

## Lisens

MIT-lisens

---

> Inspirert av andre studentprosjekter for Kartverket. Utviklet og vedlikeholdt av [jayeua](https://github.com/jayeua).
