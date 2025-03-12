# BitcionPriceTracker
# BitcoinApp

## Popis
BitcoinApp je aplikace pro sledování aktuálních cen Bitcoinu v EUR a jejich převod na CZK pomocí API od Coindesk a ČNB. Data jsou zobrazená v reálném čase a lze je uložit do databáze pro pozdější analýzu. Aplikace je postavena na .NET 8 s využitím WinForms pro uživatelské rozhraní a MSSQL pro ukládání dat.

## Technologie
- **.NET 8**
- **WinForms** (pro uživatelské rozhraní)
- **MSSQL** (pro ukládání dat)
- **Coindesk API** (pro získání cen Bitcoinu v EUR)
- **ČNB API** (pro získání kurzu EUR/CZK)

## Struktura projektu

### Services
- **CnbService**: Služba pro komunikaci s API ČNB a získání aktuálního kurzu EUR/CZK.
- **CoinDeskService**: Služba pro komunikaci s API Coindesk a získání aktuální ceny Bitcoinu v EUR.
- **CoinGeckoService**: (Pokud implementováno) Alternativní služba pro komunikaci s API CoinGecko pro získání cen Bitcoinu.
- **DatabaseService**: Služba pro práci s databází, vkládání a získávání Bitcoinových dat.

### App
- **MainForm**: Hlavní formulář aplikace, který zobrazuje data o Bitcoinu a umožňuje uživatelské interakce.

### Models
- **BitcoinData**: Model pro uchovávání informací o Bitcoinu, včetně ceny v EUR, data, poznámky, atd.

### Database
- **Procedures**: Uložené procedury v databázi pro manipulaci s daty, jako je vkládání Bitcoinových dat, čtení dat atd.
- **SQL Scripts**: Skripty pro inicializaci databáze, vytváření tabulek a dalších objektů v databázi. Včetně skriptů pro zavedení procedur.
- **Wrappers**: Třídy obalující SQL připojení a příkazy pro snadnější testování a manipulaci s databází. Jsou použity pro zjednodušení kódu a zvýšení testovatelnosti aplikace.

## Instalace

1. Klonujte repozitář:
   bash
   git clone https://github.com/bzivica/BitcoinPriceTracker
