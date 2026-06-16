# Tokenizacija platnog sistema zatvorene petlje zasnovanog na blokčejn tehnologiji

> Rešenje za tokenizaciju digitalnih novčanika u zatvorenim platnim sistemima upotrebom mikroservisne arhitekture, Merkelovog stabla i Ethereum blokčejna.

**Autor:** Đorđe Šević
**Mentor:** dr Vladimir Mandić, vanredni profesor
**Institucija:** Fakultet tehničkih nauka, Univerzitet u Novom Sadu
**Tip rada:** Diplomski (Bachelor) rad — Inženjerstvo informacionih sistema
**Datum odbrane:** 13.11.2024.

---

## Sadržaj

- [O projektu](#o-projektu)
- [Motivacija](#motivacija)
- [Teorijska osnova](#teorijska-osnova)
- [Zahtevi rešenja](#zahtevi-rešenja)
- [Arhitektura sistema](#arhitektura-sistema)
- [Mikroservisi](#mikroservisi)
- [Implementacija — ključne komponente](#implementacija--ključne-komponente)
- [Evaluacija nad ZeroCash sistemom](#evaluacija-nad-zerocash-sistemom)
- [Prednosti i ograničenja](#prednosti-i-ograničenja)
- [Predlozi za dalji razvoj](#predlozi-za-dalji-razvoj)
- [Tehnologije](#tehnologije)
- [Struktura priloga](#struktura-priloga)
- [Zaključak](#zaključak)

---

## O projektu

Ovaj rad istražuje implementaciju **tokenizacije** u sistemima plaćanja zatvorene petlje (closed-loop) korišćenjem blokčejn tehnologije, digitalnih novčanika i tokena, u cilju poboljšanja sigurnosti transakcija i zaštite podataka korisnika.

Cilj rada je dizajn i implementacija sistema za tokenizaciju platnog sistema zatvorene petlje upotrebom blokčejna, digitalnih novčanika i tokena. Blokčejn tehnologija se koristi za čuvanje korena Merkelovog stabla generisanog nad transakcijama koje su nastale kreiranjem tokena određenih digitalnih valuta unutar zatvorenog platnog sistema.

Sistem je implementiran upotrebom **mikroservisne arhitekture** i **C# / .NET** okvira, kroz četiri mikroservisa:

| Mikroservis | Odgovornost |
|---|---|
| **WalletMS** | Čuvanje i upravljanje digitalnim novčanicima i tokenima |
| **TokenMS** | Kreiranje, validacija i verzionisanje tokena |
| **TransactionMS** | Kreiranje transakcija različitih tipova (deposit, payment, withdraw) |
| **BlockchainMS** | Upis dokaza o izvršenim transakcijama u Ethereum blokčejn |

---

## Motivacija

Otvoreni sistemi plaćanja (Visa, MasterCard) nose visoke transakcione takse — trgovci globalno godišnje plate preko 138 milijardi dolara na takse, a na malim transakcijama dodatni trošak može premašiti 12,5% vrednosti same transakcije. Zatvoreni sistemi plaćanja rešavaju ovaj problem nižim transakcionim troškovima, ali zauzvrat imaju manje sredstava za ulaganje u bezbednosnu infrastrukturu koju otvoreni sistemi finansiraju upravo iz tih taksi.

Broj sajber napada u finansijskoj industriji globalno je rastao iz godine u godinu, sa 3.348 zabeleženih incidenata u 2023. godini — najviše u opisanom periodu od 2013. do 2023. godine. Prosečan gubitak usled povrede podataka u finansijskoj industriji 2023. godine iznosio je oko 5,9 miliona dolara, a jedan od najpoznatijih primera (napad na Equifax 2017. godine) izložio je lične podatke oko 147 miliona ljudi.

**Tokenizacija** se nameće kao rešenje koje ne zahteva velika finansijska ulaganja, a značajno povećava bezbednost zatvorenih platnih sistema — što je upravo problem koji ovaj rad adresira.

### Poznati primeri zatvorenih sistema plaćanja

- **Starbucks** — mobilna aplikacija sa digitalnim novčanicima; 2022. godine korisnici su na novčanicima držali 1,6 milijardi dolara, više nego što neke banke imaju u depozitima.
- **Disney MagicBand** — uređaj za ulaz u park, hotel, restorane i kupovinu unutar Disney ekosistema.
- **ClassPass** — pretplatnička fitnes platforma sa zatvorenim modelom plaćanja.
- **Cineplexx bonus kartica** — primer zatvorenog sistema na nivou Srbije, sa bonus bodovima i popustima.

---

## Teorijska osnova

### Otvoreni vs. zatvoreni sistemi plaćanja

| Karakteristika | Zatvoreni sistem | Otvoreni sistem |
|---|---|---|
| Model | Tri-partijski (transakcije se autorizuju unutar jedne platforme) | Četvoro-partijski (uključuje banku platioca, banku primaoca, platni sistem) |
| Mrežna infrastruktura | Ograničena na specifične trgovce/mrežu | Široko prihvaćena |
| Transakcione takse | Niže | Više |
| Kontrola podataka | Veća kontrola nad podacima o potrošačima | Manja kontrola |
| Fleksibilnost | Ograničena upotrebljivost | Veća fleksibilnost i prihvaćenost |
| Primeri | Starbucks, Disney MagicBand, Cineplexx | Visa, MasterCard |

### Digitalni novčanici

Tri kategorije digitalnih novčanika:

1. **Otvoreni** — izdaju ih isključivo banke; korisnici mogu slati novac na bilo koji bankovni račun.
2. **Poluzatvoreni** — transakcije su moguće samo sa partnerima vlasnika sistema; gotovina se ne može podizati.
3. **Zatvoreni** — koriste se isključivo unutar jedne kompanije/ekosistema; često se koriste za poklon kartice.

### Tokeni i zamenljivost (fungibility)

Tokeni mogu predstavljati digitalna sredstva (kriptovalute), prava pristupa ili tokenizovanu imovinu iz realnog sveta. Prema zamenljivosti razlikuju se:

- **Zamenljivi tokeni (fungible)** — identični i deljivi, npr. platni tokeni.
- **Nezamenljivi tokeni (NFT)** — kriptografski jedinstveni, prate vlasništvo nad pojedinačnim sredstvom (standard ERC-721).
- **Poluzamenljivi tokeni (semi-fungible)** — kombinuju karakteristike prethodna dva tipa.

### Blokčejn

Blokčejn je struktura podataka organizovana kao lanac blokova, gde svaki blok sadrži transakcije, vremensku oznaku, heš prethodnog bloka i nonce. Nepromenljivost se postiže kombinacijom kriptografskog heširanja, mehanizama konsenzusa (proof of work, proof of stake), decentralizacije, pametnih ugovora i Merkelovog stabla.

Sistem korišćen u ovom radu je **Ethereum** — javni blokčejn sa ugrađenim Turing-complete jezikom za pisanje pametnih ugovora.

### Heš funkcije

U implementaciji se koriste dve vrste heš funkcija:

- **Interna heš funkcija (metod univerzalnog hešovanja)** za heširanje i validaciju polja tokena — implementirana kao `HashFunctionV1`.
- **SHA-256** (iz `System.Security.Cryptography`) za generisanje čvorova Merkelovog stabla — generiše heš dužine 256 bita (64 heksadecimalne cifre), znatno bezbedniji od zastarelog SHA-1.

### Merkelovo stablo

Merkelovo stablo je kriptografska struktura koju je 1979. uveo Ralph Merkle. To je kompletno binarno stablo gde se vrednost svakog roditeljskog čvora računa kao heš konkatenacije njegove dece:

```
φ(n_parent) = hash(φ(n_left) || φ(n_right))
```

Za verifikaciju lista nije potrebno celo stablo — dovoljna je **autentifikaciona putanja** (niz heš vrednosti braće čvorova na putu do korena), čime se verifikacija obavlja u logaritamskoj vremenskoj kompleksnosti u odnosu na broj transakcija.

### Mikroservisna arhitektura i C4 model

Mikroservisna arhitektura razbija sistem na male, nezavisne servise sa fokusiranom poslovnom funkcijom, što donosi prednosti u održivosti, skalabilnosti, zamenljivosti, toleranciji na greške i pouzdanosti. Mikroservisi se dele i na **stateless** (lako skalabilni, npr. TokenMS, BlockchainMS) i **stateful** (teže skalabilni jer zahtevaju koordinaciju replika, npr. WalletMS, TransactionMS).

Za modelovanje arhitekture korišćen je **C4 model**, koji opisuje sistem kroz četiri nivoa apstrakcije: kontekst, kontejneri, komponente i kod.

---

## Zahtevi rešenja

| Oznaka | Zahtev |
|---|---|
| Z1 | Sistem za autentifikaciju i autorizaciju korisnika |
| Z2 | Kreiranje i upravljanje novčanicima |
| Z3 | Kreiranje i upravljanje tokenima |
| Z4 | Verzionisanje tokena i rad sistema sa različitim verzijama tokena |
| Z5 | Validacija tokena |
| Z6 | Kreiranje različitih tipova transakcija (deponovanje, plaćanje, skidanje sredstava) |
| Z7 | Validacija transakcije |
| Z8 | Upis dokaza o izvršetku transakcija u blokčejn |

---

## Arhitektura sistema

### Nivo 1 — Kontekst

Sistem ima dve korisničke uloge:

- **Employee** (zaposleni) — izvršava transakcije plaćanja.
- **Admin** — dodatno može deponovati i skidati sredstva sa novčanika zaposlenih.

Sistem (Payment System) komunicira sa:

- **Authentication System** — Zitadel SSO za autentifikaciju.
- **Blockchain** — javni blokčejn (npr. Ethereum) za upis dokaza o transakcijama.

### Nivo 2 — Kontejneri

| Kontejner | Opis | Tehnologije |
|---|---|---|
| Mobile App | Mobilna aplikacija za zaposlene za izvršavanje transakcija | Java Android |
| Frontend Application | Veb aplikacija za zaposlene i admine | Angular |
| API Gateway | Ulazna tačka backend sistema — autentifikacija, rutiranje, rate limiting, skaliranje | ASP.NET, Yarp |
| WalletMS | Kreiranje i upravljanje digitalnim novčanicima | ASP.NET, Entity Framework Core, Redis, Azure Key Vault |
| TokenMS | Kreiranje, validacija, verzionisanje tokena | ASP.NET |
| TransactionMS | Kreiranje i upisivanje transakcija | ASP.NET, Entity Framework Core |
| BlockchainMS | Upis dokaza o transakcijama u javni blokčejn | ASP.NET |
| WalletDb | Perzistencija novčanika i tokena | SQLite |
| TransactionBankDb | Perzistencija svih izvršenih transakcija | SQLite |

API Gateway predstavlja jedinu ulaznu tačku iz spoljašnjeg sveta — sve iza njega je zaštićeno privatnom mrežom. On ispunjava zahtev **Z1** (autentifikacija/autorizacija), a takođe obavlja rutiranje zahteva, rate limiting i omogućava skaliranje mikroservisa.

> **Napomena:** Mobilna i veb aplikacija, kao ni postojeći (pre-tokenizacioni) backend sistem plaćanja, nisu predmet ovog rada — uključeni su u dijagram radi celovitog prikaza arhitekture u kojoj novi tokenizovani sistem radi uporedo sa postojećim sistemom (ZeroCash), kako bi se mogle uočiti razlike u tačnosti i performansama.

---

## Mikroservisi

### WalletMS

Sastoji se od `WalletController`-a i četiri servisa:

- **Redis servis** — upravljanje Redis kešom; generičke metode `GetListAsync` i `StoreListAsync` koriste se za zaključavanje novčanika tokom izvršavanja transakcije (sprečava konkurentne izmene istog novčanika).
- **Azure Key Vault servis** — bezbedno čuvanje veze između korisnika i ID-a njegovog novčanika.
- **Wallet servis** — `GetWallet`, `CreateWallet`, `GetWalletsForTransaction`.
- **Token servis** — `GetTokenForTransaction`, `GetTokensForTransaction`, `UpdateTokensByTransaction`.

Ovim mikroservisom ispunjava se zahtev **Z2**.

### TokenMS

Stateless mikroservis (lako skalabilan) sa `TokenController`-om i Token servisom koji izlaže metode `CreateNewToken` i `ValidateToken`. Implementira **Factory Method** dizajnerski obrazac za kreiranje instanci tokena različitih verzija — ispunjava zahteve **Z3, Z4 i Z5**.

#### Zašto Factory Method

Pošto sistem mora da podrži višestruke verzije tokena (Z4), tačan tip objekta koji treba kreirati zavisi od verzije koja se trenutno koristi. Factory Method rešava ovo izdvajanjem logike kreiranja na jedno mesto, čime se poštuju princip jedinstvene odgovornosti (SRP) i princip otvoreno-zatvoreno (OCP) — nove verzije tokena dodaju se bez izmene postojećeg koda. Cena ovog pristupa je veća kompleksnost koda usled dodatnih klasa po svakoj verziji.

### TransactionMS

`TransactionController` izlaže tri endpointa: `CreateTransactionPayment`, `CreateTransactionDeposit`, `CreateTransactionWithdraw`. Transaction servis komunicira sa TokenMS (kreiranje tokena) i WalletMS (dobavljanje tokena), a metoda `CreateTransactionToProcess` kreira objekat koji se zatim dodaje u **transaction pool** pozivom `AddTransactionToPool`.

**Transaction Hosted Service** periodično (na 10 sekundi) prikuplja transakcije iz pool-a, generiše Merkelovo stablo i validacione putanje, te poziva BlockchainMS da upiše Merkelov koren u blokčejn. Ispunjava zahteve **Z6 i Z7**.

### BlockchainMS

`BlockchainController` sa Blockchain servisom koji upisuje Merkelov koren na javni blokčejn koristeći **Nethereum** biblioteku. Ispunjava zahtev **Z8**.

---

## Implementacija — ključne komponente

### Verzionisani tokeni (Factory Method)

Svaka verzija tokena implementira `IToken` interfejs:

```csharp
public interface IToken
{
    bool IsTokenValid();
    TokenDto ToTokenDto();
}
```

`TokenV1` implementira ovaj interfejs i koristi sopstvenu heš funkciju (`HashFunctionV1`) za potpisivanje i validaciju svih polja tokena (ID, adresa novčanika, balans, verzija, vreme kreiranja, tip i ID transakcije):

```csharp
public bool IsTokenValid()
{
    bool isValid = true;
    if (TokenDataSignature != HashFunction(CreateIntArrayForSignatureHashCalculation().ToByteArray()))
        isValid = false;
    if (TokenIdHash != HashFunction(TokenId.ToByteArray()))
        isValid = false;
    if (WalletAddressHash != HashFunction(WalletAddress.ToByteArray()))
        isValid = false;
    // ... validacija preostalih hash-ovanih polja TokenData strukture
    return isValid;
}
```

`TokenFactory` obezbeđuje tri operacije nad tokenima — uvek vraća instancu **najnovije** verzije tokena:

```csharp
public class TokenFactory
{
    public IToken GetToken(string version, TokenDto token)
    {
        switch (version)
        {
            case "1.0": return new TokenV1(token);
            default: throw new Exception();
        }
    }

    public IToken CreateToken(Guid walletAddress, decimal amount,
        Guid transactionId, TransactionTypeEnum transactionType, TokenTypeEnum tokenType)
    {
        if (transactionType == TransactionTypeEnum.Withdraw)
            throw new Exception("Token creation is not possible for withdraw transaction type.");
        return new TokenV1(walletAddress, amount, transactionId, tokenType);
    }

    public IToken UpdateToken(TokenDto tokenDto, TransactionTypeEnum transactionType,
        decimal amount, bool? isFrom)
    {
        var token = new TokenV1(tokenDto);
        if (transactionType == TransactionTypeEnum.Deposit ||
            (transactionType == TransactionTypeEnum.Payment && isFrom == false))
            token.TokenData.Balance += amount;
        if (transactionType == TransactionTypeEnum.Withdraw ||
            (transactionType == TransactionTypeEnum.Payment && isFrom == true))
            token.TokenData.Balance -= amount;
        return token;
    }
}
```

### Generisanje Merkelovog korena i validacionih putanja

`MerkleRoot` statička klasa koristi SHA-256 (preko delegata `HashFunc`) za izgradnju stabla odozdo prema gore, sažimajući parove čvorova do jedinstvenog korena:

```csharp
public static string GenerateMerkleRoot(List<string> transactions)
{
    if (transactions.Count == 0) return string.Empty;

    List<string> currentLevel = transactions.Select(HashFunc).ToList();

    while (currentLevel.Count > 1)
    {
        List<string> nextLevel = new List<string>();
        for (int i = 0; i < currentLevel.Count; i += 2)
        {
            if (i + 1 < currentLevel.Count)
                nextLevel.Add(HashFunc(currentLevel[i] + currentLevel[i + 1]));
            else
                nextLevel.Add(currentLevel[i]);
        }
        currentLevel = nextLevel;
    }
    return currentLevel[0];
}
```

SHA-256 implementacija je izdvojena u posebnu klasu radi lake zamenjivosti:

```csharp
public static class HashFunctions
{
    public static string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            var hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
```

### Pametni ugovor (Solidity)

Za upis dokaza o transakcijama u Ethereum blokčejn korišćen je minimalan pametni ugovor, razvijen u Remix IDE i deployovan na **Sepolia test mrežu** preko Infura platforme (testiranje finansirano testnim SepoliaETH tokenima dobijenim putem Google Cloud Web3 servisa):

```solidity
// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract SimpleStorage {
    string public storedData;

    function set(string memory x) public {
        storedData = x;
    }

    function get() public view returns (string memory) {
        return storedData;
    }
}
```

Funkcija `set` poziva se iz BlockchainMS-a (preko Nethereum biblioteke) i upisuje vrednost Merkelovog korena kao dokaz o paketu izvršenih transakcija.

> Napomena o trošku: svaki upis u Ethereum blokčejn zahteva gas, čija cena varira. Tokom razvoja, jedan upis je iznosio oko 0,35 USD. Što je veća frekvencija transakcija u definisanom vremenskom prozoru prikupljanja (trenutno 10 sekundi), to je manji trošak upisa *po transakciji*, jer se jedan Merkelov koren odnosi na čitav paket transakcija.

---

## Evaluacija nad ZeroCash sistemom

Rešenje je evaluirano poređenjem sa **ZeroCash** sistemom — postojećim tradicionalnim zatvorenim platnim sistemom koji ne koristi tokenizaciju ni blokčejn. Iste transakcije su izvršene u oba sistema:

| Transakcija | Tip | Vrednost | Od | Za |
|---|---|---|---|---|
| T1 | deposit | 1000.37 | — | Korisnik A |
| T2 | deposit | 2000.51 | — | Korisnik B |
| T3 | payment | 100.13 | Korisnik A | Korisnik B |
| T4 | withdraw | 253.78 | Korisnik B | — |

**Rezultat:** Stanja sredstava korisnika su identična u oba sistema nakon izvršenih transakcija, što potvrđuje tačnost i konzistentnost implementiranog rešenja. Dodatno, dokazi o transakcijama uspešno su upisani u Ethereum blokčejn (verifikovano preko Etherscan-a), a vrednosti `EthereumProof` u bazi transakcija implementiranog rešenja se poklapaju sa stvarnim `TransactionHash` vrednostima na blokčejnu.

**Bezbednosna prednost:** za razliku od ZeroCash sistema, implementirano rešenje na nivou perzistencije ne čuva direktnu informaciju o tome kom korisniku pripada koji novčanik (ta veza je izdvojena u Azure Key Vault), što smanjuje izloženost u slučaju povrede baze podataka.

**Performanse:** ZeroCash je u testiranom obimu (mali broj transakcija, lokalno okruženje) radio brže od tokenizovanog rešenja. Međutim, zbog mogućnosti horizontalnog skaliranja mikroservisa i načina rada implementiranog Merkelovog stabla, očekuje se da bi tokenizovano rešenje bilo konkurentnije pri većem volumenu transakcija.

---

## Prednosti i ograničenja

### Prednosti

- **Bezbednost** — kombinacija tokenizacije, upisa u blokčejn i zaključavanja novčanika tokom transakcije pruža otpornost na dvostruko trošenje (double spending) i neovlašćeno menjanje transakcija.
- **Niže i fleksibilnije transakcione takse** — takse se mogu definisati kao procenat vrednosti transakcije, čime se izbegava problem fiksnih taksi koje disproporcionalno pogađaju manje transakcije (karakterističan problem otvorenih sistema).
- **Kontrola pristupa putem aktivnog direktorijuma** — centralizovano upravljanje korisničkim nalozima, lako sprovođenje politika lozinki i višefaktorske autentifikacije.
- **Potpuna kontrola nad sistemom** — nezavisnost od spoljnih platnih sistema i regulativa koje obično nameću otvoreni sistemi.

### Ograničenja

- **Zavisnost od aktivnog direktorijuma** — integracija zahteva da organizacija već poseduje svoj AD; ako AD otkaže, ceo platni sistem postaje nedostupan.
- **Ručno deponovanje sredstava** — zahteva fizički kontakt sa administratorom, što ograničava skalabilnost, uvodi zavisnost od radnog vremena administratora i povećava rizik od ljudske greške.
- **Neograničena veličina Merkelovog stabla** — `GenerateMerkleRoot` metoda ne ograničava broj transakcija u jednom prozoru prikupljanja, što pri visokoj frekvenciji transakcija može degradirati performanse verifikacije.
- **Stateful mikroservisi (WalletMS, TransactionMS)** teže se skaliraju od stateless mikroservisa (TokenMS, BlockchainMS), jer zahtevaju koordinaciju između replika.

---

## Predlozi za dalji razvoj

1. **Upis dokaza u više javnih blokčejnova** — povećana otpornost i transparentnost, smanjen rizik od gubitka podataka.
2. **Zaključavanje na nivou tipa tokena**, ne celog novčanika — omogućilo bi paralelno izvršavanje više transakcija različitih tipova tokena za istog korisnika.
3. **Integracija otvorenog bankarstva** za deponovanje sredstava — eliminisala bi potrebu za fizičkim kontaktom sa administratorom i faktor ljudske greške.
4. **Menjačnica unutar sistema** za konverziju između različitih tipova tokena.

---

## Tehnologije

| Kategorija | Tehnologija |
|---|---|
| Backend | C#, .NET, ASP.NET Core |
| API Gateway | Yarp (reverse proxy) |
| ORM | Entity Framework Core |
| Baza podataka | SQLite (WalletDb, TransactionBankDb) |
| Keš / zaključavanje | Redis |
| Upravljanje tajnama | Azure Key Vault |
| Autentifikacija | Zitadel SSO (OAuth 2 / OIDC) |
| Blokčejn integracija | Nethereum (.NET biblioteka za Ethereum) |
| Pametni ugovori | Solidity, Remix IDE, Infura, Sepolia test mreža |
| Heširanje | Interni univerzalni heš (TokenV1), SHA-256 (Merkelovo stablo) |
| Mobilna aplikacija | Java (Android) |
| Frontend | Angular |
| Modelovanje arhitekture | C4 model |

---

## Struktura priloga

Kompletan izvorni kod za sve kontrolere i servisne klase nalazi se u prilozima originalnog rada:

| Prilog | Sadržaj |
|---|---|
| A | `Program.cs` — API Gateway |
| B | `appsettings.json` — API Gateway konfiguracija (Yarp rute i klasteri) |
| C | Wallet kontroler (WalletMS) |
| D | Wallet servisna klasa (WalletMS) |
| E | Token servisna klasa (WalletMS) |
| F | Redis servisna klasa (WalletMS) |
| G | Azure Key Vault servisna klasa (WalletMS) |
| H | Token kontroler (TokenMS) |
| I | Token servisna klasa (TokenMS) |
| J | Transaction kontroler (TransactionMS) |
| K | Transaction servisna klasa (TransactionMS) |
| L | TransactionPool hostovana servisna klasa (TransactionMS) |
| M | Blockchain kontroler (BlockchainMS) |
| N | Ethereum servisna klasa (BlockchainMS) |

---

## Zaključak

Rad pokazuje da zatvoreni sistemi plaćanja, uz implementaciju tokenizacije i blokčejn tehnologije, mogu značajno doprineti unapređenju bezbednosti i smanjenju troškova transakcija. Tokenizacija u kombinaciji sa blokčejnom omogućila je zaštitu osetljivih podataka korisnika i poboljšanu transparentnost sistema, dok je upotreba aktivnog direktorijuma olakšala kontrolu pristupa i upravljanje korisnicima.

Evaluacija nad ZeroCash sistemom potvrdila je tačnost i konzistentnost implementiranog rešenja, uz identifikovana ograničenja u domenu skalabilnosti pri velikom broju transakcija i zavisnosti od ručnih administrativnih procedura — što ostavlja prostor za dalja unapređenja poput multi-blockchain upisa, granularnijeg zaključavanja tokena i integracije otvorenog bankarstva.

---

## Napomena o izvoru

Ovaj README je sažetak diplomskog rada *"Jedno rešenje za tokenizaciju platnog sistema zatvorene petlje zasnovanog na blokčejn tehnologiji"*, odbranjenog na Fakultetu tehničkih nauka u Novom Sadu, 13.11.2024. Kompletan rad sa svim referencama, dijagramima i izvornim kodom dostupan je u bibliotečkom fondu Fakulteta tehničkih nauka, Novi Sad.
