# LibrarySystem – Könyvtári Adminisztrációs Rendszer
## 2. Verzió (90% funkcionalitás + Extra funkciók)

Ez a projekt egy modern könyvtári adminisztrációs rendszer, amely ASP.NET Core MVC technológiával készült. A rendszer támogatja a teljes körű adatkezelést. A felület letisztult és modern megjelenésű.

### Aktuális állapot: 2. Verzió (90%)
A fejlesztés ezen szakaszában az összes kiírt modul és funkció elérhető, plusz extra funkcióként egy Jelentések modul:
- **Hitelesítés:** Bejelentkezés és kijelentkezés könyvtárosoknak. Három hibás próbálkozás után a rendszer zárol (kizárja a felhasználót).
- **Irányítópult:** Alapstatisztikák, népszerű könyvek listája, legutóbbi kölcsönzések gyorsnézete.
- **Könyvkezelés:** Listázás, keresés (szerző, cím, azonosító és ISBN alapján), részletek megtekintése, felvétel, szerkesztés, törlés. Törlés esetén választható az egy vagy összes példány törlése.
- **Tagkezelés:** Tagok listázása, keresése (név, ID és lakcím alapján), felvétele, szerkesztése, törlése. Kölcsönzési előzmények megtekintése. Négyféle tagtípus támogatott eltérő limitekkel (polimorfizmus).
- **Kölcsönzés és Visszavétel:**
    - Kölcsönzés rögzítése könyv és tag kiválasztásával, automatikus határidő-számítással a tagtípus alapján.
    - Késedelmi díj kalkuláció: A rendszer automatikusan napi 50 Ft büntetést számol fel a határidő lejárta után.
    - Kifizetendő büntetések összesítése tagonként.
- **[EXTRA] Jelentések Modul:** 
    - Legaktívabb tagok Top 10-es listája (legtöbb kölcsönzéssel).
    - Késésben lévő kölcsönzések részletes áttekintője.
    - Legnépszerűbb könyvek összesített listája.
- **Adatbázis:** SQLite (Entity Framework Core), 100%-os modell lefedettség, objektumorientált megközelítés.

### Technológiai verem
- **Framework:** .NET 8 / ASP.NET Core MVC
- **Adatbázis:** SQLite
- **Frontend:** Vanilla CSS3 (reszponzív, letisztult, modern "Garamond" dizájn)

### Futtatás
1. Nyissa meg a `LibrarySystem.sln` fájlt.
2. Futtassa a projektet (`dotnet run` vagy `F5`).
3. **Admin belépés:** `admin` / `admin123`
