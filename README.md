# Könyvtári Nyilvántartó Rendszer - Prototípus 1 (2. Mérföldkő)

## Prototípus Áttekintés

Az 1. prototípus a rendszer magját és az alapvető böngészési funkciókat mutatja be.

### Implementált funkciók (kb. 30%)
1.  **Hitelesítés:** Könyvtáros bejelentkezés és kijelentkezés (admin/admin123).
2.  **Irányítópult (Dashboard):** Statisztikai adatok megjelenítése (könyvek száma, tagok száma, aktív kölcsönzések).
3.  **Könyvkezelés (Olvasás):** Teljes könyvlista megjelenítése, részletes adatok megtekintése.
4.  **Könyvkeresés:** Keresés cím, szerző, ISBN vagy azonosító alapján.
5.  **Tagnyilvántartás (Olvasás):** Regisztrált tagok listázása és keresése.

### Adatmodell (100% Kész)
A felhasználói felület még csak részleges, a háttérben az adatmodell már teljesen kidolgozott:
-   **Book:** Cím, Szerző, ISBN, Kiadó, Év, Példányszám, Kölcsönözhetőség állapota.
-   **LibraryMember:** Absztrakt alaposztály különböző típusú tagokkal (Hallgató, Oktató, Külsős), egyedi kölcsönzési limitekkel (max könyv, kölcsönzési idő).
-   **Loan:** Kölcsönzések rögzítése, határidő számítás, visszahozatal kezelése, késedelem számítása.
-   **Librarian:** Adminisztrátori fiók kezelése titkosított jelszavakkal (SHA-256).

## Technológiai Stack
-   **Framework:** .NET 10.0 ASP.NET Core MVC
-   **Adatbázis:** SQLite (Entity Framework Core)
-   **Frontend:** Vanilla CSS (egyedi design), HTML5, JavaScript
-   **Architektúra:** Layered Architecture (Models, Views, Controllers, Services, Data)

## Projektstruktúra
-   `Controllers/`: HTTP kérések kezelése.
-   `Models/`: Domain modellek és ViewModellek.
-   `Services/`: Üzleti logika (DataService).
-   `Data/`: Adatbázis kontextus (Entity Framework).
-   `Views/`: UI sablonok.
-   `wwwroot/`: Statikus fájlok (CSS, JS).

## Telepítés és Futtatás
1.  Klónozza a repository-t.
2.  Nyissa meg a `LibrarySystem.sln` fájlt Visual Studio-ban.
3.  Futtassa a projektet (F5).
4.  Bejelentkezés:
    -   Felhasználónév: `admin`
    -   Jelszó: `admin123`

---
