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

## 5. Következő témák (hamarosan)

---

> ℹ️ Ez a README most még csak az **első négy projektet** tartalmazza, de a szerkezete már előkészített a jövőbeli bővítéshez.

