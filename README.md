# Calculator
Slutprojekt Programmering Grund

## Note om Readme  
Sparar all text under, använde readme-ursprungligen för att dokumentera mina tankegångar under arbetets gång.  
(Har skrivit det här i Program också) Använder TODO i koden för att highlighta lite extra analys och utvärderings-tankar



## tldr bigger plan
Start with simple calc, only following the basic interpretation of the requirements  
Then do slightly more advanced version last week  
Practically I plan to analyse what's expected in this readme, then perform a basic implementation, add comments on how it can be improved in that, then branch it in git and iteratively create the advanced version before merging them together.  
  
### First Version
So based on the documentation we should  
Skapa en miniräknare som tar tal och matematiska operator från användarae via konsol. Varje resultat ska sparas i en lista. Kopiera pseudocoden som innehåller instruktioner, klistra in i ditt projekt och följa den under kodande. Analysera och utvärdera gränssnitt, designmönster och lösningar i din kod och föreslå vidare utökningar.  
(skrev ut övre på svenska så råkade defaulta till svenska efteråt för kortslutning i hjärnan)  
Pseudokod beskriver mest flowet på programmet:  
Välkomstmeddelande  
Skapa lista för att spara historik  
Användaren matar in tal och operatorn (obs, måste mata in ett tal för att kunna ta sig vidare i programmet)  
Ifall vi delar med 0 så ska vi visa ogiltig inmatning  
Lägg resultatet i en lista och visa det.  
Fråga om de vill visa tidigare resultat  
Visa det isf  
Fråga om de vill avsluta eller fortsätta  
  
#### Tolkar ner det i följande interaktioner
1. Välkommen
2. *Input*  
Finns ett par sätt att göra input-sektionen men om man väljer det i sin mest basic form kör vi på en serie restrictions: Enbart två tal och en operation. Dessa kan sen antagligen tas som en sträng eller som 3 stycken input (tal 1, tal 2, operator) 
3. Skriv ut om input är /0 eller annars felaktiskt, returnera till steg 2
4. Räkna ut och visa resultatet
5. Prompt om de vill se tidigare resultat, isf 6, annars 7
6. Lista tidigare resultat
7. Prompt om de vill avsluta eller köra igen, ifall köra igen gå till 2
8. Visa och displaya avslutnings-meddelande

Första designmönstret jag kör på helt enkelt att lista det mesta i en huvudklass.  Kommer använda mig av metoder för att bryta upp den större While-loopen
Expanderingen av detta mönster är att gå från metoder till klasser som hanterar de olika stegen, eftersom man då för en större kontroll över att expandera det ytterligare

### Commit 970a385 motsvarar 1.0
  
### Version 2
Aka, vi övergår till klasser, samt vi rör oss bort från bara 2 tal
Finns 2 primära saker som kan flyttas in i klasser - inputhantering (och då parsing), och uträkning, speciellt då uträkningarna blir mer avancerade

Röra oss bort från 2 tal betyder lite ändringar i just parsingen. Vi kan inte längre anta att vi bara har två tal, och bara en operator. Hellre än att splitta strängen tänker jag gå igenom den i delar mellan varje operator och spara delarna som en ny klass till en lista i ProcessedInput. Denna nya klass ska ha ett enum som bestämmer vilken operator / om det är ett värde, samt ett värdesfält.
När vi sedan räknar ut så går vi igenom listan baserat på order of operations och ersätter a operation b med det nya värdet c