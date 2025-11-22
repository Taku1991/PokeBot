# Mehrsprachiges Bot-System (Deutsch/Englisch)

## Übersicht
Dieser Bot unterstützt jetzt sowohl **deutsche als auch englische Befehle** gleichzeitig. Benutzer können jeden Befehl in beiden Sprachen verwenden.

## Implementierte Features

### 1. Mehrsprachige Commands (Aliases)
Alle wichtigen Commands funktionieren jetzt auf Deutsch UND Englisch:

#### Trade Commands
- **trade** / **tausch** / **tauschen** / **tau** - Startet einen Trade
- **hidetrade** / **verstecktertausch** / **vt** - Trade ohne Embed-Details
- **egg** / **ei** - Bestellt ein Ei
- **fixOT** / **reparieren** / **rep** - Repariert OT/Nickname
- **medals** / **medaillen** / **md** - Zeigt Medaillen an

#### Queue Commands
- **queueStatus** / **warteschlangeStatus** / **ws** / **position** / **pos** - Zeigt Position in der Queue
- **queueClear** / **warteschlangeRaus** / **wr** - Entfernt dich aus der Queue
- **queueList** / **warteschlangeListe** / **wl** / **liste** - Zeigt Queue-Liste (Admin)
- **queueClearAll** / **warteschlangeAllesLöschen** / **wal** - Leert die gesamte Queue (Admin)
- **deleteTradeCode** / **tauschcodeLöschen** / **tcl** - Löscht gespeicherten Tauschcode

#### Admin Commands
- **tradeUser** / **tauschBenutzer** / **tb** - Tauscht mit einem bestimmten User (Admin)
- **queueMode** / **warteschlangeModus** / **wm** - Ändert Queue-Modus (Admin)

#### General Commands
- **ping** / **pong** - Prüft ob der Bot online ist
- **hello** / **hallo** / **moin** - Begrüßt den Bot
- **info** / **infos** / **über** - Bot-Informationen
- **status** / **statistik** / **stat** - Bot-Status

### 2. Lokalisierungs-System
Erstellt in `SysBot.Pokemon.Discord/Resources/`:
- **Localization.resx** - Englische Texte (Standard)
- **Localization.de.resx** - Deutsche Texte
- **LocalizationHelper.cs** - Helper-Klasse für Übersetzungen

## Verwendung der Lokalisierung (für zukünftige Entwicklung)

### In C# Code:
```csharp
using SysBot.Pokemon.Discord.Helpers;

// Einfacher Text
string message = LocalizationHelper.GetString("Trade_NoTradesYet", username);

// Mit Formatierung
string message = LocalizationHelper.GetString("Trade_QueueFull", pokemonName);
```

### Sprache ändern (optional für später):
```csharp
// Auf Deutsch umschalten
LocalizationHelper.SetCulture("de");

// Auf Englisch umschalten
LocalizationHelper.SetCulture("en");
```

## Branch-Erstellung (Privat)

### Schritt 1: Lokalen Branch erstellen
```powershell
git checkout -b bilingual-bot
git add .
git commit -m "Add bilingual support (DE/EN) - Commands work in both languages"
```

### Schritt 2: Branch zu GitHub pushen
```powershell
git push -u origin bilingual-bot
```

### Schritt 3: Branch auf GitHub privat machen
1. Gehe zu: https://github.com/Taku1991/PokeBot/settings/branches
2. Füge eine "Branch protection rule" hinzu für `bilingual-bot`
3. **ODER** wenn du den gesamten Branch privat halten möchtest während der Entwicklung:
   - Arbeite nur lokal bis alles fertig ist
   - Pushe erst dann zum Repository

**Hinweis**: GitHub Branches sind standardmäßig so privat wie das Repository selbst. 
Wenn dein Repository privat ist, ist auch der Branch privat.

## Vorteile dieser Implementierung

✅ **Beide Sprachen gleichzeitig** - Keine Umschaltung nötig
✅ **Keine Code-Duplikation** - Nur Aliases hinzugefügt
✅ **Einfache Wartung** - Neue Features funktionieren sofort in beiden Sprachen
✅ **Rückwärtskompatibel** - Alle alten Commands funktionieren weiterhin
✅ **Erweiterbar** - Lokalisierungs-System für zukünftige Texte vorbereitet
✅ **Deutsche Showdown-Sets** - Komplette Pokémon-Sets können auf Deutsch eingegeben werden!

## Deutsche Showdown-Sets

### ✨ NEU: Komplette deutsche Eingabe möglich!

Du kannst jetzt **komplette Pokémon-Sets auf Deutsch** eingeben! Der Bot übersetzt sie automatisch.

**Beispiel:**
```
!tausch Glurak (M) @ Meisterball
Level: 100
Shiny: Ja
Sprache: Deutsch
EVs: 252 KP / 252 SpAng / 6 Init
Wesen: Mäßig
- Luftschnitt
- Flammenwurf
- Flammenblitz
- Hyperstrahl
```

**Siehe:** [GERMAN_SHOWDOWN_EXAMPLES.md](GERMAN_SHOWDOWN_EXAMPLES.md) für viele Beispiele!

### Wie es funktioniert:
1. **Automatische Erkennung**: Bot erkennt deutsche Keywords (Fähigkeit, Wesen, etc.)
2. **PKHeX-Integration**: Nutzt PKHeX's deutsche Übersetzungsdaten
3. **Echtzeit-Übersetzung**: Konvertiert alle Namen (Pokémon, Attacken, Items, Naturen)
4. **Transparent**: Funktioniert für normale Trades, Eier, und Batch-Trades

## Nächste Schritte (Optional)

1. **Weitere Commands übersetzen**: Andere Module (DumpModule, CloneModule, etc.)
2. **Ausgabetexte lokalisieren**: ReplyAsync-Nachrichten über Resource-Dateien laden
3. **Embed-Texte übersetzen**: Automatische Spracherkennung basierend auf User-Locale
4. **Help-System erweitern**: Deutsche Hilfetexte für alle Commands
5. ~~**Deutsche Showdown-Sets**~~ ✅ **FERTIG!**

## Testing

### Commands testen:
```
# Englisch
!trade Pikachu
!queueStatus
!medals

# Deutsch
!tausch Pikachu
!position
!medaillen
```

Beide Versionen sollten identisch funktionieren!

## Dateiänderungen

### Neue Dateien:
- `SysBot.Pokemon.Discord/Resources/Localization.resx`
- `SysBot.Pokemon.Discord/Resources/Localization.de.resx`
- `SysBot.Pokemon.Discord/Helpers/LocalizationHelper.cs`
- `SysBot.Pokemon/Helpers/GermanShowdownTranslator.cs` ⭐
- `BILINGUAL_README.md` (diese Datei)
- `GERMAN_SHOWDOWN_EXAMPLES.md` ⭐
- `COMMANDS_DE_EN.md`

### Geänderte Dateien:
- `SysBot.Pokemon.Discord/Commands/Bots/TradeModule.cs` ⭐ (mit deutscher Übersetzung)
- `SysBot.Pokemon.Discord/Commands/Bots/QueueModule.cs`
- `SysBot.Pokemon.Discord/Commands/General/PingModule.cs`
- `SysBot.Pokemon.Discord/Commands/General/InfoModule.cs`
- `SysBot.Pokemon.Discord/Commands/General/HelloModule.cs`
- `SysBot.Pokemon.Discord/Commands/Management/HubModule.cs`
- `SysBot.Pokemon.Discord/Helpers/TradeModule/BatchHelpers.cs` ⭐ (mit deutscher Übersetzung)

## Support

Bei Fragen oder Problemen:
1. Überprüfe die Command-Aliases in den jeweiligen Module-Dateien
2. Teste beide Sprachversionen
3. Checke die Logs für Fehler

---
**Entwickelt**: November 2025  
**Basis**: PokeBot von Taku1991 (Fork von hexbyt3)
