using Common.Module.Module.BusinessObjects;
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
        { }

        private bool isIloscWagowaChange = false;
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
            set
            {
                bool modified = SetPropertyValue(nameof(Jednostka), ref jednostka, value);              
            }
        }

        [ImmediatePostData]
        public decimal IloscWagowa
        {
            get
            {
                ////if (this.IloscMolowa != 0)
                ////{
                ////    return this.IloscMolowa * Parametr.LiczbaMolowa * this.JednostkaWagowa.Przelicznik / 1000 / this.JednostkaMolowa.Przelicznik;
                ////}
                return iloscWagowa;
            }
            set
            {
                bool modified = SetPropertyValue(nameof(IloscWagowa), ref iloscWagowa, value);
                if (!this.IsLoading && !this.IsSaving && modified)
                {
                    if (Parametr != null)
                    {
                        decimal nowaIloscMolowa = this.iloscWagowa / Parametr.LiczbaMolowa;
                        this.IloscMolowa = nowaIloscMolowa;
                    }
                        ////if ((Math.Truncate(100 * nowaIloscMolowa) / 100).Equals(0))
                        ////{
                        ////    var noweWartosciMolowe = this.GetPrzeliczonaJednostka(nowaIloscMolowa, this.JednostkaMolowa);
                        ////    this.JednostkaMolowa = noweWartosciMolowe.Item1 as JednostkaLicznosci;
                        ////    this.IloscMolowa = noweWartosciMolowe.Item2;
                        ////}
                        ////else
                        ////{
                        ////    this.IloscMolowa = nowaIloscMolowa;
                        ////}

                        ////var noweWartosci = this.GetPrzeliczonaJednostka(this.IloscWagowa, this.JednostkaWagowa);
                        ////this.JednostkaWagowa = noweWartosci.Item1 as JednostkaWagi;
                        ////this.IloscWagowa = noweWartosci.Item2;
                    
                }
            }
            
        }

        [ImmediatePostData]
        public JednostkaWagi JednostkaWagowa
        {
            get => jednostkaWagowa;
            set => SetPropertyValue(nameof(JednostkaWagowa), ref jednostkaWagowa, value);
        }

        [ImmediatePostData]
        public decimal IloscMolowa
        {
            get
            {
                ////if (this.JednostkaWagowa != null && this.Parametr != null && Parametr.LiczbaMolowa != 0 && this.IloscWagowa != 0)
                ////{
                ////    return this.IloscWagowa / this.JednostkaWagowa.Przelicznik * 1000 / Parametr.LiczbaMolowa;
                ////}
                return this.iloscMolowa;
            }

            set
            {
               bool modified = SetPropertyValue(nameof(IloscMolowa), ref iloscMolowa, value);
                if (!this.IsLoading && !this.IsSaving && modified)
                {
                    if (Parametr != null)
                    {
                        decimal nowaIloscWagowa = this.iloscMolowa * Parametr.LiczbaMolowa;
                        this.IloscWagowa = nowaIloscWagowa;
                    }
                    ////if ((Math.Truncate(100 * nowaIloscWagowa) / 100).Equals(0))
                    ////{
                    ////    var noweWartosci = this.GetPrzeliczonaJednostka(nowaIloscWagowa, this.JednostkaWagowa);
                    ////    this.JednostkaWagowa = noweWartosci.Item1 as JednostkaWagi;
                    ////    this.IloscWagowa = noweWartosci.Item2;
                    ////}
                    ////else
                    ////{
                    ////    this.IloscWagowa = nowaIloscWagowa;
                    ////}

                    ////if ((Math.Truncate(100 * this.iloscMolowa) / 100).Equals(0))
                    ////{
                    ////    var noweWartosci = this.GetPrzeliczonaJednostka(this.iloscMolowa, this.JednostkaMolowa);
                    ////    this.JednostkaMolowa = noweWartosci.Item1 as JednostkaLicznosci;
                    ////    this.IloscMolowa = noweWartosci.Item2;
                    ////}
                }
            }
        }
        
        [ImmediatePostData]
        public JednostkaLicznosci JednostkaMolowa
        {
            get => jednostkaMolowa;
            set => SetPropertyValue(nameof(JednostkaMolowa), ref jednostkaMolowa, value);            
        }

        


        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == nameof(this.JednostkaMolowa))
            { 
             
            }
        }

        private Tuple<JednostkaMiary, decimal> GetPrzeliczonaJednostka(decimal wartoscPrzeliczona, JednostkaMiary jednostka)
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
                        decimal nowyPrzelicznik = item.Przelicznik / przelicznik;
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
    }
}