# Bemutató Forgatókönyv – 2. Verzió (90% + Extrák)
## LibrarySystem

### 1. Bevezetés
- **Állapot:** A projekt 90%-ban elkészült, az összes kiírt funkció (CRUD, keresés, kölcsönzés, büntetés, zárolás) elkészült.
- **Megjelenés:** A felület új, modernebb dizájnt kapott.
- **Extrák:** Bevezettem egy új "Jelentések" modult a mélyebb statisztikai elemzésekhez.
- A technológiai stack említése (C# .NET Core MVC, SQLite Entity Framework Core, GitHub Actions CI).
- A Clean Architecture és az MVC rétegek bemutatása a kódstruktúrában (ahogy a komponens diagram is mutatja).

### 2. Demó
- **Bejelentkezés:** Próbáljunk meg háromszor rossz jelszóval belépni. Lássuk a zárolási képernyőt, demonstrálva a 3 próbálkozásos kilépést. Ezután lépjünk be a helyes (`admin`/`admin123`) adatokkal.
- ** Felület bemutatása:** Röviden mutassuk be a főmenüt, a könyvek és tagok listáját, valamint a jelentések nézetet.
- **Tag és Könyv kezelés:** 
    1. Navigáljunk a Könyvekhez, próbáljuk ki a keresést pl. azonosító alapján, mutassuk meg a példányszám kezelést az "Új könyv felvétele" gombnál.
    2. Mutassuk be, hogy a Tag típusoknál (pl. hallgató, professzor) automatikusan változik a maximális könyvek száma.
- **Kölcsönzés, Visszavétel és Jelentések:**
    1. Kölcsönözzünk ki egy könyvet egy tagnak a "Kölcsönzés rögzítése" funkcióval.
    2. A tag profilján mutassuk be az aktív kölcsönzéseket, a visszavételt, és a késedelmi díj (50 Ft / nap) működését.
    3. Nyissuk meg a **Jelentések** (Extra) nézetet, és mutassuk be az összesített statisztikákat (Legaktívabb tagok, Késésben lévő könyvek listája).

- *Tagok kezelése:* Egy új hallgató és egy új oktató felvitele. Kiemelni, hogy polimorfizmussal oldottuk meg a különböző limiteket.
- *Kölcsönzés rögzítése:* A biztonságos "autocomplete" kereső bemutatása. Egy könyv kikölcsönzése a hallgatónak, és egy másik az oktatónak.
- *Üzleti logika (Limit/Büntetés):* Demonstrálni, hogy a rendszer blokkolja a hallgatót, ha túllépi az 5 könyves limitet, vagy ha a tagot megpróbáljuk törölni aktív kölcsönzés mellett.

### 3. Kód ismertetése – Az érdekesebb részletek 
- **Téma 1:** A tagtípusok objektumorientált felépítése (Öröklődés).
  - *Magyarázat:* Az absztrakt `LibraryMember` osztályból származnak az alosztályok (`StudentMember`, stb.), amelyek automatikusan beállítják a `MaxBooks` és `LoanDays` értékeket. Nincs szükség hosszú IF-ELSE feltételekre.
  
- **Téma 2:** A példányszámok és törlés logikája (`DataService.cs`).
  - *Magyarázat:* Könyv törlésénél választhatunk egy vagy összes példány törlése közül, de a rendszer validálja, hogy van-e kikölcsönzött példány, így az adatbázis konzisztens marad.

### 4. Kihívás
- *Kihívás:* Az ID alapú adatrögzítés és a név alapú autocompletion (kiegészítés) szétválasztása a felhasználói élmény és az adatintegritás érdekében.
- *Megoldás:* Egyedi JavaScript (Debounce technikával) hívja meg a backend dinamikus keresőjét, ami ID alapján a listákban szűr, de a kölcsönzésnél biztonsági okokból csak az interaktív kiválasztást engedi, megelőzve az elgépeléseket.

- *Kihívás:* A frontend-oldali kapacitás-ellenőrzés szinkronizálása a backend szabályokkal.
- *Megoldás:* AJAX alapú lekérdezés a tag kiválasztásakor, ami azonnal frissíti a kliens-oldali korlátokat.

### 5. Összegzés
- Minden feladatkiírási pont teljesítve lett. (?)
- A szoftver stabilnak tűnik, modern megjelenésű, extra funkciókkal bővített, és Entity Framework Core alapú.

---