using Common.Module.Module.BusinessObjects;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using PrzeliczenieJednostek.Module.Helpers;
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
                    if (PrzeliczenieJednostekHelper.Truncate(this.Ilosc))
                    {
                        var noweWartosci = PrzeliczenieJednostekHelper.GetPrzeliczonaJednostka(this.Ilosc, this.Jednostka);
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
                       var przeliczoneJednostki = PrzeliczenieJednostekHelper.SetIloscMolowa(this.JednostkaWagowa, this.JednostkaMolowa, this.Session,
                            this.IloscWagowa, this.Parametr);
                        if (przeliczoneJednostki.Item2 != null)
                        {
                            this.jednostkaMolowa = przeliczoneJednostki.Item2 as JednostkaLicznosci;
                            this.iloscMolowa = przeliczoneJednostki.Item1;
                            this.OnChanged(nameof(this.IloscMolowa));
                        }
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
                    this.IloscWagowa = PrzeliczenieJednostekHelper.SetIloscWagowaPodczasZmianyJednostki(this.IloscMolowa, this.Parametr, this.JednostkaWagowa.Przelicznik, this.JednostkaMolowa.Przelicznik);
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
                       var przeliczoneJednostki = PrzeliczenieJednostekHelper.SetIloscWagowa(this.JednostkaMolowa, this.JednostkaWagowa, 
                            this.Session, this.Parametr, this.IloscMolowa);                        
                        this.iloscWagowa = przeliczoneJednostki.Item1;
                        this.jednostkaWagowa = przeliczoneJednostki.Item2 as JednostkaWagi;
                        this.OnChanged(nameof(this.JednostkaWagowa));
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
                    this.IloscMolowa = PrzeliczenieJednostekHelper.SetIloscMolowaPodczasZmianyJednostki(this.IloscWagowa, this.Parametr, this.JednostkaWagowa.Przelicznik, this.JednostkaMolowa.Przelicznik);
                }
            }
        }     
    }
}