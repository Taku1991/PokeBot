# Command-Übersicht (Deutsch/Englisch)

## 🎮 Trade Commands

| Englisch | Deutsch | Aliases | Beschreibung |
|----------|---------|---------|--------------|
| `!trade` | `!tausch` | t, tauschen, tau | Startet einen Trade mit Showdown-Set |
| `!hidetrade` | `!verstecktertausch` | ht, vt | Trade ohne Embed-Details |
| `!egg` | `!ei` | Ei | Bestellt ein Pokémon-Ei |
| `!fixOT` | `!reparieren` | fix, f, rep | Repariert OT und Nickname |
| `!medals` | `!medaillen` | ml, md | Zeigt Trade-Anzahl und Medaillen |
| `!tradeUser` | `!tauschBenutzer` | tu, tb | Trade mit einem User (Admin) |

### Beispiele:
```
# Englisch
!trade Pikachu @ Light Ball
!egg Bulbasaur
!medals

# Deutsch
!tausch Pikachu @ Kugelblitz
!ei Bisasam
!medaillen
```

---

## 📋 Queue Commands

| Englisch | Deutsch | Aliases | Beschreibung |
|----------|---------|---------|--------------|
| `!queueStatus` | `!warteschlangeStatus` | qs, ts, ws, position, pos | Zeigt deine Position in der Warteschlange |
| `!queueClear` | `!warteschlangeRaus` | qc, tc, wr | Entfernt dich aus der Queue |
| `!queueList` | `!warteschlangeListe` | ql, wl, liste | Zeigt Queue-Liste (Admin) |
| `!queueClearAll` | `!warteschlangeAllesLöschen` | qca, tca, wal | Leert komplette Queue (Admin) |
| `!queueClearUser` | `!warteschlangeBenutzerRaus` | qcu, tcu, wbr | Entfernt User aus Queue (Admin) |
| `!deleteTradeCode` | `!tauschcodeLöschen` | dtc, tcl | Löscht deinen gespeicherten Code |
| `!queueMode` | `!warteschlangeModus` | qm, wm | Ändert Queue-Modus (Admin) |

### Beispiele:
```
# Englisch
!queueStatus
!queueClear
!position

# Deutsch
!position
!warteschlangeRaus
!pos
```

---

## 📊 Status & Info Commands

| Englisch | Deutsch | Aliases | Beschreibung |
|----------|---------|---------|--------------|
| `!status` | `!statistik` | stats, stat | Zeigt Bot-Status und Statistiken |
| `!info` | `!infos` | about, über | Bot-Informationen |
| `!ping` | `!pong` | - | Prüft ob Bot online ist |
| `!hello` | `!hallo` | hi, moin | Begrüßt den Bot |

### Beispiele:
```
# Englisch
!status
!info
!ping

# Deutsch
!statistik
!infos
!pong
```

---

## 🎯 Kurze Aliases (Schnellzugriff)

Für schnelles Tippen - funktionieren in beiden Sprachen:

| Alias | Command | Sprache |
|-------|---------|---------|
| `!t` | trade | Beide |
| `!tau` | tausch | Deutsch |
| `!ht` | hidetrade | Englisch |
| `!vt` | verstecktertausch | Deutsch |
| `!ml` | medals | Englisch |
| `!md` | medaillen | Deutsch |
| `!pos` | queueStatus/position | Beide |
| `!ws` | warteschlangeStatus | Deutsch |
| `!qs` | queueStatus | Englisch |
| `!wr` | warteschlangeRaus | Deutsch |
| `!qc` | queueClear | Englisch |
| `!wl` | warteschlangeListe | Deutsch |
| `!ql` | queueList | Englisch |
| `!rep` | reparieren/fixOT | Beide |
| `!f` | fix/fixOT | Englisch |

---

## 💡 Tipps

1. **Beide Sprachen funktionieren immer**: Du kannst jederzeit zwischen Deutsch und Englisch wechseln
2. **Kurze Aliases**: Nutze kurze Versionen wie `!t` oder `!pos` zum schnellen Tippen
3. **Groß-/Kleinschreibung egal**: `!Trade`, `!trade`, und `!TRADE` funktionieren alle
4. **Sprachmix möglich**: `!tausch` und `!queueStatus` können gemischt werden

---

## 🌐 Internationale Server

### Für deutsche Hauptserver mit internationalen Spielern:
- **Deutsche Commands**: Hauptsprache für reguläre User
- **Englische Commands**: Für internationale Gäste
- **Kurze Aliases**: Sprachunabhängig und universell

### Beispiel-Szenarien:

**Deutscher Spieler:**
```
!tausch Glurak
!position
!medaillen
```

**Internationaler Spieler:**
```
!trade Charizard
!queueStatus
!medals
```

**Beide kommen in die gleiche Queue und können gleichzeitig den Bot nutzen!**

---

## 📝 Hinweise für Admins

### Admin-Only Commands:
- `!tradeUser` / `!tauschBenutzer` - Trade für einen bestimmten User
- `!queueClearAll` / `!warteschlangeAllesLöschen` - Komplette Queue leeren
- `!queueClearUser` / `!warteschlangeBenutzerRaus` - User aus Queue entfernen
- `!queueMode` / `!warteschlangeModus` - Queue-Modus ändern
- `!queueList` / `!warteschlangeListe` - Queue anzeigen

### Queue-Modi:
```
!queueMode manual
!queueMode threshold
!queueMode interval

# Oder auf Deutsch:
!warteschlangeModus manual
```

---

## 🔧 Weitere Commands (Noch nicht übersetzt)

Diese Commands funktionieren aktuell nur auf Englisch:
- `!clone` - Klonen von Pokémon
- `!dump` - Pokémon-Daten dumpen
- `!seedcheck` - Seed-Check durchführen
- `!dittoTrade` - Spezielle Ditto-Trades
- `!batchTrade` - Batch-Trades

**Nächstes Update**: Diese können bei Bedarf auch übersetzt werden!

---

**Letzte Aktualisierung**: November 2025  
**Version**: 1.0 (Bilingual)
