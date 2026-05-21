# Development Log – LibrarySystem Fejlesztési Napló

## 2026. május 21. – Első frissítés: Adatintegritás és Szigorú Validáció

### 1. Hiba javítása: SQLite NOT NULL Constraint Violation
**Probléma:** Az üres mezőkkel beküldött űrlapok adatbázis hibát (`SQLite Error 19`) okoztak.
**Megoldás:** Validációs ellenőrzések beépítése a mentés előtt.

### 2. Szigorított Validációs Szabályok
- **Könyvek:** Szerző/Cím (min 2 karakter), Év (1800-2026), ISBN (Regex ellenőrzés).
- **Tagok:** Név (Regex: betűk, pont, szóköz), Lakcím (min 10 karakter), Elérhetőség (EmailAddress).

### 3. Kölcsönzési Hozzáférés-védelem
**Új funkció:** A rendszer megakadályozza a belépést a kölcsönzési felületre, ha a tag elérte a limitjét.

## 2026. május 21. – Második frissítés: Elérhetőség szigorítása és Workflow javítások

### 1. Elérhetőség korlátozása (Email-only)
**Módosítás:** A tagok adatlapján az "Elérhetőség" mezőt szigorítottuk. Mostantól **csak érvényes e-mail cím** adható meg, telefonszám nem.
- Frissítettem a `CreateMemberViewModel` és `EditMemberViewModel` modelleket.
- A kezelőfelületen (UI) a feliratok "E-mail cím"-re változtak.
- A táblázat fejlécében is az "E-mail" felirat szerepel.

### 2. Kölcsönzési folyamat optimalizálása
**Hiba javítása:** Ha a taglistából indítottuk a kölcsönzést a "+" gombbal, a rendszer nem töltötte be automatikusan a tagot.
**Megoldás:**
- Létrehoztam új API végpontokat a `LoansController`-ben (`GetMemberDetails`, `GetBookDetails`).
- A kölcsönzési felület most már ID alapján azonnal beazonosítja és rögzíti a tagot/könyvet a betöltéskor, miközben fenntartja a keresőmező biztonsági korlátozásait.

### 3. Validáció és Biztonság tökéletesítése
- **Regex szinkronizáció:** Kijavítottam a hibát, ami miatt a név-ellenőrzés nem működött a tagfelvételnél. Mostantól a `CreateMemberViewModel` és `EditMemberViewModel` is tartalmazza a szigorú Regex szabályt (csak betűk, pont és szóköz).
- **SQLite védelem:** Minden Controller-ben aktív a `ModelState.IsValid` ellenőrzés, megakadályozva az üres vagy hibás adatok adatbázisba kerülését.

### 4. CI/CD és Projektstruktúra javítása
- **GitHub Actions:** Létrehoztam a `.github/workflows/ci.yml` fájlt az automatikus teszteléshez.

---