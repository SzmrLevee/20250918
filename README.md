# C# Oktatóanyag

## Tartalomjegyzék

1. [Rekordkiegészítés](#1-rekordkiegészítés)
2. [Generikusság](#2-generikusság)
3. [Delegate és Event](#3-delegate-és-event)
4. [Következő témák (hamarosan)](#3-következő-témák-hamarosan)

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

## 3. Delegate és Event

### Mi az a delegate?

A **delegate** (delegált) egy olyan típus, amivel **függvényeket lehet változóként tárolni és továbbadni**.
Ez azt jelenti, hogy a függvényre úgy hivatkozhatunk, mintha egy érték lenne.

```csharp
delegate int Muvelet(int elso, int masodik);
```

Ez a definíció azt mondja, hogy minden olyan függvény, amely **két int paramétert kap és int-et ad vissza**, hozzárendelhető ehhez a delegate-hez.

---

### Példa: függvény hozzárendelése

**Program.cs**

```csharp
int Osszead(int a, int b) => a + b;

Muvelet muvelet = Osszead; // Nem meghívjuk, hanem hozzárendeljük
Console.WriteLine(muvelet(2, 3)); // Kiírja: 5
```

**Magyarázat:**

* `muvelet` most egy függvényre mutat.
* Így a függvény **paraméterként is átadható**, változóban tárolható, vagy például egy listába/Dictionary-be tehető.

---

### Delegate Dictionary-ben

**Program.cs**

```csharp
Dictionary<string, Muvelet> muveletek = new Dictionary<string, Muvelet>() {
    { "+", Osszead },
    { "-", (x,y) => x - y }, // lambda kifejezés
    { "*", (x,y) => x * y },
    { "/", (x,y) => x / y }
};

foreach (var k in muveletek) {
    Console.WriteLine($"{k.Key}(8,4) = {k.Value(8,4)}");
}
```

**Kimenet:**

```
+(8,4) = 12
-(8,4) = 4
*(8,4) = 32
/(8,4) = 2
```

---

### Új szintaxis: `Func` és `Action`

A `delegate` helyett használhatunk beépített generikus típusokat:

* `Func<T1, T2, TResult>` → olyan függvényekhez, amelyek **visszatérési értékkel rendelkeznek**.
* `Action<T1, T2>` → olyan függvényekhez, amelyek **nem adnak vissza értéket** (`void`).

**Program.cs**

```csharp
Dictionary<string, Func<int, int, int>> muveletek =
    new Dictionary<string, Func<int, int, int>>() {
        { "+", (x, y) => x + y },
        { "-", (x, y) => x - y },
        { "*", (x, y) => x * y },
        { "/", (x, y) => x / y }
    };
```

---

## Event

### Mi az event?

Az **event** eseménykezelő metódusokat jelent.

* Egy osztályban definiáljuk.
* Más kód **feliratkozhat** rá (`+=`).
* Ugyanígy **le is iratkozhat** róla (`-=`).
* Az osztály maga **meghívhatja** az eseményt (`Invoke`).

Ez hasonló a WPF-ben a `Button.Click` eseményhez.

---

### Példaosztály eventtel

**PeldaOsztaly.cs**

```csharp
internal class PeldaOsztaly {
    public event Action<PeldaOsztaly, string, int> PeldaEvent;

    private int x, y, z;

    public int X {
        get => x;
        set { 
            x = value;
            PeldaEvent?.Invoke(this, nameof(X), value);
        }
    }

    public int Y {
        get => y;
        set { 
            y = value;
            PeldaEvent?.Invoke(this, nameof(Y), value);
        }
    }

    public int Z {
        get => z;
        set { 
            z = value;
            PeldaEvent?.Invoke(this, nameof(Z), value);
        }
    }
}
```

**Magyarázat:**

* `event Action<PeldaOsztaly, string, int>` azt jelenti: olyan metódusokat várunk, amik három paramétert fogadnak: az osztály példányát, a property nevét és az értéket.
* A property `set` részében meghívjuk az eseményt.

---

### Feliratkozás és leiratkozás

**Program.cs**

```csharp
PeldaOsztaly pelda = new PeldaOsztaly();

// Feliratkozás
pelda.PeldaEvent += (obj, nev, val) => Console.WriteLine($"{nev} => {val}");

pelda.X = 42;
pelda.Y = 21;

// Leiratkozás (DE vigyázat!)
pelda.PeldaEvent -= (obj, nev, val) => Console.WriteLine($"{nev} => {val}");

pelda.Z = 99;
```

**Fontos:**

* A `-=` itt **nem működik**, mert a két lambda **külön példány**.
* Ezért előbb változóban kell tárolni a delegate-et:

```csharp
Action<PeldaOsztaly, string, int> kiiro =
    (obj, nev, val) => Console.WriteLine($"{nev} => {val}");

pelda.PeldaEvent += kiiro;
pelda.Y = 42;

pelda.PeldaEvent -= kiiro; // most már valóban leiratkozott
```

---

### Függvény használata feliratkozásra

**Program.cs**

```csharp
void Kiir(PeldaOsztaly p, string nev, int val) {
    Console.WriteLine($"Kiir: {nev} - {val}");
}

pelda.PeldaEvent += Kiir;
pelda.X = 100;

pelda.PeldaEvent -= Kiir;
pelda.Y = 200;
```

---

### Többszörös feliratkozás

```csharp
pelda.PeldaEvent += Kiir;
pelda.X = 1;

pelda.PeldaEvent += Kiir;
pelda.Y = 2;

pelda.PeldaEvent -= Kiir;
pelda.Z = 3;
```

**Kimenet:**

```
Kiir: X - 1
Kiir: Y - 2
Kiir: Y - 2
Kiir: Z - 3
```

**Magyarázat:**

* Ahányszor feliratkozunk, annyiszor fut le a metódus.
* Ahányszor leiratkozunk, annyiszor csökken a hívások száma.

---

### Event meghívása (Invoke)

Kétféleképpen hívhatjuk meg az eseményt:

**Modern szintaxis**

```csharp
PeldaEvent?.Invoke(this, nameof(Z), value);
```

**Régi szintaxis**

```csharp
if (PeldaEvent != null)
    PeldaEvent(this, nameof(Z), value);
```

Az event **null**, amíg nincs senki feliratkozva.

---

## Tanácsok delegate és event használathoz

1. **Delegate** akkor kell, ha függvényt akarsz paraméterként átadni vagy tárolni.
   Például: `Func` és `Action` helyett saját típust definiálni, ha egyedi kell.

2. **Func vs Delegate:** ha elég a beépített `Func` vagy `Action`, használd inkább azt – olvashatóbb.

3. **Event**-et mindig `?.Invoke`-val hívj meg, így elkerülöd a `NullReferenceException`-t.

4. **Feliratkozás/leiratkozás:** lambdákat mindig változóban tárolj, ha le is akarod szedni őket.

5. **Többszörös feliratkozás:** figyelj rá, hogy ugyanaz a metódus többször is hozzáadható → többször fog lefutni.

6. **Delegate + Dictionary:** nagyon hasznos, ha sok művelet közül kell választani string/kulcs alapján (pl. kalkulátor).

7. **Osztályon belül**: eventet csak az osztály hívhatja meg, kívülről csak feliratkozni/leiratkozni lehet. Ez biztonságosabb.

---

## 4. Binding

## Mi a Binding lényege?

A binding célja, hogy **elkülönítsük a megjelenítést és a működést**.

* A **GUI**-t (megjelenítés) a XAML kezeli.
* Az **adatokat és logikát** a ViewModel tartalmazza.

Ez az **MVVM (Model–View–ViewModel) minta** alapja:

* **Model** – maga az adat (pl. `Ember` osztály).
* **ViewModel** – logika, propertyk, események.
* **View** – a felület (XAML).

---

## Kezdés

**App.xaml**

```xml
<Application StartupUri="MainWindow.xaml">
```

**ViewModel mappa / MainWindowViewModel.cs**

```csharp
namespace WpfAppNev.ViewModel {
    public class MainWindowViewModel {
        // Tulajdonságok és logika ide kerül
    }
}
```

**MainWindow\.xaml**

```xml
<Window ...
    xmlns:viewmodel="clr-namespace:WpfAppNev.ViewModel">

    <Window.DataContext>
        <viewmodel:MainWindowViewModel />
    </Window.DataContext>
</Window>
```

> ⚠️ Elsőre hibát dobhat, mert a WPF a fordított binárisban keresi a típust.
> **Build** után eltűnik a hiba.

---

## Első binding

**MainWindowViewModel.cs**

```csharp
public string Nev { get; set; } = "Tigris";
```

**MainWindow\.xaml**

```xml
<StackPanel Orientation="Vertical">
    <Label Content="Nev" />
    <TextBox Text="{Binding Nev}" />
</StackPanel>
```

* A binding **csak property-vel** működik, mezővel (`public string Nev = "Tigris"`) nem.
* A TextBox automatikusan megkapja a `Nev` értékét.

---

## Változások figyelése

**MainWindowViewModel.cs**

```csharp
public class MainWindowViewModel : INotifyPropertyChanged {
    private string nev = "Tigris";
    private string nev2 = "Kiscica";

    public string Nev {
        get => nev;
        set {
            nev = value;
            nev2 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nev)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nev2)));
        }
    }

    public string Nev2 {
        get => nev2;
        set {
            nev2 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nev2)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
```

**MainWindow\.xaml**

```xml
<TextBox Text="{Binding Nev, UpdateSourceTrigger=PropertyChanged}" />
<TextBox Text="{Binding Nev2}" />
```

* `INotifyPropertyChanged` → frissíti a UI-t, ha változik a property.
* `UpdateSourceTrigger=PropertyChanged` → gépelés közben is frissül.

---

## Binding Window címre

**MainWindowViewModel.cs**

```csharp
public string Cim { get; private set; } = "Cím";

public string Nev {
    get => nev;
    set {
        nev = value;
        Cim = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cim)));
    }
}
```

**MainWindow\.xaml**

```xml
<Window Title="{Binding Cim}">
```

---

## Számolt property

**MainWindowViewModel.cs**

```csharp
public int Magassag => Nev.Length * 50;
```

**MainWindow\.xaml**

```xml
<TextBox Height="{Binding Magassag}" />
```

Ha `Nev` változik, jelezni kell:

```csharp
PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Magassag)));
```

---

## Lista binding (ComboBox)

### Model

**Model mappa / Ember.cs**

```csharp
public class Ember {
    public string Nev { get; init; } = "";
    public int Fizetes { get; set; }
}
```

**Model mappa / Emberek.cs**

```csharp
public static class Emberek {
    public static IEnumerable<Ember> GetEmberek() {
        yield return new Ember { Nev = "Elso", Fizetes = 200 };
        yield return new Ember { Nev = "Masodik", Fizetes = 220 };
        yield return new Ember { Nev = "Harmadik", Fizetes = 240 };
    }
}
```

### ViewModel

**MainWindowViewModel.cs**

```csharp
public List<Ember> Emberek { get; init; } = Emberek.GetEmberek().ToList();

private Ember? valasztottEmber;
public Ember? ValasztottEmber {
    get => valasztottEmber;
    set {
        valasztottEmber = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValasztottEmber)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValasztottFizetes)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValasztottNev)));
    }
}

public int? ValasztottFizetes {
    get => ValasztottEmber?.Fizetes;
    set {
        if (ValasztottEmber != null && value != null)
            ValasztottEmber.Fizetes = value.Value;
    }
}

public string? ValasztottNev => ValasztottEmber?.Nev;
```

### XAML

**MainWindow\.xaml**

```xml
<Label Content="Válassz" />
<ComboBox ItemsSource="{Binding Emberek}"
          DisplayMemberPath="Nev"
          SelectedItem="{Binding ValasztottEmber}" />

<Label Content="Nev" />
<TextBox Text="{Binding ValasztottNev, Mode=OneWay}" IsReadOnly="True" />

<Label Content="Fizetes" />
<TextBox Text="{Binding ValasztottFizetes}" />
```

> * **SelectedItem** → az egész objektumot köti.
> * **SelectedValue** → csak egy értéket ad vissza (pl. ID).

---

## Command binding (gomb események)

### RelayCommand

**MainWindowViewModel.cs (belül)**

```csharp
public class RelayCommand : ICommand {
    private readonly Action action;
    private bool enabled;

    public RelayCommand(Action action, bool enabled = true) {
        this.action = action ?? throw new ArgumentNullException(nameof(action));
        this.enabled = enabled;
    }

    public bool Enabled {
        get => enabled;
        set {
            enabled = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => enabled;
    public void Execute(object? parameter) => action();
}
```

### Használat

**MainWindowViewModel.cs**

```csharp
public RelayCommand AddEmber { get; }

public MainWindowViewModel() {
    AddEmber = new RelayCommand(() =>
        Emberek.Add(new Ember { Nev = "Új", Fizetes = 300 })
    );
}
```

### XAML

**MainWindow\.xaml**

```xml
<Button Content="Felvételi iroda" Command="{Binding AddEmber}" />
```

---

## Tanácsok bindinghoz

1. **Property, ne mező.** Binding csak `get`/`set` propertyvel működik.
2. **INotifyPropertyChanged minden ViewModelben.** E nélkül nem frissül a GUI.
3. **Használj `nameof(...)`-ot.** Biztonságosabb, refaktorálásnál automatikusan átíródik.
4. **UpdateSourceTrigger-t állítsd be.** TextBox esetén érdemes `PropertyChanged`-re tenni.
5. **Lista → ObservableCollection.** Ha a lista gyakran változik, az `ObservableCollection<T>` automatikusan frissíti a UI-t.
6. **Commands a Click helyett.** Így a logika a ViewModelben marad, a View csak megjelenít.
7. **Keep it clean.** A `MainWindow.xaml.cs` maradjon minimális, az üzleti logika mindig a ViewModelben legyen.

---


## 5. Interface-ek

### Mi az az interface?

Az **interface** (felület) olyan szerződés, amely **meghatároz bizonyos működési elvárásokat** az osztályokkal szemben.  
Ez nem tartalmaz konkrét megvalósítást (hogyan működik), hanem csak a **mit** kérdésére ad választ.

* Olyan, mint egy tervrajz: aki implementálja, annak kötelező megvalósítania a benne leírtakat.  
* Az interface-eket C#-ban **nagy `I` betűvel** szokás kezdeni (pl. `IAllat`).  

---

### Példa: egyszerű interface

**IAllat.cs**

```csharp
internal interface IAllat {
    string Nev { get; }            // property
    Nem Neme { get; }              // enum property
    string Beszel();               // metódus

    const int Labakszama = 4;      // konstans
    static virtual string Eszik() => "Jóllaktam!"; // statikus alap-implementáció

    enum Nem { Him, Nosteny, Himnos } // belső típus
}
````

**Fontos megjegyzések:**

1. **Property-t lehet**, de **mezőt nem** írhatunk interface-be.
2. Alapértelmezetten minden **public**.
3. Lehetnek benne **konstansok**, **statikus metódusok** és belső típusok (`enum`, `record`, `class`).
4. A `static virtual` lehetőséget ad **alapmegvalósításra**, amelyet az osztály később felülírhat.

---

### Implementálás osztályban

**Kutya.cs**

```csharp
internal class Kutya : IAllat {
    public string Nev { get; init; }
    public IAllat.Nem Neme { get; init; }

    public Kutya(string nev) {
        Nev = nev;
        Neme = IAllat.Nem.Himnos;
    }

    public string Beszel() => "Vau-vau";

    public static int Labakszama() => 4;
}
```

**Magyarázat:**

* Az osztály **: IAllat** jelöléssel valósítja meg az interface-t.
* Ha valamit nem valósítunk meg, a fordító pirossal jelzi.
* A Visual Studio **Quick Actions → Implement Interface** funkciója automatikusan legenerálja a szükséges tagokat.

---

### Absztrakt osztály + interface

Lehetőség van **részleges megvalósításra** egy absztrakt osztályban:

**Negylabu.cs**

```csharp
internal abstract class Negylabu : IAllat {
    public abstract string Nev { get; }
    public abstract IAllat.Nem Neme { get; }
    public abstract string Beszel();

    public static int Labakszama() => 4;
}
```

Ezután a `Kutya` már nem közvetlenül az `IAllat`-ot valósítja meg, hanem örökölhet a `Negylabu`-ból:

```csharp
internal class Kutya : Negylabu {
    private readonly string nev;
    private readonly IAllat.Nem nem;

    public Kutya(string nev) {
        this.nev = nev;
        this.nem = IAllat.Nem.Him;
    }

    public override string Nev => nev;
    public override IAllat.Nem Neme => nem;
    public override string Beszel() => "Vau-vau";
}
```

---

### Interface öröklődés

Egy interface **örökölhet más interface-ekből**, így kibővíthetjük a szerződést.

**IMacskafele.cs**

```csharp
internal interface IMacskafele : IAllat {
    string Jatszik();
}
```

**Tigris.cs**

```csharp
internal class Tigris : IMacskafele {
    public string Nev => "Shere Khan";
    public IAllat.Nem Neme => IAllat.Nem.Him;
    public string Beszel() => "Rrrr!";
    public string Jatszik() => "A tigris vadászik.";
}
```

---

### Explicit interface implementáció

Az interface tagjai explicit módon is megvalósíthatók:

```csharp
internal class Tigris : IAllat {
    string IAllat.Nev => "Rejtett név";
    IAllat.Nem IAllat.Neme => IAllat.Nem.Him;
    string IAllat.Beszel() => "Csak interface-en keresztül látszom!";
}
```

**Használat:**

```csharp
Tigris t = new Tigris();
// Console.WriteLine(t.Nev); // ❌ nem érhető el

IAllat allat = t;
Console.WriteLine(allat.Nev); // ✅ "Rejtett név"
```

**Magyarázat:**

* Explicit megvalósítás esetén a tag **csak az interface típusán keresztül** érhető el.
* Így elrejthetünk bizonyos működést az osztály publikus API-jából.

---

### Összegző példa – több állat kezelése

**Program.cs**

```csharp
List<IAllat> allatok = new List<IAllat>() {
    new Kutya("Bodri"),
    new Tigris()
};

foreach (var allat in allatok) {
    Console.WriteLine($"{allat.Nev} ({allat.Neme}) - mondja: {allat.Beszel()}");
}
```

**Kimenet:**

```
Bodri (Himnos) - mondja: Vau-vau
Shere Khan (Him) - mondja: Rrrr!
```

---

## Tanácsok interface használatához

1. **Használj interface-t, ha közös működést akarsz előírni több osztálynak.**
   Például: `IAllat`, `IMuvelet`, `IRepository`.

2. **Ne írj bele mezőt!** Property-kkel fejezd ki az adatokat.

3. **Az osztály több interface-t is megvalósíthat**, de csak egy ősosztályt örökölhet.

4. **Absztrakt osztály** akkor hasznos, ha közös alap-implementációt is szeretnél adni.

5. **Explicit implementációval** szabályozhatod, hogy mi legyen elérhető az osztályból közvetlenül, és mi csak az interface-en keresztül.

6. **Statikus tagok** (pl. `static virtual`) modern C#-ban már támogatottak, de csak akkor használd őket, ha tényleg szükséges.
   
---



## 6. Class, Struct és Record

### Bevezetés

Ebben a részben az **összetett adattípusokról** lesz szó, illetve arról, hogyan lehet őket létrehozni, és mik a fő különbségek köztük.  

Azért van szükség összetett adatszerkezetekre, mert például egy ember adatait szeretnénk eltárolni, akinek van neve, születési ideje és fizetése.  

---

### Class példa

**HumanClass.cs**

```csharp
internal class HumanClass {
    public string Name { get; init; } = ""; // string null lehet, ezért kell alapérték
    public DateOnly DateOfBirth { get; init; }
    public int Salary { get; init; }
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
}
````

**Magyarázat:**

* `Name` property: ha nem adunk neki alapértéket, akkor `null` lehet → hibát dobhat. Ezért adtunk neki `= ""` alapértéket.
* `DateOfBirth` és `Salary`: értéktípusok (`struct`), így **nem lehetnek null**, ezért nem kell alapértéket megadni.
* `Age`: csak olvasható property, ami kiszámolja az életkort.

---

**Program.cs**

```csharp
void RaiseSalaryClass(HumanClass human) {
    human.Salary += 100_000;
}

HumanClass humanClass = new HumanClass {
    Name = "Gazsi",
    DateOfBirth = new DateOnly(420, 6, 9),
    Salary = 666_666
};

RaiseSalaryClass(humanClass);
Console.WriteLine($"{humanClass.Name} - salary: {humanClass.Salary}");
```

**Magyarázat:**

* A `RaiseSalaryClass` metódus **referencián keresztül** kapja meg az objektumot, így a `Salary` változás megmarad.
* Amikor a `Console.WriteLine` fut, a fizetés értéke már a megemelt változat lesz.

---

### Struct példa

**HumanStruct.cs**

```csharp
internal struct HumanStruct {
    public string Name { get; init; } // nincs alapérték!
    public DateOnly DateOfBirth { get; init; }
    public int Salary { get; init; }
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
}
```

**Magyarázat:**

* `struct` esetén nem adhatunk alapértéket a property-knek, különben hibaüzenetet kapunk:

  * *"A struct with field initializers must include an explicitly declared constructor."*
* A `struct` **értéktípus**, ami másképp működik, mint a class.

---

**Program.cs**

```csharp
void RaiseSalaryStruct(HumanStruct human) {
    human.Salary += 100_000;
}

HumanStruct humanStruct = new HumanStruct {
    Name = "Laci",
    DateOfBirth = new DateOnly(1990, 1, 1),
    Salary = 400_000
};

RaiseSalaryStruct(humanStruct);
Console.WriteLine($"{humanStruct.Name} - salary: {humanStruct.Salary}");
```

**Eredmény:**

* A fizetés nem változik meg, mert a `struct` **értékként másolódik át** a metódusba.
* Így a módosítás a másolaton történik, az eredeti példány érintetlen marad.

---

### Class vs Struct különbségek

* **Class** → referencia típus

  * A változó egy **referenciát** tárol az objektumhoz.
  * Ha átadjuk paraméterben, ugyanarra a példányra mutat.
  * Ezért minden módosítás érvényes lesz az eredeti objektumra is.

* **Struct** → értéktípus

  * A változó maga tartalmazza az adatokat.
  * Paraméterátadáskor **másolat készül**, így a módosítás nem érvényesül az eredeti példányon.
  * `struct` sealed, nem származhat belőle más osztály.

**Null érték példák:**

```csharp
// class null értéket kaphat
humanClass = null; // OK

// struct nem lehet null
humanStruct = null; // ❌ fordítási hiba
```

---

### Record példa

A **record** referencia típus, de ad hozzá extra funkciókat (pl. automatikus `ToString()`, `Equals`).

**HumanRecord.cs**

```csharp
internal record HumanRecord(string Name, DateOnly DateOfBirth) {
    public int Salary { get; set; }
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
}
```

**Magyarázat:**

* Az elsődleges konstruktor paramétereiből (`Name`, `DateOfBirth`) a fordító **automatikusan létrehoz property-ket**.
* A `ToString()` metódus automatikusan visszaadja a property-k neveit és értékeit.
* Az `Equals` és `==` összehasonlítás érték-alapú, nem referencia-alapú.

---

**Program.cs**

```csharp
void RaiseSalaryRecord(HumanRecord human) {
    human.Salary += 100_000;
}

HumanRecord humanRecord = new HumanRecord(
    Name: "Jani",
    DateOfBirth: new DateOnly(1985, 12, 1)
) {
    Salary = 500_000
};

RaiseSalaryRecord(humanRecord);
Console.WriteLine($"{humanRecord} - salary: {humanRecord.Salary}");
```

**Eredmény:**

* A fizetés **megváltozik**, mert a record referencia típus.
* A `Console.WriteLine` automatikusan kiírja a property-ket (pl. `HumanRecord { Name = Jani, DateOfBirth = 1985.12.01, Salary = 600000 }`).

---

### Record Struct

**HumanRecordStruct.cs**

```csharp
internal record struct HumanRecordStruct(string Name, DateOnly DateOfBirth) {
    public int Salary { get; set; }
}
```

**Magyarázat:**

* Ez egy **értéktípusú record**.
* Az elsődleges konstruktorból `get; set;` property-ket generál.
* A `ToString()` metódus szintén automatikusan implementálva van.

**Fontos különbség:**

* Paraméterátadáskor másolat készül, ezért a módosítás nem érvényesül az eredetin.

---

### Readonly Record Struct

**ReadonlyHumanRecordStruct.cs**

```csharp
internal readonly record struct ReadonlyHumanRecordStruct(string Name, DateOnly DateOfBirth);
```

**Magyarázat:**

* A `readonly` kulcsszó miatt minden property readonly.
* Nem lehet benne `set` vagy más módosítható adattag.
* Értéktípusként működik, mint egy sima `struct`.

---

## Összefoglalás

1. **Class**

   * Referencia típus.
   * Null értéket kaphat.
   * Öröklődés lehetséges.
   * Módosítás érvényesül az eredeti példányon.

2. **Struct**

   * Értéktípus.
   * Nem lehet null.
   * Sealed → nem származhat belőle más osztály.
   * Paraméterátadáskor másolat készül → módosítás nem marad meg.

3. **Record**

   * Referencia típus.
   * Automatikus `ToString()`, `Equals`, `==`.
   * Elsődleges konstruktor paraméterei alapján property-ket generál.

4. **Record Struct**

   * Értéktípus.
   * Automatikus `ToString()` és property-generálás.
   * Paraméterátadáskor másolat készül.

5. **Readonly Record Struct**

   * Értéktípus.
   * Minden property readonly.
   * Nem tartalmazhat módosítható adattagot.

```
