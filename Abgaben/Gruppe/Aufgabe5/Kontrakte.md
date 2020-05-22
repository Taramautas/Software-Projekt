<h1><i>Kontrakte</i></h1>

| Operation     | macheBuchungsvorschlag(bw:Buchungswunsch)                    |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Berechnet mögliche Buchungsvorschläge, die den Buchungswunsch erfüllen und nicht mit dem Gesamtbelegungsplan in Konflikt stehen. |
| Vorbedingung  | Der zu bw assoziierte Buchungswunsch ist erfüllbar.<br/>Der Gesamtbelegungsplan ist konsistent. |
| Nachbedingung | Eine Menge von Buchungsvorschlagsobjekten set(bv) wurde generiert.<br />Buchung wurde ohne Konflikte erstellt. |
| Ergebnisse    | Menge von Buchungsvorschlägen set(bv)                        |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |

| Operation     | starteSimulation(js:JSON-Datei)                              |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Startet die Simulation abhängig von der JSON-Datei.          |
| Vorbedingung  | Der zu js assoziierte JSON-Datei ist erfüllbar.              |
| Nachbedingung | Simulation wird mit den richtigen Parametern der JSON-Datei gestartet.                             <br />Es muss ein Ergebnis geben, obwohl keine oder zu viele gibt. |
| Ergebnisse    | Auslastungsszenario                                          |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |



| Operation     | gebeStandortdatenZurück()                                    |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Sucht mit Standort zusammenhängende Informationen zusammen und gibt sie zurück. |
| Vorbedingung  | Gültiger Standort wurde ausgewählt.                          |
| Nachbedingung | - Standort wurde mit darin befindlichen Ladezonen in Verbindung gesetzt<br/>- Steckertypen aller dieser Ladezonen wurden aufgelistet<br/>- Ladesäulen innerhalb des Standorts wurden gezählt<br/>- Auslastung des Standorts wurde aus Buchungsplan herausgezogen |
| Ergebnisse    | Anzahl der Ladesäulen, Informationen zu Ladezonen innerhalb des Standorts, vorhandene Steckertypen und Auslastung des Standorts. |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |



| Operation     | visualisiereSimulation()                                     |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Stellt Informationen aus Simulation graphisch dar.           |
| Vorbedingung  | Simulation wurde erfolgreich durchgeführt.                   |
| Nachbedingung | Aus Simulation gewonnene Informationen wurden mithilfe von Graphen, Tabellen und Diagrammen dargestellt. |
| Ergebnisse    | Graphen, Diagramme, Tabellen.                                |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | starteSimulation(), simuliereBuchungen()                     |
| Anmerkungen   | ~                                                            |



| Operation     | deleteBuchung(bu:Buchung)                                    |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Löscht die ausgewählte Buchung und gibt die Resourcen (Zeitraum, Ladesäule) wieder frei. |
| Vorbedingung  | Buchung existiert.                                           |
| Nachbedingung | Buchung wurde gelöscht.                                      |
| Ergebnisse    | Boolean                                                      |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |



| Operation     | generiereBenutzer(mail:String, id:String, pw:String, role:int) |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Legt einen Benutzer mit den Übergebenen Parametern an und pflegt sie ins System. |
| Vorbedingung  | Benutzer existiert noch nicht.                               |
| Nachbedingung | Benutzer wurde erstellt.                                     |
| Ergebnisse    | Boolean                                                      |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |

| Operation     | editBuchung(bu:Buchung, la:Ladestand, nd:Nötige Distanz, st:Start, en:Ende) |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Aktualisiert eine bestehende Buchung, mit einem oder mehreren geänderten Parametern. |
| Vorbedingung  | Eine valide Buchung besteht, neue Buchung ist im Rahmen des möglichen. |
| Nachbedingung | Eine aktualisierte valide Buchung wurde bearbeitet, und hat jetzt neue Parameterwerte. |
| Ergebnisse    | Liste der Buchungen                                          |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |



| Operation     | sendeTermin(be:Benutzer, st:Start, en:Ende)                  |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Sendet einen Termin an einen Benutzer.                       |
| Vorbedingung  | Benutzer hat eine Buchung aktzeptiert                        |
| Nachbedingung | Benutzer hat eine Terminnotification bekommen, Buchung wurde ohne Konflikte erstellt. |
| Ergebnisse    | Erfolg(Boolean)                                              |
| Ausnahmen     | ~                                                            |
| Ausgaben      | Terminnotification an Benutzer                               |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |



| Operation     | starteSimulation(dauer, Fahrzeuganzahl, Fahrzeugtypen, Ladezone, Standort, Streuung) |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Berechnet eine Simulation die durch einen Verwalter gestartet wird mit Paramatern die durch diesen eingegeben wurden. |
| Vorbedingung  | Es gibt einen Verwalter, es existiert eine Ladezone worauf das angewendet wird, alle Eingaben sind Konsistent. |
| Nachbedingung | Es muss ein Ergebnis geben egal ob zuviele Buchungen gab oder zu wenige |
| Ergebnisse    | Berechnung Auslastung                                        |
| Ausnahmen     | ~                                                            |
| Ausgaben      | Berechnung wird an evaluiereAuslastungsszenario geschickt.   |
| Typ           | Systemoperation                                              |
| Querverweise  | evaluiereAuslastungsszenario(Berechnung des Auslastungsszenarios) |
| Anmerkungen   | ~                                                            |



| Operation     | evaluiereAuslastungsszenario(Berechnung des Auslastungsszenarios) |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Wertet das Auslastungszenario aus.                           |
| Vorbedingung  | Es muss ein Auslastungsszenario geben.                       |
| Nachbedingung | Es muss Konsitent sein.                                      |
| Ergebnisse    | Information für die Visualisierung des Auslastungsszenarions. |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | starteSimulation(), visualisiereSimulation()                 |
| Anmerkungen   | ~                                                            |

| Operation     | legeBuchungAn(bv:Buchungsvorschlag)                          |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Nutzer wählt einen der Buchungsvorschläge aus.               |
| Vorbedingung  | - Vorschläge passen zum Gesamtbelegungsplan; <br />- Gesamtbelegungsplan ist konsistent;<br />- Vorschläge müssen auch trozt parallelem Zugriff korrekt verteilt werden. |
| Nachbedingung | Gesamtbelegungsplan ist konsistent.                          |
| Ergebnisse    | Visuelles Feedback ob Buchung erfolgreich angelegt.          |
| Ausnahmen     | Durch parallelele Zugriff und oder zu langes Warten wurde der Vorschlag an mehrere Nutzer gesendet und ausgewählt. Dann darf die Buchung nur bei einem eingetragen werden. |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |



| Operation     | simuliereBuchungen(Ladezonenplan, Anzahl Fahrzeuge, Batterie, SoC Start, Soc Ziel, Verteilung(Stoßzeiten, min/max Streeung), Tickzeit) |
| :------------ | :----------------------------------------------------------- |
| Beschreibung  | Systeem generiert Abfolge von Buchungen um definierten Ladezonenplan auszuwerten |
| Vorbedingung  | Konstenz bei den Parametern.                                 |
| Nachbedingung | ~                                                            |
| Ergebnisse    | Eine zeitliche Abfolge von Buchungen, die der definierten Verteilung entspricht |
| Ausnahmen     | ~                                                            |
| Ausgaben      | ~                                                            |
| Typ           | Systemoperation                                              |
| Querverweise  | ~                                                            |
| Anmerkungen   | ~                                                            |