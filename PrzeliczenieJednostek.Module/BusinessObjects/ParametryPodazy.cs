using Common.Module.Module.BusinessObjects;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzeliczenieJednostek.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class ParametryPodazy : XPObject
    {
        public ParametryPodazy(Session session) : base(session)
        {
        }

        private JednostkaLicznosci jednostkaMolowa;
        private decimal iloscMolowa;
        private JednostkaWagi jednostkaWagowa;
        private decimal iloscWagowa;
        private JednostkaMiary jednostka;
        private decimal ilosc;
        private Parametr parametr;

        public Parametr Parametr
        {
            get => parametr;
            set => SetPropertyValue(nameof(Parametr), ref parametr, value);
        }

        [ImmediatePostData]
        public decimal Ilosc
        {
            get => ilosc;
            set
            {
                bool modified = SetPropertyValue(nameof(Ilosc), ref ilosc, value);
                if (!this.IsLoading && !this.IsSaving && modified)
                {
                    if ((Math.Truncate(100 * this.Ilosc) / 100).Equals(0))
                    {
                        var noweWartosci = this.GetPrzeliczonaJednostka(this.Ilosc, this.Jednostka);
                        this.Jednostka = noweWartosci.Item1;
                        this.Ilosc = noweWartosci.Item2;
                    }
                }
            }
        }

        [ImmediatePostData]
        public JednostkaMiary Jednostka
        {
            get => jednostka;
            set => SetPropertyValue(nameof(Jednostka), ref jednostka, value);           
        }

        [ImmediatePostData]
        public decimal IloscWagowa
        {
            get => iloscWagowa;
            
            set
            {
                bool modified = SetPropertyValue(nameof(IloscWagowa), ref iloscWagowa, value);
                if (!this.IsLoading && !this.IsSaving && modified)
                {
                    if (Parametr != null)
                    {
                        this.SetIloscMolowa();
                    }              
                }
            }
        }

        [ImmediatePostData]
        public JednostkaWagi JednostkaWagowa
        {
            get => jednostkaWagowa;
            set
            {
              bool modified = SetPropertyValue(nameof(JednostkaWagowa), ref jednostkaWagowa, value);
                if (!this.IsLoading && !this.IsSaving && modified && this.JednostkaMolowa != null && this.JednostkaWagowa != null)
                {
                    this.IloscWagowa = this.IloscMolowa * this.Parametr.LiczbaMolowa * this.JednostkaWagowa.Przelicznik / this.JednostkaMolowa.Przelicznik;
                }
            }
        }

        [ImmediatePostData]
        public decimal IloscMolowa
        {
            get => this.iloscMolowa;

            set
            {
                bool modified = SetPropertyValue(nameof(IloscMolowa), ref iloscMolowa, value);
                if (!this.IsLoading && !this.IsSaving && modified)
                {
                    if (Parametr != null)
                    {
                      this.SetIloscWagowa();
                    }                  
                }
            }
        }

        [ImmediatePostData]
        public JednostkaLicznosci JednostkaMolowa
        {
            get => jednostkaMolowa;
            set
            {
               bool modified = SetPropertyValue(nameof(JednostkaMolowa), ref jednostkaMolowa, value);
                if (!this.IsLoading && !this.IsSaving && modified && this.JednostkaMolowa != null && this.JednostkaWagowa != null)
                {
                    this.IloscMolowa = this.IloscWagowa * this.JednostkaWagowa.Przelicznik / this.Parametr.LiczbaMolowa * this.JednostkaMolowa.Przelicznik;
                }
            }
        }       

       public Tuple<JednostkaMiary, decimal> GetPrzeliczonaJednostka(decimal wartoscPrzeliczona, JednostkaMiary jednostka)
        {
            decimal round = Math.Truncate(100 * wartoscPrzeliczona) / 100;
            decimal value = wartoscPrzeliczona;
            JednostkaMiary newJednostka = null;
            if (wartoscPrzeliczona != 0)
            {
                if (round.Equals(0))
                {
                    decimal przelicznik = jednostka.Przelicznik;
                    XPCollection<JednostkaMiary> wszystkieJednostkiPochodne = jednostka.JednostkaBazowa.JednostkiPochodne;
                    var wszystkiePasujaceJednostki = wszystkieJednostkiPochodne.Where(x => x.Przelicznik > przelicznik).OrderBy(x => x.Przelicznik);
                    foreach (var item in wszystkiePasujaceJednostki)
                    {
                        decimal nowyPrzelicznik = item.Przelicznik * przelicznik;
                        value = wartoscPrzeliczona * nowyPrzelicznik;
                        if ((Math.Truncate(100 * value) / 100) > 0)
                        {
                            newJednostka = item;
                            break;
                        }
                    }
                }
            }
            return new Tuple<JednostkaMiary, decimal>(newJednostka, value);
        }

        private void SetIloscMolowa()
        {
            if (this.JednostkaWagowa != null)
            {
                JednostkaLicznosci jednostkaMolowaPrzelicznik = this.Session.FindObject<TabelaKonwersji>(new BinaryOperator(nameof(JednostkaWagi), this.JednostkaWagowa)).JednostkaLicznosci;
                if (this.Session.FindObject<TabelaKonwersji>(new BinaryOperator(nameof(JednostkaWagi), this.JednostkaWagowa)) != null)
                {
                    decimal nowaIloscMolowaPrzelicznikBazowy = this.iloscWagowa * this.JednostkaWagowa.Przelicznik / this.Parametr.LiczbaMolowa / jednostkaMolowaPrzelicznik.Przelicznik;

                    if (this.JednostkaMolowa != null && !(Math.Truncate(100 * nowaIloscMolowaPrzelicznikBazowy) / 100).Equals(0) && this.JednostkaMolowa != jednostkaMolowaPrzelicznik)
                    {
                        this.iloscMolowa = nowaIloscMolowaPrzelicznikBazowy;
                        this.jednostkaMolowa = jednostkaMolowaPrzelicznik;
                    }
                    else
                    {
                        if (jednostkaMolowaPrzelicznik != null)
                        {
                            this.SetPrzeliczonaJednostkaMolowa(nowaIloscMolowaPrzelicznikBazowy, jednostkaMolowaPrzelicznik);
                        }
                    }
                }
            }
        }

        private void SetPrzeliczonaJednostkaMolowa(decimal nowaIloscMolowa, JednostkaLicznosci jednostkaMolowa)
        {
            if ((Math.Truncate(100 * nowaIloscMolowa) / 100).Equals(0) && jednostkaMolowa != null)
            {
                var przeliczonaJednostka = this.GetPrzeliczonaJednostka(nowaIloscMolowa, jednostkaMolowa);
                this.jednostkaMolowa = przeliczonaJednostka.Item1 as JednostkaLicznosci;
                this.iloscMolowa = przeliczonaJednostka.Item2;
            }
            else
            {
                this.iloscMolowa = nowaIloscMolowa;
                this.jednostkaMolowa = jednostkaMolowa;
            }
        }

        private void SetIloscWagowa()
        {
            if (this.JednostkaMolowa != null && this.JednostkaWagowa != null)
            {
                if (this.Session.FindObject<TabelaKonwersji>(new BinaryOperator(nameof(JednostkaLicznosci), this.JednostkaMolowa)) != null)
                { 
                    JednostkaWagi jednostkaWagiPrzelicznik = this.Session.FindObject<TabelaKonwersji>(new BinaryOperator(nameof(JednostkaLicznosci), this.JednostkaMolowa)).JednostkaWagi;
                    this.iloscWagowa = this.iloscMolowa * this.Parametr.LiczbaMolowa * jednostkaWagiPrzelicznik.Przelicznik / this.JednostkaMolowa.Przelicznik;
                    this.jednostkaWagowa = jednostkaWagiPrzelicznik;              
                }
            }
        }        
    }
}