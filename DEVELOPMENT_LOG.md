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

## 2026. május 25. – Harmadik frissítés: Csoportos műveletek és Típusbiztonság

### 1. Csoportos Visszavétel (Bulk Return)
**Módosítás:** Implementáltam a tömeges könyv-visszavételt, hogy megkönnyítsem a könyvtáros munkáját.
- **Könyv alapú:** A könyvlistából az összes kint lévő példány egyszerre visszavehető.
- **Tag alapú:** A tag adatlapjáról az összes nála lévő könyv egy gombbal visszavételezhető.
- **Közvetlen visszavétel:** Az "Összes kölcsönzés" listában minden sor végére került egy azonnali visszavétel gomb.

### 2. Hiba javítása: Decimal/Int típuseltérés (CS0266)
**Probléma:** A csoportos visszavételkor a büntetés összegzésénél fordítási hiba lépett fel, mert a `FineAmount` decimal típusát int-be próbálta tölteni a rendszer.
**Megoldás:** Átírtam az összesítő metódusokat a `DataService`-ben, hogy konzisztensen a decimal típust használják.

### 3. UI Konzisztencia
- Az "Összes visszavétele" gomb színét a téma arany színéhez igazítottam.
- Frissítettem a CSS-t a jobb olvashatóság érdekében a hover állapotoknál.


---