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