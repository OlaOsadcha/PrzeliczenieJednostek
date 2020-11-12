using Common.Module.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using PrzeliczenieJednostek.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzeliczenieJednostek.Module.Helpers
{
   public static class PrzeliczenieJednostekHelper
    {
        public static decimal SetIloscMolowaPodczasZmianyJednostki(decimal iloscWagowa, Parametr parametr, decimal jednostkaWagowa, decimal jednostkaMolowa)
        {
            decimal wynik = iloscWagowa / parametr.LiczbaMolowa / jednostkaWagowa * jednostkaMolowa;
            return wynik;
        }

        public static decimal SetIloscWagowaPodczasZmianyJednostki(decimal iloscMolowa, Parametr parametr, decimal jednostkaWagowa, decimal jednostkaMolowa)
        {
            decimal wynik = iloscMolowa * parametr.LiczbaMolowa * jednostkaWagowa / jednostkaMolowa;
            return wynik;
        }

        public static Tuple<decimal, JednostkaMiary> SetIloscMolowa(JednostkaMiary JednostkaWagowa, JednostkaMiary JednostkaMolowa, Session session, decimal iloscWagowa, Parametr parametr)
        {
            JednostkaMiary jednostkaMolowa = null;
            decimal iloscMolowa = 1;

            if (JednostkaWagowa != null)
            {                
                if (session.FindObject<TabelaKonwersji>(new BinaryOperator(nameof(JednostkaWagi), JednostkaWagowa)) != null)
                {
                    JednostkaLicznosci jednostkaMolowaPrzelicznik = session.FindObject<TabelaKonwersji>(new BinaryOperator(nameof(JednostkaWagi), JednostkaWagowa)).JednostkaLicznosci;
                    decimal nowaIloscMolowaPrzelicznikBazowy = iloscWagowa * JednostkaWagowa.Przelicznik / parametr.LiczbaMolowa / jednostkaMolowaPrzelicznik.Przelicznik;

                    if (JednostkaMolowa != null && !Truncate(nowaIloscMolowaPrzelicznikBazowy) && JednostkaMolowa != jednostkaMolowaPrzelicznik)
                    {
                        iloscMolowa = nowaIloscMolowaPrzelicznikBazowy;
                        jednostkaMolowa = jednostkaMolowaPrzelicznik;
                    }
                    else
                    {
                        if (jednostkaMolowaPrzelicznik != null)
                        {
                           var tuple = SetPrzeliczonaJednostkaMolowa(nowaIloscMolowaPrzelicznikBazowy, jednostkaMolowaPrzelicznik);
                            iloscMolowa = tuple.Item1;
                            jednostkaMolowa = tuple.Item2;
                        }
                    }
                }               
            }
            return new Tuple<decimal, JednostkaMiary>(iloscMolowa, jednostkaMolowa);
        }     

        public static Tuple<decimal, JednostkaMiary> SetIloscWagowa(JednostkaMiary JednostkaMolowa, JednostkaMiary JednostkaWagowa, Session session, Parametr parametr, decimal iloscMolowa)
        {
            decimal iloscWagowa = 1;
            JednostkaMiary jednostkaWagowa = null;
            if (JednostkaMolowa != null && JednostkaWagowa != null)
            {
                if (session.FindObject<TabelaKonwersji>(new BinaryOperator(nameof(JednostkaLicznosci), JednostkaMolowa)) != null)
                {
                    JednostkaWagi jednostkaWagiPrzelicznik = session.FindObject<TabelaKonwersji>(new BinaryOperator(nameof(JednostkaLicznosci), JednostkaMolowa)).JednostkaWagi;
                    decimal iloscWagowaBazowa = iloscMolowa * parametr.LiczbaMolowa * jednostkaWagiPrzelicznik.Przelicznik / JednostkaMolowa.Przelicznik;

                    if (JednostkaWagowa != null && !Truncate(iloscWagowa) && JednostkaMolowa != jednostkaWagiPrzelicznik)
                    {
                        iloscWagowa = iloscWagowaBazowa;
                        jednostkaWagowa = jednostkaWagiPrzelicznik;
                    }
                    else
                    {
                        if (jednostkaWagiPrzelicznik != null)
                        {
                            var tuple = SetPrzeliczonaJednostkaMolowa(iloscWagowaBazowa, jednostkaWagiPrzelicznik);
                            iloscWagowa = tuple.Item1;
                            jednostkaWagowa = tuple.Item2;
                        }
                    }
                }                
            }
            return new Tuple<decimal, JednostkaMiary>(iloscWagowa, jednostkaWagowa);
        }

        public static Tuple<JednostkaMiary, decimal> GetPrzeliczonaJednostka(decimal wartoscPrzeliczona, JednostkaMiary jednostka)
        {
            decimal value = wartoscPrzeliczona;
            JednostkaMiary newJednostka = null;
            if (wartoscPrzeliczona != 0)
            {
                if (Truncate(wartoscPrzeliczona))
                {
                    decimal przelicznik = jednostka.Przelicznik;
                    XPCollection<JednostkaMiary> wszystkieJednostkiPochodne = jednostka.JednostkaBazowa.JednostkiPochodne;
                    if (wszystkieJednostkiPochodne != null)
                    {
                        var wszystkiePasujaceJednostki = wszystkieJednostkiPochodne.Where(x => x.Przelicznik > przelicznik).OrderBy(x => x.Przelicznik);
                        if (wszystkiePasujaceJednostki != null)
                        {
                            foreach (var item in wszystkiePasujaceJednostki)
                            {
                                decimal nowyPrzelicznik = item.Przelicznik * przelicznik;
                                value = wartoscPrzeliczona * nowyPrzelicznik;
                                if (!Truncate(value))
                                {
                                    newJednostka = item;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return new Tuple<JednostkaMiary, decimal>(newJednostka, value);
        }

        private static Tuple<decimal, JednostkaMiary> SetPrzeliczonaJednostkaMolowa(decimal nowaIloscMolowa, JednostkaMiary staraJednostka)
        {
            decimal ilosc;
            JednostkaMiary jednostka;
            if (Truncate(nowaIloscMolowa) && staraJednostka != null)
            {
                var przeliczonaJednostka = GetPrzeliczonaJednostka(nowaIloscMolowa, staraJednostka);
                jednostka = przeliczonaJednostka.Item1 as JednostkaLicznosci;
                ilosc = przeliczonaJednostka.Item2;
            }
            else
            {
                ilosc = nowaIloscMolowa;
                jednostka = staraJednostka;
            }
            return new Tuple<decimal, JednostkaMiary>(ilosc, jednostka);
        }

        public static bool Truncate(decimal wartosc)
        {
            return (Math.Truncate(100 * wartosc) / 100).Equals(0) ? true : false;
        }
    }
}