<h1><i>Kontrakte</i></h1>

| Operation     | macheBuchungsvorschlag(b:Buchung)                            |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Berechnet mögliche Buchungsvorschläge, die die Buchung erfüllen und nicht mit dem Gesamtbelegungsplan in Konflikt stehen. |
| Vorbedingung  | Die zu b assoziierte Buchung ist erfüllbar.<br/>Der Gesamtbelegungsplan ist konsistent. |
| Nachbedingung | Eine Menge von Buchungsvorschlagsobjekten set(bv) wurde generiert.<br />Buchung wurde ohne Konflikte erstellt. Der Gesamtbelegungsplan ist konsistent. |
| Ergebnisse    | Menge von Buchungsvorschlägen set(bv)                        |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |

| Operation     | starteSimulation(js:JSON-Datei)                              |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Startet die Simulation abhängig von der JSON-Datei.          |
| Vorbedingung  | Die zu js assoziierte JSON-Datei ist erfüllbar.              |
| Nachbedingung | Die assoziierte JSON-Datei definiert set(v) mit Fahrzeuge, set(cs) mit Ladestationen, t Tackt und bd Buchungswunschverteilung.<br />Simulation wird mit set(v), set(cs), t und bd gestartet.           <br />Es muss ein Ergebnis geben. |
| Ergebnisse    | Auslastungsszenario                                          |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |



| Operation     | gebeStandortdatenZurück(Location lo)                         |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Sucht mit Standort zusammenhängende Informationen zusammen und gibt sie zurück. |
| Vorbedingung  | lo ist Standort mit vorhandener Lade-Infrastruktur.          |
| Nachbedingung | Listen mit in lo befindlichen Ladezonen, Ladesäulen und Buchungen wurden erzeugt.                                                                  <br/>Für jede Liste  gilt:<br/>	- Alle Objekte befinden sich in Standort lo;<br/>	- Jede Liste ist erschöpfend. |
| Ergebnisse    | Liste von Ladezonen;<br/>Liste von Ladesäulen;<br/>Liste von Buchungen (Standortbelegungsplan). |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | macheBuchungsvorschlag(b:Buchung)                            |
| Anmerkungen   | ~                                                            |



| Operation     | deleteBuchung(bu:Buchung)                                    |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Löscht die ausgewählte Buchung und gibt die Resourcen (Zeitraum, Ladesäule) wieder frei. |
| Vorbedingung  | Übergebene Buchung existiert und liegt in der Zukunft.<br />Die entsprechenden Rechte für die Löschung sind vorhanden. |
| Nachbedingung | Buchung wurde gelöscht. <br />Ressourcen wie der Zeitraum, Ladesäule werden freigegeben. <br />Es kann eine erneute Buchung mit den zuvor gelöschten Parametern wieder erstellt werden. |
| Ergebnisse    | Boolean                                                      |
| Ausnahmen     | Buchung wurde parallel bereits gelöscht.                     |
| Ausgaben      | Benutzer bekommt Feedback.                                   |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | editBuchung(bu:Buchung, la:Ladestand, nd:Nötige Distanz, st:Start, en:Ende) |

| Operation     | generiereBenutzer(mail:String, id:String, pw:String, role:int) |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Legt einen Benutzer mit den übergebenen Parametern an und pflegt sie ins System ein. |
| Vorbedingung  | Benutzer existiert noch nicht. Constraints der Parameter müssen erfüllt sein. |
| Nachbedingung | Benutzer wurde erstellt. <br />Eindeutige id, Mailadresse sowie das Passwort und die Rolle wurden in der Datenhaltung gespeichert und sind abrufbar.         <br />Der Benutzer hat nun die Möglichkeit sich einzuwählen. |
| Ergebnisse    | Boolean                                                      |
| Ausnahmen     | ~                                                            |
| Ausgaben      | Benutzer bekommt Feedback. (E-Mail für Admin)                |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |

| Operation     | editBuchung(bu:Buchung, la:Ladestand, nd:Nötige Distanz, st:Start, en:Ende) |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Aktualisiert eine bestehende Buchung mit einem oder mehreren geänderten Parametern. |
| Vorbedingung  | Die zu bu assoziierte Buchung existiert. Constraints der Parameter müssen erfüllt sein. |
| Nachbedingung | Buchung bu existiert weiterhin.<br/>Die Werte der Parameter wurden aktualisiert.<br />Mögliche schon gebuchte Ressourcen wurden freigegeben (Assoziationen aufgelöst).                                                             <br/>Neue Assoziationen wurden erstellt. |
| Ergebnisse    | Boolean                                                      |
| Ausnahmen     | Buchung wurde parallel bereits gelöscht oder geändert.       |
| Ausgaben      | Benutzer bekommt Feedback.                                   |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |



| Operation     | sendeTerminBestätigung(be:Benutzer, st:Start, en:Ende)       |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Sendet eine Terminbestätigung an den Nutzer.                 |
| Vorbedingung  | Benutzer hat einen Buchungsvorschlag akzeptiert.<br/>Die Buchung ist gültig und erfüllbar. Constraints der Parameter müssen gültig sein. |
| Nachbedingung | Eine E-Mail oder Browserbenachrichtigung wurde an den betreffenden Benutzer mit den übergebenen Parametern generiert. |
| Ergebnisse    | Erfolg(Boolean)                                              |
| Ausnahmen     | ~                                                            |
| Ausgaben      | Der Benutzer erhält ein Benachrichtigung zu seiner Buchung.  |
| Typ           | Systemoperation                                              |
| Querverweise  | legeBuchungAn(b:Buchung)                                     |
| Anmerkungen   | ~                                                            |



| Operation     | starteSimulation(d:Dauer, fa:Fahrzeuganzahl, ft:Fahrzeugtypen, lz:Ladezone, lo:Location, s:Streuung, t:Tickrate) |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Berechnet eine Simulation die durch einen Verwalter gestartet wird mit Parametern die durch diesen eingegeben wurden. |
| Vorbedingung  | Es gibt einen Verwalter.<br />Es existiert mindestens eine Ladezone lz oder eine Location lo worauf das angewendet wird.                                                      <br />Alle Eingaben sind Konsistent. |
| Nachbedingung | Buchungen wurden mit den Eingabeparametern d, fa, ft, lz, lo, s, t  generiert und mit diesen eine Simulation erzeugt. |
| Ergebnisse    | Berechnung der Auslastung                                    |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |



| Operation     | legeBuchungAn(b:Buchung)                                     |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Trägt eine Buchung in den Gesamtbelegungsplan ein.           |
| Vorbedingung  | Vorschläge passen zum Gesamtbelegungsplan; <br />Gesamtbelegungsplan ist konsistent;<br />Vorschläge müssen auch trozt parallelem Zugriff korrekt verteilt werden. |
| Nachbedingung | Gesamtbelegungsplan ist konsistent. <br />Buchung b wurde akzeptiert. |
| Ergebnisse    | Boolean                                                      |
| Ausnahmen     | Durch parallelele Zugriff und oder zu langes Warten wurde der Vorschlag an mehrere Nutzer gesendet und ausgewählt. Dann darf die Buchung nur bei einem eingetragen werden. |
| Ausgaben      | Benutzer bekommt Feedback.                                   |
| Typ           | Systemoperation                                              |
| Querverweise  | macheBuchungsvorschlag(b:Buchung), editBuchung(bu:Buchung, la:Ladestand, nd:Nötige Distanz, st:Start, en:Ende) |
| Anmerkungen   | ~                                                            |



| Operation     | generiereBuchungen(lp:Ladezonenplan, f:Fahrzeug, b:Batterie, socs:SoC Start, scoz:Soc Ziel, v:Verteilung(sz:Stoßzeiten, min:min, max:max, s:Streuung), t:Tickzeit) |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | System generiert Abfolge von Buchungen um definierten Ladezonenplan auszuwerten. |
| Vorbedingung  | Konstenz bei den Parametern.                                 |
| Nachbedingung | Multimodale Verteilung v der Buchungen mit Modi für Stoßzeiten sz und gegebener Streuung s muss erfüllt sein.<br />Die tatsächliche Verteilung der Fahrzeuge set(f) entspricht in etwa der vorgegebenen Verteilung. |
| Ergebnisse    | Eine zeitliche Abfolge von Buchungen, die der definierten Verteilung entspricht. |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | starteSimulation(d:Dauer, fa:Fahrzeuganzahl, ft:Fahrzeugtypen, lz:Ladezone, lo:Location, s:Streuung, t:Tickrate) |
| Anmerkungen   | ~                                                            |