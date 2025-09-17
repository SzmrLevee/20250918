using Rekord_Kiegeszites;

TestRecord peldany1 = new TestRecord("Tigris", 3, 12, "Vadász");
TestRecord peldany2 = peldany1 with { nev = "Kiscica", beosztas = "Fogyasztó" };

Console.WriteLine(peldany1);
Console.WriteLine(peldany2);