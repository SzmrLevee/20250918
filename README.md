# C# Oktatóanyag

## Tartalomjegyzék

1. [Rekordkiegészítés](#1-rekordkiegészítés)
2. [Generikusság](#2-generikusság)
3. [Következő témák (hamarosan)](#3-következő-témák-hamarosan)

---

## 1. Rekordkiegészítés

### Kódrészletek

```csharp
// Példányok létrehozása és módosítása
TestRecord peldany1 = new TestRecord("Tigris", 3, 12, "Vadász");
// A 'with' kulcsszóval új rekordpéldányt hozunk létre, ahol csak a megadott mezőket módosítjuk
TestRecord peldany2 = peldany1 with { nev = "Kiscica", beosztas = "Fogyasztó" };

Console.WriteLine(peldany1); // Kiírja az eredeti rekordot
Console.WriteLine(peldany2); // Kiírja a módosított rekordot
```

```csharp
// Rekord definíció – a mezők automatikusan init-only tulajdonságok lesznek
internal record TestRecord(string nev, int eletkor, int fizetes, string beosztas)
{
}
```

### Magyarázat és megjegyzések

**A jegyzetem:**  
> Rekord Kiegészítés  
> Hasznos, ha csak egy egy elemét szeretnék változtatnia a rekordnak. Innentől ha véletlenül megváltozik az adatsor - 1,2 mező, akkor nincsen kihatással a mi működésünkre, hiszen csak azokat változtatjuk, amelyiket mi akarjuk.

**Értelmezés és kiegészítés:**  
- A `record` értékalapú típus, amelynek `with` kulcsszava lehetővé teszi **másolat készítését módosítással**.  
- Ez biztonságosabb, olvashatóbb és kevésbé törékeny, mintha manuálisan másolnánk át minden mezőt.  
- Tipikus felhasználás: immutábilis adatszerkezetek kezelése.

---

## 2. Generikusság

### Kódrészletek

```csharp
// Generikus osztály használata
SajatLista<int> ertekek = new SajatLista<int>(10); // maximum 10 elemű lista
for (int i = 0; i < 5; i++)
{
    // Beszúrás csökkenő sorrendben: 42, 40, 38, 36, 34
    ertekek.HozzaAd(42 - i * 2);
}

// Generikus metódus hívása: típusparamétert explicit adjuk meg
ertekek.MitTudomEnMi<object>("Kiscica");

for (int i = 0; i < ertekek.ElemekSzama; ++i)
{
    Console.WriteLine(ertekek.Elem(i)); // Rendezés miatt növekvő sorrendben írja ki
}
```

```csharp
// Generikus osztály definíció
internal class SajatLista<ElemTipus> where ElemTipus : IComparable<ElemTipus>, new()
{
    ElemTipus[] ertekek;
    int elemszam;
    MyComparer comparer;

    public SajatLista(int maxElemszam)
    {
        this.ertekek = new ElemTipus[maxElemszam];
        elemszam = 0;
        comparer = new MyComparer();
    }

    // Belső comparer osztály, amely null ellenőrzést is végez
    class MyComparer : IComparer<ElemTipus>
    {
        public int Compare(ElemTipus? x, ElemTipus? y)
        {
            if (x == null) return 1;
            if (y == null) return -1;
            return x.CompareTo(y);
        }
    }

    // Generikus metódus: bármilyen típusú értéket elfogad
    public void MitTudomEnMi<MasikTipus>(MasikTipus ertek)
    {
        Console.WriteLine($"{ertek}: {elemszam}");
    }

    // Elem hozzáadása + automatikus rendezés
    public void HozzaAd(ElemTipus ertek)
    {
        if (elemszam >= ertekek.Length)
            throw new Exception("Elfogyott a hely!");

        ertekek[elemszam] = ertek;
        elemszam++;
        Array.Sort(ertekek, 0, elemszam, comparer);
    }

    // Index alapján visszaadja az elemet, határellenőrzéssel
    public ElemTipus Elem(int pozicio)
    {
        if (pozicio < 0 || pozicio > elemszam)
            throw new Exception("Nem megfelelő pozíció!");

        return ertekek[pozicio];
    }

    public int ElemekSzama => elemszam;
}
```

### Magyarázat és megjegyzések

**A jegyzetem:**  
> Generikussag - egy valaminek - pls: egy osztálynak, egy függvénynek, egy intervallumnak egy típusparamétert kell adni  
>
> Mit kell érteni - ha szeretnénk csinálni egy listát - bármilyen elemek  
>
> Hogyan működik a saát listánk? - amikor beírjuk, hogy integer, akkor beírodik a T helyére ez  
> Amit odaírok a program.cs ben a kacsacsőrök közé, az lesz  
> T az gyakran használt elnevezés-  egyes nyelvekben Template a generikussag, de én ezt most kicserélem egy sokkal fejlesztőbarátabb névre-ElemTipus  
>
> Szeretnénk a csökkenő számokat növekvőbe rakni a `ertekek.HozzaAd(42 - i * 2);` esetén.  
> Ezt a `internal class SajatLista<ElemTipus> where ElemTipus: IComparable<ElemTipus>`-al tehetjük meg, hogy össze tudja hasonlítani.  
>
> A comparernek köszönhetően ár növekvőben lesznek az eredményeink.  
>
> `internal class SajatLista<ElemTipus> where ElemTipus: IComparable<ElemTipus>, new()`  
> Következőkben paraméter nélküli konstruktorra kötelezzük.  
>
> **Példa arra, hogy hogyan lehet generikus osztályt írni. - függvényből is!**  
> ```csharp
> public void MitTudomEnMi<MasikTipus>(MasikTipus ertek)
> {
>     Console.WriteLine($"{ertek}: {elemszam}");
> }
> ```  
>
> **Hogyan tudjuk használni?:**  
> ```
> ertekek.MitTudomEnMi("Kiscica");
> ```  
> Ebből már tudni fogja hogy string típusú paraméter.  
>
> **Ha ki akarjuk cserélni:** `ertekek.MitTudomEnMi<object>("Kiscica");`  
> Most már object típusú.  
>
> Néha nem tudja kitalálni – hiba – oda kell írni!  

**Értelmezés és kiegészítés:**  
- A `where ElemTipus : IComparable<ElemTipus>` megszorítás lehetővé teszi a **rendezést**.  
- A `new()` megszorítás paraméter nélküli konstruktor meglétét követeli.  
- A `MitTudomEnMi` bemutatja a **generikus metódus** szintaxist, amely különböző típusokkal hívható meg.  
- A példakód futtatás után rendezett listát ad: `34, 36, 38, 40, 42`.  
- A belső `MyComparer` gondoskodik arról, hogy a `null` értékek is kezelve legyenek.

---

## 3. Következő témák (hamarosan)

---

> ℹ️ Ez a README most még csak az **első két projektet** tartalmazza, de a szerkezete már előkészített a jövőbeli bővítéshez.
