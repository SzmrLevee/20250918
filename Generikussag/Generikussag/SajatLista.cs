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
