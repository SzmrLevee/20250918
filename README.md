# C# Oktatóanyag – Rekordkiegészítés és Generikusság

> Ez a dokumentum egy **folyamatosan bővülő** anyag, amely C# nyelvi elemeket mutat be gyakorlati példákkal.  
> Jelenleg két fő fejezet található benne: **rekordkiegészítés** és **generikusság**.  
> Később további **négy fejezet** érkezik ehhez a tananyaghoz.

---

## Tartalomjegyzék

1. [Rekordkiegészítés](#1-rekordkiegészítés)
   - Külön fájlok
   - Egyben
2. [Generikusság](#2-generikusság)
   - Külön fájlok
   - Egyben
3. [Következő témák (hamarosan)](#3-következő-témák-hamarosan)

---

## 1. Rekordkiegészítés

A **rekordkiegészítés** (`with`) lehetőséget ad arra, hogy egy meglévő rekord példányból készítsünk másolatot, **csak a megadott mezők módosításával**.

### Külön fájlok

#### Program.cs

```csharp
using Rekord_Kiegeszites;

TestRecord peldany1 = new TestRecord("Tigris", 3, 12, "Vadász");
TestRecord peldany2 = peldany1 with { nev = "Kiscica", beosztas = "Fogyasztó" };

Console.WriteLine(peldany1);
Console.WriteLine(peldany2);
```

#### TestRecord.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rekord_Kiegeszites
{
    internal record TestRecord(string nev, int eletkor, int fizetes, string beosztas)
    {
    }
}
```

### Egyben

```csharp
using Rekord_Kiegeszites;

TestRecord peldany1 = new TestRecord("Tigris", 3, 12, "Vadász");
TestRecord peldany2 = peldany1 with { nev = "Kiscica", beosztas = "Fogyasztó" };

Console.WriteLine(peldany1);
Console.WriteLine(peldany2);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rekord_Kiegeszites
{
    internal record TestRecord(string nev, int eletkor, int fizetes, string beosztas)
    {
    }
}
```

---

## 2. Generikusság

A **generikusság** lehetővé teszi, hogy típusparamétereket adjunk osztályoknak és metódusoknak, így **bármilyen típushoz újrafelhasználhatók** lesznek.

### Külön fájlok

#### Program.cs

```csharp
using Generikussag;

SajatLista<int> ertekek = new SajatLista<int>(10);
for (int i = 0; i < 5; i++)
{
    ertekek.HozzaAd(42 - i * 2);
}

ertekek.MitTudomEnMi<object>("Kiscica");

for (int i = 0; i < ertekek.ElemekSzama; ++i)
{
    Console.WriteLine(ertekek.Elem(i));
}
```

#### SajatLista.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generikussag
{
    // EGY típusú ElemTipus paraméteres
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

        class MyComparer : IComparer<ElemTipus>
        {
            public int Compare(ElemTipus? x, ElemTipus? y)
            {
                if (x == null) return 1;
                if (y == null) return -1;
                return x.CompareTo(y);
            }
        }

        public void MitTudomEnMi<MasikTipus>(MasikTipus ertek)
        {
            Console.WriteLine($"{ertek}: {elemszam}");
        }

        public void HozzaAd(ElemTipus ertek)
        {
            if (elemszam >= ertekek.Length)
                throw new Exception("Elfogyott a hely!");

            ertekek[elemszam] = ertek;
            elemszam++;
            Array.Sort(ertekek, 0, elemszam, comparer);
        }

        public ElemTipus Elem(int pozicio)
        {
            if (pozicio < 0 || pozicio > elemszam)
                throw new Exception("Nem megfelelő pozíció!");

            return ertekek[pozicio];
        }

        public int ElemekSzama => elemszam;
    }
}
```

### Egyben

```csharp
using Generikussag;

SajatLista<int> ertekek = new SajatLista<int>(10);
for (int i = 0; i<5; i++)
{
    ertekek.HozzaAd(42 - i * 2);
}

ertekek.MitTudomEnMi<object>("Kiscica");

for (int i = 0;i<ertekek.ElemekSzama; ++i)
{
    Console.WriteLine(ertekek.Elem(i));
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generikussag
{
    //EGY típusú T paraméteres
    internal class SajatLista<ElemTipus> where ElemTipus: IComparable<ElemTipus>, new()
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

        class MyComparer : IComparer<ElemTipus>
        {
            public int Compare(ElemTipus? x, ElemTipus? y)
            {
                if(x == null)
                {
                    return 1;
                }
                if (y == null)
                {
                    return -1;
                }
                return x.CompareTo(y);
            }
        }

        public void MitTudomEnMi<MasikTipus>(MasikTipus ertek)
        {
            Console.WriteLine($"{ertek}: {elemszam}");
        }

        public void HozzaAd (ElemTipus ertek)
        {
            if (elemszam >= ertekek.Length)
            {
                throw new Exception("Elfogyott a hely!");
            }
            ertekek[elemszam] = ertek;
            elemszam++;
            Array.Sort(ertekek, 0, elemszam, comparer);
        }
        public ElemTipus Elem(int pozicio)
        {
            if (pozicio < 0 || pozicio > elemszam)
            {
                throw new Exception("Nem megfelelő pozíció!");
            }
            return ertekek[pozicio];
        }
        public int ElemekSzama => elemszam;
    }
}
```

---

## 3. Következő témák (hamarosan)

Ez a dokumentum később további **négy új fejezettel** fog bővülni.  
A tervezett sorrend és tartalom itt lesz majd felsorolva.

---

> ℹ️ Ez a README most még csak az **első két projektet** tartalmazza, de a szerkezete már előkészített a jövőbeli bővítéshez.  
> Így könnyedén hozzáadhatók az új fejezetek mind külön fájl, mind „egyben” formában.
